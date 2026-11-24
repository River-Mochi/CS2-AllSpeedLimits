// <copyright file="PersonalVehicleStatusSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Runtime/PersonalVehicleStatusSystem.cs
// Purpose: Snapshot counts for the Options UI.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;
    using Colossal.Serialization.Entities;
    using Game.Citizens;
    using Game.Tools;
    using Game;
    using System;               // DateTime, StringComparison
    using System.Text;           // Shared partial-class generator imports
    using CS2Shared.RiverMochi; // LogUtils
    using Game.Common;          // Deleted, Destroyed, Owner
    using Game.Objects;         // Unspawned
    using Game.Prefabs;         // BicycleData, PrefabBase, PrefabRef, PrefabSystem
    using Game.Vehicles;        // CarCurrentLane, CarTrailer, ParkedCar, PersonalCar
    using Unity.Collections;    // Allocator, NativeArray
    using Unity.Entities;       // Entity, EntityQuery, SystemAPI

    public partial class PersonalVehicleStatusSystem : GameSystemBase
    {
        public readonly struct Snapshot
        {
            public readonly long BikeGroupTotal;
            public readonly long BikeGroupParked;
            public readonly long BikeGroupActive;
            public readonly long ScooterTotal;
            public readonly long BikeOnlyTotal;

            public readonly long CarGroupTotal;
            public readonly long CarGroupParked;
            public readonly long CarGroupActive;

            public readonly long TrailerTotal;

            // Status row3: TOTAL OC hidden cars (car-group only)
            public readonly long CarHiddenAtBorder;

            // Diagnostic only
            public readonly long CarHiddenInBuildings;

            public readonly DateTime SnapshotTimeLocal;

            public Snapshot(
                long bikeGroupTotal,
                long bikeGroupParked,
                long bikeGroupActive,
                long scooterTotal,
                long bikeOnlyTotal,
                long carGroupTotal,
                long carGroupParked,
                long carGroupActive,
                long trailerTotal,
                long carHiddenAtBorder,
                long carHiddenInBuildings,
                DateTime snapshotTimeLocal)
            {
                BikeGroupTotal = bikeGroupTotal;
                BikeGroupParked = bikeGroupParked;
                BikeGroupActive = bikeGroupActive;
                ScooterTotal = scooterTotal;
                BikeOnlyTotal = bikeOnlyTotal;

                CarGroupTotal = carGroupTotal;
                CarGroupParked = carGroupParked;
                CarGroupActive = carGroupActive;

                TrailerTotal = trailerTotal;
                CarHiddenAtBorder = carHiddenAtBorder;
                CarHiddenInBuildings = carHiddenInBuildings;

                SnapshotTimeLocal = snapshotTimeLocal;
            }
        }

        private PrefabSystem m_PrefabSystem = null!;
        private EntityQuery m_PersonalVehicleQuery;
        private EntityQuery m_TrailerQuery;
        private EntityQuery m_CitizenLocQuery;

        protected override void OnCreate( )
        {
            base.OnCreate();

            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            m_PersonalVehicleQuery = SystemAPI.QueryBuilder()
                .WithAll<PrefabRef, Game.Vehicles.PersonalCar>()
                .WithNone<CarTrailer, Deleted, Temp>()
                .WithNone<Destroyed>()
                .Build();

            m_TrailerQuery = SystemAPI.QueryBuilder()
                .WithAll<PrefabRef, Game.Vehicles.PersonalCar, CarTrailer>()
                .WithNone<Deleted, Temp, Destroyed>()
                .Build();

            m_CitizenLocQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Citizens.Citizen, Game.Citizens.CurrentBuilding, Game.Citizens.HouseholdMember>()
                .WithNone<Deleted, Temp, Destroyed>()
                .Build();

            Enabled = false;
        }

        protected override void OnUpdate( )
        {
        }

        // Builds the snapshot used by Options UI rows.
        // Row3 uses TOTAL OC hidden (ParkedCar + Unspawned + OC lane) for car-group only.
        public Snapshot BuildSnapshot( )
        {
            ComponentLookup<PrefabRef> prefabRefLookup = GetComponentLookup<PrefabRef>(isReadOnly: true);
            ComponentLookup<BicycleData> bicycleDataLookup = GetComponentLookup<BicycleData>(isReadOnly: true);

            ComponentLookup<ParkedCar> parkedLookup = GetComponentLookup<ParkedCar>(isReadOnly: true);
            ComponentLookup<CarCurrentLane> currentLaneLookup = GetComponentLookup<CarCurrentLane>(isReadOnly: true);

            ComponentLookup<Unspawned> unspawnedLookup = GetComponentLookup<Unspawned>(isReadOnly: true);
            ComponentLookup<Owner> ownerLookup = GetComponentLookup<Owner>(isReadOnly: true);

            ComponentLookup<Game.Net.OutsideConnection> outsideConnLookup =
                GetComponentLookup<Game.Net.OutsideConnection>(isReadOnly: true);

            ComponentLookup<Game.Net.ConnectionLane> connLaneLookup =
                GetComponentLookup<Game.Net.ConnectionLane>(isReadOnly: true);

            bool IsOutsideConnectionLane(Entity lane)
            {
                if (lane == Entity.Null)
                {
                    return false;
                }

                if (outsideConnLookup.HasComponent(lane))
                {
                    return true;
                }

                if (!connLaneLookup.HasComponent(lane))
                {
                    return false;
                }

                Entity cur = lane;
                for (int i = 0; i < 6; i++)
                {
                    if (outsideConnLookup.HasComponent(cur))
                    {
                        return true;
                    }

                    if (!ownerLookup.HasComponent(cur))
                    {
                        break;
                    }

                    Owner o = ownerLookup[cur];
                    if (o.m_Owner == Entity.Null)
                    {
                        break;
                    }

                    cur = o.m_Owner;
                }

                return false;
            }

            long bikeGroupTotal = 0;
            long bikeGroupParked = 0;
            long bikeGroupActive = 0;
            long scooterTotal = 0;
            Dictionary<Entity, bool> scooterByPrefab = new Dictionary<Entity, bool>();

            long carGroupTotal = 0;
            long carGroupParked = 0;
            long carGroupActive = 0;

            long totalOcHidden = 0;
            long carHiddenInBuildings = 0;

            using (NativeArray<Entity> entities = m_PersonalVehicleQuery.ToEntityArray(Allocator.Temp))
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Entity e = entities[i];

                    if (!prefabRefLookup.HasComponent(e))
                    {
                        continue;
                    }

                    Entity prefabEntity = prefabRefLookup[e].m_Prefab;
                    if (prefabEntity == Entity.Null)
                    {
                        continue;
                    }

                    bool isParked = parkedLookup.HasComponent(e);
                    bool isActive = !isParked && currentLaneLookup.HasComponent(e);

                    if (!isParked && !isActive)
                    {
                        continue;
                    }

                    bool isBikeGroup = bicycleDataLookup.HasComponent(prefabEntity);

                    if (isBikeGroup)
                    {
                        bikeGroupTotal++;

                        if (isParked)
                            bikeGroupParked++;
                        else
                            bikeGroupActive++;

                        if (!scooterByPrefab.TryGetValue(prefabEntity, out bool isScooter))
                        {
                            isScooter = IsElectricScooterPrefab(prefabEntity);
                            scooterByPrefab.Add(prefabEntity, isScooter);
                        }

                        if (isScooter)
                        {
                            scooterTotal++;
                        }

                        continue;
                    }

                    carGroupTotal++;

                    if (isParked)
                        carGroupParked++;
                    else
                        carGroupActive++;

                    if (isParked && unspawnedLookup.HasComponent(e))
                    {
                        Entity lane = parkedLookup[e].m_Lane;

                        if (IsOutsideConnectionLane(lane))
                        {
                            totalOcHidden++;
                        }
                        else if (ownerLookup.HasComponent(e))
                        {
                            Owner o = ownerLookup[e];
                            if (o.m_Owner != Entity.Null)
                            {
                                carHiddenInBuildings++;
                            }
                        }
                    }
                }
            }

            long trailerTotal = m_TrailerQuery.CalculateEntityCount();

            long bikeOnlyTotal = bikeGroupTotal - scooterTotal;
            if (bikeOnlyTotal < 0)
            {
                bikeOnlyTotal = 0;
            }

            return new Snapshot(
                bikeGroupTotal: bikeGroupTotal,
                bikeGroupParked: bikeGroupParked,
                bikeGroupActive: bikeGroupActive,
                scooterTotal: scooterTotal,
                bikeOnlyTotal: bikeOnlyTotal,
                carGroupTotal: carGroupTotal,
                carGroupParked: carGroupParked,
                carGroupActive: carGroupActive,
                trailerTotal: trailerTotal,
                carHiddenAtBorder: totalOcHidden,
                carHiddenInBuildings: carHiddenInBuildings,
                snapshotTimeLocal: DateTime.Now);
        }

        private bool IsElectricScooterPrefab(Entity prefabEntity)
        {
            if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
            {
                return false;
            }

            string n = prefabBase.name;
            if (string.IsNullOrEmpty(n))
            {
                return false;
            }

            return n.StartsWith("ElectricScooter", StringComparison.OrdinalIgnoreCase);
        }
    }
}

