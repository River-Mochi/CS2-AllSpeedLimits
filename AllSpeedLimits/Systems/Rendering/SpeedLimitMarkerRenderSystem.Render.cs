// <copyright file="SpeedLimitMarkerRenderSystem.Render.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Rendering/SpeedLimitMarkerRenderSystem.Render.cs
// Purpose: Render pass, marker placement, screen hit bounds, and hover state.

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

    public partial class SpeedLimitMarkerRenderSystem
    {
        private void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            try
            {
                if (!m_SegmentSpeedToolSystem.IsActive)
                {
#if DEBUG
                    ResetMarkerVisibilityDiagnostics();
#endif
                    ClearMarkerHoverState();
                    return;
                }

                if (m_RenderingSystem.hideOverlay)
                {
#if DEBUG
                    ResetMarkerVisibilityDiagnostics();
#endif
                    ClearMarkerHoverState();
                    return;
                }

                // Player toggled the floating numbers off from the panel title bar.
                if (m_Settings?.HideSpeedMarkers == true)
                {
#if DEBUG
                    ResetMarkerVisibilityDiagnostics();
#endif
                    ClearMarkerHoverState();
                    return;
                }

                using NativeArray<Entity> entities = m_CustomSpeedQuery.ToEntityArray(Allocator.Temp);
                if (entities.Length == 0)
                {
#if DEBUG
                    ResetMarkerVisibilityDiagnostics();
#endif
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
                int markerGroupStride = GetMarkerGroupStride(normalizedZoom);
                float duplicateDistancePx = Mathf.Lerp(
                    s_MarkerDuplicateMinDistancePx,
                    s_MarkerDuplicateMaxDistancePx,
                    normalizedZoom);
                // Extra duplicate cleanup only in the middle zoom range; this is zero at close/far zoom.
                duplicateDistancePx += s_MarkerDuplicateMidZoomBoostPx *
                    Mathf.Sin(normalizedZoom * Mathf.PI);
                float duplicateDistanceSq = duplicateDistancePx * duplicateDistancePx;
#if DEBUG
                int eligibleMarkerCount = 0;
                int eligibleWaterMarkerCount = 0;
                int proximityRejectedCount = 0;
                int proximityRejectedWaterMarkerCount = 0;
                int duplicateRejectedCount = 0;
                int drawnMarkerCount = 0;
                int drawnWaterMarkerCount = 0;
#endif

                ClearFrameMarkerCollections();
                if (groupMarkers)
                {
                    BuildFrameMarkerIdentities(entities);
                    BuildVisibleMarkerGroups(entities, markerGroupStride);
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

#if DEBUG
                    eligibleMarkerCount++;
                    if (identity.IsWaterwayType)
                    {
                        eligibleWaterMarkerCount++;
                    }
#endif

                    Curve curve = EntityManager.GetComponentData<Curve>(edge);
                    float3 position = MathUtils.Position(curve.m_Bezier, 0.5f);
                    // Height above segment midpoint. Water sits a little higher so the number clears
                    // the waterway selection band. Roads/rails sit lower close to the camera, but
                    // ease back upward at far zoom for readability.
                    float roadMarkerHeight = Mathf.Lerp(7.0f, 8.2f, normalizedZoom);
                    // Underground subway markers clear the terrain without sitting at road-marker height.
                    float undergroundSubwayMarkerHeight = Mathf.Lerp(12.0f, 14.0f, normalizedZoom);
                    position.y += identity.IsWaterwayType
                        ? 11.4f
                        : (identity.IsUndergroundSubway ? undergroundSubwayMarkerHeight : roadMarkerHeight);
                    Vector3 markerPosition = position;

                    if (normalizedZoom <= s_MarkerProximityStartZoom &&
                        hoverCamera != null &&
                        !ShouldDrawProximityMarker(hoverCamera, markerPosition, normalizedZoom))
                    {
#if DEBUG
                        proximityRejectedCount++;
                        if (identity.IsWaterwayType)
                        {
                            proximityRejectedWaterMarkerCount++;
                        }
#endif
                        continue;
                    }

                    int cacheKey = GetTextMeshCacheKey(identity.GroupKey.SpeedKmh, identity.GroupKey.VisualKind);

                    if (!m_TextMeshCache.TryGetValue(cacheKey, out TextMeshInfo meshInfo))
                    {
                        meshInfo = CreateTextMesh(identity.GroupKey.SpeedKmh, identity.GroupKey.VisualKind);
                        m_TextMeshCache[cacheKey] = meshInfo;
                    }

                    if (meshInfo.Mesh == null || meshInfo.Material == null)
                    {
                        continue;
                    }

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
                        float waterBaseScale = Mathf.Lerp(1.35f, 6.4f, normalizedZoom);
                        float waterMidZoomBoost = 0.9f * Mathf.Sin(normalizedZoom * Mathf.PI);
                        textScaleMultiplier = waterBaseScale + waterMidZoomBoost;
                    }
                    else
                    {
                        // Roads/rails: smaller close-up, with a mid-zoom readability bump.
                        // This keeps near-camera labels quieter without shrinking the scanning range.
                        float roadBaseScale = Mathf.Lerp(0.82f, 4.6f, normalizedZoom);
                        float roadMidZoomBoost = 0.9f * Mathf.Sin(normalizedZoom * Mathf.PI);
                        float roadFarZoomBoost = 2.6f *
                            Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((normalizedZoom - 0.72f) / 0.28f));
                        textScaleMultiplier = roadBaseScale + roadMidZoomBoost + roadFarZoomBoost;
                    }

                    if (hoverCamera != null)
                    {
                        textScaleMultiplier = ApplyReadableScreenScale(
                            hoverCamera,
                            markerPosition,
                            meshInfo.Mesh,
                            textScaleMultiplier,
                            normalizedZoom);
                    }

                    Rect screenBounds = default;
                    bool hasScreenBounds = hoverCamera != null &&
                        TryGetMarkerScreenBounds(
                            hoverCamera,
                            markerPosition,
                            meshInfo.Mesh,
                            textScaleMultiplier,
                            out screenBounds);

                    // Keep the screen-space density limit active after topology grouping ends.
                    // At close-mid zoom proximity is still deliberately broad; tying this to
                    // groupMarkers caused one wheel notch to jump from grouped representatives
                    // to every nearby edge (the observed 196 -> 1007 marker flood).
                    if (hasScreenBounds &&
                        ShouldSkipNearbyDuplicateMarker(identity.GroupKey, screenBounds.center, duplicateDistanceSq))
                    {
#if DEBUG
                        duplicateRejectedCount++;
#endif
                        continue;
                    }

                    if (hasScreenBounds)
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

#if DEBUG
                    drawnMarkerCount++;
                    if (identity.IsWaterwayType)
                    {
                        drawnWaterMarkerCount++;
                    }
#endif
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

#if DEBUG
                LogMarkerVisibilityTransition(
                    zoomLevel,
                    normalizedZoom,
                    groupMarkers,
                    entities.Length,
                    m_FrameVisibleMarkerEdges.Count,
                    eligibleMarkerCount,
                    eligibleWaterMarkerCount,
                    proximityRejectedCount,
                    proximityRejectedWaterMarkerCount,
                    duplicateRejectedCount,
                    drawnMarkerCount,
                    drawnWaterMarkerCount);
#endif

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


        private static bool ShouldDrawProximityMarker(
            Camera camera,
            Vector3 markerPosition,
            float normalizedZoom)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(markerPosition);
            if (viewportPoint.z <= 0f)
            {
                return false;
            }

            GetProximityLimits(normalizedZoom, out float maxCameraDepth, out float viewportRadius);
            if (viewportPoint.z > maxCameraDepth)
            {
                return false;
            }

            Vector2 fromCenter = new Vector2(viewportPoint.x - 0.5f, viewportPoint.y - 0.5f);

            return fromCenter.sqrMagnitude <= viewportRadius * viewportRadius;
        }

        private static void GetProximityLimits(
            float normalizedZoom,
            out float maxCameraDepth,
            out float viewportRadius)
        {
            // At map scale this starts broad enough to preserve the grouped representatives. The
            // depth limit stays broad through the group-to-proximity handoff, then contracts only
            // during truly close zoom so a single wheel notch cannot make every marker disappear.
            float proximityBlend = Mathf.Clamp01(
                (s_MarkerProximityStartZoom - normalizedZoom) / s_MarkerProximityStartZoom);
            float depthBlend = Mathf.Clamp01(
                (proximityBlend - s_ProximityDepthTightenStart) /
                (1f - s_ProximityDepthTightenStart));
            maxCameraDepth = Mathf.Lerp(
                s_ProximityStartCameraDepth,
                s_CloseMarkerMaxCameraDepthMin,
                depthBlend);
            viewportRadius = Mathf.Lerp(
                s_ProximityStartViewportRadius,
                s_CloseMarkerViewportRadiusMin,
                proximityBlend);
        }

#if DEBUG
        private void ResetMarkerVisibilityDiagnostics()
        {
            m_LastMarkerVisibilityDiagnosticZone = -1;
        }

        private void LogMarkerVisibilityTransition(
            float zoomLevel,
            float normalizedZoom,
            bool groupMarkers,
            int queriedMarkerCount,
            int groupedRepresentativeCount,
            int eligibleMarkerCount,
            int eligibleWaterMarkerCount,
            int proximityRejectedCount,
            int proximityRejectedWaterMarkerCount,
            int duplicateRejectedCount,
            int drawnMarkerCount,
            int drawnWaterMarkerCount)
        {
            int zone = normalizedZoom > s_MarkerProximityStartZoom
                ? 0
                : normalizedZoom >= s_MarkerGroupingStartZoom
                    ? 1
                    : normalizedZoom >= s_MarkerProximityStartZoom * (1f - s_ProximityDepthTightenStart)
                        ? 2
                        : 3;
            if (zone == m_LastMarkerVisibilityDiagnosticZone)
            {
                return;
            }

            m_LastMarkerVisibilityDiagnosticZone = zone;
            GetProximityLimits(normalizedZoom, out float maxCameraDepth, out float viewportRadius);
            string zoneName = zone switch
            {
                0 => "grouped-map",
                1 => "grouped-proximity",
                2 => "close-proximity",
                _ => "tight-proximity"
            };
            LogUtils.Info(
                () => $"[ASL] MarkerVisibility zone={zoneName} zoom={zoomLevel:0} normalized={normalizedZoom:0.000} " +
                    $"grouped={groupMarkers} queried={queriedMarkerCount} representatives={groupedRepresentativeCount} " +
                    $"eligible={eligibleMarkerCount} waterEligible={eligibleWaterMarkerCount} " +
                    $"proximityRejected={proximityRejectedCount} waterProximityRejected={proximityRejectedWaterMarkerCount} " +
                    $"duplicateRejected={duplicateRejectedCount} drawn={drawnMarkerCount} " +
                    $"waterDrawn={drawnWaterMarkerCount} depthLimit={maxCameraDepth:0} viewportRadius={viewportRadius:0.000}");
        }
#endif


        private static float ApplyReadableScreenScale(
            Camera camera,
            Vector3 markerPosition,
            Mesh mesh,
            float textScaleMultiplier,
            float normalizedZoom)
        {
            float localTextHeight = Mathf.Max(mesh.bounds.size.y, 0.01f);
            float pixelsPerWorldUnit;

            if (camera.orthographic)
            {
                pixelsPerWorldUnit = camera.pixelHeight / Mathf.Max(camera.orthographicSize * 2f, 0.01f);
            }
            else
            {
                // The marker tooltip is React UI, so it stays readable after projection. The blue
                // floating number is a world mesh. Keep a minimum projected pixel height here so it
                // behaves more like CS2's AreaUtils.CalculateLabelScale map labels instead of
                // shrinking away at far zoom.
                float cameraDepth = camera.WorldToScreenPoint(markerPosition).z;
                if (cameraDepth <= 0.01f)
                {
                    return textScaleMultiplier;
                }

                float verticalWorldSize = 2f *
                    cameraDepth *
                    Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f);
                pixelsPerWorldUnit = camera.pixelHeight / Mathf.Max(verticalWorldSize, 0.01f);
            }

            if (pixelsPerWorldUnit <= 0.0001f)
            {
                return textScaleMultiplier;
            }

            float farReadability = Mathf.SmoothStep(
                0f,
                1f,
                Mathf.Clamp01((normalizedZoom - s_MarkerReadableScaleStartZoom) /
                    (1f - s_MarkerReadableScaleStartZoom)));
            float targetPixelHeight = Mathf.Lerp(
                s_MarkerReadableCloseHeightPx,
                s_MarkerReadableFarHeightPx,
                farReadability);
            float currentPixelHeight = localTextHeight * textScaleMultiplier * pixelsPerWorldUnit;

            if (currentPixelHeight >= targetPixelHeight)
            {
                return textScaleMultiplier;
            }

            float readableScale = targetPixelHeight / (localTextHeight * pixelsPerWorldUnit);
            return Mathf.Max(textScaleMultiplier, readableScale);
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
    }
}
