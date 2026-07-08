// <copyright file="CustomSpeedReapplySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Runtime/CustomSpeedReapplySystem.cs
// Purpose: Re-applies CustomSpeed values to updated road/rail lane entities.

namespace RoadRailSpeeds.Systems
{
    using System.Diagnostics.CodeAnalysis;
    using Game.Common;
    using Game.Net;
    using RoadRailSpeeds.Components;
    using Unity.Burst;
    using Unity.Burst.Intrinsics;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;

    public partial class CustomSpeedReapplySystem : SystemBase
    {
        private EntityQuery m_EntitiesToRestoreQuery;
        private EntityCommandBufferSystem m_CommandBufferSystem = null!;

        protected override void OnCreate()
        {
            base.OnCreate();

            // Modern build form of GetEntityQuery(new EntityQueryDesc{...}); this cached EntityQuery is
            // what the Burst job below schedules against.
            m_EntitiesToRestoreQuery = SystemAPI.QueryBuilder()
                .WithAll<CustomSpeed>()
                .WithAny<Updated>()
                .Build();

            // Command buffer lets the job remove Updated safely after it re-applies speed.
            m_CommandBufferSystem =
                World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer.ParallelWriter commandBuffer =
                m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            JobHandle jobHandle = new RestoreSpeedJob
            {
                EntityType = EntityManager.GetEntityTypeHandle(),
                EntityManager = EntityManager,
                CommandBuffer = commandBuffer
            }.ScheduleParallel(m_EntitiesToRestoreQuery, Dependency);

            m_CommandBufferSystem.AddJobHandleForProducer(jobHandle);
            Dependency = jobHandle;
        }

        [BurstCompile]
        [SuppressMessage(
            "ReSharper",
            "ForCanBeConvertedToForeach",
            Justification = "Burst jobs are clearer and safer with indexed loops.")]
        private struct RestoreSpeedJob : IJobChunk
        {
            [ReadOnly]
            public EntityTypeHandle EntityType;

            [ReadOnly]
            public EntityManager EntityManager;

            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            public void Execute(
                in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                NativeArray<Entity> entities = chunk.GetNativeArray(EntityType);

                for (int i = 0; i < entities.Length; i++)
                {
                    Entity entity = entities[i];

                    if (!EntityManager.HasComponent<CustomSpeed>(entity))
                    {
                        continue;
                    }

                    CustomSpeed customSpeed = EntityManager.GetComponentData<CustomSpeed>(entity);
                    SetSpeed(entity, customSpeed.m_Speed);

                    // Updated means the game touched this entity; remove it after restoring speed.
                    CommandBuffer.RemoveComponent<Updated>(unfilteredChunkIndex, entity);
                }
            }

            private void SetSpeed(Entity entity, float speedKmh)
            {
                if (!EntityManager.HasBuffer<SubLane>(entity))
                {
                    return;
                }

                // Game lane speed uses 2x m/s, so km/h converts by dividing by 1.8.
                float speedGameUnits = speedKmh / 1.8f;

                DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(entity);

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

                    // Preserve flags so lane connections/pathing are not damaged.
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
        }
    }
}
