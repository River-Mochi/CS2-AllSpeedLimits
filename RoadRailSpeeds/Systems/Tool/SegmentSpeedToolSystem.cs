// <copyright file="SegmentSpeedToolSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolSystem.cs
// Purpose: Tool lifecycle and shared state for segment speed editing.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;
    using Game.Net;
    using Game.Prefabs;
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

        private readonly List<Entity> m_SelectedEdges = new();
        private readonly HashSet<Entity> m_TempSelection = new();
        private readonly HashSet<Entity> m_HighlightedByThisTool = new();

        private bool m_IsDragging;
        private Entity m_HoverEntity = Entity.Null;

        // Drag path-fill state and reusable BFS buffers.
        private const int kMaxPathSearch = 6000;
        private Entity m_PathStartEdge = Entity.Null;
        private Entity m_PathEndEdge = Entity.Null;
        private readonly HashSet<Entity> m_PathVisited = new();
        private readonly Dictionary<Entity, Entity> m_PathParent = new();
        // List + head index avoids Queue<T> ambiguity in the CS2 toolchain.
        private readonly List<Entity> m_PathFrontier = new();
        private readonly List<Entity> m_PathScratch = new();
        private readonly List<Entity> m_PathResult = new();

        // Selection type filter.
        public bool IncludeRoads { get; set; } = true;
        public bool IncludeRails { get; set; } = true;
        public bool IncludeWater { get; set; } = true;

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
        }

        [Preserve]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            m_SelectedEdges.Clear();
            m_TempSelection.Clear();
            m_HighlightedByThisTool.Clear();
            m_IsDragging = false;
            m_PathStartEdge = Entity.Null;
            m_PathEndEdge = Entity.Null;

            requireNet =
                Layer.Road |
                Layer.TrainTrack |
                Layer.TramTrack |
                Layer.SubwayTrack |
                Layer.Waterway;
            requireNetArrows = false;

            // Always open on the surface; the player toggles underground mode when they want it.
            requireUnderground = false;

            // Reset filters on each activation.
            IncludeRoads = true;
            IncludeRails = true;
            IncludeWater = true;

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
            m_IsDragging = false;
            m_HoverEntity = Entity.Null;
            m_PathStartEdge = Entity.Null;
            m_PathEndEdge = Entity.Null;

            requireNet = Layer.None;
            requireNetArrows = false;
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
                layerMask |= Layer.Road | Layer.TramTrack;
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
                RaycastFlags.SubElements |
                RaycastFlags.Markers;

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
