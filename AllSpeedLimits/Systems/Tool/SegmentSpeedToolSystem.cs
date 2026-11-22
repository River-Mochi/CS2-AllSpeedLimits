// <copyright file="SegmentSpeedToolSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolSystem.cs
// Purpose: Tool lifecycle and shared state for segment speed editing.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;
    using Game.Common;
    using Game.Net;
    using Game.Prefabs;
    using Game.Rendering;
    using Game.Tools;
    using Unity.Entities;
    using UnityEngine.Scripting;

    /// <summary>
    /// Selects road, rail, and waterway segments for speed editing.
    /// </summary>
    public partial class SegmentSpeedToolSystem : ToolBaseSystem
    {
        public const string kToolID = "SpeedLimitTool";

        private SegmentSpeedToolUISystem m_UISystem = null!;
        private RenderingSystem m_RenderingSystem = null!;
        private bool m_EnabledNativeMarkers;
        private readonly HashSet<Entity> m_RecentlyEditedEdges = new();
        private float m_RecentlyEditedUntil;

        private readonly List<Entity> m_SelectedEdges = new();
        private readonly HashSet<Entity> m_TempSelection = new();
        private readonly HashSet<Entity> m_HighlightedByThisTool = new();

        private bool m_IsDragging;
        private Entity m_HoverEntity = Entity.Null;

        // Drag path-fill state and reusable weighted-search buffers. Bound total visited nodes
        // so a drag preview cannot scan an entire city network on the game thread.
        private const int kMaxPathSearch = 1200;
        private const float kPrefabChangePenalty = 9.9f;
        private const float kJunctionPenalty = 10f;
        private Entity m_PathStartEdge = Entity.Null;
        private Entity m_PathEndEdge = Entity.Null;
        private readonly HashSet<Entity> m_PathVisited = new();
        private readonly Dictionary<Entity, Entity> m_PathArrivalEdge = new();
        private readonly List<Entity> m_PathResult = new();

        // Selection type filter.
        public bool IncludeRoads { get; set; } = true;
        public bool IncludePaths { get; set; } = true;
        public bool IncludeRails { get; set; } = true;
        public bool IncludeWater { get; set; } = true;
        public bool ShowInvisible { get; private set; }

        // Reveal the game's dashed markers. Preserve visibility already enabled by other tools.
        public void SetShowInvisible(bool enabled)
        {
            if (ShowInvisible == enabled)
            {
                return;
            }

            if (enabled)
            {
                m_EnabledNativeMarkers = !m_RenderingSystem.markersVisible;
                if (m_EnabledNativeMarkers)
                {
                    m_RenderingSystem.markersVisible = true;
                }
            }
            else
            {
                if (m_EnabledNativeMarkers && m_RenderingSystem.markersVisible)
                {
                    m_RenderingSystem.markersVisible = false;
                }

                m_EnabledNativeMarkers = false;
            }

            ShowInvisible = enabled;
        }

        public void MarkSelectionEdited()
        {
            m_RecentlyEditedEdges.Clear();
            foreach (Entity edge in m_SelectedEdges)
            {
                m_RecentlyEditedEdges.Add(edge);
            }

            m_RecentlyEditedUntil = UnityEngine.Time.unscaledTime + 1.5f;
        }

        public bool IsRecentlyEdited(Entity edge)
        {
            return UnityEngine.Time.unscaledTime < m_RecentlyEditedUntil &&
                m_RecentlyEditedEdges.Contains(edge);
        }

        public override string toolID => kToolID;

        public bool IsActive => m_ToolSystem?.activeTool == this;

        public Entity HoverEntity => m_HoverEntity;

        public IReadOnlyList<Entity> SelectedEdges => m_SelectedEdges;

        public IReadOnlyCollection<Entity> PreviewEdges => m_TempSelection;

        public bool IsDraggingSelection => m_IsDragging;

        /// <summary>
        /// Enables or disables the tool.
        /// </summary>
        public void ToggleTool(bool enable)
        {
            // Idempotent: enable means ensure active, disable means ensure inactive.
            if (enable)
            {
                if (m_ToolSystem.activeTool != this)
                {
                    m_ToolSystem.selected = Entity.Null;
                    m_ToolSystem.activeTool = this;
                }

                return;
            }

            if (m_ToolSystem.activeTool == this)
            {
                ClearSelection();
                m_ToolSystem.activeTool = m_DefaultToolSystem;
            }
        }

        [Preserve]
        protected override void OnCreate()
        {
            // ToolBaseSystem tools start disabled and are activated by the game's tool system.
            Enabled = false;

            m_UISystem = World.GetOrCreateSystemManaged<SegmentSpeedToolUISystem>();

            // base.OnCreate initializes tool input actions such as applyAction.
            base.OnCreate();
            m_RenderingSystem = World.GetOrCreateSystemManaged<RenderingSystem>();
        }

        [Preserve]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            m_SelectedEdges.Clear();
            m_TempSelection.Clear();
            m_HighlightedByThisTool.Clear();
            m_RecentlyEditedEdges.Clear();
            m_RecentlyEditedUntil = 0f;
            m_IsDragging = false;
            m_PathStartEdge = Entity.Null;
            m_PathEndEdge = Entity.Null;

            requireNet =
                Layer.Road |
                Layer.Pathway |
                Layer.PublicTransportRoad |
                Layer.TrainTrack |
                Layer.TramTrack |
                Layer.SubwayTrack |
                Layer.Waterway;
            requireNetArrows = false;

            // Always open on the surface; the player toggles underground mode when they want it.
            requireUnderground = false;

            // Reset filters on each activation.
            IncludeRoads = true;
            IncludePaths = true;
            IncludeRails = true;
            IncludeWater = true;
            SetShowInvisible(false);

            // Enables left-click apply behavior while this tool is active.
            applyAction.shouldBeEnabled = true;
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();

            applyAction.shouldBeEnabled = false;
            m_ToolSystem.selected = Entity.Null;
            RemoveAllHighlights();

            m_SelectedEdges.Clear();
            m_TempSelection.Clear();
            m_RecentlyEditedEdges.Clear();
            m_RecentlyEditedUntil = 0f;
            m_IsDragging = false;
            m_HoverEntity = Entity.Null;
            m_PathStartEdge = Entity.Null;
            m_PathEndEdge = Entity.Null;

            requireNet = Layer.None;
            requireNetArrows = false;
            SetShowInvisible(false);
        }

        public override void InitializeRaycast()
        {
            base.InitializeRaycast();

            // Raycast only selectable network layers.
            m_ToolRaycastSystem.typeMask = TypeMask.Net;

            Layer layerMask = Layer.None;
            if (IncludeRoads)
            {
                // Tram roads share the Road layer, so group trams with roads.
                layerMask |= Layer.Road | Layer.TramTrack | Layer.PublicTransportRoad;
            }

            // Pathway prefabs can carry bikes or motor vehicles. The entity filter below
            // separates these after raycasting, including internal building subnets.
            if (IncludeRoads || IncludePaths)
            {
                layerMask |= Layer.Pathway;
                if (ShowInvisible)
                {
                    layerMask |= Layer.MarkerPathway;
                }
            }

            if (IncludeRails)
            {
                layerMask |= Layer.TrainTrack | Layer.SubwayTrack;
            }

            if (IncludeWater)
            {
                layerMask |= Layer.Waterway;
            }

            m_ToolRaycastSystem.netLayerMask = layerMask;

            m_ToolRaycastSystem.raycastFlags =
                RaycastFlags.Cargo |
                RaycastFlags.Passenger |
                RaycastFlags.ElevateOffset |
                RaycastFlags.SubElements;

            // Hidden marker networks must not intercept clicks on visible paths when the
            // player has not enabled them. Vanilla's net tool gates this flag the same way.
            if (ShowInvisible)
            {
                m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Markers;
            }

            // Match vanilla surface/underground raycast behavior.
            m_ToolRaycastSystem.collisionMask = requireUnderground
                ? CollisionMask.Underground
                : (CollisionMask.OnGround | CollisionMask.Overground);
        }

        // Use the game's built-in underground toggle.
        public override bool allowUnderground => true;

        // Keep the underground toggle and raycast in sync.
        public override void SetUnderground(bool underground)
        {
            requireUnderground = underground;
        }

        public override PrefabBase GetPrefab()
        {
            // This tool edits existing segments; it does not place a prefab.
            return null!;
        }

        public override bool TrySetPrefab(PrefabBase prefab)
        {
            return false;
        }


        public void DeactivateTool()
        {
            if (IsActive)
            {
                m_ToolSystem.activeTool = m_DefaultToolSystem;
                ClearSelection();
            }
        }
    }
}
