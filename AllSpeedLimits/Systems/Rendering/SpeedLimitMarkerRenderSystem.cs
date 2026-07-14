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
            Water = 2,
            Rail = 3,
            Subway = 4
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
            public readonly bool IsWaterwayType;
            public readonly bool IsUndergroundSubway;
            public readonly MarkerGroupKey GroupKey;

            public MarkerRenderIdentity(
                float speedKmh,
                bool isWaterwayType,
                bool isUndergroundSubway,
                MarkerGroupKey groupKey)
            {
                SpeedKmh = speedKmh;
                IsWaterwayType = isWaterwayType;
                IsUndergroundSubway = isUndergroundSubway;
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
        private EntityQuery m_OverlaySettingsQuery;
        private readonly Dictionary<int, TextMeshInfo> m_TextMeshCache = new Dictionary<int, TextMeshInfo>();
        private readonly Dictionary<Entity, MarkerRenderIdentity> m_FrameMarkerIdentities =
            new Dictionary<Entity, MarkerRenderIdentity>();
        private readonly HashSet<Entity> m_FrameVisibleMarkerEdges = new HashSet<Entity>();
        private readonly HashSet<Entity> m_FrameVisitedMarkerEdges = new HashSet<Entity>();
        private readonly List<Entity> m_FrameMarkerStack = new List<Entity>();
        private readonly List<Entity> m_FrameMarkerGroup = new List<Entity>();
        private readonly Dictionary<MarkerGroupKey, List<Vector2>> m_FrameDrawnMarkerCenters =
            new Dictionary<MarkerGroupKey, List<Vector2>>();
        // Floating number color knobs. Text-only markers, not road-selection outlines.
        private static readonly Color s_DefaultMarkerTextColor = new Color(1f, 1f, 1f, 1f);
        // Compensated for the game overlay text material. This is tuned from the captured July 5
        // reference (#6BE8EA), not the raw color shown in the C# value.
        private static readonly Color s_CustomMarkerTextColor = new Color(0.55f, 0.94f, 0.94f, 1f);
        private static readonly Color s_RailMarkerTextColor = new Color(0.45f, 1.00f, 0.20f, 1f);
        private static readonly Color s_SubwayMarkerTextColor = new Color(1.00f, 0.30f, 0.82f, 1f);
        private static readonly Color s_WaterMarkerOutlineColor = new Color(1f, 1f, 1f, 1f);
        // Grouping handles map-scale density. Proximity begins before grouping ends so the two
        // modes overlap instead of creating a blank zoom notch during the handoff.
        private const float s_MarkerGroupingStartZoom = 0.08f;
        private const float s_MarkerProximityStartZoom = 0.24f;
        private const float s_MarkerDuplicateMinDistancePx = 70f;
        private const float s_MarkerDuplicateMaxDistancePx = 180f;
        private const float s_MarkerDuplicateMidZoomBoostPx = 48f;
        // Proximity begins broad and then contracts gradually through the handoff. This prevents
        // the final low-zoom wheel click from changing a city-scale depth range into only the
        // ground immediately below the camera.
        private const float s_ProximityStartViewportRadius = 0.72f;
        private const float s_CloseMarkerViewportRadiusMin = 0.50f;
        // Ground near the camera projects at the bottom of a tilted CS2 view. Bias the focus just
        // above screen center so close-range markers stay in the useful central viewing area.
        private const float s_ProximityViewportFocusY = 0.56f;
        private const float s_ProximityStartCameraDepth = 4600f;
        private const float s_CloseMarkerMaxCameraDepthMin = 1600f;
        private const float s_ProximityDepthTightenStart = 0.25f;
        // Waterways are sparse and often farther from the camera than nearby streets. Keep their
        // marker search a little wider so they arrive with the rest of the useful local view.
        private const float s_WaterProximityDepthMultiplier = 1.35f;
        private const float s_WaterProximityViewportRadiusBonus = 0.08f;
        // Marker tooltip hit-test knobs. Screen-distance math only; no physics raycasts.
        // Increase padding/min size for easier hover, decrease when tooltip feels too eager.
        // This keeps hover target a little larger than the visible glyphs so marker tooltips stay easy to trigger.
        private const float s_MarkerTooltipPaddingPx = 6f;
        private const float s_MarkerTooltipMinWidthPx = 52f;
        private const float s_MarkerTooltipMinHeightPx = 30f;
        // Minimum visible glyph height for floating number meshes. Tooltip size is separate React UI.
        private const float s_MarkerReadableCloseHeightPx = 18f;
        private const float s_MarkerReadableFarHeightPx = 29f;
        private const float s_MarkerReadableScaleStartZoom = 0.45f;

        private Setting? m_Settings;
        private int m_FaceColorID;

        private string? m_LastTheme;
        private Setting.SpeedUnit m_LastUnitPreference = Setting.SpeedUnit.Auto;
        private bool m_LastDoubleSpeedDisplay;
        private string m_MarkerTooltipText = string.Empty;
        private float m_MarkerTooltipX;
        private float m_MarkerTooltipY;
#if DEBUG
        // Temporary visibility telemetry: reset when the marker tool stops, then log each zoom
        // handoff once so a user report can show whether grouping or proximity removed markers.
        private int m_LastMarkerVisibilityDiagnosticZone = -1;
#endif

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

            m_OverlaySettingsQuery = GetEntityQuery(ComponentType.ReadOnly<OverlayConfigurationData>());
            m_FaceColorID = Shader.PropertyToID("_FaceColor");

            // Unity render-pipeline event.
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


        public bool TryGetMarkerTooltip(out string text, out float x, out float y)
        {
            text = m_MarkerTooltipText;
            x = m_MarkerTooltipX;
            y = m_MarkerTooltipY;

            return !string.IsNullOrEmpty(m_MarkerTooltipText);
        }
    }
}
