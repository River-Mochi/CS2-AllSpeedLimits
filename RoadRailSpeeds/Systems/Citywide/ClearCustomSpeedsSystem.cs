// <copyright file="ClearCustomSpeedsSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Citywide/ClearCustomSpeedsSystem.cs
// Purpose: Handles batched requests to remove custom speeds in the loaded city.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Net;
    using RoadRailSpeeds.Components;
    using RoadRailSpeeds.Data;
    using Unity.Entities;
    using UnityEngine.Scripting;
    using Temp = Game.Tools.Temp;

    public partial class ClearCustomSpeedsSystem : GameSystemBase
    {
        public enum ClearScope
        {
            All,
            Roads,
            Rails,
            Waterways
        }

        // Tweak this if citywide reset feels too slow or too bursty on low-end hardware.
        private const int kClearCustomSpeedsBatchSize = 96;

        private bool m_ClearRequested;
        private bool m_ClearInProgress;
        private int m_ClearIndex;
        private int m_ClearTotal;
        private int m_ClearedCount;
        private ClearScope m_ClearScope = ClearScope.All;
        private readonly List<Entity> m_PendingClearEntities = new();

        public bool IsClearInProgress => m_ClearInProgress;

        public int ClearTotal => m_ClearTotal;

        public int ClearedCount => m_ClearedCount;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();

            // Keep enabled so the settings button can request work at any time.
            Enabled = true;
        }

        public void RequestClearAllCustomSpeeds()
        {
            RequestClearCustomSpeeds(ClearScope.All);
        }

        public void RequestClearCustomSpeeds(ClearScope scope)
        {
            if (m_ClearInProgress)
            {
                LogUtils.Info(() => $"{Mod.ModTag} Clear custom speeds already in progress.");
                return;
            }

            m_ClearScope = scope;
            m_ClearRequested = true;
            LogUtils.Info(() => $"{Mod.ModTag} Clear custom speeds requested. scope={scope}");
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (!m_ClearRequested && !m_ClearInProgress)
            {
                return;
            }

            try
            {
                if (m_ClearRequested)
                {
                    m_ClearRequested = false;
                    BeginClearCustomSpeeds();
                }

                ProcessClearBatch();
            }
            catch (System.Exception ex)
            {
                ResetClearState();
                LogUtils.Error(() => $"{Mod.ModTag} Failed to clear custom speeds: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private void BeginClearCustomSpeeds()
        {
            m_PendingClearEntities.Clear();

            // Read-only gather (the actual removal happens later in ProcessClearBatch), so the modern
            // SystemAPI.Query idiom fits. CustomSpeed marks edges this mod changed; MatchesScope keeps
            // only the ones in the requested road/rail/water scope.
            foreach (var (_, entity) in SystemAPI
                .Query<RefRO<CustomSpeed>>()
                .WithAll<Edge>()
                .WithEntityAccess())
            {
                if (MatchesScope(entity, m_ClearScope))
                {
                    m_PendingClearEntities.Add(entity);
                }
            }

            if (m_PendingClearEntities.Count == 0)
            {
                LogUtils.Info(() => $"{Mod.ModTag} No custom speeds matched clear scope {m_ClearScope}.");
                ResetClearState();
                return;
            }

            m_ClearIndex = 0;
            m_ClearTotal = m_PendingClearEntities.Count;
            m_ClearedCount = 0;
            m_ClearInProgress = true;

            LogUtils.Info(
                () => $"{Mod.ModTag} Clearing {m_ClearTotal} custom-speed segments in batches of {kClearCustomSpeedsBatchSize}. scope={m_ClearScope}");
        }

        private void ProcessClearBatch()
        {
            if (!m_ClearInProgress)
            {
                return;
            }

            int endIndex = System.Math.Min(
                m_PendingClearEntities.Count,
                m_ClearIndex + kClearCustomSpeedsBatchSize);

            for (; m_ClearIndex < endIndex; m_ClearIndex++)
            {
                Entity entity = m_PendingClearEntities[m_ClearIndex];
                if (!EntityManager.Exists(entity))
                {
                    continue;
                }

                RestoreOriginalSpeedIfKnown(entity);
                RemoveCustomSpeedTracking(entity);
                m_ClearedCount++;
            }

            if (m_ClearIndex < m_PendingClearEntities.Count)
            {
                return;
            }

            PersistentSpeedLimitStorage.Save();

            int clearedCount = m_ClearedCount;
            ResetClearState();

            LogUtils.Info(() => $"{Mod.ModTag} Cleared custom speeds from {clearedCount} segments.");
        }

        private void ResetClearState()
        {
            m_ClearRequested = false;
            m_ClearInProgress = false;
            m_ClearIndex = 0;
            m_ClearTotal = 0;
            m_ClearedCount = 0;
            m_ClearScope = ClearScope.All;
            m_PendingClearEntities.Clear();
        }

        private void RestoreOriginalSpeedIfKnown(Entity entity)
        {
            float? originalSpeed = SpeedLimitDataManager.GetOriginalSpeed(entity.Index)
                ?? PersistentSpeedLimitStorage.GetDefaultSpeedLimit(entity.Index);

            if (!originalSpeed.HasValue)
            {
                // No stored default speed means the safest fallback is removing CustomSpeed.
                // The game then uses the prefab/default lane speed.
                LogUtils.WarnOnce(
                    $"ClearCustomSpeedsSystem.NoOriginalSpeed.{entity.Index}",
                    () => $"{Mod.ModTag} No original speed found for entity {entity.Index}; removing CustomSpeed only.");
                return;
            }

            float speedGameUnits = originalSpeed.Value / 1.8f;

            if (!EntityManager.HasBuffer<SubLane>(entity))
            {
                return;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(entity);
            for (int i = 0; i < subLanes.Length; i++)
            {
                RestoreLaneSpeed(subLanes[i].m_SubLane, speedGameUnits);
            }
        }

        private void RestoreLaneSpeed(Entity laneEntity, float speedGameUnits)
        {
            if (EntityManager.HasComponent<CarLane>(laneEntity))
            {
                CarLane carLane = EntityManager.GetComponentData<CarLane>(laneEntity);
                carLane.m_SpeedLimit = speedGameUnits;
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

        private void RemoveCustomSpeedTracking(Entity entity)
        {
            if (EntityManager.HasComponent<CustomSpeed>(entity))
            {
                EntityManager.RemoveComponent<CustomSpeed>(entity);
            }

            SpeedLimitDataManager.RemoveOriginalSpeed(entity.Index);
            SpeedLimitDataManager.RemoveCustomSpeedLimit(entity.Index);
            PersistentSpeedLimitStorage.RemoveSpeedLimit(entity.Index, saveImmediately: false);
        }

        private bool MatchesScope(Entity entity, ClearScope scope)
        {
            if (scope == ClearScope.All)
            {
                return true;
            }

            Entity baseEntity = GetBaseEntity(entity);

            return scope switch
            {
                ClearScope.Roads => IsRoadOnly(baseEntity),
                ClearScope.Rails => IsRail(baseEntity),
                ClearScope.Waterways => IsWaterway(baseEntity),
                _ => true
            };
        }

        private Entity GetBaseEntity(Entity entity)
        {
            return EntityManager.HasComponent<Temp>(entity)
                ? EntityManager.GetComponentData<Temp>(entity).m_Original
                : entity;
        }

        private bool IsRoadOnly(Entity entity)
        {
            return EntityManager.HasComponent<Road>(entity) &&
                !IsRail(entity) &&
                !IsWaterway(entity);
        }

        private bool IsRail(Entity entity)
        {
            if (EntityManager.HasComponent<TrainTrack>(entity) ||
                EntityManager.HasComponent<TramTrack>(entity) ||
                EntityManager.HasComponent<SubwayTrack>(entity))
            {
                return true;
            }

            if (!EntityManager.HasBuffer<SubLane>(entity))
            {
                return false;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(entity);
            for (int i = 0; i < subLanes.Length; i++)
            {
                if (EntityManager.HasComponent<TrackLane>(subLanes[i].m_SubLane))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsWaterway(Entity entity)
        {
            return EntityManager.HasComponent<Waterway>(entity);
        }
    }
}
