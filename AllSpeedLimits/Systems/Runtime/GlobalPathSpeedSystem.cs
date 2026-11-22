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
// Covers visible/invisible PathwayPrefab compositions and existing bicycle-capable
// CarLane components. Individual ASL CustomSpeed edges always take precedence.
namespace RoadRailSpeeds.Systems
{
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

        private PrefabSystem m_PrefabSystem = null!;
        private NativeArray<Entity> m_PathEdges;
        private int m_EdgeIndex;
        private int m_PendingPercent = 100;
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
            if (mode == GameMode.Game && Mod.Settings != null &&
                Mod.Settings.GlobalPathSpeedPercent != 100)
            {
                ScheduleApply(Mod.Settings.GlobalPathSpeedPercent);
            }
        }

        public void ScheduleApply(int percent)
        {
            m_PendingPercent = math.clamp(percent, 100, 500);
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
                    ApplyPrefabSpeeds(m_PendingPercent / 100f);
                    BeginBatch();
                }

                if (m_PathEdges.IsCreated)
                {
                    ProcessBatch();
                }

                if (!m_Pending && !m_PathEdges.IsCreated)
                {
                    LogUtils.Info(() =>
                        $"{Mod.ModTag} Global paths {m_PendingPercent}%: prefabs={m_PrefabsChanged} " +
                        $"compositions={m_CompositionsChanged} lanes={m_LanesChanged} " +
                        $"customEdgesPreserved={m_CustomEdgesSkipped}. " +
                        "Pathway prefab/composition data and applicable bicycle lanes updated.");
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
            vanillaSpeedMs = 0f;
            if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefab) ||
                !(prefab is PathwayPrefab pathPrefab))
            {
                return false;
            }

            vanillaSpeedMs = pathPrefab.m_SpeedLimit / 3.6f;
            return vanillaSpeedMs > 0f;
        }

        private void ApplyPrefabSpeeds(float multiplier)
        {
            foreach ((RefRW<PathwayData> data, Entity prefabEntity) in
                SystemAPI.Query<RefRW<PathwayData>>()
                    .WithAll<PrefabData>()
                    .WithNone<Deleted, Temp, Overridden>()
                    .WithEntityAccess())
            {
                if (TryGetBaseSpeed(prefabEntity, out float baseMs))
                {
                    float speedMs = baseMs * multiplier;
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
                if (TryGetBaseSpeed(reference.ValueRO.m_Prefab, out float baseMs))
                {
                    float speedMs = baseMs * multiplier;
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

            foreach ((RefRO<PrefabRef> prefab, DynamicBuffer<Game.Net.SubLane> lanes, Entity edge) in
                SystemAPI.Query<RefRO<PrefabRef>, DynamicBuffer<Game.Net.SubLane>>()
                    .WithAll<Edge>()
                    .WithNone<Road, Deleted, Temp>().WithNone<Overridden>()
                    .WithEntityAccess())
            {
                if (lanes.Length > 0 && pathways.HasComponent(prefab.ValueRO.m_Prefab))
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
