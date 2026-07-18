// <copyright file="CustomSpeedReapplySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Runtime/CustomSpeedReapplySystem.cs
// Purpose: Re-applies CustomSpeed values after CS2 refreshes road/rail lane data.

namespace RoadRailSpeeds.Systems
{
    using System;
    using Colossal.Serialization.Entities;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Common;
    using Game.Net;
    using RoadRailSpeeds.Components;
    using RoadRailSpeeds.Data;
    using Unity.Burst;
    using Unity.Burst.Intrinsics;
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine.Scripting;
    using PrefabBase = Game.Prefabs.PrefabBase;
    using PrefabRef = Game.Prefabs.PrefabRef;
    using PrefabSystem = Game.Prefabs.PrefabSystem;
    using RoadPrefab = Game.Prefabs.RoadPrefab;
    using Temp = Game.Tools.Temp;
    using TrackPrefab = Game.Prefabs.TrackPrefab;
    using WaterwayPrefab = Game.Prefabs.WaterwayPrefab;

    public partial class CustomSpeedReapplySystem : GameSystemBase
    {
        private const float kSpeedComparisonToleranceKmh = 0.5f;
        private const float kMaximumSupportedSpeedKmh = 400f;

        private EntityQuery m_AllCustomEdgesQuery;
        private EntityQuery m_UpdatedLanesQuery;
        private EntityQuery m_PathfindUpdatedLanesQuery;
        private EntityQuery m_UnmarkedSupportedEdgesQuery;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();

            m_AllCustomEdgesQuery = SystemAPI.QueryBuilder()
                .WithAll<CustomSpeed, SubLane>()
                .WithNone<Deleted, Temp>()
                .Build();

            // LaneDataSystem refreshes lane entities, not their owning edge. Watch the same two
            // game-owned event tags it consumes, then restore an owning edge's explicit override.
            m_UpdatedLanesQuery = SystemAPI.QueryBuilder()
                .WithAll<Owner, Updated, Lane>()
                .WithAny<CarLane, TrackLane>()
                .WithNone<Deleted, Temp>()
                .Build();

            m_PathfindUpdatedLanesQuery = SystemAPI.QueryBuilder()
                .WithAll<Owner, PathfindUpdated, Lane>()
                .WithAny<CarLane, TrackLane>()
                .WithNone<Updated, Deleted, Temp>()
                .Build();

            m_UnmarkedSupportedEdgesQuery = SystemAPI.QueryBuilder()
                .WithAll<Edge, PrefabRef, SubLane>()
                .WithAny<Road, TrainTrack, TramTrack, SubwayTrack, Waterway>()
                .WithNone<CustomSpeed, Deleted, Temp>()
                .Build();

            RequireAnyForUpdate(m_UpdatedLanesQuery, m_PathfindUpdatedLanesQuery);
        }

        [Preserve]
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (mode != GameMode.Game ||
                (purpose != Purpose.NewGame && purpose != Purpose.LoadGame))
            {
                return;
            }

            int adoptedEdges = purpose == Purpose.LoadGame
                ? AdoptExistingCustomSpeeds()
                : 0;

            // CustomSpeed is serialized in the city, but old saves can contain lane values that
            // CS2 refreshed from the prefab. Repair every custom edge once after loading finishes.
            int restoredEdges = RestoreSpeeds(m_AllCustomEdgesQuery);
#if DEBUG
            LogUtils.Info(
                () => $"{Mod.ModTag} Load speed reconciliation adopted={adoptedEdges} reapplied={restoredEdges}.");
#endif
        }

        [Preserve]
        protected override void OnUpdate()
        {
            // CS2 owns both event tags and removes them in its cleanup systems. We only observe
            // lanes after LaneDataSystem has recalculated their runtime data.
            RestoreUpdatedLaneSpeedsJob job = new()
            {
                m_OwnerType = SystemAPI.GetComponentTypeHandle<Owner>(isReadOnly: true),
                m_CarLaneType = SystemAPI.GetComponentTypeHandle<CarLane>(),
                m_TrackLaneType = SystemAPI.GetComponentTypeHandle<TrackLane>(),
                m_CustomSpeedLookup =
                    SystemAPI.GetComponentLookup<CustomSpeed>(isReadOnly: true)
            };

            Dependency = job.ScheduleParallel(m_UpdatedLanesQuery, Dependency);
            Dependency = job.ScheduleParallel(m_PathfindUpdatedLanesQuery, Dependency);
        }

        private int AdoptExistingCustomSpeeds()
        {
            // DanielVNZ's original mod used a differently named CustomSpeed component, so it is
            // unavailable after that assembly is removed. Its lane values remain serialized in the
            // city. Adopt only uniform, non-prefab lane speeds; mixed lane layouts are left alone.
            ComponentLookup<PrefabRef> prefabRefLookup =
                SystemAPI.GetComponentLookup<PrefabRef>(isReadOnly: true);
            BufferLookup<SubLane> subLaneLookup =
                SystemAPI.GetBufferLookup<SubLane>(isReadOnly: true);
            ComponentLookup<CarLane> carLaneLookup =
                SystemAPI.GetComponentLookup<CarLane>(isReadOnly: true);
            ComponentLookup<TrackLane> trackLaneLookup =
                SystemAPI.GetComponentLookup<TrackLane>(isReadOnly: true);

            Dependency.Complete();

            PrefabSystem prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            PersistentSpeedLimitStorageSystem storageSystem =
                World.GetOrCreateSystemManaged<PersistentSpeedLimitStorageSystem>();
            bool storageReady = storageSystem.EnsureInitialized();

            using NativeArray<Entity> edges =
                m_UnmarkedSupportedEdgesQuery.ToEntityArray(Allocator.Temp);
            using NativeList<Entity> adoptedEntities =
                new NativeList<Entity>(edges.Length, Allocator.Temp);
            using NativeList<float> adoptedSpeeds =
                new NativeList<float>(edges.Length, Allocator.Temp);
            using NativeList<float> prefabSpeeds =
                new NativeList<float>(edges.Length, Allocator.Temp);

            for (int i = 0; i < edges.Length; i++)
            {
                Entity edge = edges[i];
                if (!prefabSystem.TryGetPrefab(prefabRefLookup[edge], out PrefabBase prefabBase) ||
                    !TryGetPrefabSpeedKmh(prefabBase, out float prefabSpeedKmh) ||
                    !TryGetUniformStoredLaneSpeedKmh(
                        edge,
                        subLaneLookup,
                        carLaneLookup,
                        trackLaneLookup,
                        out float storedSpeedKmh) ||
                    Math.Abs(storedSpeedKmh - prefabSpeedKmh) <= kSpeedComparisonToleranceKmh)
                {
                    continue;
                }

                adoptedEntities.Add(edge);
                adoptedSpeeds.Add(storedSpeedKmh);
                prefabSpeeds.Add(prefabSpeedKmh);
            }

            if (adoptedEntities.Length == 0)
            {
                return 0;
            }

            // One structural change avoids a load-time sync point for every migrated segment.
            EntityManager.AddComponent<CustomSpeed>(adoptedEntities.AsArray());

            for (int i = 0; i < adoptedEntities.Length; i++)
            {
                Entity edge = adoptedEntities[i];
                float currentSpeedKmh = adoptedSpeeds[i];
                float prefabSpeedKmh = prefabSpeeds[i];

                EntityManager.SetComponentData(edge, new CustomSpeed(currentSpeedKmh));
                SpeedLimitDataManager.StoreOriginalSpeed(edge.Index, prefabSpeedKmh);
                SpeedLimitDataManager.AddCustomSpeedLimit(edge.Index, currentSpeedKmh);

                if (storageReady)
                {
                    PersistentSpeedLimitStorage.StoreSpeedLimit(
                        edge.Index,
                        prefabSpeedKmh,
                        currentSpeedKmh);
                }
            }

            if (storageReady)
            {
                // JSON is secondary recovery data; serialize one snapshot for the whole migration.
                PersistentSpeedLimitStorage.Save();
            }

            return adoptedEntities.Length;
        }

        private static bool TryGetPrefabSpeedKmh(PrefabBase prefabBase, out float speedKmh)
        {
            speedKmh = prefabBase switch
            {
                RoadPrefab roadPrefab => roadPrefab.m_SpeedLimit / 2f,
                TrackPrefab trackPrefab => trackPrefab.m_SpeedLimit / 2f,
                WaterwayPrefab waterwayPrefab => waterwayPrefab.m_SpeedLimit / 2f,
                _ => -1f
            };

            return IsSupportedSpeed(speedKmh);
        }

        private static bool TryGetUniformStoredLaneSpeedKmh(
            Entity edge,
            BufferLookup<SubLane> subLaneLookup,
            ComponentLookup<CarLane> carLaneLookup,
            ComponentLookup<TrackLane> trackLaneLookup,
            out float speedKmh)
        {
            speedKmh = -1f;
            DynamicBuffer<SubLane> subLanes = subLaneLookup[edge];
            CarLaneFlags ignoredCarLaneFlags =
                CarLaneFlags.Unsafe | CarLaneFlags.SideConnection;
            float totalSpeedKmh = 0f;
            float minimumSpeedKmh = float.MaxValue;
            float maximumSpeedKmh = float.MinValue;
            int speedCount = 0;

            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity laneEntity = subLanes[i].m_SubLane;
                float laneSpeedKmh;

                if (carLaneLookup.HasComponent(laneEntity))
                {
                    CarLane carLane = carLaneLookup[laneEntity];
                    if ((carLane.m_Flags & ignoredCarLaneFlags) != 0)
                    {
                        continue;
                    }

                    // Vanilla city/district policies change m_SpeedLimit at runtime. DanielVNZ's
                    // mod wrote both fields, so m_DefaultSpeedLimit is the migration evidence that
                    // distinguishes a saved road override from a temporary policy-adjusted speed.
                    laneSpeedKmh = carLane.m_DefaultSpeedLimit * 1.8f;
                }
                else if (trackLaneLookup.HasComponent(laneEntity))
                {
                    laneSpeedKmh = trackLaneLookup[laneEntity].m_SpeedLimit * 1.8f;
                }
                else
                {
                    continue;
                }

                if (!IsSupportedSpeed(laneSpeedKmh))
                {
                    return false;
                }

                totalSpeedKmh += laneSpeedKmh;
                minimumSpeedKmh = Math.Min(minimumSpeedKmh, laneSpeedKmh);
                maximumSpeedKmh = Math.Max(maximumSpeedKmh, laneSpeedKmh);
                speedCount++;
            }

            if (speedCount == 0 ||
                maximumSpeedKmh - minimumSpeedKmh > kSpeedComparisonToleranceKmh)
            {
                return false;
            }

            speedKmh = totalSpeedKmh / speedCount;
            return true;
        }

        private static bool IsSupportedSpeed(float speedKmh)
        {
            return !float.IsNaN(speedKmh) &&
                   !float.IsInfinity(speedKmh) &&
                   speedKmh > 0f &&
                   speedKmh <= kMaximumSupportedSpeedKmh;
        }

        [BurstCompile]
        private struct RestoreUpdatedLaneSpeedsJob : IJobChunk
        {
            [ReadOnly]
            public ComponentTypeHandle<Owner> m_OwnerType;

            public ComponentTypeHandle<CarLane> m_CarLaneType;

            public ComponentTypeHandle<TrackLane> m_TrackLaneType;

            [ReadOnly]
            public ComponentLookup<CustomSpeed> m_CustomSpeedLookup;

            public void Execute(
                in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                NativeArray<Owner> owners = chunk.GetNativeArray(ref m_OwnerType);
                NativeArray<CarLane> carLanes = chunk.GetNativeArray(ref m_CarLaneType);
                NativeArray<TrackLane> trackLanes = chunk.GetNativeArray(ref m_TrackLaneType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    Entity edge = owners[i].m_Owner;
                    if (!m_CustomSpeedLookup.TryGetComponent(edge, out CustomSpeed customSpeed))
                    {
                        continue;
                    }

                    float speedGameUnits = customSpeed.m_Speed / 1.8f;
                    if (carLanes.Length != 0)
                    {
                        CarLane carLane = carLanes[i];
                        carLane.m_DefaultSpeedLimit = speedGameUnits;
                        carLane.m_SpeedLimit = speedGameUnits;
                        carLanes[i] = carLane;
                    }

                    if (trackLanes.Length != 0)
                    {
                        TrackLane trackLane = trackLanes[i];
                        trackLane.m_SpeedLimit = speedGameUnits;
                        trackLanes[i] = trackLane;
                    }
                }
            }
        }

        private int RestoreSpeeds(EntityQuery edgeQuery)
        {
            // These lookups declare component access through this system. This is one load-time
            // reconciliation, so a Burst job would add complexity without useful ongoing work.
            ComponentLookup<CustomSpeed> customSpeedLookup =
                SystemAPI.GetComponentLookup<CustomSpeed>(isReadOnly: true);
            BufferLookup<SubLane> subLaneLookup =
                SystemAPI.GetBufferLookup<SubLane>(isReadOnly: true);
            ComponentLookup<CarLane> carLaneLookup =
                SystemAPI.GetComponentLookup<CarLane>();
            ComponentLookup<TrackLane> trackLaneLookup =
                SystemAPI.GetComponentLookup<TrackLane>();

            Dependency.Complete();

            using NativeArray<Entity> edges = edgeQuery.ToEntityArray(Allocator.Temp);
            int restoredEdges = 0;

            for (int i = 0; i < edges.Length; i++)
            {
                Entity edge = edges[i];
                float speedGameUnits = customSpeedLookup[edge].m_Speed / 1.8f;
                DynamicBuffer<SubLane> subLanes = subLaneLookup[edge];
                bool restoredAnyLane = false;

                for (int j = 0; j < subLanes.Length; j++)
                {
                    Entity laneEntity = subLanes[j].m_SubLane;
                    if (carLaneLookup.HasComponent(laneEntity))
                    {
                        CarLane carLane = carLaneLookup[laneEntity];

                        // LaneDataSystem copies default -> current during network updates. A road
                        // custom speed must therefore own both fields or CS2 silently restores vanilla.
                        carLane.m_DefaultSpeedLimit = speedGameUnits;
                        carLane.m_SpeedLimit = speedGameUnits;
                        carLaneLookup[laneEntity] = carLane;
                        restoredAnyLane = true;
                    }
                    else if (trackLaneLookup.HasComponent(laneEntity))
                    {
                        TrackLane trackLane = trackLaneLookup[laneEntity];
                        trackLane.m_SpeedLimit = speedGameUnits;
                        trackLaneLookup[laneEntity] = trackLane;
                        restoredAnyLane = true;
                    }
                }

                if (restoredAnyLane)
                {
                    restoredEdges++;
                }
            }

            return restoredEdges;
        }
    }
}
