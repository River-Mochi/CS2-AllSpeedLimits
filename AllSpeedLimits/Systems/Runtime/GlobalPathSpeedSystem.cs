// <copyright file="GlobalPathSpeedSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Portions adapted from MIT-licensed CS2-Bikes-Paths.
// Attribution in THIRD_PARTY_NOTICES.md.
//
// One-time global pathway speed changes, never a background citywide scan.
// Bike-only and mixed paths use the bike scale; pedestrian-only paths use the
// separate pedestrian scale. Vehicle roads are excluded from both.
// Individual ASL CustomSpeed edges always take precedence.
namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;
    using Colossal.Serialization.Entities;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Common;
    using Game.Net;
    using Game.Prefabs;
    using Game.Tools;
    using RoadRailSpeeds.Components;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;

    public partial class GlobalPathSpeedSystem : GameSystemBase
    {
        private const int kEdgesPerUpdate = 4096;

        private enum PathCategory
        {
            None,
            Bike,
            Pedestrian
        }

        private PrefabSystem m_PrefabSystem = null!;
        private NativeArray<Entity> m_PathEdges;
        private int m_EdgeIndex;
        private int m_PendingPercent = 100;
        private int m_PendingPedestrianPercent = 100;
        private bool m_Pending;
        private int m_PrefabsChanged;
        private int m_CompositionsChanged;
        private int m_LanesChanged;
        private int m_CustomEdgesSkipped;

        public bool IsApplying => m_Pending || m_PathEdges.IsCreated;
        public int ProcessedEdges => m_EdgeIndex;
        public int TotalEdges => m_PathEdges.IsCreated ? m_PathEdges.Length : 0;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            DisposeBatch();
            base.OnDestroy();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            DisposeBatch();
            if (mode == GameMode.Game && Mod.Settings != null)
            {
                // Reconcile both categories, including 100%, so an older
                // combined slider cannot leave pedestrian prefabs scaled.
                ScheduleApply(
                    Mod.Settings.GlobalPathSpeedPercent,
                    Mod.Settings.GlobalPedPathSpeedPercent);
            }
        }

        public void ScheduleApply(int bikePercent, int pedestrianPercent)
        {
            m_PendingPercent = math.clamp(bikePercent, 30, 500);
            m_PendingPedestrianPercent = math.clamp(pedestrianPercent, 30, 500);
            DisposeBatch();
            m_Pending = true;
            Enabled = true;
        }

        protected override void OnUpdate()
        {
            try
            {
                if (m_Pending)
                {
                    m_Pending = false;
                    m_PrefabsChanged = 0;
                    m_CompositionsChanged = 0;
                    m_LanesChanged = 0;
                    m_CustomEdgesSkipped = 0;
                    ApplyPrefabSpeeds();
                    BeginBatch();
                }

                if (m_PathEdges.IsCreated)
                {
                    ProcessBatch();
                }

                if (!m_Pending && !m_PathEdges.IsCreated)
                {
                    LogUtils.Info(() =>
                        $"{Mod.ModTag} Global paths bike={m_PendingPercent}% pedestrian={m_PendingPedestrianPercent}%: " +
                        $"prefabs={m_PrefabsChanged} compositions={m_CompositionsChanged} " +
                        $"bikeLanes={m_LanesChanged} customEdgesPreserved={m_CustomEdgesSkipped}.");
                    Enabled = false;
                }
            }
            catch (System.Exception exception)
            {
                LogUtils.Error(() =>
                    $"{Mod.ModTag} Global path speed update failed: {exception.Message}", exception);
                m_Pending = false;
                DisposeBatch();
                Enabled = false;
            }
        }

        private bool TryGetBaseSpeed(Entity prefabEntity, out float vanillaSpeedMs)
        {
            return TryGetBaseSpeed(prefabEntity, out vanillaSpeedMs, out _);
        }

        private bool TryGetBaseSpeed(
            Entity prefabEntity, out float vanillaSpeedMs, out PathCategory category)
        {
            vanillaSpeedMs = 0f;
            category = PathCategory.None;
            if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefab) ||
                !(prefab is PathwayPrefab pathPrefab))
            {
                return false;
            }

            category = GetPathCategory(prefabEntity);
            if (category == PathCategory.None)
            {
                return false;
            }

            vanillaSpeedMs = pathPrefab.m_SpeedLimit / 3.6f;
            return vanillaSpeedMs > 0f;
        }

        private float GetPathScale(PathCategory category)
        {
            return (category == PathCategory.Bike
                ? m_PendingPercent : m_PendingPedestrianPercent) / 100f;
        }

        // A mixed bike/pedestrian path belongs to Bike. Pure pedestrian paths
        // belong to Pedestrian. Any car-capable network fails closed.
        private PathCategory GetPathCategory(Entity prefabEntity)
        {
            if (!EntityManager.HasBuffer<NetGeometrySection>(prefabEntity))
            {
                return PathCategory.None;
            }

            bool hasBicycle = false;
            bool hasPedestrian = false;
            DynamicBuffer<NetGeometrySection> sections =
                EntityManager.GetBuffer<NetGeometrySection>(prefabEntity);
            for (int i = 0; i < sections.Length; i++)
            {
                if (HasVehicleLaneInSection(
                    sections[i].m_Section, ref hasBicycle, ref hasPedestrian, 0))
                {
                    return PathCategory.None;
                }
            }

            if (hasBicycle)
            {
                return PathCategory.Bike;
            }

            return hasPedestrian ? PathCategory.Pedestrian : PathCategory.None;
        }

        private bool HasVehicleLaneInSection(
            Entity sectionEntity, ref bool hasBicycle, ref bool hasPedestrian, int depth)
        {
            // Fail closed if a third-party asset contains unexpected recursion.
            if (depth > 8 || sectionEntity == Entity.Null)
            {
                return true;
            }

            if (EntityManager.HasBuffer<NetSectionPiece>(sectionEntity))
            {
                DynamicBuffer<NetSectionPiece> pieces =
                    EntityManager.GetBuffer<NetSectionPiece>(sectionEntity);
                for (int i = 0; i < pieces.Length; i++)
                {
                    Entity pieceEntity = pieces[i].m_Piece;
                    if (!EntityManager.HasBuffer<NetPieceLane>(pieceEntity))
                    {
                        continue;
                    }

                    DynamicBuffer<NetPieceLane> lanes =
                        EntityManager.GetBuffer<NetPieceLane>(pieceEntity);
                    for (int j = 0; j < lanes.Length; j++)
                    {
                        Entity lanePrefab = lanes[j].m_Lane;
                        if (EntityManager.HasComponent<CarLaneData>(lanePrefab))
                        {
                            RoadTypes types =
                                EntityManager.GetComponentData<CarLaneData>(lanePrefab).m_RoadTypes;
                            if ((types & RoadTypes.Car) != RoadTypes.None)
                            {
                                return true;
                            }

                            if ((types & RoadTypes.Bicycle) != RoadTypes.None)
                            {
                                hasBicycle = true;
                            }
                        }
                        else if (EntityManager.HasComponent<PedestrianLaneData>(lanePrefab))
                        {
                            hasPedestrian = true;
                        }
                    }
                }
            }

            if (EntityManager.HasBuffer<NetSubSection>(sectionEntity))
            {
                DynamicBuffer<NetSubSection> sections =
                    EntityManager.GetBuffer<NetSubSection>(sectionEntity);
                for (int i = 0; i < sections.Length; i++)
                {
                    if (HasVehicleLaneInSection(
                        sections[i].m_SubSection, ref hasBicycle, ref hasPedestrian, depth + 1))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ApplyPrefabSpeeds()
        {
            foreach ((RefRW<PathwayData> data, Entity prefabEntity) in
                SystemAPI.Query<RefRW<PathwayData>>()
                    .WithAll<PrefabData>()
                    .WithNone<Deleted, Temp, Overridden>()
                    .WithEntityAccess())
            {
                if (TryGetBaseSpeed(prefabEntity, out float baseMs, out PathCategory category))
                {
                    float speedMs = baseMs * GetPathScale(category);
                    if (math.abs(data.ValueRO.m_SpeedLimit - speedMs) > 0.0001f)
                    {
                        data.ValueRW.m_SpeedLimit = speedMs;
                        m_PrefabsChanged++;
                    }
                }
            }

            foreach ((RefRW<PathwayComposition> data, RefRO<PrefabRef> reference) in
                SystemAPI.Query<RefRW<PathwayComposition>, RefRO<PrefabRef>>()
                    .WithAll<NetCompositionData>()
                    .WithNone<Deleted, Temp, Overridden>())
            {
                if (TryGetBaseSpeed(reference.ValueRO.m_Prefab, out float baseMs, out PathCategory category))
                {
                    float speedMs = baseMs * GetPathScale(category);
                    if (math.abs(data.ValueRO.m_SpeedLimit - speedMs) > 0.0001f)
                    {
                        data.ValueRW.m_SpeedLimit = speedMs;
                        m_CompositionsChanged++;
                    }
                }
            }
        }

        private void BeginBatch()
        {
            ComponentLookup<PathwayData> pathways =
                SystemAPI.GetComponentLookup<PathwayData>(true);
            using NativeList<Entity> edges = new NativeList<Entity>(Allocator.Temp);
            Dictionary<Entity, bool> eligiblePrefabs = new Dictionary<Entity, bool>();

            foreach ((RefRO<PrefabRef> prefab, DynamicBuffer<Game.Net.SubLane> lanes, Entity edge) in
                SystemAPI.Query<RefRO<PrefabRef>, DynamicBuffer<Game.Net.SubLane>>()
                    .WithAll<Edge>()
                    .WithNone<Road, Deleted, Temp>().WithNone<Overridden>()
                    .WithEntityAccess())
            {
                Entity prefabEntity = prefab.ValueRO.m_Prefab;
                if (lanes.Length == 0 || !pathways.HasComponent(prefabEntity))
                {
                    continue;
                }

                if (!eligiblePrefabs.TryGetValue(prefabEntity, out bool eligible))
                {
                    eligible = TryGetBaseSpeed(prefabEntity, out _, out PathCategory category) &&
                        category == PathCategory.Bike;
                    eligiblePrefabs.Add(prefabEntity, eligible);
                }

                if (eligible)
                {
                    edges.Add(edge);
                }
            }

            if (edges.Length > 0)
            {
                m_PathEdges = new NativeArray<Entity>(edges.Length, Allocator.Persistent);
                NativeArray<Entity>.Copy(edges.AsArray(), m_PathEdges);
            }

            m_EdgeIndex = 0;
        }

        private void ProcessBatch()
        {
            ComponentLookup<PrefabRef> refs = SystemAPI.GetComponentLookup<PrefabRef>(true);
            ComponentLookup<PathwayData> pathways = SystemAPI.GetComponentLookup<PathwayData>(true);
            BufferLookup<Game.Net.SubLane> sublanes = SystemAPI.GetBufferLookup<Game.Net.SubLane>(true);
            ComponentLookup<Game.Net.CarLane> carLanes =
                SystemAPI.GetComponentLookup<Game.Net.CarLane>(false);
            int end = math.min(m_EdgeIndex + kEdgesPerUpdate, m_PathEdges.Length);

            while (m_EdgeIndex < end)
            {
                Entity edge = m_PathEdges[m_EdgeIndex++];
                if (!refs.HasComponent(edge) || !sublanes.HasBuffer(edge))
                {
                    continue;
                }

                Entity prefabEntity = refs[edge].m_Prefab;
                if (!pathways.HasComponent(prefabEntity))
                {
                    continue;
                }

                // Individual ASL changes must not be overwritten by the global multiplier.
                if (EntityManager.HasComponent<CustomSpeed>(edge))
                {
                    m_CustomEdgesSkipped++;
                    continue;
                }

                float speed = pathways[prefabEntity].m_SpeedLimit;
                if (speed <= 0f)
                {
                    continue;
                }

                DynamicBuffer<Game.Net.SubLane> lanes = sublanes[edge];
                for (int i = 0; i < lanes.Length; i++)
                {
                    Entity laneEntity = lanes[i].m_SubLane;
                    if (!carLanes.TryGetComponent(laneEntity, out Game.Net.CarLane lane))
                    {
                        continue;
                    }

                    if (math.abs(lane.m_DefaultSpeedLimit - speed) > 0.0001f ||
                        math.abs(lane.m_SpeedLimit - speed) > 0.0001f)
                    {
                        lane.m_DefaultSpeedLimit = speed;
                        lane.m_SpeedLimit = speed;
                        carLanes[laneEntity] = lane;
                        m_LanesChanged++;
                    }
                }
            }

            if (m_EdgeIndex >= m_PathEdges.Length)
            {
                DisposeBatch();
            }
        }

        private void DisposeBatch()
        {
            if (m_PathEdges.IsCreated)
            {
                m_PathEdges.Dispose();
                m_PathEdges = default;
            }

            m_EdgeIndex = 0;
        }
    }
}
