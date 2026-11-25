// <copyright file="BicycleSpeedSystem.Diagnostics.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Read-only reports adapted from MIT-licensed CS2-Bikes-Paths.
namespace RoadRailSpeeds.Systems
{
    using Colossal.Serialization.Entities;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.Common;
    using Game.Prefabs;
    using Game.Tools;
    using Unity.Entities;
    using Unity.Mathematics;
    using System;
    using System.Collections.Generic;
    using Game.Objects;
    using Game.Vehicles;
    using Unity.Collections;

    public partial class BicycleSpeedSystem
    {
        public void LogPersonalVehicleDiagnostics()
        {
            DumpBikeInstancesReport();
            DumpCarGroupInstancesReport();
            DumpCitizenVehicleEligibilityReport();
        }

        private const int kSampleMax = 10;

        private void DumpCarGroupInstancesReport( )
        {
            LogUtils.Info(( ) =>
                "\n==================== [ASL] CAR GROUP INSTANCES (LIVE) ====================\n" +
                "Meaning: PersonalCar instances excluding BicycleData prefabs.\n" +
                "Status classification:\n" +
                "  Parked  = ParkedCar\n" +
                "  Active  = CarCurrentLane (Parked wins)\n" +
                "  Pending = neither ParkedCar nor CarCurrentLane (diagnostic only)\n" +
                "Hidden types (subset of Parked):\n" +
                "  Hidden in buildings = ParkedCar + Unspawned + Owner + NOT OC lane\n" +
                "  Hidden at border OC = ParkedCar + Unspawned + Owner + OC lane\n" +
                "Notes:\n" +
                "  - Trailers (CarTrailer) are excluded from car-group counts and logged separately.\n");

            // -----------------------------------------------------
            // Build bicycle-group prefab set (BicycleData on prefab).
            // -----------------------------------------------------
            HashSet<Entity> bikeGroupPrefabs = new HashSet<Entity>();

            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PrefabData>>()
                .WithAll<BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                bikeGroupPrefabs.Add(prefabEntity);
            }

            // -----------------------------------------------------------
            // Trailer totals (separate; trailers are not drivable "cars").
            // -----------------------------------------------------------
            int trailerTotal = 0;
            int trailerUnspawned = 0;

            EntityQuery trailerQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Vehicles.PersonalCar, Game.Vehicles.CarTrailer, PrefabRef>()
                .WithNone<Deleted, Temp, Destroyed>()
                .Build();

            using (NativeArray<Entity> trailerEntities = trailerQuery.ToEntityArray(Allocator.Temp))
            {
                for (int i = 0; i < trailerEntities.Length; i++)
                {
                    Entity e = trailerEntities[i];
                    trailerTotal++;

                    if (SystemAPI.HasComponent<Unspawned>(e))
                    {
                        trailerUnspawned++;
                    }
                }
            }

            // --------------------------------------------------------
            // Car-group totals (exclude bike group + exclude trailers).
            // --------------------------------------------------------
            int allLiveTotal = 0;
            int parked = 0;
            int active = 0;
            int pending = 0;

            int statusTotal = 0; // parked + active only

            int unspawnedTotal = 0;
            int unspawnedWithOwner = 0;

            int parkedUnspawnedTotal = 0;
            int hiddenInBuildings = 0;
            int hiddenAtBorderOc = 0;

            List<Entity> hiddenAtBorderHead = new List<Entity>(kSampleMax);
            List<Entity> hiddenAtBorderTail = new List<Entity>(kSampleMax);

            foreach ((RefRO<PrefabRef> prefabRefRO, Entity e) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Game.Vehicles.PersonalCar>()
                .WithNone<Game.Vehicles.CarTrailer>()
                .WithNone<Deleted, Temp, Destroyed>()
                .WithEntityAccess())
            {
                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                // Exclude bicycles + e-scooters (bike row owns those).
                if (bikeGroupPrefabs.Contains(prefabEntity))
                {
                    continue;
                }

                bool hasParked = SystemAPI.HasComponent<ParkedCar>(e);
                bool hasLane = SystemAPI.HasComponent<CarCurrentLane>(e);

                bool hasUnspawned = SystemAPI.HasComponent<Unspawned>(e);

                bool hasOwner = false;
                if (SystemAPI.HasComponent<Owner>(e))
                {
                    Owner o = SystemAPI.GetComponent<Owner>(e);
                    hasOwner = o.m_Owner != Entity.Null;
                }

                bool isParked = hasParked;
                bool isActive = !isParked && hasLane;
                bool isPending = !isParked && !isActive;

                allLiveTotal++;

                if (isParked)
                {
                    parked++;
                }
                else if (isActive)
                {
                    active++;
                }
                else
                {
                    pending++;
                }

                if (!isPending)
                {
                    statusTotal++;
                }

                if (hasUnspawned)
                {
                    unspawnedTotal++;

                    if (hasOwner)
                    {
                        unspawnedWithOwner++;
                    }
                }

                // Hidden parked classification
                if (isParked && hasUnspawned && hasOwner)
                {
                    parkedUnspawnedTotal++;

                    ParkedCar pc = SystemAPI.GetComponent<ParkedCar>(e);
                    Entity lane = pc.m_Lane;

                    bool isOcLane = IsOutsideConnectionLane(lane);

                    if (isOcLane)
                    {
                        hiddenAtBorderOc++;
                        AddHeadTailSample(e, hiddenAtBorderHead, hiddenAtBorderTail, kSampleMax, kSampleMax);
                    }
                    else
                    {
                        hiddenInBuildings++;
                    }
                }
            }

            LogUtils.Info(( ) =>
            {
                var sb = new System.Text.StringBuilder();

                sb.AppendLine($"[ASL] Trailers: Total={trailerTotal}, Unspawned={trailerUnspawned}");
                sb.AppendLine($"[ASL] CarGroup AllLive: Total={allLiveTotal}, Parked={parked}, Active={active}, Pending={pending}");
                sb.AppendLine($"[ASL] CarGroup StatusTotal(Parked+Active)={statusTotal}");

                sb.AppendLine($"[ASL] Unspawned: Total={unspawnedTotal}, WithOwner={unspawnedWithOwner}");

                sb.AppendLine($"[ASL] Parked&&Unspawned&&Owner: Total={parkedUnspawnedTotal}");
                sb.AppendLine($"[ASL] Hidden split: Buildings={hiddenInBuildings}, BorderOC={hiddenAtBorderOc}");

                sb.Append("[ASL] BorderOC samples Head (Index:Version): ");
                AppendEntitySamples(sb, hiddenAtBorderHead);
                sb.AppendLine();

                sb.Append("[ASL] BorderOC samples Tail (Index:Version): ");
                AppendEntitySamples(sb, hiddenAtBorderTail);
                sb.AppendLine();

                return sb.ToString();
            });
        }

        private void DumpBikeInstancesReport( )
        {
            LogUtils.Info(( ) =>
                "\n==================== [ASL] BIKE INSTANCES (LIVE) ====================\n" +
                "Meaning: live BicycleData vehicles (bikes + e-scooters).\n" +
                "Live excludes: Deleted, Temp, Destroyed.\n" +
                "Classification:\n" +
                "  Parked = ParkedCar\n" +
                "  Active = CarCurrentLane (Parked wins if both)\n" +
                "  Other  = neither ParkedCar nor CarCurrentLane\n");

            HashSet<Entity> groupPrefabs = new HashSet<Entity>();
            HashSet<Entity> scooterPrefabs = new HashSet<Entity>();

            int groupPrefabCount = 0;
            int missingPrefabBase = 0;

            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PrefabData>>()
                .WithAll<BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                groupPrefabCount++;
                groupPrefabs.Add(prefabEntity);

                if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                {
                    missingPrefabBase++;
                    continue;
                }

                string name = prefabBase.name ?? string.Empty;
                if (IsScooterName(name))
                {
                    scooterPrefabs.Add(prefabEntity);
                }
            }

            if (groupPrefabs.Count == 0)
            {
                LogUtils.Info(( ) => "[ASL] Bike instances: BicycleData prefab set is empty.");
                return;
            }

            int total = 0;
            int active = 0;
            int parked = 0;
            int other = 0;

            int bikesTotal = 0;
            int scootersTotal = 0;

            int bikesActive = 0;
            int bikesParked = 0;

            int scootersActive = 0;
            int scootersParked = 0;

            int unspawnedTotal = 0;
            int parkedUnspawned = 0;
            int activeUnspawned = 0;

            foreach ((RefRO<PrefabRef> prefabRefRO, Entity e) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Car>()
                .WithNone<Deleted, Temp, Destroyed>()
                .WithEntityAccess())
            {
                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                if (!groupPrefabs.Contains(prefabEntity))
                {
                    continue;
                }

                bool isScooter = scooterPrefabs.Contains(prefabEntity);

                bool hasParked = SystemAPI.HasComponent<ParkedCar>(e);
                bool hasLane = SystemAPI.HasComponent<CarCurrentLane>(e);
                bool hasUnspawned = SystemAPI.HasComponent<Unspawned>(e);

                bool isParked = hasParked;
                bool isActive = !isParked && hasLane;

                total++;

                if (isScooter)
                {
                    scootersTotal++;
                }
                else
                {
                    bikesTotal++;
                }

                if (isParked)
                {
                    parked++;

                    if (isScooter)
                    {
                        scootersParked++;
                    }
                    else
                    {
                        bikesParked++;
                    }
                }
                else if (isActive)
                {
                    active++;

                    if (isScooter)
                    {
                        scootersActive++;
                    }
                    else
                    {
                        bikesActive++;
                    }
                }
                else
                {
                    other++;
                }

                if (hasUnspawned)
                {
                    unspawnedTotal++;

                    if (isParked)
                    {
                        parkedUnspawned++;
                    }
                    else if (isActive)
                    {
                        activeUnspawned++;
                    }
                }
            }

            LogUtils.Info(( ) =>
            {
                var sb = new System.Text.StringBuilder();

                sb.AppendLine($"[ASL] Bicycle-group prefabs: Count={groupPrefabCount}, MissingPrefabBase={missingPrefabBase}");
                sb.AppendLine($"[ASL] BikeGroup: Total={total}, Active={active}, Parked={parked}, Other={other}");
                sb.AppendLine($"[ASL] Split: Bikes={bikesTotal} (A={bikesActive}, P={bikesParked}), Scooters={scootersTotal} (A={scootersActive}, P={scootersParked})");
                sb.AppendLine($"[ASL] Unspawned: Total={unspawnedTotal}, Parked={parkedUnspawned}, Active={activeUnspawned}");
                return sb.ToString();
            });
        }

        private bool IsOutsideConnectionLane(Entity laneEntity)
        {
            if (laneEntity == Entity.Null)
            {
                return false;
            }

            // Direct marker seen in SE screenshots.
            if (SystemAPI.HasComponent<Game.Net.OutsideConnection>(laneEntity))
            {
                return true;
            }

            // Lane itself often has ConnectionLane component.
            if (!SystemAPI.HasComponent<Game.Net.ConnectionLane>(laneEntity))
            {
                return false;
            }

            // Owner-chain probe (limited) for OutsideConnection marker.
            Entity cur = laneEntity;
            for (int i = 0; i < 6; i++)
            {
                if (SystemAPI.HasComponent<Game.Net.OutsideConnection>(cur))
                {
                    return true;
                }

                if (!SystemAPI.HasComponent<Owner>(cur))
                {
                    break;
                }

                Owner o = SystemAPI.GetComponent<Owner>(cur);
                if (o.m_Owner == Entity.Null)
                {
                    break;
                }

                cur = o.m_Owner;
            }

            return false;
        }

        private static void AddHeadTailSample(Entity e, List<Entity> head, List<Entity> tail, int headMax, int tailMax)
        {
            if (head.Count < headMax)
            {
                head.Add(e);
            }

            if (tail.Count == tailMax)
            {
                tail.RemoveAt(0);
            }

            tail.Add(e);
        }

        private static void AppendEntitySamples(System.Text.StringBuilder sb, List<Entity> items)
        {
            if (items == null || items.Count == 0)
            {
                sb.Append("<none>");
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                Entity e = items[i];
                sb.Append(e.Index);
                sb.Append(':');
                sb.Append(e.Version);
            }
        }

        private void DumpCitizenVehicleEligibilityReport( )
        {
            LogUtils.Info(( ) =>
                "\n==================== [ASL] CITIZEN VEHICLE ELIGIBILITY ====================\n" +
                "Meaning: counts citizens and whether BicycleOwner / CarKeeper components are present+enabled.\n" +
                "This does NOT measure bike usage; it only shows eligibility flags.\n");

            const int kMaxSamples = 10;

            int citizenTotal = 0;

            int bicycleOwnerPresent = 0;
            int bicycleOwnerEnabled = 0;
            int bicycleOwnerEnabledNullBike = 0;

            int carKeeperPresent = 0;
            int carKeeperEnabled = 0;

            int bothPresent = 0;
            int bothEnabled = 0;

            var sampleBikeEnabled = new List<Entity>(kMaxSamples);
            var sampleCarEnabled = new List<Entity>(kMaxSamples);
            var sampleBothEnabled = new List<Entity>(kMaxSamples);

            EntityQuery q = SystemAPI.QueryBuilder()
                .WithAll<Game.Citizens.Citizen>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp, Game.Common.Destroyed>()
                .Build();

            using (NativeArray<Entity> citizens = q.ToEntityArray(Allocator.Temp))
            {
                for (int i = 0; i < citizens.Length; i++)
                {
                    Entity c = citizens[i];
                    citizenTotal++;

                    bool hasBikeOwner = EntityManager.HasComponent<Game.Citizens.BicycleOwner>(c);
                    bool hasCarKeeper = EntityManager.HasComponent<Game.Citizens.CarKeeper>(c);

                    if (hasBikeOwner)
                    {
                        bicycleOwnerPresent++;

                        if (EntityManager.IsComponentEnabled<Game.Citizens.BicycleOwner>(c))
                        {
                            bicycleOwnerEnabled++;

                            Game.Citizens.BicycleOwner bo = EntityManager.GetComponentData<Game.Citizens.BicycleOwner>(c);
                            if (bo.m_Bicycle == Entity.Null)
                            {
                                bicycleOwnerEnabledNullBike++;
                            }

                            if (sampleBikeEnabled.Count < kMaxSamples)
                            {
                                sampleBikeEnabled.Add(c);
                            }
                        }
                    }

                    if (hasCarKeeper)
                    {
                        carKeeperPresent++;

                        if (EntityManager.IsComponentEnabled<Game.Citizens.CarKeeper>(c))
                        {
                            carKeeperEnabled++;

                            if (sampleCarEnabled.Count < kMaxSamples)
                            {
                                sampleCarEnabled.Add(c);
                            }
                        }
                    }

                    if (hasBikeOwner && hasCarKeeper)
                    {
                        bothPresent++;

                        bool bikeEnabledNow = EntityManager.IsComponentEnabled<Game.Citizens.BicycleOwner>(c);
                        bool carEnabledNow = EntityManager.IsComponentEnabled<Game.Citizens.CarKeeper>(c);

                        if (bikeEnabledNow && carEnabledNow)
                        {
                            bothEnabled++;

                            if (sampleBothEnabled.Count < kMaxSamples)
                            {
                                sampleBothEnabled.Add(c);
                            }
                        }
                    }
                }
            }

            LogUtils.Info(( ) =>
            {
                var sb = new System.Text.StringBuilder();

                sb.AppendLine($"[ASL] Citizens: Total={citizenTotal}");

                sb.AppendLine($"[ASL] BicycleOwner: Present={bicycleOwnerPresent}, Enabled={bicycleOwnerEnabled}, EnabledWithNullBike={bicycleOwnerEnabledNullBike}");
                sb.AppendLine($"[ASL] CarKeeper: Present={carKeeperPresent}, Enabled={carKeeperEnabled}");

                sb.AppendLine($"[ASL] Both components: Present={bothPresent}, BothEnabled={bothEnabled}");

                sb.Append("[ASL] Sample BicycleOwner Enabled (Citizen Index:Version): ");
                AppendEntitySamples(sb, sampleBikeEnabled);
                sb.AppendLine();

                sb.Append("[ASL] Sample CarKeeper Enabled (Citizen Index:Version): ");
                AppendEntitySamples(sb, sampleCarEnabled);
                sb.AppendLine();

                sb.Append("[ASL] Sample BothEnabled (Citizen Index:Version): ");
                AppendEntitySamples(sb, sampleBothEnabled);
                sb.AppendLine();

                return sb.ToString();
            });

            static void AppendEntitySamples(System.Text.StringBuilder sb, List<Entity> items)
            {
                if (items == null || items.Count == 0)
                {
                    sb.Append("<none>");
                    return;
                }

                for (int i = 0; i < items.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }

                    Entity e = items[i];
                    sb.Append(e.Index);
                    sb.Append(':');
                    sb.Append(e.Version);
                }
            }
        }


        private static bool IsScooterName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return name.StartsWith("ElectricScooter", StringComparison.OrdinalIgnoreCase) ||
                   name.StartsWith("Scooter", StringComparison.OrdinalIgnoreCase);
        }

    }
}
