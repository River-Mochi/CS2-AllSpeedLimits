// <copyright file="CityRoadGroupApplySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Citywide/CityRoadGroupApplySystem.cs
// Purpose: Applies a chosen speed to road groups across the loaded city in small batches.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;      // List
    using CS2Shared.RiverMochi;            // LogUtils
    using Game;                            // GameSystemBase
    using Game.Common;                     // Deleted
    using Game.Net;                        // Edge, Curve, Road, lanes
    using Game.Prefabs;                    // PrefabBase, PrefabRef, PrefabSystem
    using RoadRailSpeeds.Components;       // CustomSpeed
    using RoadRailSpeeds.Data;             // PersistentSpeedLimitStorage, SpeedLimitDataManager
    using Unity.Collections;               // NativeList (batched structural changes)
    using Unity.Entities;                  // Entity, RefRO, SystemAPI
    using Unity.Mathematics;               // math
    using UnityEngine.Scripting;           // Preserve
    using CarLane = Game.Net.CarLane;      // Live car lane component
    using SubLane = Game.Net.SubLane;      // Live sub-lane buffer
    using Temp = Game.Tools.Temp;          // Temp
    using TrackLane = Game.Net.TrackLane;  // Live track lane component

    public partial class CityRoadGroupApplySystem : GameSystemBase
    {
        public enum RoadGroup
        {
            Small = 0,
            Medium = 1,
            Large = 2,
            Highway = 3
        }

        private enum ApplyTarget
        {
            RoadGroup,
            Train,
            Subway
        }

        // Tweak this if citywide apply feels too slow or too bursty on low-end hardware.
        private const int kApplyRoadGroupBatchSize = 96;
        private const float kMinSpeedKmh = 5f;
        private const float kMaxRoadSpeedKmh = 400f;

        private bool m_ApplyRequested;
        private bool m_ApplyInProgress;
        private int m_ApplyIndex;
        private int m_ApplyTotal;
        private int m_AppliedCount;
        private float m_TargetSpeedKmh;
        private RoadGroup m_RoadGroup = RoadGroup.Medium;
        // Which target the pending apply hits. RoadGroup uses PrefabMatchesRoadGroup; Train/Subway gather by
        // the live TrainTrack / SubwayTrack component (tram is grouped with roads elsewhere). The batch
        // state machine is shared; only the entity collection and per-edge apply differ.
        private ApplyTarget m_ApplyTarget = ApplyTarget.RoadGroup;
        private readonly List<Entity> m_PendingApplyEntities = new();
        // Road-group classification depends only on the prefab, but many edges share one prefab.
        // Caching the (prefab -> matches group?) result turns thousands of managed prefab lookups
        // into one per unique prefab, which is what was spiking on Small/Highway gathers.
        private readonly Dictionary<Entity, bool> m_PrefabGroupMatchCache = new();
        private PrefabSystem m_PrefabSystem = null!;

        public bool IsApplyInProgress => m_ApplyInProgress;

        public int ApplyTotal => m_ApplyTotal;

        public int AppliedCount => m_AppliedCount;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();

            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Keep enabled so the panel can request work at any time.
            Enabled = true;
        }

        public void RequestApplyRoadGroupSpeed(RoadGroup roadGroup, float speedKmh)
        {
            if (m_ApplyInProgress)
            {
                LogUtils.Info(() => $"{Mod.ModTag} Road-group speed apply already in progress.");
                return;
            }

            m_ApplyTarget = ApplyTarget.RoadGroup;
            m_RoadGroup = roadGroup;
            m_TargetSpeedKmh = math.clamp(speedKmh, kMinSpeedKmh, kMaxRoadSpeedKmh);
            m_ApplyRequested = true;

            LogUtils.Info(
                () => $"{Mod.ModTag} Road-group speed apply requested. group={roadGroup}, targetKmh={m_TargetSpeedKmh:0.##}");
        }

        public void RequestApplyTrainSpeed(float speedKmh)
        {
            RequestApplyTrackSpeed(ApplyTarget.Train, speedKmh);
        }

        public void RequestApplySubwaySpeed(float speedKmh)
        {
            RequestApplyTrackSpeed(ApplyTarget.Subway, speedKmh);
        }

        private void RequestApplyTrackSpeed(ApplyTarget target, float speedKmh)
        {
            if (m_ApplyInProgress)
            {
                LogUtils.Info(() => $"{Mod.ModTag} Rail speed apply already in progress.");
                return;
            }

            // Rail reuses the road max cap (400 km/h) so the panel's slider range stays consistent.
            m_ApplyTarget = target;
            m_TargetSpeedKmh = math.clamp(speedKmh, kMinSpeedKmh, kMaxRoadSpeedKmh);
            m_ApplyRequested = true;

            LogUtils.Info(
                () => $"{Mod.ModTag} Rail speed apply requested. target={target}, targetKmh={m_TargetSpeedKmh:0.##}");
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (!m_ApplyRequested && !m_ApplyInProgress)
            {
                return;
            }

            try
            {
                if (m_ApplyRequested)
                {
                    m_ApplyRequested = false;
                    BeginApply();
                }

                ProcessApplyBatch();
            }
            catch (System.Exception ex)
            {
                ResetApplyState();
                LogUtils.Error(
                    () => $"{Mod.ModTag} Failed to apply road-group speed: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }
        }

        private void BeginApply()
        {
            m_PendingApplyEntities.Clear();

            // Read-only gather (the edits happen later in ProcessApplyBatch), so SystemAPI.Query is the
            // modern fit. Rail = train + subway only; tram is grouped with roads elsewhere in this mod.
            if (m_ApplyTarget == ApplyTarget.Train)
            {
                foreach (var (_, entity) in SystemAPI
                    .Query<RefRO<PrefabRef>>()
                    .WithAll<Edge, Curve, TrainTrack>()
                    .WithNone<Deleted, Temp>()
                    .WithEntityAccess())
                {
                    m_PendingApplyEntities.Add(entity);
                }
            }
            else if (m_ApplyTarget == ApplyTarget.Subway)
            {
                foreach (var (_, entity) in SystemAPI
                    .Query<RefRO<PrefabRef>>()
                    .WithAll<Edge, Curve, SubwayTrack>()
                    .WithNone<Deleted, Temp>()
                    .WithEntityAccess())
                {
                    m_PendingApplyEntities.Add(entity);
                }
            }
            else
            {
                m_PrefabGroupMatchCache.Clear();
                foreach (var (prefabRef, entity) in SystemAPI
                    .Query<RefRO<PrefabRef>>()
                    .WithAll<Edge, Curve, Road>()
                    .WithNone<Deleted, Temp>()
                    .WithEntityAccess())
                {
                    // Per-edge gate (cheap component reads), kept out of the cache.
                    if (!IsRoadOnly(entity))
                    {
                        continue;
                    }

                    // The expensive prefab/group lookup is cached by prefab entity.
                    Entity prefabEntity = prefabRef.ValueRO.m_Prefab;
                    if (!m_PrefabGroupMatchCache.TryGetValue(prefabEntity, out bool matches))
                    {
                        matches = PrefabMatchesRoadGroup(entity, m_RoadGroup);
                        m_PrefabGroupMatchCache[prefabEntity] = matches;
                    }

                    if (matches)
                    {
                        m_PendingApplyEntities.Add(entity);
                    }
                }
            }

            if (m_PendingApplyEntities.Count == 0)
            {
                LogUtils.Info(
                    () => $"{Mod.ModTag} No segments matched the citywide apply request. target={m_ApplyTarget}, group={m_RoadGroup}.");
                ResetApplyState();
                return;
            }

            m_ApplyIndex = 0;
            m_ApplyTotal = m_PendingApplyEntities.Count;
            m_AppliedCount = 0;
            m_ApplyInProgress = true;

            LogUtils.Info(
                () => $"{Mod.ModTag} Applying {m_TargetSpeedKmh:0.##} km/h to {m_ApplyTotal} segments (target={m_ApplyTarget}, group={m_RoadGroup}) in batches of {kApplyRoadGroupBatchSize}.");
        }

        private void ProcessApplyBatch()
        {
            if (!m_ApplyInProgress)
            {
                return;
            }

            int endIndex = System.Math.Min(
                m_PendingApplyEntities.Count,
                m_ApplyIndex + kApplyRoadGroupBatchSize);

            // One batched structural add for this frame's slice, instead of an AddComponent per entity.
            using (NativeList<Entity> toAdd = new NativeList<Entity>(kApplyRoadGroupBatchSize, Allocator.Temp))
            {
                for (int i = m_ApplyIndex; i < endIndex; i++)
                {
                    Entity entity = m_PendingApplyEntities[i];
                    if (EntityManager.Exists(entity) && !EntityManager.HasComponent<CustomSpeed>(entity))
                    {
                        toAdd.Add(entity);
                    }
                }

                if (toAdd.Length > 0)
                {
                    EntityManager.AddComponent<CustomSpeed>(toAdd.AsArray());
                }
            }

            for (; m_ApplyIndex < endIndex; m_ApplyIndex++)
            {
                Entity entity = m_PendingApplyEntities[m_ApplyIndex];
                if (!EntityManager.Exists(entity))
                {
                    continue;
                }

                if (m_ApplyTarget == ApplyTarget.RoadGroup)
                {
                    ApplySpeedToRoad(entity, m_TargetSpeedKmh);
                }
                else
                {
                    ApplySpeedToRail(entity, m_TargetSpeedKmh);
                }

                m_AppliedCount++;
            }

            if (m_ApplyIndex < m_PendingApplyEntities.Count)
            {
                return;
            }

            PersistentSpeedLimitStorage.Save();

            int appliedCount = m_AppliedCount;
            ApplyTarget target = m_ApplyTarget;
            float targetSpeed = m_TargetSpeedKmh;
            ResetApplyState();

            LogUtils.Info(
                () => $"{Mod.ModTag} Applied {targetSpeed:0.##} km/h to {appliedCount} segments (target={target}).");
        }

        private void ResetApplyState()
        {
            m_ApplyRequested = false;
            m_ApplyInProgress = false;
            m_ApplyIndex = 0;
            m_ApplyTotal = 0;
            m_AppliedCount = 0;
            m_TargetSpeedKmh = 0f;
            m_RoadGroup = RoadGroup.Medium;
            m_ApplyTarget = ApplyTarget.RoadGroup;
            m_PendingApplyEntities.Clear();
        }

        private void ApplySpeedToRoad(Entity entity, float speedKmh)
        {
            float speedGameUnits = speedKmh / 1.8f;
            float originalSpeed = GetStoredOrCurrentSpeed(entity);

            if (originalSpeed > 0f)
            {
                SpeedLimitDataManager.StoreOriginalSpeed(entity.Index, originalSpeed);
            }

            PersistentSpeedLimitStorage.StoreSpeedLimit(
                entity.Index,
                originalSpeed > 0f ? originalSpeed : speedKmh,
                speedKmh,
                saveImmediately: false);

            // CustomSpeed was already added in the batched structural pass in ProcessApplyBatch.
            EntityManager.SetComponentData(entity, new CustomSpeed(speedKmh));
            SpeedLimitDataManager.AddCustomSpeedLimit(entity.Index, speedKmh);
            SetCarLaneSpeedsImmediate(entity, speedGameUnits);
        }

        // Rail mirror of ApplySpeedToRoad: same persistence/CustomSpeed bookkeeping, but writes
        // TrackLane speeds (trains/subways) instead of CarLane speeds.
        private void ApplySpeedToRail(Entity entity, float speedKmh)
        {
            float speedGameUnits = speedKmh / 1.8f;
            float originalSpeed = GetStoredOrCurrentRailSpeed(entity);

            if (originalSpeed > 0f)
            {
                SpeedLimitDataManager.StoreOriginalSpeed(entity.Index, originalSpeed);
            }

            PersistentSpeedLimitStorage.StoreSpeedLimit(
                entity.Index,
                originalSpeed > 0f ? originalSpeed : speedKmh,
                speedKmh,
                saveImmediately: false);

            // CustomSpeed was already added in the batched structural pass in ProcessApplyBatch.
            EntityManager.SetComponentData(entity, new CustomSpeed(speedKmh));
            SpeedLimitDataManager.AddCustomSpeedLimit(entity.Index, speedKmh);
            SetTrackLaneSpeedsImmediate(entity, speedGameUnits);
        }

        private float GetStoredOrCurrentRailSpeed(Entity entity)
        {
            float? storedDefault = PersistentSpeedLimitStorage.GetDefaultSpeedLimit(entity.Index);
            if (storedDefault.HasValue)
            {
                return storedDefault.Value;
            }

            float currentSpeed = GetAverageTrackLaneSpeed(entity);
            if (currentSpeed > 0f)
            {
                return currentSpeed;
            }

            if (!TryGetTrackPrefab(entity, out TrackPrefab trackPrefab))
            {
                return -1f;
            }

            return trackPrefab.m_SpeedLimit / 2f;
        }

        private void SetTrackLaneSpeedsImmediate(Entity entity, float speedGameUnits)
        {
            if (!EntityManager.HasBuffer<SubLane>(entity))
            {
                return;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(entity);
            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity laneEntity = subLanes[i].m_SubLane;
                if (!EntityManager.HasComponent<TrackLane>(laneEntity))
                {
                    continue;
                }

                TrackLane trackLane = EntityManager.GetComponentData<TrackLane>(laneEntity);
                trackLane.m_SpeedLimit = speedGameUnits;
                EntityManager.SetComponentData(laneEntity, trackLane);
            }
        }

        private float GetAverageTrackLaneSpeed(Entity entity)
        {
            if (!EntityManager.HasBuffer<SubLane>(entity))
            {
                return -1f;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(entity);
            float totalSpeed = 0f;
            int count = 0;

            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity laneEntity = subLanes[i].m_SubLane;
                if (!EntityManager.HasComponent<TrackLane>(laneEntity))
                {
                    continue;
                }

                TrackLane trackLane = EntityManager.GetComponentData<TrackLane>(laneEntity);
                totalSpeed += trackLane.m_SpeedLimit * 1.8f;
                count++;
            }

            return count > 0 ? totalSpeed / count : -1f;
        }

        private bool TryGetTrackPrefab(Entity entity, out TrackPrefab trackPrefab)
        {
            trackPrefab = null!;

            if (!EntityManager.HasComponent<PrefabRef>(entity))
            {
                return false;
            }

            PrefabRef prefabRef = EntityManager.GetComponentData<PrefabRef>(entity);
            if (!m_PrefabSystem.TryGetPrefab(prefabRef, out PrefabBase prefabBase) || !(prefabBase is TrackPrefab prefab))
            {
                return false;
            }

            trackPrefab = prefab;
            return true;
        }

        private float GetStoredOrCurrentSpeed(Entity entity)
        {
            float? storedDefault = PersistentSpeedLimitStorage.GetDefaultSpeedLimit(entity.Index);
            if (storedDefault.HasValue)
            {
                return storedDefault.Value;
            }

            float currentSpeed = GetAverageCarLaneSpeed(entity);
            if (currentSpeed > 0f)
            {
                return currentSpeed;
            }

            if (!TryGetRoadPrefab(entity, out RoadPrefab roadPrefab, out _))
            {
                return -1f;
            }

            return roadPrefab.m_SpeedLimit / 2f;
        }

        private void SetCarLaneSpeedsImmediate(Entity entity, float speedGameUnits)
        {
            if (!EntityManager.HasBuffer<SubLane>(entity))
            {
                return;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(entity);
            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity laneEntity = subLanes[i].m_SubLane;
                if (!EntityManager.HasComponent<CarLane>(laneEntity))
                {
                    continue;
                }

                CarLane carLane = EntityManager.GetComponentData<CarLane>(laneEntity);
                carLane.m_SpeedLimit = speedGameUnits;
                EntityManager.SetComponentData(laneEntity, carLane);
            }
        }

        private float GetAverageCarLaneSpeed(Entity entity)
        {
            if (!EntityManager.HasBuffer<SubLane>(entity))
            {
                return -1f;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(entity);
            CarLaneFlags ignoredFlags = CarLaneFlags.Unsafe | CarLaneFlags.SideConnection;
            float totalSpeed = 0f;
            int count = 0;

            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity laneEntity = subLanes[i].m_SubLane;
                if (!EntityManager.HasComponent<CarLane>(laneEntity))
                {
                    continue;
                }

                CarLane carLane = EntityManager.GetComponentData<CarLane>(laneEntity);
                if ((carLane.m_Flags & ignoredFlags) != 0)
                {
                    continue;
                }

                totalSpeed += carLane.m_SpeedLimit * 1.8f;
                count++;
            }

            return count > 0 ? totalSpeed / count : -1f;
        }

        // Prefab-only part of the road-group test (no per-edge checks), so the result can be cached
        // per prefab. The caller checks IsRoadOnly separately for each edge.
        private bool PrefabMatchesRoadGroup(Entity entity, RoadGroup roadGroup)
        {
            if (!TryGetRoadPrefab(entity, out RoadPrefab roadPrefab, out Entity prefabEntity))
            {
                return false;
            }

            string groupName = GetPrefabGroupName(prefabEntity);

            return roadGroup switch
            {
                RoadGroup.Small => groupName == "RoadsSmallRoads",
                RoadGroup.Medium => groupName == "RoadsMediumRoads",
                RoadGroup.Large => groupName == "RoadsLargeRoads",
                RoadGroup.Highway => roadPrefab.m_HighwayRules || groupName == "RoadsHighways",
                _ => false
            };
        }

        private bool TryGetRoadPrefab(Entity entity, out RoadPrefab roadPrefab, out Entity prefabEntity)
        {
            roadPrefab = null!;
            prefabEntity = Entity.Null;

            if (!EntityManager.HasComponent<PrefabRef>(entity))
            {
                return false;
            }

            PrefabRef prefabRef = EntityManager.GetComponentData<PrefabRef>(entity);
            prefabEntity = prefabRef.m_Prefab;

            if (!m_PrefabSystem.TryGetPrefab(prefabRef, out PrefabBase prefabBase) || !(prefabBase is RoadPrefab prefab))
            {
                return false;
            }

            roadPrefab = prefab;
            return true;
        }

        private string GetPrefabGroupName(Entity prefabEntity)
        {
            if (prefabEntity == Entity.Null || !EntityManager.HasComponent<UIObjectData>(prefabEntity))
            {
                return string.Empty;
            }

            UIObjectData uiObjectData = EntityManager.GetComponentData<UIObjectData>(prefabEntity);
            if (uiObjectData.m_Group == Entity.Null ||
                !m_PrefabSystem.TryGetPrefab(uiObjectData.m_Group, out PrefabBase groupPrefab) ||
                groupPrefab == null)
            {
                return string.Empty;
            }

            return groupPrefab.name ?? string.Empty;
        }

        private bool IsRoadOnly(Entity entity)
        {
            if (!EntityManager.HasComponent<Road>(entity) ||
                EntityManager.HasComponent<TrainTrack>(entity) ||
                EntityManager.HasComponent<TramTrack>(entity) ||
                EntityManager.HasComponent<SubwayTrack>(entity) ||
                EntityManager.HasComponent<Waterway>(entity))
            {
                return false;
            }

            if (!EntityManager.HasBuffer<SubLane>(entity))
            {
                return true;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(entity);
            for (int i = 0; i < subLanes.Length; i++)
            {
                if (EntityManager.HasComponent<TrackLane>(subLanes[i].m_SubLane))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
