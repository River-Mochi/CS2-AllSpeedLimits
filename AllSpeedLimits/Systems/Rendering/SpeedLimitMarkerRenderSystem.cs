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

        private enum MarkerVisualKind
        {
            Default = 0,
            Custom = 1,
            Rail = 2
        }

        private enum MarkerNetworkKind
        {
            Road = 0,
            Rail = 1,
            Water = 2
        }

        private readonly struct MarkerGroupKey : IEquatable<MarkerGroupKey>
        {
            public readonly int SpeedKmh;
            public readonly MarkerVisualKind VisualKind;
            public readonly MarkerNetworkKind NetworkKind;

            public MarkerGroupKey(int speedKmh, MarkerVisualKind visualKind, MarkerNetworkKind networkKind)
            {
                SpeedKmh = speedKmh;
                VisualKind = visualKind;
                NetworkKind = networkKind;
            }

            public bool Equals(MarkerGroupKey other)
            {
                return SpeedKmh == other.SpeedKmh &&
                    VisualKind == other.VisualKind &&
                    NetworkKind == other.NetworkKind;
            }

            public override bool Equals(object? obj)
            {
                return obj is MarkerGroupKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return (SpeedKmh * 397) ^ ((int)VisualKind * 31) ^ (int)NetworkKind;
            }
        }

        private readonly struct MarkerRenderIdentity
        {
            public readonly float SpeedKmh;
            public readonly int RoundedSpeedKmh;
            public readonly bool IsWaterwayType;
            public readonly MarkerVisualKind VisualKind;
            public readonly MarkerGroupKey GroupKey;

            public MarkerRenderIdentity(
                float speedKmh,
                int roundedSpeedKmh,
                bool isWaterwayType,
                MarkerVisualKind visualKind,
                MarkerGroupKey groupKey)
            {
                SpeedKmh = speedKmh;
                RoundedSpeedKmh = roundedSpeedKmh;
                IsWaterwayType = isWaterwayType;
                VisualKind = visualKind;
                GroupKey = groupKey;
            }
        }

        private RenderingSystem m_RenderingSystem = null!;
        private OverlayRenderSystem m_OverlayRenderSystem = null!;
        private SegmentSpeedToolSystem m_SegmentSpeedToolSystem = null!;
        private CityConfigurationSystem m_CityConfigurationSystem = null!;
        private CameraUpdateSystem m_CameraUpdateSystem = null!;
        private PrefabSystem m_PrefabSystem = null!;

        private EntityQuery m_CustomSpeedQuery;
        private readonly Dictionary<int, TextMeshInfo> m_TextMeshCache = new Dictionary<int, TextMeshInfo>();
        private readonly Dictionary<Entity, MarkerRenderIdentity> m_FrameMarkerIdentities =
            new Dictionary<Entity, MarkerRenderIdentity>();
        private readonly HashSet<Entity> m_FrameVisibleMarkerEdges = new HashSet<Entity>();
        private readonly HashSet<Entity> m_FrameVisitedMarkerEdges = new HashSet<Entity>();
        private readonly List<Entity> m_FrameMarkerStack = new List<Entity>();
        private readonly List<Entity> m_FrameMarkerGroup = new List<Entity>();
        private readonly Dictionary<MarkerGroupKey, List<Vector2>> m_FrameDrawnMarkerCenters =
            new Dictionary<MarkerGroupKey, List<Vector2>>();
        // Floating number color knobs. These are text-only markers, not road-selection outlines.
        private static readonly Color s_DefaultMarkerTextColor = new Color(1f, 1f, 1f, 1f);
        private static readonly Color s_CustomMarkerTextColor = new Color(0.24f, 0.88f, 1.00f, 1f);
        private static readonly Color s_RailMarkerTextColor = new Color(0.45f, 1.00f, 0.20f, 1f);
        private const float s_MarkerGroupingStartZoom = 0.35f;
        private const float s_MarkerDuplicateMinDistancePx = 64f;
        private const float s_MarkerDuplicateMaxDistancePx = 120f;
        private const float s_MarkerDuplicateMidZoomBoostPx = 28f;
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
                float zoomLevel = m_CameraUpdateSystem != null ? m_CameraUpdateSystem.zoom : 5000f;
                float rawZoom = Mathf.Clamp01((zoomLevel - 1000f) / 13000f);
                float normalizedZoom = Mathf.Pow(rawZoom, 0.6f);
                bool groupMarkers = normalizedZoom >= s_MarkerGroupingStartZoom;
                float duplicateDistancePx = Mathf.Lerp(
                    s_MarkerDuplicateMinDistancePx,
                    s_MarkerDuplicateMaxDistancePx,
                    normalizedZoom);
                // Extra duplicate cleanup only in the middle zoom range; this is zero at close/far zoom.
                duplicateDistancePx += s_MarkerDuplicateMidZoomBoostPx *
                    Mathf.Sin(normalizedZoom * Mathf.PI);
                float duplicateDistanceSq = duplicateDistancePx * duplicateDistancePx;

                ClearFrameMarkerCollections();
                if (groupMarkers)
                {
                    BuildFrameMarkerIdentities(entities);
                    BuildVisibleMarkerGroups(entities);
                }

                foreach (Entity edge in entities)
                {
                    if (!EntityManager.Exists(edge))
                    {
                        continue;
                    }

                    MarkerRenderIdentity identity;
                    if (groupMarkers)
                    {
                        if (!m_FrameVisibleMarkerEdges.Contains(edge) ||
                            !m_FrameMarkerIdentities.TryGetValue(edge, out identity))
                        {
                            continue;
                        }
                    }
                    else if (!TryGetMarkerRenderIdentity(edge, out identity))
                    {
                        continue;
                    }

                    int cacheKey = GetTextMeshCacheKey(identity.RoundedSpeedKmh, identity.VisualKind);

                    if (!m_TextMeshCache.TryGetValue(cacheKey, out TextMeshInfo meshInfo))
                    {
                        meshInfo = CreateTextMesh(identity.RoundedSpeedKmh, identity.VisualKind);
                        m_TextMeshCache[cacheKey] = meshInfo;
                    }

                    if (meshInfo.Mesh == null || meshInfo.Material == null)
                    {
                        continue;
                    }

                    Curve curve = EntityManager.GetComponentData<Curve>(edge);
                    float3 position = MathUtils.Position(curve.m_Bezier, 0.5f);
                    // Height above segment midpoint. Water sits a little higher so the number clears
                    // the waterway selection band. Roads/rails sit lower close to the camera, but
                    // ease back upward at far zoom for readability.
                    float roadMarkerHeight = Mathf.Lerp(7.0f, 8.2f, normalizedZoom);
                    position.y += identity.IsWaterwayType ? 11.4f : roadMarkerHeight;
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
                    if (identity.IsWaterwayType)
                    {
                        float waterBaseScale = Mathf.Lerp(2.0f, 5.6f, normalizedZoom);
                        float waterMidZoomBoost = 0.95f * Mathf.Sin(normalizedZoom * Mathf.PI);
                        textScaleMultiplier = waterBaseScale + waterMidZoomBoost;
                    }
                    else
                    {
                        // Roads/rails: smaller close-up, with a mid-zoom readability bump.
                        // This keeps near-camera labels quieter without shrinking the scanning range.
                        float roadBaseScale = Mathf.Lerp(1.25f, 3.15f, normalizedZoom);
                        float roadMidZoomBoost = 0.82f * Mathf.Sin(normalizedZoom * Mathf.PI);
                        textScaleMultiplier = roadBaseScale + roadMidZoomBoost;
                    }

                    Rect screenBounds = default;
                    bool hasScreenBounds = hoverCamera != null &&
                        TryGetMarkerScreenBounds(
                            hoverCamera,
                            markerPosition,
                            meshInfo.Mesh,
                            textScaleMultiplier,
                            out screenBounds);

                    if (groupMarkers &&
                        hasScreenBounds &&
                        ShouldSkipNearbyDuplicateMarker(identity.GroupKey, screenBounds.center, duplicateDistanceSq))
                    {
                        continue;
                    }

                    if (groupMarkers && hasScreenBounds)
                    {
                        RegisterDrawnMarkerCenter(identity.GroupKey, screenBounds.center);
                    }

                    if (canUpdateMarkerTooltip && hasScreenBounds)
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
                                bestTooltipText = FormatMarkerTooltip(identity.SpeedKmh);
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

        private TextMeshInfo CreateTextMesh(int speedKmh, MarkerVisualKind visualKind)
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
                Color textColor = GetMarkerTextColor(visualKind);

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
                string styleSuffix = GetMarkerVisualKindSuffix(visualKind);

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

        private void ClearFrameMarkerCollections()
        {
            m_FrameMarkerIdentities.Clear();
            m_FrameVisibleMarkerEdges.Clear();
            m_FrameVisitedMarkerEdges.Clear();
            m_FrameMarkerStack.Clear();
            m_FrameMarkerGroup.Clear();

            foreach (List<Vector2> centers in m_FrameDrawnMarkerCenters.Values)
            {
                centers.Clear();
            }
        }

        private void BuildFrameMarkerIdentities(NativeArray<Entity> entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Entity edge = entities[i];
                if (!EntityManager.Exists(edge))
                {
                    continue;
                }

                if (TryGetMarkerRenderIdentity(edge, out MarkerRenderIdentity identity))
                {
                    m_FrameMarkerIdentities[edge] = identity;
                }
            }
        }

        private void BuildVisibleMarkerGroups(NativeArray<Entity> entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Entity edge = entities[i];
                if (m_FrameVisitedMarkerEdges.Contains(edge) ||
                    !m_FrameMarkerIdentities.TryGetValue(edge, out MarkerRenderIdentity identity))
                {
                    continue;
                }

                m_FrameMarkerGroup.Clear();
                CollectConnectedMarkerGroup(edge, identity.GroupKey);

                Entity representative = ChooseMarkerGroupRepresentative();
                if (representative != Entity.Null)
                {
                    m_FrameVisibleMarkerEdges.Add(representative);
                }
            }
        }

        private void CollectConnectedMarkerGroup(Entity startEdge, MarkerGroupKey groupKey)
        {
            m_FrameMarkerStack.Clear();
            m_FrameMarkerStack.Add(startEdge);

            while (m_FrameMarkerStack.Count > 0)
            {
                int lastIndex = m_FrameMarkerStack.Count - 1;
                Entity edge = m_FrameMarkerStack[lastIndex];
                m_FrameMarkerStack.RemoveAt(lastIndex);
                if (m_FrameVisitedMarkerEdges.Contains(edge) ||
                    !m_FrameMarkerIdentities.TryGetValue(edge, out MarkerRenderIdentity identity) ||
                    !identity.GroupKey.Equals(groupKey))
                {
                    continue;
                }

                m_FrameVisitedMarkerEdges.Add(edge);
                m_FrameMarkerGroup.Add(edge);

                if (!EntityManager.HasComponent<Edge>(edge))
                {
                    continue;
                }

                Edge edgeData = EntityManager.GetComponentData<Edge>(edge);
                AddSameGroupNeighbors(edge, edgeData.m_Start, groupKey);
                AddSameGroupNeighbors(edge, edgeData.m_End, groupKey);
            }
        }

        private void AddSameGroupNeighbors(Entity currentEdge, Entity node, MarkerGroupKey groupKey)
        {
            if (!EntityManager.HasBuffer<ConnectedEdge>(node))
            {
                return;
            }

            DynamicBuffer<ConnectedEdge> connectedEdges = EntityManager.GetBuffer<ConnectedEdge>(node);
            if (connectedEdges.Length > 2)
            {
                return;
            }

            for (int i = 0; i < connectedEdges.Length; i++)
            {
                Entity nextEdge = connectedEdges[i].m_Edge;
                if (nextEdge == currentEdge ||
                    m_FrameVisitedMarkerEdges.Contains(nextEdge) ||
                    !m_FrameMarkerIdentities.TryGetValue(nextEdge, out MarkerRenderIdentity nextIdentity) ||
                    !nextIdentity.GroupKey.Equals(groupKey))
                {
                    continue;
                }

                m_FrameMarkerStack.Add(nextEdge);
            }
        }

        private Entity ChooseMarkerGroupRepresentative()
        {
            if (m_FrameMarkerGroup.Count == 0)
            {
                return Entity.Null;
            }

            if (m_FrameMarkerGroup.Count == 1)
            {
                return m_FrameMarkerGroup[0];
            }

            float3 center = default;
            int validCount = 0;
            for (int i = 0; i < m_FrameMarkerGroup.Count; i++)
            {
                Entity edge = m_FrameMarkerGroup[i];
                if (!EntityManager.HasComponent<Curve>(edge))
                {
                    continue;
                }

                Curve curve = EntityManager.GetComponentData<Curve>(edge);
                center += MathUtils.Position(curve.m_Bezier, 0.5f);
                validCount++;
            }

            if (validCount == 0)
            {
                return m_FrameMarkerGroup[0];
            }

            center /= (float)validCount;
            Entity bestEdge = m_FrameMarkerGroup[0];
            float bestDistanceSq = float.MaxValue;
            for (int i = 0; i < m_FrameMarkerGroup.Count; i++)
            {
                Entity edge = m_FrameMarkerGroup[i];
                if (!EntityManager.HasComponent<Curve>(edge))
                {
                    continue;
                }

                Curve curve = EntityManager.GetComponentData<Curve>(edge);
                float distanceSq = math.distancesq(center, MathUtils.Position(curve.m_Bezier, 0.5f));
                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    bestEdge = edge;
                }
            }

            return bestEdge;
        }

        private bool TryGetMarkerRenderIdentity(Entity edge, out MarkerRenderIdentity identity)
        {
            identity = default;

            if (!EntityManager.HasComponent<CustomSpeed>(edge))
            {
                return false;
            }

            CustomSpeed customSpeed = EntityManager.GetComponentData<CustomSpeed>(edge);
            int speedKmh = Mathf.RoundToInt(customSpeed.m_Speed);
            bool isWaterwayType = IsWaterwayEdge(edge);
            bool isDefaultSpeed = IsDefaultSpeed(edge, customSpeed.m_Speed);
            MarkerVisualKind visualKind = GetMarkerVisualKind(edge, isDefaultSpeed);
            MarkerNetworkKind networkKind = GetMarkerNetworkKind(edge);

            identity = new MarkerRenderIdentity(
                customSpeed.m_Speed,
                speedKmh,
                isWaterwayType,
                visualKind,
                new MarkerGroupKey(speedKmh, visualKind, networkKind));

            return true;
        }

        private MarkerNetworkKind GetMarkerNetworkKind(Entity edge)
        {
            if (IsWaterwayEdge(edge))
            {
                return MarkerNetworkKind.Water;
            }

            return IsTrainOrSubwayEdge(edge)
                ? MarkerNetworkKind.Rail
                : MarkerNetworkKind.Road;
        }

        private MarkerVisualKind GetMarkerVisualKind(Entity edge, bool isDefaultSpeed)
        {
            if (isDefaultSpeed)
            {
                return MarkerVisualKind.Default;
            }

            return IsTrainOrSubwayEdge(edge)
                ? MarkerVisualKind.Rail
                : MarkerVisualKind.Custom;
        }

        private bool IsTrainOrSubwayEdge(Entity edge)
        {
            if (EntityManager.HasComponent<TramTrack>(edge))
            {
                return false;
            }

            return EntityManager.HasComponent<TrainTrack>(edge) ||
                EntityManager.HasComponent<SubwayTrack>(edge);
        }

        private bool ShouldSkipNearbyDuplicateMarker(
            MarkerGroupKey groupKey,
            Vector2 screenCenter,
            float duplicateDistanceSq)
        {
            if (!m_FrameDrawnMarkerCenters.TryGetValue(groupKey, out List<Vector2> centers))
            {
                return false;
            }

            for (int i = 0; i < centers.Count; i++)
            {
                Vector2 delta = centers[i] - screenCenter;
                if (delta.sqrMagnitude <= duplicateDistanceSq)
                {
                    return true;
                }
            }

            return false;
        }

        private void RegisterDrawnMarkerCenter(MarkerGroupKey groupKey, Vector2 screenCenter)
        {
            if (!m_FrameDrawnMarkerCenters.TryGetValue(groupKey, out List<Vector2> centers))
            {
                centers = new List<Vector2>();
                m_FrameDrawnMarkerCenters[groupKey] = centers;
            }

            centers.Add(screenCenter);
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

        private static Color GetMarkerTextColor(MarkerVisualKind visualKind)
        {
            return visualKind switch
            {
                MarkerVisualKind.Default => s_DefaultMarkerTextColor,
                MarkerVisualKind.Rail => s_RailMarkerTextColor,
                _ => s_CustomMarkerTextColor
            };
        }

        private static string GetMarkerVisualKindSuffix(MarkerVisualKind visualKind)
        {
            return visualKind switch
            {
                MarkerVisualKind.Default => "default",
                MarkerVisualKind.Rail => "rail",
                _ => "custom"
            };
        }

        private static int GetTextMeshCacheKey(int speedKmh, MarkerVisualKind visualKind)
        {
            return (speedKmh * 4) + (int)visualKind;
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
