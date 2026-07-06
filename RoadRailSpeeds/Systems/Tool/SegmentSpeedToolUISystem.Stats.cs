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
    using Game.Prefabs;
    using Game.Vehicles;
    using Unity.Collections;
    using Unity.Entities;

    public partial class SegmentSpeedToolUISystem
    {
        // Tweak this if the open panel should refresh city vehicle stats more or less often.
        private const int kVehicleStatsRefreshFrames = 512;

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

            public CityVehicleStats(
                int carTotal,
                int carActive,
                int carParked,
                int bikeTotal,
                int bikeActive,
                int bikeParked,
                int industryTotal,
                int industryActive,
                int industryParked)
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
            }
        }

        private void RefreshVehicleStatsIfNeeded(bool force)
        {
            int frame = UnityEngine.Time.frameCount;
            if (!force &&
                m_LastVehicleStatsFrame >= 0 &&
                frame - m_LastVehicleStatsFrame < kVehicleStatsRefreshFrames)
            {
                return;
            }

            m_LastVehicleStatsFrame = frame;

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

            // Read-only count, so the modern DOTS idiom SystemAPI.Query fits: it iterates the matching
            // chunks directly with no NativeArray copy. The query guarantees PrefabRef and PersonalCar
            // and excludes trailers/deleted/destroyed/temp. WithEntityAccess gives the entity for the
            // ParkedCar/CarCurrentLane lookups; BicycleData is checked on the prefab entity.
            foreach (var (prefabRef, vehicle) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Game.Vehicles.PersonalCar>()
                .WithNone<Game.Vehicles.CarTrailer, Game.Common.Deleted, Game.Common.Destroyed>()
                .WithNone<Game.Tools.Temp>()
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

            using (NativeArray<Entity> deliveryTruckEntities = m_DeliveryTruckStatsQuery.ToEntityArray(Allocator.Temp))
            {
                for (int i = 0; i < deliveryTruckEntities.Length; i++)
                {
                    Entity vehicle = deliveryTruckEntities[i];

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
                industryParked);
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
        }
    }
}
