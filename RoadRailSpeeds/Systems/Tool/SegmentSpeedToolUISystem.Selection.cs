// <copyright file="SegmentSpeedToolUISystem.Selection.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolUISystem.Selection.cs
// Purpose: Selection speed reads, apply/reset logic, and lane-speed writes for the tool panel.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;      // IReadOnlyList
    using System.Linq;                    // Average
    using Game.Net;                      // Edge, Lane data
    using RoadRailSpeeds.Components;      // CustomSpeed
    using RoadRailSpeeds.Data;            // SpeedLimitDataManager
    using Unity.Entities;                 // Entity
    using Unity.Mathematics;              // math
    using CarLane = Game.Net.CarLane;     // car lane speed
    using CarLaneFlags = Game.Net.CarLaneFlags; // lane flags
    using PrefabBase = Game.Prefabs.PrefabBase; // prefab base
    using PrefabRef = Game.Prefabs.PrefabRef; // edge prefab ref
    using PrefabSystem = Game.Prefabs.PrefabSystem; // prefab lookup
    using RoadPrefab = Game.Prefabs.RoadPrefab; // road speed prefab
    using SubLane = Game.Net.SubLane;     // edge sublanes
    using Temp = Game.Tools.Temp;         // temp selection
    using TrackLane = Game.Net.TrackLane; // track lane speed
    using TrackPrefab = Game.Prefabs.TrackPrefab; // rail speed prefab
    using Waterway = Game.Net.Waterway;   // waterway type checks
    using WaterwayPrefab = Game.Prefabs.WaterwayPrefab; // waterway speed prefab

    public partial class SegmentSpeedToolUISystem
    {
        private const float kMinSpeedKmh = 5f;
        private const float kMaxRoadSpeedKmh = 400f;
        private const float kMaxTrackSpeedKmh = 400f;
        private const float kMaxWaterwaySpeedKmh = 240f;
        private const float kSpeedComparisonTolerance = 0.5f;

        private struct SelectionSpeedInfo
        {
            public bool HasCurrentSpeed;
            public float CurrentSpeed;
            public bool CurrentSpeedMixed;
            public bool HasVanillaSpeed;
            public float VanillaSpeed;
            public bool VanillaSpeedMixed;
            public bool ContainsTrackType;
            public bool ContainsWaterwayType;
        }

        private SelectionSpeedInfo GetSelectionSpeedInfo(IReadOnlyList<Entity> edges)
        {
            SelectionSpeedInfo info = new SelectionSpeedInfo
            {
                CurrentSpeed = -1f,
                VanillaSpeed = -1f
            };

            foreach (Entity edge in edges)
            {
                Entity baseEdge = GetBaseEdge(edge);

                float currentSpeed = GetStreetSpeed(baseEdge);
                AddSpeedSample(
                    currentSpeed,
                    ref info.HasCurrentSpeed,
                    ref info.CurrentSpeed,
                    ref info.CurrentSpeedMixed);

                float vanillaSpeed = GetVanillaSpeed(baseEdge);
                AddSpeedSample(
                    vanillaSpeed,
                    ref info.HasVanillaSpeed,
                    ref info.VanillaSpeed,
                    ref info.VanillaSpeedMixed);

                if (IsTrackType(baseEdge))
                {
                    info.ContainsTrackType = true;
                }

                if (IsWaterwayType(baseEdge))
                {
                    info.ContainsWaterwayType = true;
                }
            }

            return info;
        }

        private static void AddSpeedSample(
            float speed,
            ref bool hasSpeed,
            ref float firstSpeed,
            ref bool mixed)
        {
            if (speed <= 0f)
            {
                return;
            }

            if (!hasSpeed)
            {
                hasSpeed = true;
                firstSpeed = speed;
                return;
            }

            if (math.abs(firstSpeed - speed) > kSpeedComparisonTolerance)
            {
                mixed = true;
            }
        }

        private float GetStreetSpeed(Entity edge)
        {
            Entity baseEdge = GetBaseEdge(edge);

            if (EntityManager.HasComponent<CustomSpeed>(baseEdge))
            {
                return EntityManager.GetComponentData<CustomSpeed>(baseEdge).m_Speed;
            }

            float averageSpeed = GetAverageSpeed(edge);
            return averageSpeed > 0f ? averageSpeed : GetVanillaSpeed(baseEdge);
        }

        private float GetVanillaSpeed(Entity edge)
        {
            Entity baseEdge = GetBaseEdge(edge);

            if (!EntityManager.HasComponent<PrefabRef>(baseEdge))
            {
                return -1f;
            }

            PrefabRef prefabRef = EntityManager.GetComponentData<PrefabRef>(baseEdge);
            PrefabSystem prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            if (!prefabSystem.TryGetPrefab(prefabRef, out PrefabBase prefabBase) || prefabBase == null)
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

        private float GetAverageSpeed(Entity edge)
        {
            m_Speeds.Clear();

            if (!EntityManager.HasBuffer<SubLane>(edge))
            {
                return -1f;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);
            CarLaneFlags ignoredCarLaneFlags = CarLaneFlags.Unsafe | CarLaneFlags.SideConnection;

            foreach (SubLane subLane in subLanes)
            {
                Entity laneEntity = subLane.m_SubLane;

                if (EntityManager.HasComponent<CarLane>(laneEntity))
                {
                    CarLane carLane = EntityManager.GetComponentData<CarLane>(laneEntity);
                    if ((carLane.m_Flags & ignoredCarLaneFlags) == 0)
                    {
                        m_Speeds.Add(carLane.m_SpeedLimit * 1.8f);
                    }
                }
                else if (EntityManager.HasComponent<TrackLane>(laneEntity))
                {
                    TrackLane trackLane = EntityManager.GetComponentData<TrackLane>(laneEntity);
                    m_Speeds.Add(trackLane.m_SpeedLimit * 1.8f);
                }
            }

            return m_Speeds.Count > 0 ? m_Speeds.Average() : -1f;
        }

        private bool IsTrackType(Entity edge)
        {
            if (!EntityManager.HasBuffer<SubLane>(edge))
            {
                return false;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);

            foreach (SubLane subLane in subLanes)
            {
                if (EntityManager.HasComponent<TrackLane>(subLane.m_SubLane))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsWaterwayType(Entity edge)
        {
            Entity baseEdge = GetBaseEdge(edge);
            return EntityManager.HasComponent<Waterway>(baseEdge);
        }

        private void HandleApplySpeed(float newSpeed)
        {
            if (m_SelectedEntity == Entity.Null || m_SelectedEdges.Count == 0)
            {
                return;
            }

            IReadOnlyList<Entity> edges = m_SegmentSpeedTool.IsActive && m_SegmentSpeedTool.SelectedEdges.Count > 0
                ? m_SegmentSpeedTool.SelectedEdges
                : m_SelectedEdges;

            newSpeed = math.clamp(newSpeed, kMinSpeedKmh, GetMaxSpeedKmh(edges));

            if (m_SegmentSpeedTool.IsActive && m_SegmentSpeedTool.SelectedEdges.Count > 0)
            {
                m_SegmentSpeedTool.ApplySpeedToSelection(newSpeed);
            }
            else
            {
                ApplySpeedToEdges(newSpeed);
            }

            m_InitialSpeedBinding.Value = newSpeed;
            m_CurrentSpeedMixedBinding.Value = false;
            RequestUpdate();
        }

        private void HandleApplySelectionMultiplier(float multiplier)
        {
            if (m_SelectedEntity == Entity.Null || m_SelectedEdges.Count == 0)
            {
                return;
            }

            IReadOnlyList<Entity> edges = m_SegmentSpeedTool.IsActive && m_SegmentSpeedTool.SelectedEdges.Count > 0
                ? m_SegmentSpeedTool.SelectedEdges
                : m_SelectedEdges;

            if (edges.Count == 0)
            {
                return;
            }

            for (int i = 0; i < edges.Count; i++)
            {
                Entity edge = edges[i];
                Entity targetEdge = GetBaseEdge(edge);

                if (targetEdge == Entity.Null || !EntityManager.Exists(targetEdge))
                {
                    continue;
                }

                float currentSpeed = GetStreetSpeed(targetEdge);
                if (currentSpeed <= 0f)
                {
                    continue;
                }

                float newSpeed = math.clamp(currentSpeed * multiplier, kMinSpeedKmh, GetMaxSpeedKmh(targetEdge));
                ApplySpeedToSingleEdge(edge, newSpeed, saveImmediately: false);
            }

            PersistentSpeedLimitStorage.Save();
            RefreshSelectionSpeedBindings();
        }

        private bool IsRoadOnly(Entity edge)
        {
            bool isTrackType = IsTrackType(edge);
            bool isRoadType = EntityManager.HasComponent<Road>(edge);
            bool isWaterwayType = IsWaterwayType(edge);

            return isRoadType && !isTrackType && !isWaterwayType;
        }

        private float GetMaxSpeedKmh(Entity edge)
        {
            if (IsWaterwayType(edge))
            {
                return kMaxWaterwaySpeedKmh;
            }

            return IsTrackType(edge) ? kMaxTrackSpeedKmh : kMaxRoadSpeedKmh;
        }

        private float GetMaxSpeedKmh(IReadOnlyList<Entity> edges)
        {
            if (edges.Count == 0)
            {
                return kMaxRoadSpeedKmh;
            }

            float maxSpeed = float.MaxValue;
            for (int i = 0; i < edges.Count; i++)
            {
                Entity baseEdge = GetBaseEdge(edges[i]);
                if (baseEdge == Entity.Null || !EntityManager.Exists(baseEdge))
                {
                    continue;
                }

                maxSpeed = math.min(maxSpeed, GetMaxSpeedKmh(baseEdge));
            }

            return maxSpeed < float.MaxValue ? maxSpeed : kMaxRoadSpeedKmh;
        }

        private void ApplySpeedToEdges(float newSpeed)
        {
            float speedGameUnits = newSpeed / 1.8f;
            Entity aggregate = m_SelectedInfoUISystem.selectedEntity;

            if (aggregate != Entity.Null)
            {
                StoreOriginalSpeedIfNeeded(aggregate, newSpeed);
            }

            foreach (Entity edge in m_SelectedEdges)
            {
                Entity targetEdge = GetBaseEdge(edge);

                if (!EntityManager.HasComponent<CustomSpeed>(targetEdge))
                {
                    EntityManager.AddComponent<CustomSpeed>(targetEdge);
                }

                EntityManager.SetComponentData(targetEdge, new CustomSpeed(newSpeed));

                SpeedLimitDataManager.AddCustomSpeedLimit(targetEdge.Index, newSpeed);
                PersistentSpeedLimitStorage.StoreSpeedLimit(
                    targetEdge.Index,
                    PersistentSpeedLimitStorage.GetDefaultSpeedLimit(targetEdge.Index) ?? GetAverageSpeed(edge),
                    newSpeed);

                SetLaneSpeedsImmediate(edge, speedGameUnits);
            }
        }

        private void ApplySpeedToSingleEdge(Entity edge, float newSpeed, bool saveImmediately)
        {
            Entity targetEdge = GetBaseEdge(edge);
            if (targetEdge == Entity.Null || !EntityManager.Exists(targetEdge))
            {
                return;
            }

            float originalSpeed =
                PersistentSpeedLimitStorage.GetDefaultSpeedLimit(targetEdge.Index) ??
                SpeedLimitDataManager.GetOriginalSpeed(targetEdge.Index) ??
                GetAverageSpeed(edge);

            if (originalSpeed <= 0f)
            {
                float vanillaSpeed = GetVanillaSpeed(targetEdge);
                if (vanillaSpeed > 0f)
                {
                    originalSpeed = vanillaSpeed;
                }
            }

            if (originalSpeed > 0f)
            {
                SpeedLimitDataManager.StoreOriginalSpeed(targetEdge.Index, originalSpeed);
                PersistentSpeedLimitStorage.StoreSpeedLimit(targetEdge.Index, originalSpeed, newSpeed, saveImmediately);
            }

            if (!EntityManager.HasComponent<CustomSpeed>(targetEdge))
            {
                EntityManager.AddComponent<CustomSpeed>(targetEdge);
            }

            EntityManager.SetComponentData(targetEdge, new CustomSpeed(newSpeed));
            SpeedLimitDataManager.AddCustomSpeedLimit(targetEdge.Index, newSpeed);
            SetLaneSpeedsImmediate(edge, newSpeed / 1.8f);
        }

        private void StoreOriginalSpeedIfNeeded(Entity aggregate, float newSpeed)
        {
            bool alreadyModified = false;

            foreach (Entity edge in m_SelectedEdges)
            {
                Entity targetEdge = GetBaseEdge(edge);
                if (EntityManager.HasComponent<CustomSpeed>(targetEdge))
                {
                    alreadyModified = true;
                    break;
                }
            }

            if (alreadyModified)
            {
                float? existingDefault = PersistentSpeedLimitStorage.GetDefaultSpeedLimit(aggregate.Index);
                if (existingDefault.HasValue)
                {
                    PersistentSpeedLimitStorage.StoreSpeedLimit(aggregate.Index, existingDefault.Value, newSpeed);
                }

                return;
            }

            float originalSpeed = GetAverageSpeed(m_SelectedEntity);
            if (originalSpeed <= 0f)
            {
                return;
            }

            // Store original road speed once so reset can restore it later.
            SpeedLimitDataManager.StoreOriginalSpeed(aggregate.Index, originalSpeed);
            PersistentSpeedLimitStorage.StoreSpeedLimit(aggregate.Index, originalSpeed, newSpeed);
        }

        private void HandleResetSpeed()
        {
            if (m_SegmentSpeedTool.IsActive && m_SegmentSpeedTool.SelectedEdges.Count > 0)
            {
                ResetSpeedForEdges(m_SegmentSpeedTool.SelectedEdges);
                RefreshSelectionSpeedBindings();
                return;
            }

            if (m_SelectedEntity == Entity.Null || m_SelectedEdges.Count == 0)
            {
                return;
            }

            ResetSpeedForEdges(m_SelectedEdges);
            RefreshSelectionSpeedBindings();
        }

        private void ResetSpeedForEdges(IReadOnlyList<Entity> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                Entity edge = edges[i];
                Entity targetEdge = GetBaseEdge(edge);
                if (targetEdge == Entity.Null || !EntityManager.Exists(targetEdge))
                {
                    continue;
                }

                float? originalSpeed = PersistentSpeedLimitStorage.GetDefaultSpeedLimit(targetEdge.Index)
                    ?? SpeedLimitDataManager.GetOriginalSpeed(targetEdge.Index);

                if (!originalSpeed.HasValue || originalSpeed.Value <= 0f)
                {
                    float vanillaSpeed = GetVanillaSpeed(targetEdge);
                    if (vanillaSpeed > 0f)
                    {
                        originalSpeed = vanillaSpeed;
                    }
                }

                if (!originalSpeed.HasValue || originalSpeed.Value <= 0f)
                {
                    continue;
                }

                SetLaneSpeedsImmediate(edge, originalSpeed.Value / 1.8f);

                if (EntityManager.HasComponent<CustomSpeed>(targetEdge))
                {
                    EntityManager.RemoveComponent<CustomSpeed>(targetEdge);
                }

                SpeedLimitDataManager.RemoveCustomSpeedLimit(targetEdge.Index);
                SpeedLimitDataManager.RemoveOriginalSpeed(targetEdge.Index);
                PersistentSpeedLimitStorage.RemoveSpeedLimit(targetEdge.Index);
            }
        }

        private void RefreshSelectionSpeedBindings()
        {
            if (m_SelectedEdges.Count == 0)
            {
                return;
            }

            SelectionSpeedInfo selectionSpeed = GetSelectionSpeedInfo(m_SelectedEdges);
            if (!selectionSpeed.HasCurrentSpeed)
            {
                return;
            }

            m_InitialSpeedBinding.Value = selectionSpeed.CurrentSpeed;
            m_CurrentSpeedMixedBinding.Value = selectionSpeed.CurrentSpeedMixed;
            m_VanillaSpeedBinding.Value = selectionSpeed.HasVanillaSpeed ? selectionSpeed.VanillaSpeed : -1f;
            m_VanillaSpeedMixedBinding.Value = selectionSpeed.VanillaSpeedMixed;
            m_IsTrackTypeBinding.Value = selectionSpeed.ContainsTrackType;
            m_IsWaterwayTypeBinding.Value = selectionSpeed.ContainsWaterwayType;

            RequestUpdate();
        }

        private void SetLaneSpeedsImmediate(Entity edge, float speedGameUnits)
        {
            if (!EntityManager.HasBuffer<SubLane>(edge))
            {
                return;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);

            for (int i = 0; i < subLanes.Length; i++)
            {
                SetLaneSpeed(subLanes[i].m_SubLane, speedGameUnits);
            }
        }

        private void SetLaneSpeed(Entity laneEntity, float speedGameUnits)
        {
            if (EntityManager.HasComponent<CarLane>(laneEntity))
            {
                CarLane carLane = EntityManager.GetComponentData<CarLane>(laneEntity);
                CarLaneFlags originalFlags = carLane.m_Flags;

                carLane.m_DefaultSpeedLimit = speedGameUnits;
                carLane.m_SpeedLimit = speedGameUnits;
                carLane.m_Flags = originalFlags;
                EntityManager.SetComponentData(laneEntity, carLane);
                return;
            }

            if (EntityManager.HasComponent<TrackLane>(laneEntity))
            {
                TrackLane trackLane = EntityManager.GetComponentData<TrackLane>(laneEntity);
                trackLane.m_SpeedLimit = speedGameUnits;
                EntityManager.SetComponentData(laneEntity, trackLane);
            }
        }

        private Entity GetBaseEdge(Entity edge)
        {
            return EntityManager.HasComponent<Temp>(edge)
                ? EntityManager.GetComponentData<Temp>(edge).m_Original
                : edge;
        }
    }
}
