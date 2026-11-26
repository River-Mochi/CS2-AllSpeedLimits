// <copyright file="PathSpeedOverrideSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

namespace RoadRailSpeeds.Systems
{
    using Game;
    using Game.Common;
    using Game.Net;
    using Game.Prefabs;
    using Game.Tools;
    using RoadRailSpeeds.Components;
    using Unity.Burst;
    using Unity.Burst.Intrinsics;
    using Unity.Collections;
    using Unity.Entities;
    using UnityEngine.Scripting;

    // Global path tuning can write lanes without Updated/PathfindUpdated. Explicit ASL
    // segment choices win, without depending on or changing the other mod's settings.
    public partial class PathSpeedOverrideSystem : GameSystemBase
    {
        private EntityQuery m_CustomPaths;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            m_CustomPaths = SystemAPI.QueryBuilder()
                .WithAll<CustomSpeed, Edge, PrefabRef, Game.Net.SubLane>()
                .WithNone<Road, TrainTrack, TramTrack, SubwayTrack, Waterway>()
                .WithNone<Deleted, Temp, Overridden>()
                .Build();
            RequireForUpdate(m_CustomPaths);
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return 64;
        }

        [Preserve]
        protected override void OnUpdate()
        {
            Dependency = new RestorePathsJob
            {
                Speeds = SystemAPI.GetComponentTypeHandle<CustomSpeed>(true),
                Prefabs = SystemAPI.GetComponentTypeHandle<PrefabRef>(true),
                SubLanes = SystemAPI.GetBufferTypeHandle<Game.Net.SubLane>(true),
                Pathways = SystemAPI.GetComponentLookup<PathwayData>(true),
                CarLanes = SystemAPI.GetComponentLookup<Game.Net.CarLane>()
            }.Schedule(m_CustomPaths, Dependency);
        }

        [BurstCompile]
        private struct RestorePathsJob : IJobChunk
        {
            [ReadOnly] public ComponentTypeHandle<CustomSpeed> Speeds;
            [ReadOnly] public ComponentTypeHandle<PrefabRef> Prefabs;
            [ReadOnly] public BufferTypeHandle<Game.Net.SubLane> SubLanes;
            [ReadOnly] public ComponentLookup<PathwayData> Pathways;
            public ComponentLookup<Game.Net.CarLane> CarLanes;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                NativeArray<CustomSpeed> speeds = chunk.GetNativeArray(ref Speeds);
                NativeArray<PrefabRef> prefabs = chunk.GetNativeArray(ref Prefabs);
                BufferAccessor<Game.Net.SubLane> buffers = chunk.GetBufferAccessor(ref SubLanes);
                for (int i = 0; i < chunk.Count; i++)
                {
                    if (!Pathways.HasComponent(prefabs[i].m_Prefab))
                    {
                        continue;
                    }

                    float speed = speeds[i].m_Speed / 1.8f;
                    DynamicBuffer<Game.Net.SubLane> lanes = buffers[i];
                    for (int j = 0; j < lanes.Length; j++)
                    {
                        Entity entity = lanes[j].m_SubLane;
                        if (CarLanes.TryGetComponent(entity, out Game.Net.CarLane lane) &&
                            (lane.m_DefaultSpeedLimit != speed || lane.m_SpeedLimit != speed))
                        {
                            lane.m_DefaultSpeedLimit = speed;
                            lane.m_SpeedLimit = speed;
                            CarLanes[entity] = lane;
                        }
                    }
                }
            }
        }
    }
}
