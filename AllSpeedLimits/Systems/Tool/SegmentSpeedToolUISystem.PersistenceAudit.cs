// <copyright file="SegmentSpeedToolUISystem.PersistenceAudit.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolUISystem.PersistenceAudit.cs
// Purpose: Debug-only, read-only audit of saved lane speeds and marker metadata.

#if DEBUG
namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;
    using System.Globalization;
    using CS2Shared.RiverMochi;
    using Game.Net;
    using RoadRailSpeeds.Components;
    using Unity.Collections;
    using Unity.Entities;

    using CarLane = Game.Net.CarLane;
    using PrefabBase = Game.Prefabs.PrefabBase;
    using PrefabSystem = Game.Prefabs.PrefabSystem;
    using SubLane = Game.Net.SubLane;
    using TrackLane = Game.Net.TrackLane;

    public partial class SegmentSpeedToolUISystem
    {
        private const int kPersistenceAuditSamplesPerState = 4;

        private enum PersistenceAuditState
        {
            Healthy,
            RecoverableNoMarker,
            DefaultNoMarker,
            ComponentLaneMismatch,
            StaleMarker,
            MixedLanes,
            MissingPrefab,
            NoSupportedLanes
        }

        private sealed class PersistenceAuditCounts
        {
            public int Total;
            public int Healthy;
            public int RecoverableNoMarker;
            public int DefaultNoMarker;
            public int ComponentLaneMismatch;
            public int StaleMarker;
            public int MixedLanes;
            public int MissingPrefab;
            public int NoSupportedLanes;

            public void Record(PersistenceAuditState state)
            {
                Total++;

                switch (state)
                {
                    case PersistenceAuditState.Healthy:
                        Healthy++;
                        break;
                    case PersistenceAuditState.RecoverableNoMarker:
                        RecoverableNoMarker++;
                        break;
                    case PersistenceAuditState.DefaultNoMarker:
                        DefaultNoMarker++;
                        break;
                    case PersistenceAuditState.ComponentLaneMismatch:
                        ComponentLaneMismatch++;
                        break;
                    case PersistenceAuditState.StaleMarker:
                        StaleMarker++;
                        break;
                    case PersistenceAuditState.MixedLanes:
                        MixedLanes++;
                        break;
                    case PersistenceAuditState.MissingPrefab:
                        MissingPrefab++;
                        break;
                    case PersistenceAuditState.NoSupportedLanes:
                        NoSupportedLanes++;
                        break;
                }
            }
        }

        private struct PersistentLaneAuditInfo
        {
            public int CarLaneCount;
            public int TrackLaneCount;
            public int SpeedCount;
            public float SpeedSumKmh;
            public float MinSpeedKmh;
            public float MaxSpeedKmh;
            public int CarDefaultSpeedCount;
            public float CarDefaultSpeedSumKmh;
            public float MinCarDefaultSpeedKmh;
            public float MaxCarDefaultSpeedKmh;

            public readonly float AverageSpeedKmh =>
                SpeedCount > 0 ? SpeedSumKmh / SpeedCount : -1f;

            public readonly bool HasMixedSpeeds =>
                SpeedCount > 1 && MaxSpeedKmh - MinSpeedKmh > kSpeedComparisonTolerance;

            public readonly float AverageCarDefaultSpeedKmh =>
                CarDefaultSpeedCount > 0 ? CarDefaultSpeedSumKmh / CarDefaultSpeedCount : -1f;

            public void AddCar(float currentSpeedKmh, float defaultSpeedKmh)
            {
                CarLaneCount++;
                AddEffectiveSpeed(currentSpeedKmh);

                if (defaultSpeedKmh <= 0f)
                {
                    return;
                }

                if (CarDefaultSpeedCount == 0)
                {
                    MinCarDefaultSpeedKmh = defaultSpeedKmh;
                    MaxCarDefaultSpeedKmh = defaultSpeedKmh;
                }
                else
                {
                    MinCarDefaultSpeedKmh = System.Math.Min(MinCarDefaultSpeedKmh, defaultSpeedKmh);
                    MaxCarDefaultSpeedKmh = System.Math.Max(MaxCarDefaultSpeedKmh, defaultSpeedKmh);
                }

                CarDefaultSpeedCount++;
                CarDefaultSpeedSumKmh += defaultSpeedKmh;
            }

            public void AddTrack(float speedKmh)
            {
                TrackLaneCount++;
                AddEffectiveSpeed(speedKmh);
            }

            private void AddEffectiveSpeed(float speedKmh)
            {
                if (speedKmh <= 0f)
                {
                    return;
                }

                if (SpeedCount == 0)
                {
                    MinSpeedKmh = speedKmh;
                    MaxSpeedKmh = speedKmh;
                }
                else
                {
                    MinSpeedKmh = System.Math.Min(MinSpeedKmh, speedKmh);
                    MaxSpeedKmh = System.Math.Max(MaxSpeedKmh, speedKmh);
                }

                SpeedCount++;
                SpeedSumKmh += speedKmh;
            }
        }

        private void LogSpeedPersistenceAuditToLog(
            PrefabSystem prefabSystem,
            NativeArray<Entity> edges)
        {
            PersistenceAuditCounts totals = new PersistenceAuditCounts();
            Dictionary<string, PersistenceAuditCounts> countsByKind =
                new Dictionary<string, PersistenceAuditCounts>(System.StringComparer.Ordinal);
            Dictionary<PersistenceAuditState, int> sampleCounts =
                new Dictionary<PersistenceAuditState, int>();
            List<string> samples = new List<string>();

            for (int i = 0; i < edges.Length; i++)
            {
                Entity edge = GetBaseEdge(edges[i]);
                string kind = GetPersistenceAuditKind(edge);
                bool hasCustomSpeed = EntityManager.HasComponent<CustomSpeed>(edge);
                float customSpeedKmh = hasCustomSpeed
                    ? EntityManager.GetComponentData<CustomSpeed>(edge).m_Speed
                    : -1f;

                PersistentLaneAuditInfo laneInfo = ReadPersistentLaneSpeeds(edge);
                bool hasPrefab = TryGetPrefab(
                    edge,
                    prefabSystem,
                    out PrefabBase prefabBase,
                    out _);
                float prefabSpeedKmh = hasPrefab ? GetPrefabSpeedKmh(prefabBase) : -1f;
                string prefabName = hasPrefab ? prefabBase.name ?? "<unnamed>" : "<missing>";

                PersistenceAuditState state = ClassifyPersistenceState(
                    hasCustomSpeed,
                    customSpeedKmh,
                    hasPrefab && prefabSpeedKmh > 0f,
                    prefabSpeedKmh,
                    laneInfo);

                totals.Record(state);
                if (!countsByKind.TryGetValue(kind, out PersistenceAuditCounts kindCounts))
                {
                    kindCounts = new PersistenceAuditCounts();
                    countsByKind.Add(kind, kindCounts);
                }

                kindCounts.Record(state);

                if (ShouldLogPersistenceSample(state, sampleCounts))
                {
                    samples.Add(BuildPersistenceAuditSample(
                        state,
                        edge,
                        kind,
                        prefabName,
                        prefabSpeedKmh,
                        hasCustomSpeed,
                        customSpeedKmh,
                        laneInfo));
                }
            }

            LogUtils.Info(
                () => $"{Mod.ModTag} Speed persistence audit BEGIN readOnly=true effectiveCarSpeed=CarLane.m_SpeedLimit carDefaultSpeed=CarLane.m_DefaultSpeedLimit trackSpeed=TrackLane.m_SpeedLimit toleranceKmh={kSpeedComparisonTolerance:0.##}");
            LogPersistenceAuditCounts("all", totals);

            foreach (KeyValuePair<string, PersistenceAuditCounts> item in countsByKind)
            {
                LogPersistenceAuditCounts(item.Key, item.Value);
            }

            for (int i = 0; i < samples.Count; i++)
            {
                string sample = samples[i];
                LogUtils.Info(() => $"{Mod.ModTag} Speed persistence audit sample: {sample}");
            }

            LogUtils.Info(() => $"{Mod.ModTag} Speed persistence audit END samples={samples.Count}");
        }

        private static PersistenceAuditState ClassifyPersistenceState(
            bool hasCustomSpeed,
            float customSpeedKmh,
            bool hasPrefabSpeed,
            float prefabSpeedKmh,
            PersistentLaneAuditInfo laneInfo)
        {
            if (!hasPrefabSpeed)
            {
                return PersistenceAuditState.MissingPrefab;
            }

            if (laneInfo.SpeedCount == 0)
            {
                return PersistenceAuditState.NoSupportedLanes;
            }

            if (laneInfo.HasMixedSpeeds)
            {
                return PersistenceAuditState.MixedLanes;
            }

            float laneSpeedKmh = laneInfo.AverageSpeedKmh;
            if (!hasCustomSpeed)
            {
                return System.Math.Abs(laneSpeedKmh - prefabSpeedKmh) > kSpeedComparisonTolerance
                    ? PersistenceAuditState.RecoverableNoMarker
                    : PersistenceAuditState.DefaultNoMarker;
            }

            if (System.Math.Abs(customSpeedKmh - laneSpeedKmh) > kSpeedComparisonTolerance)
            {
                return PersistenceAuditState.ComponentLaneMismatch;
            }

            if (System.Math.Abs(customSpeedKmh - prefabSpeedKmh) <= kSpeedComparisonTolerance)
            {
                return PersistenceAuditState.StaleMarker;
            }

            return PersistenceAuditState.Healthy;
        }

        private PersistentLaneAuditInfo ReadPersistentLaneSpeeds(Entity edge)
        {
            PersistentLaneAuditInfo result = default;
            if (!EntityManager.HasBuffer<SubLane>(edge))
            {
                return result;
            }

            CarLaneFlags ignoredCarLaneFlags = CarLaneFlags.Unsafe | CarLaneFlags.SideConnection;
            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);
            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity laneEntity = subLanes[i].m_SubLane;
                if (EntityManager.HasComponent<CarLane>(laneEntity))
                {
                    CarLane carLane = EntityManager.GetComponentData<CarLane>(laneEntity);
                    if ((carLane.m_Flags & ignoredCarLaneFlags) == 0)
                    {
                        result.AddCar(
                            carLane.m_SpeedLimit * 1.8f,
                            carLane.m_DefaultSpeedLimit * 1.8f);
                    }
                }
                else if (EntityManager.HasComponent<TrackLane>(laneEntity))
                {
                    TrackLane trackLane = EntityManager.GetComponentData<TrackLane>(laneEntity);
                    result.AddTrack(trackLane.m_SpeedLimit * 1.8f);
                }
            }

            return result;
        }

        private string GetPersistenceAuditKind(Entity edge)
        {
            if (EntityManager.HasComponent<Waterway>(edge))
            {
                return "water";
            }

            if (EntityManager.HasComponent<SubwayTrack>(edge))
            {
                return "subway";
            }

            if (EntityManager.HasComponent<TrainTrack>(edge))
            {
                return "train";
            }

            if (EntityManager.HasComponent<TramTrack>(edge))
            {
                return "tram";
            }

            if (IsTrackType(edge))
            {
                return "rail-other";
            }

            return IsRoadOnly(edge) ? "road" : "other";
        }

        private static bool ShouldLogPersistenceSample(
            PersistenceAuditState state,
            Dictionary<PersistenceAuditState, int> sampleCounts)
        {
            if (state == PersistenceAuditState.Healthy ||
                state == PersistenceAuditState.DefaultNoMarker)
            {
                return false;
            }

            int count = sampleCounts.TryGetValue(state, out int existing) ? existing : 0;
            if (count >= kPersistenceAuditSamplesPerState)
            {
                return false;
            }

            sampleCounts[state] = count + 1;
            return true;
        }

        private static string BuildPersistenceAuditSample(
            PersistenceAuditState state,
            Entity edge,
            string kind,
            string prefabName,
            float prefabSpeedKmh,
            bool hasCustomSpeed,
            float customSpeedKmh,
            PersistentLaneAuditInfo laneInfo)
        {
            CultureInfo invariant = CultureInfo.InvariantCulture;
            string custom = hasCustomSpeed
                ? customSpeedKmh.ToString("0.##", invariant)
                : "<none>";
            string laneAverage = laneInfo.SpeedCount > 0
                ? laneInfo.AverageSpeedKmh.ToString("0.##", invariant)
                : "<none>";
            string laneRange = laneInfo.SpeedCount > 0
                ? $"{laneInfo.MinSpeedKmh.ToString("0.##", invariant)}..{laneInfo.MaxSpeedKmh.ToString("0.##", invariant)}"
                : "<none>";
            string prefab = prefabSpeedKmh > 0f
                ? prefabSpeedKmh.ToString("0.##", invariant)
                : "<none>";
            string carDefaultAverage = laneInfo.CarDefaultSpeedCount > 0
                ? laneInfo.AverageCarDefaultSpeedKmh.ToString("0.##", invariant)
                : "<none>";
            string carDefaultRange = laneInfo.CarDefaultSpeedCount > 0
                ? $"{laneInfo.MinCarDefaultSpeedKmh.ToString("0.##", invariant)}..{laneInfo.MaxCarDefaultSpeedKmh.ToString("0.##", invariant)}"
                : "<none>";

            return $"state={GetPersistenceAuditStateLabel(state)} entity={edge.Index}:{edge.Version} kind={kind} prefab=\"{prefabName}\" prefabKmh={prefab} customKmh={custom} effectiveLaneAverageKmh={laneAverage} effectiveLaneRangeKmh={laneRange} carDefaultAverageKmh={carDefaultAverage} carDefaultRangeKmh={carDefaultRange} carLanes={laneInfo.CarLaneCount} trackLanes={laneInfo.TrackLaneCount}";
        }

        private static string GetPersistenceAuditStateLabel(PersistenceAuditState state)
        {
            return state switch
            {
                PersistenceAuditState.Healthy => "healthy",
                PersistenceAuditState.RecoverableNoMarker => "recoverable-no-marker",
                PersistenceAuditState.DefaultNoMarker => "default-no-marker",
                PersistenceAuditState.ComponentLaneMismatch => "component-lane-mismatch",
                PersistenceAuditState.StaleMarker => "stale-marker",
                PersistenceAuditState.MixedLanes => "mixed-lanes",
                PersistenceAuditState.MissingPrefab => "missing-prefab",
                PersistenceAuditState.NoSupportedLanes => "no-supported-lanes",
                _ => "unknown"
            };
        }

        private static void LogPersistenceAuditCounts(string kind, PersistenceAuditCounts counts)
        {
            LogUtils.Info(
                () => $"{Mod.ModTag} Speed persistence audit kind={kind} total={counts.Total} healthy={counts.Healthy} recoverableNoMarker={counts.RecoverableNoMarker} defaultNoMarker={counts.DefaultNoMarker} componentLaneMismatch={counts.ComponentLaneMismatch} staleMarker={counts.StaleMarker} mixedLanes={counts.MixedLanes} missingPrefab={counts.MissingPrefab} noSupportedLanes={counts.NoSupportedLanes}");
        }
    }
}
#endif
