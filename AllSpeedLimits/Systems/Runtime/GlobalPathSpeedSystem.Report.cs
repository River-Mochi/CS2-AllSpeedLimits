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
            int matchingPrefabs = 0;
            int mismatchedPrefabs = 0;
            int bikePrefabs = 0;
            int pedestrianPrefabs = 0;
            int compositions = 0;
            int matchingCompositions = 0;
            int mismatchedCompositions = 0;
            int bikeCompositions = 0;
            int pedestrianCompositions = 0;
            int bikePathEdges = 0;
            int pedestrianPathEdges = 0;
            int excludedPathEdges = 0;
            int excludedUnknownEdges = 0;
            int excludedCarLanes = 0;
            int customEdges = 0;
            int cars = 0;
            int matchingCarLanes = 0;
            int mismatchedCarLanes = 0;
            int mismatchedVehicleLanes = 0;
            int mismatchedBicycleLanes = 0;
            int mismatchedOtherLanes = 0;
            int mismatchSamples = 0;
            int pedestrian = 0;
            int samples = 0;
            int bikePercent = Mod.Settings?.GlobalPathSpeedPercent ?? 100;
            int pedestrianPercent = Mod.Settings?.GlobalPedPathSpeedPercent ?? 100;
            const float toleranceMs = 0.001f;
            LogUtils.Info(() =>
                $"{Mod.ModTag} Path speed report BEGIN bikePercent={bikePercent} " +
                $"pedestrianPercent={pedestrianPercent} busy={IsApplying} " +
                "expectedPathwayMs=vanillaPrefabKmh/3.6*categoryPercent/100; " +
                "CarLane m/s, ASL labels use x1.8.");

            foreach ((RefRO<PathwayData> data, Entity entity) in
                SystemAPI.Query<RefRO<PathwayData>>()
                    .WithAll<PrefabData>().WithNone<Deleted, Temp, Overridden>().WithEntityAccess())
            {
                if (!TryGetBaseSpeed(entity, out float vanillaMs, out PathCategory category))
                {
                    continue;
                }

                prefabs++;
                if (category == PathCategory.Bike) bikePrefabs++;
                else pedestrianPrefabs++;
                float expectedMs = vanillaMs *
                    (category == PathCategory.Bike ? bikePercent : pedestrianPercent) / 100f;
                float actualMs = data.ValueRO.m_SpeedLimit;
                if (System.Math.Abs(actualMs - expectedMs) <= toleranceMs)
                {
                    matchingPrefabs++;
                }
                else
                {
                    mismatchedPrefabs++;
                }

                if (samples < 5)
                {
                    float current = actualMs * 3.6f;
                    float expected = expectedMs * 3.6f;
                    float vanilla = vanillaMs * 3.6f;
                    string prefabName = m_PrefabSystem.TryGetPrefab(entity, out PrefabBase pathPrefab)
                        ? pathPrefab.name : "Unknown";
                    LogUtils.Info(() =>
                        $"{Mod.ModTag} Path prefab sample: category={category} name={prefabName} " +
                        $"entity={entity.Index}:{entity.Version} " +
                        $"vanillaKmh={vanilla:0.##} expectedKmh={expected:0.##} actualKmh={current:0.##} " +
                        $"ratioPercent={actualMs / vanillaMs * 100f:0.#}.");
                    samples++;
                }
            }

            foreach ((RefRO<PathwayComposition> composition, RefRO<PrefabRef> reference) in
                SystemAPI.Query<RefRO<PathwayComposition>, RefRO<PrefabRef>>()
                    .WithAll<NetCompositionData>()
                    .WithNone<Deleted, Temp, Overridden>())
            {
                if (!TryGetBaseSpeed(reference.ValueRO.m_Prefab, out float vanillaMs, out PathCategory category))
                {
                    continue;
                }

                compositions++;
                if (category == PathCategory.Bike) bikeCompositions++;
                else pedestrianCompositions++;
                float expectedMs = vanillaMs *
                    (category == PathCategory.Bike ? bikePercent : pedestrianPercent) / 100f;
                if (System.Math.Abs(composition.ValueRO.m_SpeedLimit - expectedMs) <= toleranceMs)
                {
                    matchingCompositions++;
                }
                else
                {
                    mismatchedCompositions++;
                }
            }

            ComponentLookup<PathwayData> pathData = SystemAPI.GetComponentLookup<PathwayData>(true);
            ComponentLookup<Game.Net.CarLane> carLanes = SystemAPI.GetComponentLookup<Game.Net.CarLane>(true);
            ComponentLookup<Game.Net.PedestrianLane> pedLanes = SystemAPI.GetComponentLookup<Game.Net.PedestrianLane>(true);
            ComponentLookup<PrefabRef> laneReferences = SystemAPI.GetComponentLookup<PrefabRef>(true);
            ComponentLookup<CarLaneData> lanePrefabData = SystemAPI.GetComponentLookup<CarLaneData>(true);

            foreach ((RefRO<PrefabRef> reference, DynamicBuffer<Game.Net.SubLane> lanes, Entity edge) in
                SystemAPI.Query<RefRO<PrefabRef>, DynamicBuffer<Game.Net.SubLane>>()
                    .WithAll<Edge>().WithNone<Road, Deleted, Temp>().WithNone<Overridden>().WithEntityAccess())
            {
                if (!pathData.HasComponent(reference.ValueRO.m_Prefab))
                {
                    continue;
                }

                if (!TryGetBaseSpeed(reference.ValueRO.m_Prefab, out _, out PathCategory category))
                {
                    // Exclude vehicle roads and unsupported geometry from both
                    // global sliders, preserving their original lane speeds.
                    excludedPathEdges++;
                    bool containsCarLane = false;
                    for (int i = 0; i < lanes.Length; i++)
                    {
                        Entity excludedLane = lanes[i].m_SubLane;
                        if (carLanes.TryGetComponent(excludedLane, out Game.Net.CarLane excluded))
                        {
                            containsCarLane = true;
                            if (excludedCarLanes == 0)
                            {
                                float preservedKmh = excluded.m_SpeedLimit * 1.8f;
                                LogUtils.Info(() =>
                                    $"{Mod.ModTag} Excluded vehicle pathway sample: " +
                                    $"edge={edge.Index}:{edge.Version} " +
                                    $"preservedLaneASLkmh={preservedKmh:0.##}.");
                            }
                            excludedCarLanes++;
                        }
                    }
                    if (!containsCarLane)
                    {
                        excludedUnknownEdges++;
                    }
                    continue;
                }

                if (category == PathCategory.Bike) bikePathEdges++;
                else pedestrianPathEdges++;
                bool custom = EntityManager.HasComponent<CustomSpeed>(edge);
                if (custom) customEdges++;
                for (int i = 0; i < lanes.Length; i++)
                {
                    Entity laneEntity = lanes[i].m_SubLane;
                    if (category == PathCategory.Bike &&
                        carLanes.TryGetComponent(laneEntity, out Game.Net.CarLane lane))
                    {
                        cars++;
                        float expectedSpeed = pathData[reference.ValueRO.m_Prefab].m_SpeedLimit;
                        bool matches = System.Math.Abs(lane.m_SpeedLimit - expectedSpeed) <= toleranceMs &&
                            System.Math.Abs(lane.m_DefaultSpeedLimit - expectedSpeed) <= toleranceMs;
                        if (!custom)
                        {
                            if (matches)
                            {
                                matchingCarLanes++;
                            }
                            else
                            {
                                mismatchedCarLanes++;

                                // Diagnose the baseline lane differences before any global
                                // Apply normalizes them. Distinguish hidden vehicle roads
                                // from bicycle lanes; never modify components in this report.
                                RoadTypes roadTypes = RoadTypes.None;
                                Entity lanePrefabEntity = Entity.Null;
                                if (laneReferences.TryGetComponent(laneEntity, out PrefabRef laneReference))
                                {
                                    lanePrefabEntity = laneReference.m_Prefab;
                                    if (lanePrefabData.HasComponent(lanePrefabEntity))
                                    {
                                        roadTypes = lanePrefabData[lanePrefabEntity].m_RoadTypes;
                                    }
                                }

                                if ((roadTypes & RoadTypes.Car) != RoadTypes.None)
                                {
                                    mismatchedVehicleLanes++;
                                }
                                else if ((roadTypes & RoadTypes.Bicycle) != RoadTypes.None)
                                {
                                    mismatchedBicycleLanes++;
                                }
                                else
                                {
                                    mismatchedOtherLanes++;
                                }

                                if (mismatchSamples < 12)
                                {
                                    string edgeName = m_PrefabSystem.TryGetPrefab(
                                        reference.ValueRO.m_Prefab, out PrefabBase edgePrefab)
                                        ? edgePrefab.name : "Unknown";
                                    string laneName = lanePrefabEntity != Entity.Null &&
                                        m_PrefabSystem.TryGetPrefab(lanePrefabEntity, out PrefabBase lanePrefab)
                                        ? lanePrefab.name : "Unknown";
                                    float aslkMh = lane.m_SpeedLimit * 1.8f;
                                    float defaultAslKmh = lane.m_DefaultSpeedLimit * 1.8f;
                                    float expectedAslKmh = expectedSpeed * 1.8f;
                                    LogUtils.Info(() =>
                                        $"{Mod.ModTag} Path mismatch sample: edgePrefab={edgeName} " +
                                        $"lanePrefab={laneName} roadTypes={roadTypes} " +
                                        $"ASLkmh={aslkMh:0.##} laneDefaultASLkmh={defaultAslKmh:0.##} " +
                                        $"expectedASLkmh={expectedAslKmh:0.##}.");
                                    mismatchSamples++;
                                }
                            }
                        }

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
                $"{Mod.ModTag} Path speed report END prefabs={prefabs} " +
                $"bikePrefabs={bikePrefabs} pedestrianPrefabs={pedestrianPrefabs} " +
                $"prefabMatches={matchingPrefabs} prefabMismatches={mismatchedPrefabs} " +
                $"compositions={compositions} bikeCompositions={bikeCompositions} " +
                $"pedestrianCompositions={pedestrianCompositions} " +
                $"compositionMatches={matchingCompositions} compositionMismatches={mismatchedCompositions} " +
                $"bikePathEdges={bikePathEdges} pedestrianPathEdges={pedestrianPathEdges} " +
                $"excludedPathEdges={excludedPathEdges} excludedUnknownEdges={excludedUnknownEdges} " +
                $"excludedVehicleLanesPreserved={excludedCarLanes} " +
                $"customEdges={customEdges} bikeCapableLanes={cars} " +
                $"nonCustomLaneMatches={matchingCarLanes} nonCustomLaneMismatches={mismatchedCarLanes} " +
                $"mismatchVehicleLanes={mismatchedVehicleLanes} mismatchBicycleLanes={mismatchedBicycleLanes} " +
                $"mismatchOtherLanes={mismatchedOtherLanes} " +
                $"pedestrianLanesOnBikePaths={pedestrian} samples={samples} mismatchSamples={mismatchSamples}.");
        }
    }
}
