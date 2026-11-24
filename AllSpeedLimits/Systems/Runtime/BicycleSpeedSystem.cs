// <copyright file="BicycleSpeedSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Bicycle prefab tuning adapted from MIT-licensed CS2-Bikes-Paths.
// Apply on demand or at city load; no ongoing vehicle-instance scan.
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

    public partial class BicycleSpeedSystem : GameSystemBase
    {
        private PrefabSystem m_PrefabSystem = null!;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            Enabled = false;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            Enabled = mode == GameMode.Game;
        }

        public void ScheduleApply()
        {
            Enabled = true;
        }

        protected override void OnUpdate()
        {
            try
            {
                if (HasLegacyBikesAndPaths())
                {
                    LogUtils.Info(() => $"{Mod.ModTag} Bicycle tuning skipped: update Bikes + Paths to its ASL-compatible version.");
                    return;
                }

                int percent = Mod.Settings?.BicycleSpeedPercent ?? 100;
                float speedScalar = math.clamp(percent, 30, 1000) / 100f;
                float accelBrake = math.sqrt(speedScalar);
                int eligible = 0;
                int changed = 0;

                foreach ((RefRW<CarData> data, Entity entity) in SystemAPI.Query<RefRW<CarData>>()
                    .WithAll<PrefabData, BicycleData>()
                    .WithNone<Deleted, Temp, Overridden>()
                    .WithEntityAccess())
                {
                    if (!TryGetBicyclePrefab(entity, out BicyclePrefab prefab))
                    {
                        continue;
                    }

                    eligible++;
                    GetTarget(prefab, speedScalar, accelBrake, out float maxSpeed, out float acceleration, out float braking);
                    if (!Matches(data.ValueRO, maxSpeed, acceleration, braking))
                    {
                        data.ValueRW.m_MaxSpeed = maxSpeed;
                        data.ValueRW.m_Acceleration = acceleration;
                        data.ValueRW.m_Braking = braking;
                        changed++;
                    }
                }

                LogUtils.Info(() => $"{Mod.ModTag} Bicycle and scooter speed: {percent}% prefabs={eligible} changed={changed}.");
            }
            catch (System.Exception exception)
            {
                LogUtils.Error(() => $"{Mod.ModTag} Bicycle speed update failed: {exception.Message}", exception);
            }
            finally
            {
                Enabled = false;
            }
        }

        public void LogBicycleReport()
        {
            if (HasLegacyBikesAndPaths())
            {
                LogUtils.Info(() => $"{Mod.ModTag} Bicycle report: legacy Bikes + Paths owns vehicle tuning. Update B+P.");
                return;
            }

            int percent = Mod.Settings?.BicycleSpeedPercent ?? 100;
            float speedScalar = math.clamp(percent, 30, 1000) / 100f;
            float accelBrake = math.sqrt(speedScalar);
            int eligible = 0;
            int mismatches = 0;

            foreach ((RefRO<CarData> data, Entity entity) in SystemAPI.Query<RefRO<CarData>>()
                .WithAll<PrefabData, BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                if (!TryGetBicyclePrefab(entity, out BicyclePrefab prefab))
                {
                    continue;
                }

                eligible++;
                GetTarget(prefab, speedScalar, accelBrake, out float maxSpeed, out float acceleration, out float braking);
                CarData actual = data.ValueRO;
                bool match = Matches(actual, maxSpeed, acceleration, braking);
                if (!match)
                {
                    mismatches++;
                }

                if (eligible <= 12)
                {
                    string name = prefab.name;
                    LogUtils.Info(() =>
                        $"{Mod.ModTag} Bicycle prefab {name}: maxKmh={actual.m_MaxSpeed * 3.6f:0.##} " +
                        $"expectedKmh={maxSpeed * 3.6f:0.##} accel={actual.m_Acceleration:0.##} " +
                        $"brake={actual.m_Braking:0.##} match={match}.");
                }
            }

            LogUtils.Info(() => $"{Mod.ModTag} Bicycle speed report: percent={percent} prefabs={eligible} mismatches={mismatches}.");
        }

        private bool TryGetBicyclePrefab(Entity entity, out BicyclePrefab prefab)
        {
            if (m_PrefabSystem.TryGetPrefab(entity, out PrefabBase basePrefab) && basePrefab is BicyclePrefab bicycle)
            {
                prefab = bicycle;
                return true;
            }

            prefab = null!;
            return false;
        }

        private static void GetTarget(BicyclePrefab prefab, float speedScalar, float accelBrake,
            out float maxSpeed, out float acceleration, out float braking)
        {
            float baseMaxMs = prefab.m_MaxSpeed / 3.6f;
            maxSpeed = baseMaxMs <= 0f ? 0f : math.max(0.01f, baseMaxMs * speedScalar);
            acceleration = prefab.m_Acceleration <= 0f ? 0f : prefab.m_Acceleration * accelBrake;
            braking = prefab.m_Braking <= 0f ? 0f : prefab.m_Braking * accelBrake;
        }

        private static bool Matches(CarData data, float maxSpeed, float acceleration, float braking)
        {
            return math.abs(data.m_MaxSpeed - maxSpeed) <= 0.0001f &&
                math.abs(data.m_Acceleration - acceleration) <= 0.0001f &&
                math.abs(data.m_Braking - braking) <= 0.0001f;
        }

        private bool HasLegacyBikesAndPaths()
        {
            // Older B+P releases do not relinquish writes. Do not fight their vehicle tuning.
            foreach (ComponentSystemBase system in World.Systems)
            {
                System.Type type = system.GetType();
                if (type.FullName == "BikesAndPaths.BikesAndPathsSystem" &&
                    type.Assembly.GetType("BikesAndPaths.AslOwnership") == null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
