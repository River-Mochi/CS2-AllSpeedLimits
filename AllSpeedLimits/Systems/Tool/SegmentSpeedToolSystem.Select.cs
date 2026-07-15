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
    using Colossal.Entities;
    using Game.Common;
    using Game.Net;
    using Game.Tools;
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

        // Shortest connected same-category edge chain.
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

            // Use List + head index as a queue.
            while (head < m_PathFrontier.Count && m_PathVisited.Count < kMaxPathSearch)
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
                    if (m_PathVisited.Count >= kMaxPathSearch)
                    {
                        break;
                    }

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

            // Walk parents back from end to start.
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
