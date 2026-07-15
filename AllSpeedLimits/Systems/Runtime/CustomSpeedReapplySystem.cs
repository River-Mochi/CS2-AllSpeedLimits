// <copyright file="CustomSpeedReapplySystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Runtime/CustomSpeedReapplySystem.cs
// Purpose: Re-applies CustomSpeed values after CS2 rebuilds road/rail lane entities.

namespace RoadRailSpeeds.Systems
{
    using Game;
    using Game.Common;
    using Game.Net;
    using RoadRailSpeeds.Components;
    using Unity.Burst;
    using Unity.Burst.Intrinsics;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using UnityEngine.Scripting;
    using Temp = Game.Tools.Temp;

    public partial class CustomSpeedReapplySystem : GameSystemBase
    {
        private EntityQuery m_EntitiesToRestoreQuery;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();

            m_EntitiesToRestoreQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<CustomSpeed>(),
                    ComponentType.ReadOnly<Updated>(),
                    ComponentType.ReadOnly<SubLane>()
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>()
                }
            });

            RequireForUpdate(m_EntitiesToRestoreQuery);
        }

        [Preserve]
        protected override void OnUpdate()
        {
            RestoreSpeedJob job = new RestoreSpeedJob
            {
                CustomSpeedType = GetComponentTypeHandle<CustomSpeed>(isReadOnly: true),
                SubLaneType = GetBufferTypeHandle<SubLane>(isReadOnly: true),
                CarLaneLookup = GetComponentLookup<CarLane>(),
                TrackLaneLookup = GetComponentLookup<TrackLane>()
            };

            // This system is registered in CS2's ModificationEnd phase. The job only updates
            // lane data, so it needs no generic Unity simulation ECB and must not remove the
            // game-owned Updated tag; Game.Common.CleanUpSystem removes that tag later.
            Dependency = job.Schedule(m_EntitiesToRestoreQuery, Dependency);
        }

        [BurstCompile]
        private struct RestoreSpeedJob : IJobChunk
        {
            [ReadOnly]
            public ComponentTypeHandle<CustomSpeed> CustomSpeedType;

            [ReadOnly]
            public BufferTypeHandle<SubLane> SubLaneType;

            public ComponentLookup<CarLane> CarLaneLookup;

            public ComponentLookup<TrackLane> TrackLaneLookup;

            public void Execute(
                in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                NativeArray<CustomSpeed> customSpeeds = chunk.GetNativeArray(ref CustomSpeedType);
                BufferAccessor<SubLane> subLaneBuffers = chunk.GetBufferAccessor(ref SubLaneType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    float speedGameUnits = customSpeeds[i].m_Speed / 1.8f;
                    DynamicBuffer<SubLane> subLanes = subLaneBuffers[i];

                    for (int j = 0; j < subLanes.Length; j++)
                    {
                        Entity laneEntity = subLanes[j].m_SubLane;
                        if (CarLaneLookup.HasComponent(laneEntity))
                        {
                            CarLane carLane = CarLaneLookup[laneEntity];
                            carLane.m_DefaultSpeedLimit = speedGameUnits;
                            carLane.m_SpeedLimit = speedGameUnits;
                            CarLaneLookup[laneEntity] = carLane;
                        }
                        else if (TrackLaneLookup.HasComponent(laneEntity))
                        {
                            TrackLane trackLane = TrackLaneLookup[laneEntity];
                            trackLane.m_SpeedLimit = speedGameUnits;
                            TrackLaneLookup[laneEntity] = trackLane;
                        }
                    }
                }
            }
        }
    }
}
