// <copyright file="SegmentSpeedToolSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolSystem.cs
// Purpose: In-game selection tool for road, rail, and waterway speed editing.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;   // List, HashSet
    using Colossal.Entities;            // EntityManager.Exists
    using Game;                         // GameSystemBase
    using Game.Common;                  // BatchesUpdated, Highlighted, Temp
    using Game.Input;                   // applyAction, cancelAction
    using Game.Net;                     // Edge, TrackLane
    using Game.Prefabs;                 // PrefabBase
    using Game.Tools;                   // ToolBaseSystem, ControlPoint
    using RoadRailSpeeds.Components;    // CustomSpeed
    using RoadRailSpeeds.Data;          // PersistentSpeedLimitStorage, SpeedLimitEntry
    using Unity.Entities;               // Entity
    using Unity.Jobs;                   // JobHandle
    using UnityEngine.Scripting;        // Preserve
    using CarLane = Game.Net.CarLane;   // CarLane
    using SubLane = Game.Net.SubLane;   // SubLane

    /// <summary>
    /// Custom tool for selecting road, rail, and waterway segments to adjust their speed limits.
    /// Click an individual segment or click-and-drag across multiple segments.
    /// Selection is finalized when mouse button is released.
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

        // Drag path-fill: m_PathStartEdge is the segment the player pressed on; m_PathEndEdge is the
        // segment currently under the cursor. While dragging we select the shortest connected chain
        // between them, so a long road/rail run can be picked by pressing the first segment and
        // releasing on the last one (no need to trace every segment in between). The collections are
        // reused across the drag to avoid per-move allocations during the breadth-first search.
        private const int kMaxPathSearch = 6000;
        private Entity m_PathStartEdge = Entity.Null;
        private Entity m_PathEndEdge = Entity.Null;
        private readonly HashSet<Entity> m_PathVisited = new();
        private readonly Dictionary<Entity, Entity> m_PathParent = new();
        // BFS frontier as a List with a head index instead of Queue<T>: the CS2 toolchain references
        // both System and mscorlib, which makes Queue<T> ambiguous (CS0433). List<T> resolves cleanly.
        private readonly List<Entity> m_PathFrontier = new();
        private readonly List<Entity> m_PathScratch = new();
        private readonly List<Entity> m_PathResult = new();

        private enum EdgeCategory
        {
            None,
            Road,
            Rail,
            Water
        }

        // Selection type filter. Default = everything (current behavior). When a type is off, it is
        // removed from the raycast layer mask, so the ray passes THROUGH it and hits whatever is
        // below (e.g. turn Rails off to select a road under an elevated rail without grabbing both).
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
        /// Toggle the tool on/off. Called by the React/COHTML UI button.
        /// </summary>
        public void ToggleTool(bool enable)
        {
            // Idempotent on purpose: enable=true means "ensure active", enable=false means
            // "ensure inactive". The old version toggled on every enable=true call, so if the
            // toolbar button's onSelect fired twice for a single click the tool activated and then
            // immediately deactivated, leaving no panel open. The toolbar button now sends the
            // explicit target state (!toolActive), so close still works on a second click while
            // a duplicate click event can no longer flip the tool back off.
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

            // Fresh start: re-enable all three selection-type filters every time the tool opens.
            // A leftover filter (e.g. Water-only from a previous session) would make the initial
            // "pick a segment" state silently ignore road and rail clicks, so the tool looks broken.
            // The matching UI chips are re-synced on activation in SegmentSpeedToolUISystem.
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

            // Detect network segments that can receive speed changes. The layer mask is built from
            // the player's type filter so excluded types are invisible to the ray.
            m_ToolRaycastSystem.typeMask = TypeMask.Net;

            Layer layerMask = Layer.None;
            if (IncludeRoads)
            {
                // Tram tracks share streets and a tram-road edge carries BOTH the Road and TramTrack
                // layers, so trams are grouped with Roads. Otherwise "Rails only" would still grab
                // tram-roads (they have the TramTrack bit), which is the leak you saw.
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

            // The default collision mask is OnGround | Overground, which can't hit buried subway
            // tracks. While underground mode is on, switch to Underground so the raycast selects the
            // tunnels; otherwise stay on the surface. This mirrors the vanilla tools exactly.
            m_ToolRaycastSystem.collisionMask = requireUnderground
                ? CollisionMask.Underground
                : (CollisionMask.OnGround | CollisionMask.Overground);
        }

        // Let the game show its built-in underground toggle in the tool options bar while RRS is
        // active (undergroundModeSupported = activeTool.allowUnderground). No custom RRS button needed.
        public override bool allowUnderground => true;

        // The underground toggle / underground view calls this on the active tool. We store it in
        // requireUnderground so the toggle state, the underground view, and the raycast stay in sync.
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

        protected override bool GetAllowApply()
        {
            if (GetRaycastResult(out var controlPoint))
            {
                if (controlPoint.m_OriginalEntity != Entity.Null &&
                    EntityManager.HasComponent<Edge>(controlPoint.m_OriginalEntity) &&
                    IsEdgeTypeAllowed(controlPoint.m_OriginalEntity))
                {
                    m_ToolSystem.selected = controlPoint.m_OriginalEntity;
                    return true;
                }
            }

            m_ToolSystem.selected = Entity.Null;
            return false;
        }

        // Authoritative selection-type filter, by component (the raycast layer mask leaks for
        // tram-roads, which carry BOTH Road and TramTrack). Grouping confirmed via Scene Explorer:
        //   Rails = TrainTrack + SubwayTrack
        //   Roads = Road + TramTrack (on-road tram updates AND dedicated tram tracks; trams run on streets)
        private bool IsEdgeTypeAllowed(Entity edge)
        {
            if (edge == Entity.Null || !EntityManager.Exists(edge))
            {
                return false;
            }

            if (EntityManager.HasComponent<Waterway>(edge))
            {
                return IncludeWater;
            }

            if (EntityManager.HasComponent<TrainTrack>(edge) ||
                EntityManager.HasComponent<SubwayTrack>(edge))
            {
                return IncludeRails;
            }

            if (EntityManager.HasComponent<Road>(edge) ||
                EntityManager.HasComponent<TramTrack>(edge))
            {
                return IncludeRoads;
            }

            return true;
        }

        [Preserve]
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            ControlPoint currentPoint;
            bool hasHit = GetRaycastResult(out currentPoint);

            // IMPORTANT: road/rail selection must stay on Highlighted.
            // Daniel's original path gives the clean continuous outer edge outline players expect.
            // Do not replace this with OverlayBuffer.DrawCurve; DrawCurve was tested and rejected
            // because it draws ugly overlapping caps at road/rail segment joins.
            // Waterways are the only custom overlay case; see SegmentSelectionOverlayRenderSystem.
            if (!m_IsDragging && m_SelectedEdges.Count == 0)
            {
                if (hasHit &&
                    currentPoint.m_OriginalEntity != Entity.Null &&
                    EntityManager.HasComponent<Edge>(currentPoint.m_OriginalEntity) &&
                    IsEdgeTypeAllowed(currentPoint.m_OriginalEntity))
                {
                    if (currentPoint.m_OriginalEntity != m_HoverEntity)
                    {
                        ClearHoverHighlight();
                        if (HighlightEdge(currentPoint.m_OriginalEntity))
                        {
                            m_HoverEntity = currentPoint.m_OriginalEntity;
                        }
                    }
                }
                else
                {
                    ClearHoverHighlight();
                }
            }

            if (applyAction.WasPressedThisFrame())
            {
                m_IsDragging = true;
                m_TempSelection.Clear();
                ClearHoverHighlight();
                RemoveAllHighlights();
                m_PathStartEdge = Entity.Null;
                m_PathEndEdge = Entity.Null;

                if (hasHit &&
                    currentPoint.m_OriginalEntity != Entity.Null &&
                    EntityManager.HasComponent<Edge>(currentPoint.m_OriginalEntity) &&
                    IsEdgeTypeAllowed(currentPoint.m_OriginalEntity))
                {
                    m_PathStartEdge = currentPoint.m_OriginalEntity;
                    m_PathEndEdge = m_PathStartEdge;
                    RebuildPathSelection();
                }
            }

            if (m_IsDragging && applyAction.IsPressed())
            {
                Entity hovered =
                    hasHit &&
                    currentPoint.m_OriginalEntity != Entity.Null &&
                    EntityManager.HasComponent<Edge>(currentPoint.m_OriginalEntity) &&
                    IsEdgeTypeAllowed(currentPoint.m_OriginalEntity)
                        ? currentPoint.m_OriginalEntity
                        : Entity.Null;

                if (hovered != Entity.Null)
                {
                    if (m_PathStartEdge == Entity.Null)
                    {
                        // Drag began off any selectable edge; adopt the first valid one as the start.
                        m_PathStartEdge = hovered;
                        m_PathEndEdge = hovered;
                        RebuildPathSelection();
                    }
                    else if (hovered != m_PathEndEdge)
                    {
                        // Cursor moved to a different segment: refill the connected path to it.
                        m_PathEndEdge = hovered;
                        RebuildPathSelection();
                    }
                }

                // If hovered == Entity.Null (cursor off the network) keep the current preview and the
                // remembered start, so moving back onto the road resumes filling from the same start.
            }

            if (m_IsDragging && applyAction.WasReleasedThisFrame())
            {
                m_IsDragging = false;
                FinalizeSelection();
            }

            if (cancelAction != null && cancelAction.WasPressedThisFrame())
            {
                m_ToolSystem.selected = Entity.Null;
                m_ToolSystem.activeTool = m_DefaultToolSystem;
                return inputDeps;
            }

            return inputDeps;
        }

        private void FinalizeSelection()
        {
            m_SelectedEdges.Clear();
            m_SelectedEdges.AddRange(m_TempSelection);

            if (m_SelectedEdges.Count > 0)
            {
                Entity aggregate = Entity.Null;

                if (EntityManager.HasComponent<Aggregated>(m_SelectedEdges[0]))
                {
                    aggregate = EntityManager.GetComponentData<Aggregated>(m_SelectedEdges[0]).m_Aggregate;
                }

                // Tell the UI which aggregate and exact segment list the tool selected.
                m_UISystem.OnSegmentsSelectedByTool(aggregate, m_SelectedEdges);
            }
            else
            {
                RemoveAllHighlights();
            }
        }

        // Rebuilds m_TempSelection as the connected chain from the pressed (start) edge to the edge
        // currently under the cursor (end), and re-highlights it. Called whenever the drag's end edge
        // changes, so the preview tracks the cursor.
        private void RebuildPathSelection()
        {
            RemoveAllHighlights();
            m_TempSelection.Clear();

            if (m_PathStartEdge == Entity.Null || !EntityManager.Exists(m_PathStartEdge))
            {
                return;
            }

            if (m_PathEndEdge == Entity.Null ||
                m_PathEndEdge == m_PathStartEdge ||
                !EntityManager.Exists(m_PathEndEdge))
            {
                m_TempSelection.Add(m_PathStartEdge);
                HighlightEdge(m_PathStartEdge);
                return;
            }

            List<Entity>? path = FindConnectedPath(m_PathStartEdge, m_PathEndEdge);
            if (path == null)
            {
                // Not connected within the search budget (or different net type): fall back to just
                // the two clicked segments so the action still does something predictable.
                m_TempSelection.Add(m_PathStartEdge);
                HighlightEdge(m_PathStartEdge);
                m_TempSelection.Add(m_PathEndEdge);
                HighlightEdge(m_PathEndEdge);
                return;
            }

            for (int i = 0; i < path.Count; i++)
            {
                m_TempSelection.Add(path[i]);
                HighlightEdge(path[i]);
            }
        }

        // Breadth-first search over the edge graph: edges meet at shared nodes, and each node lists
        // its touching edges in a ConnectedEdge buffer. Returns the shortest chain of same-category
        // edges from start to end, or null if they are not connected within kMaxPathSearch. Reuses
        // member collections to avoid per-move garbage while dragging.
        private List<Entity>? FindConnectedPath(Entity startEdge, Entity endEdge)
        {
            EdgeCategory category = GetEdgeCategory(startEdge);
            if (category == EdgeCategory.None || category != GetEdgeCategory(endEdge))
            {
                return null;
            }

            m_PathVisited.Clear();
            m_PathParent.Clear();
            m_PathFrontier.Clear();

            m_PathVisited.Add(startEdge);
            m_PathFrontier.Add(startEdge);

            bool found = false;
            int head = 0;

            // head walks the frontier like a queue; the cap bounds how many edges we expand.
            while (head < m_PathFrontier.Count && head < kMaxPathSearch)
            {
                Entity current = m_PathFrontier[head++];
                if (current == endEdge)
                {
                    found = true;
                    break;
                }

                CollectConnectedEdges(current);
                for (int i = 0; i < m_PathScratch.Count; i++)
                {
                    Entity neighbor = m_PathScratch[i];
                    if (m_PathVisited.Contains(neighbor) ||
                        !EntityManager.Exists(neighbor) ||
                        !EntityManager.HasComponent<Edge>(neighbor) ||
                        GetEdgeCategory(neighbor) != category ||
                        !IsEdgeTypeAllowed(neighbor))
                    {
                        continue;
                    }

                    m_PathVisited.Add(neighbor);
                    m_PathParent[neighbor] = current;
                    m_PathFrontier.Add(neighbor);
                }
            }

            if (!found && !m_PathVisited.Contains(endEdge))
            {
                return null;
            }

            // Walk parents back from end to start to recover the chain (order does not matter to the
            // selection, only the set of edges).
            m_PathResult.Clear();
            Entity step = endEdge;
            m_PathResult.Add(step);
            int safety = 0;
            while (step != startEdge && safety++ < kMaxPathSearch)
            {
                if (!m_PathParent.TryGetValue(step, out Entity previous))
                {
                    return null;
                }

                step = previous;
                m_PathResult.Add(step);
            }

            return m_PathResult;
        }

        private void CollectConnectedEdges(Entity edge)
        {
            m_PathScratch.Clear();
            if (!EntityManager.HasComponent<Edge>(edge))
            {
                return;
            }

            Edge edgeData = EntityManager.GetComponentData<Edge>(edge);
            AddNodeConnectedEdges(edgeData.m_Start, edge);
            AddNodeConnectedEdges(edgeData.m_End, edge);
        }

        private void AddNodeConnectedEdges(Entity node, Entity selfEdge)
        {
            if (node == Entity.Null || !EntityManager.HasBuffer<ConnectedEdge>(node))
            {
                return;
            }

            DynamicBuffer<ConnectedEdge> connected = EntityManager.GetBuffer<ConnectedEdge>(node);
            for (int i = 0; i < connected.Length; i++)
            {
                Entity other = connected[i].m_Edge;
                if (other != selfEdge && other != Entity.Null)
                {
                    m_PathScratch.Add(other);
                }
            }
        }

        // Same grouping as IsEdgeTypeAllowed so the path stays within one network type (a multimodal
        // node never bridges, say, a road run into a rail run).
        private EdgeCategory GetEdgeCategory(Entity edge)
        {
            if (edge == Entity.Null || !EntityManager.Exists(edge))
            {
                return EdgeCategory.None;
            }

            if (EntityManager.HasComponent<Waterway>(edge))
            {
                return EdgeCategory.Water;
            }

            if (EntityManager.HasComponent<TrainTrack>(edge) ||
                EntityManager.HasComponent<SubwayTrack>(edge))
            {
                return EdgeCategory.Rail;
            }

            if (EntityManager.HasComponent<Road>(edge) ||
                EntityManager.HasComponent<TramTrack>(edge))
            {
                return EdgeCategory.Road;
            }

            return EdgeCategory.None;
        }

        public void ClearSelection()
        {
            RemoveAllHighlights();
            m_ToolSystem.selected = Entity.Null;
            m_SelectedEdges.Clear();
            m_TempSelection.Clear();
            m_IsDragging = false;
            m_HoverEntity = Entity.Null;
            m_PathStartEdge = Entity.Null;
            m_PathEndEdge = Entity.Null;
        }

        private void ClearHoverHighlight()
        {
            if (m_HoverEntity == Entity.Null || !EntityManager.Exists(m_HoverEntity))
            {
                m_HoverEntity = Entity.Null;
                return;
            }

            if (m_HighlightedByThisTool.Contains(m_HoverEntity) &&
                EntityManager.HasComponent<Highlighted>(m_HoverEntity))
            {
                EntityManager.RemoveComponent<Highlighted>(m_HoverEntity);
                AddBatchesUpdated(m_HoverEntity);
            }

            m_HighlightedByThisTool.Remove(m_HoverEntity);
            m_HoverEntity = Entity.Null;
        }

        private bool HighlightEdge(Entity edge)
        {
            if (edge == Entity.Null || !EntityManager.Exists(edge))
            {
                return false;
            }

            // Waterways use SegmentSelectionOverlayRenderSystem instead. Do not tag them
            // Highlighted because that path is road/rail-oriented; water has prefab geometry that
            // doesn't produce a clean Highlighted outline, so it stays on the overlay.
            if (EntityManager.HasComponent<Waterway>(edge))
            {
                return false;
            }

            // Road/rail selection uses the game's Highlighted component, exactly like the vanilla
            // bulldoze/road tools. The game renders it from the real road mesh silhouette, so it
            // covers segments, nodes and intersections with no gaps. The outline color comes from
            // RenderingSettingsData.m_HoveredColor (vanilla cyan); if HoverColors is installed it
            // owns that color. We only tag/untag edges this tool added, tracked below.
            if (EntityManager.HasComponent<Highlighted>(edge))
            {
                // Already highlighted (e.g. by the game under the cursor): claim it for cleanup so
                // our tag is removed cleanly when the tool closes.
                m_HighlightedByThisTool.Add(edge);
                return true;
            }
            else
            {
                EntityManager.AddComponent<Highlighted>(edge);
                m_HighlightedByThisTool.Add(edge);
            }

            AddBatchesUpdated(edge);
            return true;
        }

        private void AddBatchesUpdated(Entity entity)
        {
            if (!EntityManager.HasComponent<BatchesUpdated>(entity))
            {
                EntityManager.AddComponent<BatchesUpdated>(entity);
            }
        }

        private void RemoveAllHighlights()
        {
            // Only remove Highlighted tags that RRS added. Daniel's original global
            // query was fine for a single-purpose road tool, but it is too broad now:
            // other tools/mods may own their own Highlighted state.
            foreach (Entity entity in m_HighlightedByThisTool)
            {
                if (!EntityManager.Exists(entity))
                {
                    continue;
                }

                if (EntityManager.HasComponent<Highlighted>(entity))
                {
                    EntityManager.RemoveComponent<Highlighted>(entity);
                    AddBatchesUpdated(entity);
                }
            }

            m_HighlightedByThisTool.Clear();
        }

        public void ApplySpeedToSelection(float speedKmh)
        {
            if (m_SelectedEdges.Count == 0)
            {
                return;
            }

            // Game lane speed uses 2x m/s, so km/h converts by dividing by 1.8.
            float speedGameUnits = speedKmh / 1.8f;

            foreach (Entity edge in m_SelectedEdges)
            {
                Entity targetEdge = edge;

                if (EntityManager.HasComponent<Temp>(edge))
                {
                    var temp = EntityManager.GetComponentData<Temp>(edge);
                    targetEdge = temp.m_Original;
                }

                SpeedLimitEntry? existingEntry = PersistentSpeedLimitStorage.GetSpeedLimit(targetEdge.Index);
                float originalSpeed = 0f;

                if (existingEntry == null)
                {
                    if (EntityManager.HasBuffer<SubLane>(edge))
                    {
                        DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);
                        CarLaneFlags ignore = CarLaneFlags.Unsafe | CarLaneFlags.SideConnection;

                        float totalSpeed = 0f;
                        int count = 0;

                        foreach (SubLane subLane in subLanes)
                        {
                            if (EntityManager.HasComponent<CarLane>(subLane.m_SubLane))
                            {
                                CarLane carLane = EntityManager.GetComponentData<CarLane>(subLane.m_SubLane);

                                if ((carLane.m_Flags & ignore) != 0)
                                {
                                    continue;
                                }

                                totalSpeed += carLane.m_SpeedLimit * 1.8f;
                                count++;
                            }
                            else if (EntityManager.HasComponent<Game.Net.TrackLane>(subLane.m_SubLane))
                            {
                                var trackLane = EntityManager.GetComponentData<Game.Net.TrackLane>(subLane.m_SubLane);

                                totalSpeed += trackLane.m_SpeedLimit * 1.8f;
                                count++;
                            }
                        }

                        if (count > 0)
                        {
                            originalSpeed = totalSpeed / count;
                        }
                    }
                }
                else
                {
                    originalSpeed = existingEntry.DefaultSpeedKmh;
                }

                // Store original and custom speeds so reset can restore the original value.
                PersistentSpeedLimitStorage.StoreSpeedLimit(targetEdge.Index, originalSpeed, speedKmh);

                if (!EntityManager.HasComponent<CustomSpeed>(targetEdge))
                {
                    EntityManager.AddComponent<CustomSpeed>(targetEdge);
                }

                EntityManager.SetComponentData(targetEdge, new CustomSpeed(speedKmh));

                SetCarLaneSpeedsImmediate(edge, speedGameUnits);
            }
        }

        private void SetCarLaneSpeedsImmediate(Entity edge, float speedGameUnits)
        {
            if (!EntityManager.HasBuffer<SubLane>(edge))
            {
                return;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);

            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity laneEntity = subLanes[i].m_SubLane;

                if (EntityManager.HasComponent<CarLane>(laneEntity))
                {
                    CarLane carLane = EntityManager.GetComponentData<CarLane>(laneEntity);

                    // Preserve flags so speed changes do not break lane connectivity.
                    CarLaneFlags originalFlags = carLane.m_Flags;

                    carLane.m_SpeedLimit = speedGameUnits;
                    carLane.m_Flags = originalFlags;

                    EntityManager.SetComponentData(laneEntity, carLane);
                }
                else if (EntityManager.HasComponent<Game.Net.TrackLane>(laneEntity))
                {
                    var trackLane = EntityManager.GetComponentData<Game.Net.TrackLane>(laneEntity);

                    trackLane.m_SpeedLimit = speedGameUnits;

                    EntityManager.SetComponentData(laneEntity, trackLane);
                }
            }
        }

        public void ResetSpeedToOriginal()
        {
            if (m_SelectedEdges.Count == 0)
            {
                return;
            }

            foreach (Entity edge in m_SelectedEdges)
            {
                Entity targetEdge = edge;

                if (EntityManager.HasComponent<Temp>(edge))
                {
                    var temp = EntityManager.GetComponentData<Temp>(edge);
                    targetEdge = temp.m_Original;
                }

                float? originalSpeed = PersistentSpeedLimitStorage.GetDefaultSpeedLimit(targetEdge.Index);

                if (!originalSpeed.HasValue)
                {
                    continue;
                }

                float speedGameUnits = originalSpeed.Value / 1.8f;

                if (EntityManager.HasBuffer<SubLane>(edge))
                {
                    DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);

                    foreach (SubLane subLane in subLanes)
                    {
                        if (EntityManager.HasComponent<CarLane>(subLane.m_SubLane))
                        {
                            CarLane carLane = EntityManager.GetComponentData<CarLane>(subLane.m_SubLane);

                            carLane.m_SpeedLimit = speedGameUnits;

                            EntityManager.SetComponentData(subLane.m_SubLane, carLane);
                        }
                        else if (EntityManager.HasComponent<Game.Net.TrackLane>(subLane.m_SubLane))
                        {
                            var trackLane = EntityManager.GetComponentData<Game.Net.TrackLane>(subLane.m_SubLane);

                            trackLane.m_SpeedLimit = speedGameUnits;

                            EntityManager.SetComponentData(subLane.m_SubLane, trackLane);
                        }
                    }
                }

                if (EntityManager.HasComponent<CustomSpeed>(targetEdge))
                {
                    EntityManager.RemoveComponent<CustomSpeed>(targetEdge);
                }

                PersistentSpeedLimitStorage.RemoveSpeedLimit(targetEdge.Index);
            }
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
