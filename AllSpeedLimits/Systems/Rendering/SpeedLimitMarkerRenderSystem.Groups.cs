// <copyright file="SpeedLimitMarkerRenderSystem.Groups.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Rendering/SpeedLimitMarkerRenderSystem.Groups.cs
// Purpose: Marker grouping, connected-segment representative selection, and duplicate suppression.

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

    public partial class SpeedLimitMarkerRenderSystem
    {
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

        private void BuildVisibleMarkerGroups(NativeArray<Entity> entities, int markerGroupStride)
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
                AddVisibleMarkerGroupRepresentatives(markerGroupStride);
            }
        }

        private static int GetMarkerGroupStride(float normalizedZoom)
        {
            if (normalizedZoom >= 0.82f)
            {
                return int.MaxValue;
            }

            if (normalizedZoom >= 0.68f)
            {
                return 10;
            }

            if (normalizedZoom >= 0.56f)
            {
                return 7;
            }

            if (normalizedZoom >= 0.48f)
            {
                return 4;
            }

            return 2;
        }

        private void AddVisibleMarkerGroupRepresentatives(int markerGroupStride)
        {
            if (m_FrameMarkerGroup.Count == 0)
            {
                return;
            }

            if (markerGroupStride >= m_FrameMarkerGroup.Count)
            {
                Entity representative = ChooseMarkerGroupRepresentative(0, m_FrameMarkerGroup.Count);
                if (representative != Entity.Null)
                {
                    m_FrameVisibleMarkerEdges.Add(representative);
                }

                return;
            }

            for (int startIndex = 0; startIndex < m_FrameMarkerGroup.Count; startIndex += markerGroupStride)
            {
                int endIndex = Math.Min(startIndex + markerGroupStride, m_FrameMarkerGroup.Count);
                Entity representative = ChooseMarkerGroupRepresentative(startIndex, endIndex);
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

        private Entity ChooseMarkerGroupRepresentative(int startIndex, int endIndex)
        {
            if (startIndex >= endIndex)
            {
                return Entity.Null;
            }

            if (endIndex - startIndex == 1)
            {
                return m_FrameMarkerGroup[startIndex];
            }

            float3 center = default;
            int validCount = 0;
            for (int i = startIndex; i < endIndex; i++)
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
                return m_FrameMarkerGroup[startIndex];
            }

            center /= (float)validCount;
            Entity bestEdge = m_FrameMarkerGroup[startIndex];
            float bestDistanceSq = float.MaxValue;
            for (int i = startIndex; i < endIndex; i++)
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
    }
}
