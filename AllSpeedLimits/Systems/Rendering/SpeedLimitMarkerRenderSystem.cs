// <copyright file="SpeedLimitMarkerRenderSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Rendering/SpeedLimitMarkerRenderSystem.cs
// Purpose: Renders floating speed numbers above changed segments while the tool is active.

namespace RoadRailSpeeds.Systems
{
    using System;
    using System.Collections.Generic;
    using Colossal.Mathematics;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.City;
    using Game.Input;
    using Game.Net;
    using Game.Prefabs;
    using Game.Rendering;
    using Game.UI;
    using RoadRailSpeeds.Components;
    using TMPro;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;
    using SubLane = Game.Net.SubLane;
    using TrackLane = Game.Net.TrackLane;

    [Preserve]
    public partial class SpeedLimitMarkerRenderSystem : GameSystemBase
    {
        private struct TextMeshInfo
        {
            public Mesh? Mesh;
            public Material? Material;
        }

        private RenderingSystem m_RenderingSystem = null!;
        private OverlayRenderSystem m_OverlayRenderSystem = null!;
        private SegmentSpeedToolSystem m_SegmentSpeedToolSystem = null!;
        private CityConfigurationSystem m_CityConfigurationSystem = null!;
        private CameraUpdateSystem m_CameraUpdateSystem = null!;
        private PrefabSystem m_PrefabSystem = null!;

        private EntityQuery m_CustomSpeedQuery;
        private readonly Dictionary<int, TextMeshInfo> m_TextMeshCache = new Dictionary<int, TextMeshInfo>();
        // Floating number color knobs. These are text-only markers, not road-selection outlines.
        private static readonly Color s_DefaultMarkerTextColor = new Color(1f, 1f, 1f, 1f);
        private static readonly Color s_CustomMarkerTextColor = new Color(0.24f, 0.88f, 1.00f, 1f);
        // Marker tooltip hit-test knobs. Screen-distance math only; no physics raycasts.
        // Increase padding/min size for easier hover, decrease them when the tooltip feels too eager.
        // Keep the hover target a little larger than the visible glyphs so marker tooltips stay easy to trigger.
        private const float s_MarkerTooltipPaddingPx = 6f;
        private const float s_MarkerTooltipMinWidthPx = 52f;
        private const float s_MarkerTooltipMinHeightPx = 30f;

        private Setting? m_Settings;
        private int m_FaceColorID;

        private string? m_LastTheme;
        private Setting.SpeedUnit m_LastUnitPreference = Setting.SpeedUnit.Auto;
        private bool m_LastDoubleSpeedDisplay;
        private string m_MarkerTooltipText = string.Empty;
        private float m_MarkerTooltipX;
        private float m_MarkerTooltipY;

        [Preserve]
        public SpeedLimitMarkerRenderSystem()
        {
        }

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();

            m_RenderingSystem = World.GetOrCreateSystemManaged<RenderingSystem>();
            m_OverlayRenderSystem = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
            m_SegmentSpeedToolSystem = World.GetOrCreateSystemManaged<SegmentSpeedToolSystem>();
            m_CityConfigurationSystem = World.GetOrCreateSystemManaged<CityConfigurationSystem>();
            m_CameraUpdateSystem = World.GetExistingSystemManaged<CameraUpdateSystem>();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            m_Settings = Mod.Settings;
            m_LastUnitPreference = m_Settings?.SpeedUnitPreference ?? Setting.SpeedUnit.Auto;
            m_LastDoubleSpeedDisplay = m_Settings?.DoubleSpeedDisplay ?? false;

            // Modern build form of GetEntityQuery(new EntityQueryDesc{...}); the render pass below reads
            // this cached EntityQuery to find segments with a custom speed.
            m_CustomSpeedQuery = SystemAPI.QueryBuilder()
                .WithAll<Edge, Curve, CustomSpeed>()
                .Build();

            m_FaceColorID = Shader.PropertyToID("_FaceColor");

            // Unity render-pipeline event. This is not Harmony patching.
            RenderPipelineManager.beginContextRendering += Render;
        }

        [Preserve]
        protected override void OnDestroy()
        {
            RenderPipelineManager.beginContextRendering -= Render;
            ClearTextMeshCache();

            base.OnDestroy();
        }

        [Preserve]
        protected override void OnUpdate()
        {
            m_Settings ??= Mod.Settings;

            if (!m_SegmentSpeedToolSystem.IsActive || m_RenderingSystem.hideOverlay)
            {
                ClearMarkerHoverState();
            }

            if (m_SegmentSpeedToolSystem.IsActive)
            {
                string currentTheme = GetCurrentMapTheme();

                if (m_LastTheme == null)
                {
                    m_LastTheme = currentTheme;
                }
                else if (m_LastTheme != currentTheme)
                {
                    ClearTextMeshCache();
                    m_LastTheme = currentTheme;
                }
            }

            if (m_Settings == null)
            {
                return;
            }

            Setting.SpeedUnit currentPreference = m_Settings.SpeedUnitPreference;
            bool currentDoubleDisplay = m_Settings.DoubleSpeedDisplay;

            if (currentPreference != m_LastUnitPreference)
            {
                ClearTextMeshCache();
                m_LastUnitPreference = currentPreference;
            }

            if (currentDoubleDisplay != m_LastDoubleSpeedDisplay)
            {
                ClearTextMeshCache();
                m_LastDoubleSpeedDisplay = currentDoubleDisplay;
            }
        }

        private void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            try
            {
                if (!m_SegmentSpeedToolSystem.IsActive)
                {
                    ClearMarkerHoverState();
                    return;
                }

                if (m_RenderingSystem.hideOverlay)
                {
                    ClearMarkerHoverState();
                    return;
                }

                // Player toggled the floating numbers off from the panel title bar.
                if (m_Settings?.HideSpeedMarkers == true)
                {
                    ClearMarkerHoverState();
                    return;
                }

                using NativeArray<Entity> entities = m_CustomSpeedQuery.ToEntityArray(Allocator.Temp);
                if (entities.Length == 0)
                {
                    ClearMarkerHoverState();
                    return;
                }

                Camera? hoverCamera = GetGameCamera(cameras);
                // Marker tooltips describe the floating world speed sign, so they stay available
                // even when the panel-only help tooltips are disabled.
                bool canUpdateMarkerTooltip = hoverCamera != null;
                Vector3 mousePosition = InputManager.instance.mousePosition;
                bool hasHover = false;
                float bestDistanceSq = float.MaxValue;
                string bestTooltipText = string.Empty;
                float bestTooltipX = 0f;
                float bestTooltipY = 0f;

                foreach (Entity edge in entities)
                {
                    if (!EntityManager.Exists(edge))
                    {
                        continue;
                    }

                    CustomSpeed customSpeed = EntityManager.GetComponentData<CustomSpeed>(edge);
                    bool isWaterwayType = IsWaterwayEdge(edge);
                    int speedKmh = Mathf.RoundToInt(customSpeed.m_Speed);
                    bool isDefaultSpeed = IsDefaultSpeed(edge, customSpeed.m_Speed);
                    int cacheKey = GetTextMeshCacheKey(speedKmh, isDefaultSpeed);

                    if (!m_TextMeshCache.TryGetValue(cacheKey, out TextMeshInfo meshInfo))
                    {
                        meshInfo = CreateTextMesh(speedKmh, isDefaultSpeed);
                        m_TextMeshCache[cacheKey] = meshInfo;
                    }

                    if (meshInfo.Mesh == null || meshInfo.Material == null)
                    {
                        continue;
                    }

                    Curve curve = EntityManager.GetComponentData<Curve>(edge);
                    float zoomLevel = m_CameraUpdateSystem != null ? m_CameraUpdateSystem.zoom : 5000f;
                    float rawZoom = Mathf.Clamp01((zoomLevel - 1000f) / 13000f);
                    float normalizedZoom = Mathf.Pow(rawZoom, 0.6f);
                    float3 position = MathUtils.Position(curve.m_Bezier, 0.5f);
                    // Height above segment midpoint. Water is already good. Roads/rails sit lower
                    // close to the camera, but ease back upward at far zoom for readability.
                    float roadMarkerHeight = Mathf.Lerp(7.0f, 8.2f, normalizedZoom);
                    position.y += isWaterwayType ? 10.8f : roadMarkerHeight;
                    Vector3 markerPosition = position;

                    // Floating world-speed marker size:
                    // 1. textMesh.fontSize below sets the base glyph size before world scaling.
                    //    Raise/lower it when every zoom level should look bigger/smaller.
                    // 2. normalizedZoom is 0 near the ground and 1 when zoomed far out.
                    //    Mathf.Lerp(closeScale, farScale, normalizedZoom) blends between them.
                    // 3. Raise 1st lerp value to make close and near-mid zoom bigger.
                    //    Raise 2nd lerp value to make far zoom bigger.
                    //    Lower either value to shrink that end of the zoom range.
                    // 4. If only middle zoom feels wrong, tune normalizedZoom above:
                    //    smaller Pow exponent grows sooner; larger exponent grows later.
                    float textScaleMultiplier;
                    if (isWaterwayType)
                    {
                        textScaleMultiplier = Mathf.Lerp(2.0f, 4.45f, normalizedZoom);
                    }
                    else
                    {
                        // Roads/rails: smaller close-up, with a mid-zoom readability bump.
                        // This keeps near-camera labels quieter without shrinking the scanning range.
                        float roadBaseScale = Mathf.Lerp(1.38f, 2.75f, normalizedZoom);
                        float roadMidZoomBoost = 0.36f * Mathf.Sin(normalizedZoom * Mathf.PI);
                        textScaleMultiplier = roadBaseScale + roadMidZoomBoost;
                    }

                    if (canUpdateMarkerTooltip &&
                        hoverCamera != null &&
                        TryGetMarkerScreenBounds(
                            hoverCamera,
                            markerPosition,
                            meshInfo.Mesh,
                            textScaleMultiplier,
                            out Rect screenBounds))
                    {
                        if (screenBounds.Contains(new Vector2(mousePosition.x, mousePosition.y)))
                        {
                            Vector2 center = screenBounds.center;
                            float dx = center.x - mousePosition.x;
                            float dy = center.y - mousePosition.y;
                            float distanceSq = (dx * dx) + (dy * dy);

                            if (!hasHover || distanceSq <= bestDistanceSq)
                            {
                                hasHover = true;
                                bestDistanceSq = distanceSq;
                                bestTooltipText = FormatMarkerTooltip(customSpeed.m_Speed);
                                // UI marker tooltip expects screen coordinates. X is the marker center;
                                // Y is just below the screen bounds so React can center the tooltip under it.
                                bestTooltipX = center.x;
                                bestTooltipY = Screen.height - screenBounds.yMin;
                            }
                        }
                    }

                    foreach (Camera camera in cameras)
                    {
                        if (camera.cameraType != CameraType.Game &&
                            camera.cameraType != CameraType.SceneView)
                        {
                            continue;
                        }

                        Quaternion rotation = Quaternion.LookRotation(
                            camera.transform.forward,
                            camera.transform.up);

                        Matrix4x4 matrix = Matrix4x4.TRS(
                            markerPosition,
                            rotation,
                            new Vector3(textScaleMultiplier, textScaleMultiplier, textScaleMultiplier));

                        Graphics.DrawMesh(
                            meshInfo.Mesh,
                            matrix,
                            meshInfo.Material,
                            0,
                            camera,
                            0,
                            null,
                            castShadows: false,
                            receiveShadows: false);
                    }
                }

                if (hasHover)
                {
                    SetMarkerHoverState(bestTooltipText, bestTooltipX, bestTooltipY);
                }
                else
                {
                    ClearMarkerHoverState();
                }
            }
            catch (Exception ex)
            {
                ClearMarkerHoverState();

                // Render runs often. Warn once so one repeated render failure cannot spam the log.
                LogUtils.WarnOnce(
                    "SpeedLimitMarkerRenderSystem.Render",
                    () => $"SpeedLimitMarkerRenderSystem.Render failed: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }
        }

        public bool TryGetMarkerTooltip(out string text, out float x, out float y)
        {
            text = m_MarkerTooltipText;
            x = m_MarkerTooltipX;
            y = m_MarkerTooltipY;

            return !string.IsNullOrEmpty(m_MarkerTooltipText);
        }

        private TextMeshInfo CreateTextMesh(int speedKmh, bool isDefaultSpeed)
        {
            // The game shares ONE TextMeshPro instance (OverlayRenderSystem.GetTextMesh) across
            // every world-UI overlay label, including vanilla road and district name labels.
            // Anything we leave mutated on it leaks: the game later regenerates those labels
            // through our state and caches them with it, so a custom-speed cyan face color turns
            // every road name cyan and it stays cyan until the game is reloaded. We snapshot the
            // shared instance up front and restore it in the finally below so we leave no trace.
            TextMeshPro textMesh = m_OverlayRenderSystem.GetTextMesh();

            Vector2 prevSizeDelta = textMesh.rectTransform.sizeDelta;
            float prevFontSize = textMesh.fontSize;
            TextAlignmentOptions prevAlignment = textMesh.alignment;
            Color prevColor = textMesh.color;
            float prevCharacterSpacing = textMesh.characterSpacing;
            FontStyles prevFontStyle = textMesh.fontStyle;
            string prevText = textMesh.text;

            try
            {
                bool isEUMap = IsMapEuropean();
                bool showMetric = m_Settings?.ShouldShowMetric(isEUMap) ?? isEUMap;
                bool doubleDisplay = m_Settings?.DoubleSpeedDisplay ?? false;
                int multiplier = doubleDisplay ? 2 : 1;
                Color textColor = isDefaultSpeed ? s_DefaultMarkerTextColor : s_CustomMarkerTextColor;

                textMesh.rectTransform.sizeDelta = new Vector2(176f, 92f);
                // Base font size for floating speed number before zoom scaling is applied above.
                textMesh.fontSize = 31f;
                textMesh.alignment = TextAlignmentOptions.Center;
                textMesh.color = textColor;
                textMesh.characterSpacing = 0f;
                textMesh.fontStyle = FontStyles.Bold;

                string speedText;
                if (showMetric)
                {
                    speedText = (speedKmh * multiplier).ToString();
                }
                else
                {
                    int speedMph = Mathf.RoundToInt(speedKmh * 0.621371f);
                    speedText = (speedMph * multiplier).ToString();
                }

                textMesh.text = speedText;
                textMesh.ForceMeshUpdate(ignoreActiveState: true, forceTextReparsing: true);

                TMP_TextInfo textInfo = textMesh.textInfo;
                if (textInfo.meshInfo.Length == 0)
                {
                    return default;
                }

                TMP_MeshInfo tmpMeshInfo = textInfo.meshInfo[0];
                if (tmpMeshInfo.vertexCount == 0)
                {
                    return default;
                }

                string unitSuffix = showMetric ? "kmh" : "mph";
                string doubleSuffix = doubleDisplay ? "_2x" : string.Empty;
                string styleSuffix = isDefaultSpeed ? "default" : "custom";

                Mesh mesh = new Mesh
                {
                    name = $"SpeedLimit_{styleSuffix}_{speedKmh}_{unitSuffix}{doubleSuffix}",
                    vertices = tmpMeshInfo.vertices,
                    triangles = tmpMeshInfo.triangles,
                    uv = tmpMeshInfo.uvs0,
                    uv2 = tmpMeshInfo.uvs2,
                    colors32 = tmpMeshInfo.colors32
                };

                mesh.RecalculateBounds();
                Material material = new Material(tmpMeshInfo.material)
                {
                    name = $"SpeedLimitMaterial_{styleSuffix}_{speedKmh}_{unitSuffix}{doubleSuffix}"
                };

                material.SetColor(m_FaceColorID, textColor);

                // Keep the sign number crisp and readable without a TMP outline.
                material.SetFloat("_FaceDilate", 0f);
                material.SetFloat("_OutlineWidth", 0f);
                material.SetFloat("_GlowPower", 0f);
                material.SetFloat("_WeightNormal", 0.35f);
                material.SetFloat("_WeightBold", 0.85f);

                m_OverlayRenderSystem.CopyFontAtlasParameters(tmpMeshInfo.material, material);

                return new TextMeshInfo
                {
                    Mesh = mesh,
                    Material = material
                };
            }
            catch (Exception ex)
            {
                LogUtils.Error(
                    () => $"Failed to create speed text mesh for {speedKmh} km/h: {ex.GetType().Name}: {ex.Message}",
                    ex);

                return default;
            }
            finally
            {
                // Restore the shared TextMeshPro to the exact state we found it. This is what keeps
                // vanilla road/district name labels white and normal-weight; without it our marker
                // color and bold style bleed into every cached world-UI label.
                textMesh.rectTransform.sizeDelta = prevSizeDelta;
                textMesh.fontSize = prevFontSize;
                textMesh.alignment = prevAlignment;
                textMesh.color = prevColor;
                textMesh.characterSpacing = prevCharacterSpacing;
                textMesh.fontStyle = prevFontStyle;
                textMesh.SetText(prevText);
            }
        }

        private bool IsTrackEdge(Entity edge)
        {
            if (EntityManager.HasComponent<TrainTrack>(edge) ||
                EntityManager.HasComponent<TramTrack>(edge) ||
                EntityManager.HasComponent<SubwayTrack>(edge))
            {
                return true;
            }

            if (!EntityManager.HasBuffer<SubLane>(edge))
            {
                return false;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);
            for (int i = 0; i < subLanes.Length; i++)
            {
                if (EntityManager.HasComponent<TrackLane>(subLanes[i].m_SubLane))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsWaterwayEdge(Entity edge)
        {
            return EntityManager.HasComponent<Waterway>(edge);
        }

        private static Camera? GetGameCamera(List<Camera> cameras)
        {
            foreach (Camera camera in cameras)
            {
                if (camera.cameraType == CameraType.Game)
                {
                    return camera;
                }
            }

            return null;
        }

        private static bool TryGetMarkerScreenBounds(
            Camera camera,
            Vector3 markerPosition,
            Mesh mesh,
            float textScaleMultiplier,
            out Rect screenBounds)
        {
            screenBounds = default;
            Vector3 center = camera.WorldToScreenPoint(markerPosition);

            if (center.z <= 0f)
            {
                return false;
            }

            Bounds meshBounds = mesh.bounds;
            float halfWorldWidth = Mathf.Max(meshBounds.extents.x * textScaleMultiplier, 0.01f);
            float halfWorldHeight = Mathf.Max(meshBounds.extents.y * textScaleMultiplier, 0.01f);

            Vector3 right = camera.transform.right * halfWorldWidth;
            Vector3 up = camera.transform.up * halfWorldHeight;

            float minX = center.x;
            float maxX = center.x;
            float minY = center.y;
            float maxY = center.y;

            if (!ExpandScreenBounds(camera.WorldToScreenPoint(markerPosition - right - up), ref minX, ref maxX, ref minY, ref maxY))
            {
                return false;
            }

            if (!ExpandScreenBounds(camera.WorldToScreenPoint(markerPosition - right + up), ref minX, ref maxX, ref minY, ref maxY))
            {
                return false;
            }

            if (!ExpandScreenBounds(camera.WorldToScreenPoint(markerPosition + right - up), ref minX, ref maxX, ref minY, ref maxY))
            {
                return false;
            }

            if (!ExpandScreenBounds(camera.WorldToScreenPoint(markerPosition + right + up), ref minX, ref maxX, ref minY, ref maxY))
            {
                return false;
            }

            float width = Mathf.Max(maxX - minX, s_MarkerTooltipMinWidthPx);
            float height = Mathf.Max(maxY - minY, s_MarkerTooltipMinHeightPx);
            float centerX = (minX + maxX) * 0.5f;
            float centerY = (minY + maxY) * 0.5f;

            screenBounds = new Rect(
                centerX - (width * 0.5f) - s_MarkerTooltipPaddingPx,
                centerY - (height * 0.5f) - s_MarkerTooltipPaddingPx,
                width + (s_MarkerTooltipPaddingPx * 2f),
                height + (s_MarkerTooltipPaddingPx * 2f));

            return screenBounds.xMax >= 0f &&
                screenBounds.xMin <= Screen.width &&
                screenBounds.yMax >= 0f &&
                screenBounds.yMin <= Screen.height;
        }

        private static bool ExpandScreenBounds(
            Vector3 point,
            ref float minX,
            ref float maxX,
            ref float minY,
            ref float maxY)
        {
            if (point.z <= 0f)
            {
                return false;
            }

            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);
            minY = Mathf.Min(minY, point.y);
            maxY = Mathf.Max(maxY, point.y);

            return true;
        }

        private string FormatMarkerTooltip(float speedKmh)
        {
            bool doubleDisplay = m_Settings?.DoubleSpeedDisplay ?? false;
            int multiplier = doubleDisplay ? 2 : 1;
            float displayKmh = speedKmh * multiplier;
            int roundedKmh = Mathf.RoundToInt(displayKmh);
            int roundedMph = Mathf.RoundToInt(displayKmh * 0.621371f);

            string kmh = $"{roundedKmh} km/h";
            string mph = $"{roundedMph} mph";

            // Show the same unit as the big floating sign first, then the alternate, pipe-separated:
            //   sign in km/h -> "80 km/h | 50 mph"   sign in mph -> "50 mph | 80 km/h"
            bool isEUMap = IsMapEuropean();
            bool showMetric = m_Settings?.ShouldShowMetric(isEUMap) ?? isEUMap;

            return showMetric ? $"{kmh} | {mph}" : $"{mph} | {kmh}";
        }

        private void SetMarkerHoverState(string text, float x, float y)
        {
            m_MarkerTooltipText = text;
            m_MarkerTooltipX = x;
            m_MarkerTooltipY = y;
        }

        private void ClearMarkerHoverState()
        {
            m_MarkerTooltipText = string.Empty;
            m_MarkerTooltipX = 0f;
            m_MarkerTooltipY = 0f;
        }

        private bool IsDefaultSpeed(Entity edge, float speedKmh)
        {
            float vanillaSpeed = GetVanillaSpeed(edge);
            return vanillaSpeed > 0f && Mathf.Abs(vanillaSpeed - speedKmh) <= 0.5f;
        }

        private float GetVanillaSpeed(Entity edge)
        {
            if (!EntityManager.HasComponent<PrefabRef>(edge))
            {
                return -1f;
            }

            PrefabRef prefabRef = EntityManager.GetComponentData<PrefabRef>(edge);
            if (!m_PrefabSystem.TryGetPrefab(prefabRef, out PrefabBase prefabBase) || prefabBase == null)
            {
                return -1f;
            }

            return prefabBase switch
            {
                RoadPrefab roadPrefab => roadPrefab.m_SpeedLimit / 2f,
                TrackPrefab trackPrefab => trackPrefab.m_SpeedLimit / 2f,
                WaterwayPrefab waterwayPrefab => waterwayPrefab.m_SpeedLimit / 2f,
                _ => -1f
            };
        }

        private static int GetTextMeshCacheKey(int speedKmh, bool isDefaultSpeed)
        {
            return (speedKmh * 2) + (isDefaultSpeed ? 1 : 0);
        }

        private string GetCurrentMapTheme()
        {
            try
            {
                if (m_CityConfigurationSystem.defaultTheme != Entity.Null)
                {
                    ThemePrefab theme =
                        m_PrefabSystem.GetPrefab<ThemePrefab>(m_CityConfigurationSystem.defaultTheme);

                    return theme?.name ?? "Unknown";
                }
            }
            catch (Exception ex)
            {
                LogUtils.Warn(
                    () => $"Failed to get map theme: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }

            return "Unknown";
        }

        private bool IsMapEuropean()
        {
            string theme = GetCurrentMapTheme();

            // CS2 theme names are usually "North American" or "European".
            return !theme.Equals("North American", StringComparison.Ordinal);
        }

        private void ClearTextMeshCache()
        {
            foreach (TextMeshInfo meshInfo in m_TextMeshCache.Values)
            {
                if (meshInfo.Mesh != null)
                {
                    UnityEngine.Object.Destroy(meshInfo.Mesh);
                }

                if (meshInfo.Material != null)
                {
                    UnityEngine.Object.Destroy(meshInfo.Material);
                }
            }

            m_TextMeshCache.Clear();
        }
    }
}
