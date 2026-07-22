// <copyright file="SegmentSpeedToolSystem.Select.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolSystem.Select.cs
// Purpose: Segment raycast, drag selection, path-fill, and highlights.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;
    using Colossal.Collections;
    using Colossal.Entities;
    using Game.Common;
    using Game.Net;
    using Game.Prefabs;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using UnityEngine.Scripting;

    public partial class SegmentSpeedToolSystem
    {
        private enum EdgeCategory
        {
            None,
            Road,
            Rail,
            Water
        }

        private struct PathCandidate : ILessThan<PathCandidate>
        {
            public Entity m_Node;
            public Entity m_Edge;
            public float m_Cost;

            public bool LessThan(PathCandidate other)
            {
                return m_Cost < other.m_Cost;
            }
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

        // Component-level filter; raycast layers can overlap on tram roads.
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

            // Road/rail use Highlighted; waterways use the custom overlay system.
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

                // Keep preview if the cursor leaves the network mid-drag.
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

                // Send the selected segment list to the UI.
                m_UISystem.OnSegmentsSelectedByTool(aggregate, m_SelectedEdges);
            }
            else
            {
                RemoveAllHighlights();
            }
        }

        // Rebuild drag preview from start edge to current end edge.
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
                // If no path is found, keep the two endpoints.
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

        // Lowest-cost connected same-category edge chain.
        private List<Entity>? FindConnectedPath(Entity startEdge, Entity endEdge)
        {
            EdgeCategory category = GetEdgeCategory(startEdge);
            if (category == EdgeCategory.None || category != GetEdgeCategory(endEdge))
            {
                return null;
            }

            // CS2 stores each continuous network in order. Prefer that bounded list so dragging
            // along a long road does not search every side road and exhaust the city-search cap.
            if (TryFindAggregatePath(startEdge, endEdge, category))
            {
                return m_PathResult;
            }

            Edge endData = EntityManager.GetComponentData<Edge>(endEdge);
            Entity startPrefab = EntityManager.HasComponent<PrefabRef>(startEdge)
                ? EntityManager.GetComponentData<PrefabRef>(startEdge).m_Prefab
                : Entity.Null;
            NativeMinHeap<PathCandidate> frontier =
                new NativeMinHeap<PathCandidate>(64, Allocator.Temp);

            m_PathVisited.Clear();
            m_PathArrivalEdge.Clear();

            frontier.Insert(new PathCandidate
            {
                m_Node = endData.m_Start,
                m_Edge = endEdge,
                m_Cost = 0f
            });

            if (endData.m_End != endData.m_Start)
            {
                frontier.Insert(new PathCandidate
                {
                    m_Node = endData.m_End,
                    m_Edge = endEdge,
                    m_Cost = 0f
                });
            }

            try
            {
                Entity foundNode = Entity.Null;
                while (frontier.Length > 0 && m_PathVisited.Count < kMaxPathSearch)
                {
                    PathCandidate current = frontier.Extract();

                    // Match the vanilla bulldozer path search: reaching the start edge is the
                    // goal, even if its opposite node was already examined by another route.
                    if (current.m_Edge == startEdge)
                    {
                        m_PathArrivalEdge[current.m_Node] = startEdge;
                        foundNode = current.m_Node;
                        break;
                    }

                    if (!m_PathVisited.Add(current.m_Node))
                    {
                        continue;
                    }

                    m_PathArrivalEdge[current.m_Node] = current.m_Edge;

                    if (!EntityManager.HasBuffer<ConnectedEdge>(current.m_Node))
                    {
                        continue;
                    }

                    DynamicBuffer<ConnectedEdge> connected =
                        EntityManager.GetBuffer<ConnectedEdge>(current.m_Node);
                    Entity currentPrefab = Entity.Null;
                    if (EntityManager.HasComponent<PrefabRef>(current.m_Edge))
                    {
                        currentPrefab =
                            EntityManager.GetComponentData<PrefabRef>(current.m_Edge).m_Prefab;
                    }

                    for (int i = 0; i < connected.Length; i++)
                    {
                        Entity candidateEdge = connected[i].m_Edge;
                        if (candidateEdge == current.m_Edge ||
                            candidateEdge == Entity.Null ||
                            !EntityManager.Exists(candidateEdge) ||
                            !EntityManager.HasComponent<Edge>(candidateEdge) ||
                            GetEdgeCategory(candidateEdge) != category ||
                            !IsEdgeTypeAllowed(candidateEdge))
                        {
                            continue;
                        }

                        Edge candidateData = EntityManager.GetComponentData<Edge>(candidateEdge);
                        Entity neighbor;
                        if (candidateData.m_Start == current.m_Node)
                        {
                            neighbor = candidateData.m_End;
                        }
                        else if (candidateData.m_End == current.m_Node)
                        {
                            neighbor = candidateData.m_Start;
                        }
                        else
                        {
                            continue;
                        }

                        if (neighbor == Entity.Null ||
                            (m_PathVisited.Contains(neighbor) && candidateEdge != startEdge))
                        {
                            continue;
                        }

                        float cost = current.m_Cost;
                        if (EntityManager.HasComponent<Curve>(candidateEdge))
                        {
                            cost += EntityManager.GetComponentData<Curve>(candidateEdge).m_Length;
                        }

                        if (connected.Length > 2)
                        {
                            cost += kJunctionPenalty;
                        }

                        if (startPrefab != Entity.Null &&
                            currentPrefab != Entity.Null &&
                            startPrefab != currentPrefab)
                        {
                            cost += kPrefabChangePenalty;
                        }

                        frontier.Insert(new PathCandidate
                        {
                            m_Node = neighbor,
                            m_Edge = candidateEdge,
                            m_Cost = cost
                        });
                    }
                }

                if (foundNode == Entity.Null)
                {
                    return null;
                }

                // Each settled node stores the edge used to reach it, so walking that edge to
                // its opposite node reconstructs the same ordered chain as the vanilla tool.
                m_PathResult.Clear();
                Entity step = foundNode;
                int safety = 0;
                while (m_PathArrivalEdge.TryGetValue(step, out Entity pathEdge) &&
                       pathEdge != Entity.Null &&
                       safety++ < kMaxPathSearch)
                {
                    m_PathResult.Add(pathEdge);
                    if (pathEdge == endEdge)
                    {
                        return m_PathResult;
                    }

                    Edge pathData = EntityManager.GetComponentData<Edge>(pathEdge);
                    step = pathData.m_End == step ? pathData.m_Start : pathData.m_End;
                }
            }
            finally
            {
                frontier.Dispose();
            }

            return null;
        }

        private bool TryFindAggregatePath(
            Entity startEdge,
            Entity endEdge,
            EdgeCategory category)
        {
            if (!EntityManager.HasComponent<Aggregated>(startEdge) ||
                !EntityManager.HasComponent<Aggregated>(endEdge))
            {
                return false;
            }

            Entity startAggregate =
                EntityManager.GetComponentData<Aggregated>(startEdge).m_Aggregate;
            Entity endAggregate =
                EntityManager.GetComponentData<Aggregated>(endEdge).m_Aggregate;

            if (startAggregate == Entity.Null ||
                startAggregate != endAggregate ||
                !EntityManager.Exists(startAggregate) ||
                !EntityManager.HasBuffer<AggregateElement>(startAggregate))
            {
                return false;
            }

            DynamicBuffer<AggregateElement> elements =
                EntityManager.GetBuffer<AggregateElement>(startAggregate);
            int startIndex = -1;
            int endIndex = -1;

            for (int i = 0; i < elements.Length; i++)
            {
                Entity edge = elements[i].m_Edge;
                if (edge == startEdge)
                {
                    startIndex = i;
                }

                if (edge == endEdge)
                {
                    endIndex = i;
                }

                if (startIndex >= 0 && endIndex >= 0)
                {
                    break;
                }
            }

            if (startIndex < 0 || endIndex < 0)
            {
                return false;
            }

            m_PathResult.Clear();
            int direction = startIndex <= endIndex ? 1 : -1;
            for (int i = startIndex; ; i += direction)
            {
                Entity edge = elements[i].m_Edge;
                if (edge == Entity.Null ||
                    !EntityManager.Exists(edge) ||
                    GetEdgeCategory(edge) != category ||
                    !IsEdgeTypeAllowed(edge))
                {
                    m_PathResult.Clear();
                    return false;
                }

                m_PathResult.Add(edge);
                if (i == endIndex)
                {
                    return true;
                }
            }
        }

        // Keep path search within one network type.
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

            // Waterways use SegmentSelectionOverlayRenderSystem.
            if (EntityManager.HasComponent<Waterway>(edge))
            {
                return false;
            }

            // Road/rail use the game's Highlighted component.
            if (EntityManager.HasComponent<Highlighted>(edge))
            {
                // Claim existing highlight for cleanup.
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
            // Only remove Highlighted tags this tool owns.
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
    }
}
