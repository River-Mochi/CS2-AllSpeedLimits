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
    using Game.SceneFlow;
    using Game.Tools;
    using Unity.Entities;
    using Unity.Mathematics;
    using System;
    using System.Collections.Generic;
    using Game.Objects;
    using Game.Vehicles;
    using Unity.Collections;

    public partial class BicycleSpeedSystem : GameSystemBase
    {
        private PrefabSystem m_PrefabSystem = null!;

        // Only remember the three fields that ASL actually owns, not other CarData fields.
        private readonly Dictionary<Entity, BicycleSpeedSnapshot> m_OwnedValues = new Dictionary<Entity, BicycleSpeedSnapshot>();

        private readonly struct BicycleSpeedSnapshot
        {
            public readonly BicyclePrefab Prefab;
            public readonly float3 Before;
            public readonly float3 Applied;

            public BicycleSpeedSnapshot(BicyclePrefab prefab, float3 before, float3 applied)
            {
                Prefab = prefab;
                Before = before;
                Applied = applied;
            }
        }

        private static bool TuningEnabled => Mod.Settings?.EnableBicycleSpeedTuning == true;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            Enabled = false;
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            PersonalVehicleStatus.InvalidateCache();
            Enabled = mode == GameMode.Game && (TuningEnabled || m_OwnedValues.Count != 0);
            if (mode == GameMode.Game && !Enabled)
            {
                LogUtils.Info(() => $"{Mod.ModTag} Bicycle and scooter tuning disabled; no prefab writes.");
            }
        }

        public void ScheduleApply()
        {
            // Settings can change at the main menu. Apply when a city is loaded instead.
            if (GameManager.instance?.gameMode == GameMode.Game)
            {
                Enabled = TuningEnabled || m_OwnedValues.Count != 0;
            }
        }

        protected override void OnUpdate()
        {
            try
            {
                if (!TuningEnabled && m_OwnedValues.Count == 0)
                {
                    return;
                }

                if (HasLegacyBikesAndPaths())
                {
                    // Do not restore over an older B+P that still owns tuning.
                    m_OwnedValues.Clear();
                    LogUtils.Info(() => $"{Mod.ModTag} Bicycle tuning skipped: remove legacy Bikes + Paths when using ASL.");
                    return;
                }

                if (!TuningEnabled)
                {
                    RestoreOwnedValues();
                    return;
                }

                int percent = Mod.Settings?.BicycleSpeedPercent ?? 100;
                float speedScalar = math.clamp(percent, 30, 1000) / 100f;
                float accelBrake = math.sqrt(speedScalar);
                int eligible = 0;
                int changed = 0;
                HashSet<Entity> seen = new HashSet<Entity>();

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
                    seen.Add(entity);
                    CarData current = data.ValueRO;
                    float3 before = ReadTunedValues(current);

                    if (m_OwnedValues.TryGetValue(entity, out BicycleSpeedSnapshot previous) && previous.Prefab == prefab)
                    {
                        // Retain the original value only while the current field still matches
                        // ASL's last write. A later external change becomes the new baseline.
                        before.x = before.x == previous.Applied.x ? previous.Before.x : before.x;
                        before.y = before.y == previous.Applied.y ? previous.Before.y : before.y;
                        before.z = before.z == previous.Applied.z ? previous.Before.z : before.z;
                    }

                    GetTarget(prefab, speedScalar, accelBrake, out float maxSpeed, out float acceleration, out float braking);
                    if (!Matches(current, maxSpeed, acceleration, braking))
                    {
                        data.ValueRW.m_MaxSpeed = maxSpeed;
                        data.ValueRW.m_Acceleration = acceleration;
                        data.ValueRW.m_Braking = braking;
                        changed++;
                    }

                    float3 applied = ReadTunedValues(data.ValueRO);
                    if (math.any(before != applied))
                    {
                        m_OwnedValues[entity] = new BicycleSpeedSnapshot(prefab, before, applied);
                    }
                    else
                    {
                        m_OwnedValues.Remove(entity);
                    }
                }

                // Prefabs can be recreated when changing cities.
                List<Entity> stale = new List<Entity>();
                foreach (Entity entity in m_OwnedValues.Keys)
                {
                    if (!seen.Contains(entity))
                    {
                        stale.Add(entity);
                    }
                }

                foreach (Entity entity in stale)
                {
                    m_OwnedValues.Remove(entity);
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

        private static float3 ReadTunedValues(CarData data)
        {
            return new float3(data.m_MaxSpeed, data.m_Acceleration, data.m_Braking);
        }

        private void RestoreOwnedValues()
        {
            int restored = 0;
            int externalFieldsPreserved = 0;

            foreach ((RefRW<CarData> data, Entity entity) in SystemAPI.Query<RefRW<CarData>>()
                .WithAll<PrefabData, BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                if (!m_OwnedValues.TryGetValue(entity, out BicycleSpeedSnapshot snapshot) ||
                    !TryGetBicyclePrefab(entity, out BicyclePrefab prefab) || snapshot.Prefab != prefab)
                {
                    continue;
                }

                float3 current = ReadTunedValues(data.ValueRO);
                bool changed = false;

                // Compare each field independently. Never overwrite a newer external value.
                if (snapshot.Before.x != snapshot.Applied.x)
                {
                    if (current.x == snapshot.Applied.x)
                    {
                        data.ValueRW.m_MaxSpeed = snapshot.Before.x;
                        changed = true;
                    }
                    else
                    {
                        externalFieldsPreserved++;
                    }
                }

                if (snapshot.Before.y != snapshot.Applied.y)
                {
                    if (current.y == snapshot.Applied.y)
                    {
                        data.ValueRW.m_Acceleration = snapshot.Before.y;
                        changed = true;
                    }
                    else
                    {
                        externalFieldsPreserved++;
                    }
                }

                if (snapshot.Before.z != snapshot.Applied.z)
                {
                    if (current.z == snapshot.Applied.z)
                    {
                        data.ValueRW.m_Braking = snapshot.Before.z;
                        changed = true;
                    }
                    else
                    {
                        externalFieldsPreserved++;
                    }
                }

                if (changed)
                {
                    restored++;
                }
            }

            m_OwnedValues.Clear();
            LogUtils.Info(() => $"{Mod.ModTag} Bicycle and scooter tuning disabled: restoredPrefabs={restored}, externalFieldsPreserved={externalFieldsPreserved}.");
        }

        public void LogBicycleReport()
        {
            if (!TuningEnabled)
            {
                LogUtils.Info(() => $"{Mod.ModTag} Bicycle speed report: disabled; ASL does not control vehicle speeds.");
                return;
            }

            if (HasLegacyBikesAndPaths())
            {
                LogUtils.Info(() => $"{Mod.ModTag} Bicycle report: legacy Bikes + Paths owns vehicle tuning. Remove B+P when using ASL.");
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
