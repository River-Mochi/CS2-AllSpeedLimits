// <copyright file="SegmentSpeedToolUISystem.Stats.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolUISystem.Stats.cs
// Purpose: Lightweight city vehicle totals and stat-binding resets for the tool panel.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;       // Dictionary, KeyValuePair
    using System.Linq;                      // OrderByDescending, ThenBy

    using CS2Shared.RiverMochi;             // LogUtils

    using Game.Common;                      // Deleted, Destroyed
    using Game.Prefabs;
    using Game.Tools;                       // Temp
    using Game.Vehicles;

    using Unity.Entities;

    public partial class SegmentSpeedToolUISystem
    {
        // Tweak this if the open panel should refresh city vehicle stats more or less often.
        // Use real time instead of frames so low FPS does not make the stats look stale.
        private const float kVehicleStatsRefreshSeconds = 2f;
        private const int kVehicleStatsReportMaxRows = 80;

        private readonly struct CityVehicleStats
        {
            public readonly int CarTotal;
            public readonly int CarActive;
            public readonly int CarParked;
            public readonly int BikeTotal;
            public readonly int BikeActive;
            public readonly int BikeParked;
            public readonly int IndustryTotal;
            public readonly int IndustryActive;
            public readonly int IndustryParked;
            public readonly int BusTotal;
            public readonly int BusActive;
            public readonly int BusParked;
            public readonly int TaxiTotal;
            public readonly int TaxiActive;
            public readonly int TaxiParked;

            public CityVehicleStats(
                int carTotal,
                int carActive,
                int carParked,
                int bikeTotal,
                int bikeActive,
                int bikeParked,
                int industryTotal,
                int industryActive,
                int industryParked,
                int busTotal,
                int busActive,
                int busParked,
                int taxiTotal,
                int taxiActive,
                int taxiParked)
            {
                CarTotal = carTotal;
                CarActive = carActive;
                CarParked = carParked;
                BikeTotal = bikeTotal;
                BikeActive = bikeActive;
                BikeParked = bikeParked;
                IndustryTotal = industryTotal;
                IndustryActive = industryActive;
                IndustryParked = industryParked;
                BusTotal = busTotal;
                BusActive = busActive;
                BusParked = busParked;
                TaxiTotal = taxiTotal;
                TaxiActive = taxiActive;
                TaxiParked = taxiParked;
            }
        }

        private sealed class VehicleReportRow
        {
            public int Active;
            public int Parked;
            public int Other;
        }

        private void RefreshVehicleStatsIfNeeded(bool force)
        {
            float now = UnityEngine.Time.unscaledTime;
            if (!force &&
                m_LastVehicleStatsRefreshTime >= 0f &&
                now - m_LastVehicleStatsRefreshTime < kVehicleStatsRefreshSeconds)
            {
                return;
            }

            m_LastVehicleStatsRefreshTime = now;

            CityVehicleStats stats = BuildCityVehicleStats();

            if (m_CityCarTotalBinding.Value != stats.CarTotal)
            {
                m_CityCarTotalBinding.Value = stats.CarTotal;
            }

            if (m_CityCarActiveBinding.Value != stats.CarActive)
            {
                m_CityCarActiveBinding.Value = stats.CarActive;
            }

            if (m_CityCarParkedBinding.Value != stats.CarParked)
            {
                m_CityCarParkedBinding.Value = stats.CarParked;
            }

            if (m_CityBikeTotalBinding.Value != stats.BikeTotal)
            {
                m_CityBikeTotalBinding.Value = stats.BikeTotal;
            }

            if (m_CityBikeActiveBinding.Value != stats.BikeActive)
            {
                m_CityBikeActiveBinding.Value = stats.BikeActive;
            }

            if (m_CityBikeParkedBinding.Value != stats.BikeParked)
            {
                m_CityBikeParkedBinding.Value = stats.BikeParked;
            }

            if (m_CityIndustryTotalBinding.Value != stats.IndustryTotal)
            {
                m_CityIndustryTotalBinding.Value = stats.IndustryTotal;
            }

            if (m_CityIndustryActiveBinding.Value != stats.IndustryActive)
            {
                m_CityIndustryActiveBinding.Value = stats.IndustryActive;
            }

            if (m_CityIndustryParkedBinding.Value != stats.IndustryParked)
            {
                m_CityIndustryParkedBinding.Value = stats.IndustryParked;
            }

            if (m_CityBusTotalBinding.Value != stats.BusTotal)
            {
                m_CityBusTotalBinding.Value = stats.BusTotal;
            }

            if (m_CityBusActiveBinding.Value != stats.BusActive)
            {
                m_CityBusActiveBinding.Value = stats.BusActive;
            }

            if (m_CityBusParkedBinding.Value != stats.BusParked)
            {
                m_CityBusParkedBinding.Value = stats.BusParked;
            }

            if (m_CityTaxiTotalBinding.Value != stats.TaxiTotal)
            {
                m_CityTaxiTotalBinding.Value = stats.TaxiTotal;
            }

            if (m_CityTaxiActiveBinding.Value != stats.TaxiActive)
            {
                m_CityTaxiActiveBinding.Value = stats.TaxiActive;
            }

            if (m_CityTaxiParkedBinding.Value != stats.TaxiParked)
            {
                m_CityTaxiParkedBinding.Value = stats.TaxiParked;
            }
        }

        private CityVehicleStats BuildCityVehicleStats()
        {
            int carTotal = 0;
            int carActive = 0;
            int carParked = 0;
            int bikeTotal = 0;
            int bikeActive = 0;
            int bikeParked = 0;
            int industryTotal = 0;
            int industryActive = 0;
            int industryParked = 0;
            int busTotal = 0;
            int busActive = 0;
            int busParked = 0;
            int taxiTotal = 0;
            int taxiActive = 0;
            int taxiParked = 0;

            // Read-only count, so the modern DOTS idiom SystemAPI.Query fits: it iterates the matching
            // chunks directly with no NativeArray copy. The query guarantees PrefabRef and PersonalCar
            // and excludes trailers/deleted/destroyed/temp. WithEntityAccess gives the entity for the
            // ParkedCar/CarCurrentLane lookups; BicycleData is checked on the prefab entity.
            foreach (var (prefabRef, vehicle) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Game.Vehicles.PersonalCar>()
                .WithNone<Game.Vehicles.CarTrailer, Deleted, Destroyed>()
                .WithNone<Temp>()
                .WithEntityAccess())
            {
                Entity prefab = prefabRef.ValueRO.m_Prefab;
                if (prefab == Entity.Null)
                {
                    continue;
                }

                bool isParked = SystemAPI.HasComponent<ParkedCar>(vehicle);
                bool isActive = !isParked && SystemAPI.HasComponent<CarCurrentLane>(vehicle);
                if (!isParked && !isActive)
                {
                    continue;
                }

                if (SystemAPI.HasComponent<BicycleData>(prefab))
                {
                    bikeTotal++;
                    if (isParked)
                    {
                        bikeParked++;
                    }
                    else
                    {
                        bikeActive++;
                    }

                    continue;
                }

                carTotal++;
                if (isParked)
                {
                    carParked++;
                }
                else
                {
                    carActive++;
                }
            }

            // Third stats row: all road-using work/service/freight cars, while leaving private cars,
            // bicycles, taxis, public transit, trailers, rail, water, and air vehicles out.
            foreach (var (prefabRef, vehicle) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Game.Vehicles.Vehicle, Game.Vehicles.Car>()
                .WithNone<Game.Vehicles.PersonalCar, Game.Vehicles.PublicTransport, Game.Vehicles.Taxi>()
                .WithNone<Game.Vehicles.CarTrailer, Deleted, Destroyed>()
                .WithNone<Temp>()
                .WithEntityAccess())
            {
                Entity prefab = prefabRef.ValueRO.m_Prefab;
                if (prefab == Entity.Null)
                {
                    continue;
                }

                if (SystemAPI.HasComponent<BicycleData>(prefab) ||
                    SystemAPI.HasComponent<PublicTransportVehicleData>(prefab) ||
                    SystemAPI.HasComponent<TaxiData>(prefab))
                {
                    continue;
                }

                bool isParked = SystemAPI.HasComponent<ParkedCar>(vehicle);
                bool isActive = !isParked && SystemAPI.HasComponent<CarCurrentLane>(vehicle);
                if (!isParked && !isActive)
                {
                    continue;
                }

                industryTotal++;
                if (isParked)
                {
                    industryParked++;
                }
                else
                {
                    industryActive++;
                }
            }

            // Road buses only: public transport vehicles whose prefab declares TransportType.Bus.
            // This keeps trains, trams, subway, ships, and aircraft out of the road stats table.
            foreach (var (prefabRef, vehicle) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Game.Vehicles.Vehicle, Game.Vehicles.Car, Game.Vehicles.PublicTransport>()
                .WithNone<Game.Vehicles.CarTrailer, Deleted, Destroyed>()
                .WithNone<Temp>()
                .WithEntityAccess())
            {
                Entity prefab = prefabRef.ValueRO.m_Prefab;
                if (prefab == Entity.Null ||
                    !SystemAPI.HasComponent<PublicTransportVehicleData>(prefab) ||
                    EntityManager.GetComponentData<PublicTransportVehicleData>(prefab).m_TransportType != TransportType.Bus)
                {
                    continue;
                }

                bool isParked = SystemAPI.HasComponent<ParkedCar>(vehicle);
                bool isActive = !isParked && SystemAPI.HasComponent<CarCurrentLane>(vehicle);
                if (!isParked && !isActive)
                {
                    continue;
                }

                busTotal++;
                if (isParked)
                {
                    busParked++;
                }
                else
                {
                    busActive++;
                }
            }

            // Road taxis only. Count both live Taxi component and TaxiData prefab so the row remains
            // robust if one side changes in a future game update.
            foreach (var (prefabRef, vehicle) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Game.Vehicles.Vehicle, Game.Vehicles.Car, Game.Vehicles.Taxi>()
                .WithNone<Game.Vehicles.CarTrailer, Deleted, Destroyed>()
                .WithNone<Temp>()
                .WithEntityAccess())
            {
                Entity prefab = prefabRef.ValueRO.m_Prefab;
                if (prefab == Entity.Null ||
                    (!SystemAPI.HasComponent<TaxiData>(prefab) &&
                        !SystemAPI.HasComponent<Game.Vehicles.Taxi>(vehicle)))
                {
                    continue;
                }

                bool isParked = SystemAPI.HasComponent<ParkedCar>(vehicle);
                bool isActive = !isParked && SystemAPI.HasComponent<CarCurrentLane>(vehicle);
                if (!isParked && !isActive)
                {
                    continue;
                }

                taxiTotal++;
                if (isParked)
                {
                    taxiParked++;
                }
                else
                {
                    taxiActive++;
                }
            }

            return new CityVehicleStats(
                carTotal,
                carActive,
                carParked,
                bikeTotal,
                bikeActive,
                bikeParked,
                industryTotal,
                industryActive,
                industryParked,
                busTotal,
                busActive,
                busParked,
                taxiTotal,
                taxiActive,
                taxiParked);
        }

        public void LogVehicleStatsReportToLog()
        {
            try
            {
                CityVehicleStats stats = BuildCityVehicleStats();
                PrefabSystem prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
                Dictionary<string, VehicleReportRow> rows = new Dictionary<string, VehicleReportRow>(System.StringComparer.Ordinal);

                int scannedRoadCars = 0;
                int excludedBikes = 0;
                int excludedPrivate = 0;
                int excludedPublicTransport = 0;
                int excludedTaxi = 0;
                int includedCandidates = 0;
                int includedActive = 0;
                int includedParked = 0;
                int includedOther = 0;
                int deliveryTruckHints = 0;
                int maintenanceHints = 0;
                int garbageHints = 0;
                int emergencyHints = 0;
                int hearseHints = 0;

                foreach (var (prefabRef, vehicle) in SystemAPI
                    .Query<RefRO<PrefabRef>>()
                    .WithAll<Game.Vehicles.Vehicle, Game.Vehicles.Car>()
                    .WithNone<Game.Vehicles.CarTrailer, Deleted, Destroyed>()
                    .WithNone<Temp>()
                    .WithEntityAccess())
                {
                    scannedRoadCars++;

                    Entity prefab = prefabRef.ValueRO.m_Prefab;
                    if (prefab == Entity.Null)
                    {
                        continue;
                    }

                    bool prefabIsBike = SystemAPI.HasComponent<BicycleData>(prefab);
                    bool prefabIsPublicTransport = SystemAPI.HasComponent<PublicTransportVehicleData>(prefab);
                    bool prefabIsTaxi = SystemAPI.HasComponent<TaxiData>(prefab);
                    bool vehicleIsPersonal = SystemAPI.HasComponent<Game.Vehicles.PersonalCar>(vehicle);
                    bool vehicleIsPublicTransport = SystemAPI.HasComponent<Game.Vehicles.PublicTransport>(vehicle);
                    bool vehicleIsTaxi = SystemAPI.HasComponent<Game.Vehicles.Taxi>(vehicle);

                    if (prefabIsBike)
                    {
                        excludedBikes++;
                        continue;
                    }

                    if (vehicleIsPersonal)
                    {
                        excludedPrivate++;
                        continue;
                    }

                    if (prefabIsPublicTransport || vehicleIsPublicTransport)
                    {
                        excludedPublicTransport++;
                        continue;
                    }

                    if (prefabIsTaxi || vehicleIsTaxi)
                    {
                        excludedTaxi++;
                        continue;
                    }

                    includedCandidates++;

                    bool isParked = SystemAPI.HasComponent<ParkedCar>(vehicle);
                    bool isActive = !isParked && SystemAPI.HasComponent<CarCurrentLane>(vehicle);

                    if (SystemAPI.HasComponent<Game.Vehicles.DeliveryTruck>(vehicle))
                    {
                        deliveryTruckHints++;
                    }

                    if (SystemAPI.HasComponent<Game.Vehicles.MaintenanceVehicle>(vehicle))
                    {
                        maintenanceHints++;
                    }

                    if (SystemAPI.HasComponent<Game.Vehicles.GarbageTruck>(vehicle))
                    {
                        garbageHints++;
                    }

                    if (SystemAPI.HasComponent<Game.Vehicles.Ambulance>(vehicle) ||
                        SystemAPI.HasComponent<Game.Vehicles.FireEngine>(vehicle) ||
                        SystemAPI.HasComponent<Game.Vehicles.PoliceCar>(vehicle))
                    {
                        emergencyHints++;
                    }

                    if (SystemAPI.HasComponent<Game.Vehicles.Hearse>(vehicle))
                    {
                        hearseHints++;
                    }

                    string prefabName = GetVehicleStatsPrefabName(prefabSystem, prefab);
                    if (!rows.TryGetValue(prefabName, out VehicleReportRow row))
                    {
                        row = new VehicleReportRow();
                        rows[prefabName] = row;
                    }

                    if (isParked)
                    {
                        includedParked++;
                        row.Parked++;
                    }
                    else if (isActive)
                    {
                        includedActive++;
                        row.Active++;
                    }
                    else
                    {
                        includedOther++;
                        row.Other++;
                    }
                }

                LogUtils.Info(() => $"{Mod.ModTag} Vehicle stats report BEGIN");
                LogUtils.Info(
                    () => $"{Mod.ModTag} Vehicle stats panel rows: bikes active={stats.BikeActive} parked={stats.BikeParked} total={stats.BikeTotal}; private active={stats.CarActive} parked={stats.CarParked} total={stats.CarTotal}; roadWork active={stats.IndustryActive} parked={stats.IndustryParked} total={stats.IndustryTotal}; buses active={stats.BusActive} parked={stats.BusParked} total={stats.BusTotal}; taxis active={stats.TaxiActive} parked={stats.TaxiParked} total={stats.TaxiTotal}");
                LogUtils.Info(
                    () => $"{Mod.ModTag} Vehicle stats road-car scan: scanned={scannedRoadCars}, includedCandidates={includedCandidates}, countedActive={includedActive}, countedParked={includedParked}, pendingOther={includedOther}, excludedBikes={excludedBikes}, excludedPrivate={excludedPrivate}, excludedPublicTransit={excludedPublicTransport}, excludedTaxi={excludedTaxi}");
                LogUtils.Info(
                    () => $"{Mod.ModTag} Vehicle stats included component hints: deliveryTruck={deliveryTruckHints}, maintenance={maintenanceHints}, garbage={garbageHints}, emergency={emergencyHints}, hearse={hearseHints}");

                int logged = 0;
                foreach (KeyValuePair<string, VehicleReportRow> item in rows
                    .OrderByDescending(row => row.Value.Active + row.Value.Parked + row.Value.Other)
                    .ThenBy(row => row.Key))
                {
                    if (logged >= kVehicleStatsReportMaxRows)
                    {
                        break;
                    }

                    VehicleReportRow row = item.Value;
                    int total = row.Active + row.Parked + row.Other;
                    LogUtils.Info(
                        () => $"{Mod.ModTag} Vehicle stats prefab: total={total}, active={row.Active}, parked={row.Parked}, other={row.Other}, prefab='{item.Key}'");
                    logged++;
                }

                if (rows.Count > logged)
                {
                    LogUtils.Info(
                        () => $"{Mod.ModTag} Vehicle stats prefab: omitted {rows.Count - logged} additional prefab rows.");
                }

                LogUtils.Info(() => $"{Mod.ModTag} Vehicle stats report END");
            }
            catch (System.Exception ex)
            {
                LogUtils.Warn(() => $"{Mod.ModTag} Failed to log vehicle stats report: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private static string GetVehicleStatsPrefabName(PrefabSystem prefabSystem, Entity prefab)
        {
            try
            {
                PrefabBase prefabBase = prefabSystem.GetPrefab<PrefabBase>(prefab);
                return prefabBase?.name ?? $"Entity({prefab.Index}:{prefab.Version})";
            }
            catch
            {
                return $"Entity({prefab.Index}:{prefab.Version})";
            }
        }

        private void ClearCityVehicleStatsBindings()
        {
            m_CityCarTotalBinding.Value = 0;
            m_CityCarActiveBinding.Value = 0;
            m_CityCarParkedBinding.Value = 0;
            m_CityBikeTotalBinding.Value = 0;
            m_CityBikeActiveBinding.Value = 0;
            m_CityBikeParkedBinding.Value = 0;
            m_CityIndustryTotalBinding.Value = 0;
            m_CityIndustryActiveBinding.Value = 0;
            m_CityIndustryParkedBinding.Value = 0;
            m_CityBusTotalBinding.Value = 0;
            m_CityBusActiveBinding.Value = 0;
            m_CityBusParkedBinding.Value = 0;
            m_CityTaxiTotalBinding.Value = 0;
            m_CityTaxiActiveBinding.Value = 0;
            m_CityTaxiParkedBinding.Value = 0;
        }
    }
}
