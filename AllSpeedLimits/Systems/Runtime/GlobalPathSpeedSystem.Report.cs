// <copyright file="GlobalPathSpeedSystem.Report.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// One-click, read-only diagnostics. Never scans while the panel is idle.
namespace RoadRailSpeeds.Systems
{
    using CS2Shared.RiverMochi;
    using Game.Common;
    using Game.Net;
    using Game.Prefabs;
    using Game.Tools;
    using RoadRailSpeeds.Components;
    using Unity.Entities;

    public partial class GlobalPathSpeedSystem
    {
        public void LogPathReport()
        {
            int prefabs = 0;
            int edges = 0;
            int customEdges = 0;
            int cars = 0;
            int pedestrian = 0;
            int samples = 0;
            LogUtils.Info(() =>
                $"{Mod.ModTag} Path speed report BEGIN percent={Mod.Settings?.GlobalPathSpeedPercent ?? 100} busy={IsApplying}.");

            foreach ((RefRO<PathwayData> data, Entity entity) in
                SystemAPI.Query<RefRO<PathwayData>>()
                    .WithAll<PrefabData>().WithNone<Deleted, Temp, Overridden>().WithEntityAccess())
            {
                prefabs++;
                if (samples < 5 && TryGetBaseSpeed(entity, out float vanillaMs))
                {
                    float current = data.ValueRO.m_SpeedLimit * 3.6f;
                    float vanilla = vanillaMs * 3.6f;
                    LogUtils.Info(() =>
                        $"{Mod.ModTag} Path prefab sample: {entity.Index}:{entity.Version} vanillaKmh={vanilla:0.##} pathwayKmh={current:0.##}.");
                    samples++;
                }
            }

            ComponentLookup<PathwayData> pathData = SystemAPI.GetComponentLookup<PathwayData>(true);
            ComponentLookup<Game.Net.CarLane> carLanes = SystemAPI.GetComponentLookup<Game.Net.CarLane>(true);
            ComponentLookup<Game.Net.PedestrianLane> pedLanes = SystemAPI.GetComponentLookup<Game.Net.PedestrianLane>(true);

            foreach ((RefRO<PrefabRef> reference, DynamicBuffer<Game.Net.SubLane> lanes, Entity edge) in
                SystemAPI.Query<RefRO<PrefabRef>, DynamicBuffer<Game.Net.SubLane>>()
                    .WithAll<Edge>().WithNone<Road, Deleted, Temp, Overridden>().WithEntityAccess())
            {
                if (!pathData.HasComponent(reference.ValueRO.m_Prefab))
                {
                    continue;
                }

                edges++;
                bool custom = EntityManager.HasComponent<CustomSpeed>(edge);
                if (custom) customEdges++;
                for (int i = 0; i < lanes.Length; i++)
                {
                    Entity laneEntity = lanes[i].m_SubLane;
                    if (carLanes.TryGetComponent(laneEntity, out Game.Net.CarLane lane))
                    {
                        cars++;
                        if (samples < 13)
                        {
                            float speed = lane.m_SpeedLimit * 1.8f;
                            float defaultSpeed = lane.m_DefaultSpeedLimit * 1.8f;
                            LogUtils.Info(() =>
                                $"{Mod.ModTag} Path lane sample: edge={edge.Index}:{edge.Version} " +
                                $"lane={laneEntity.Index}:{laneEntity.Version} custom={custom} " +
                                $"ASLkmh={speed:0.##} defaultASLkmh={defaultSpeed:0.##}.");
                            samples++;
                        }
                    }
                    else if (pedLanes.HasComponent(laneEntity))
                    {
                        pedestrian++;
                    }
                }
            }

            LogUtils.Info(() =>
                $"{Mod.ModTag} Path speed report END prefabs={prefabs} edges={edges} customEdges={customEdges} " +
                $"bikeCapableLanes={cars} pedestrianOnlyLanes={pedestrian} samples={samples}.");
        }
    }
}
