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
    using BorderDistrict = Game.Areas.BorderDistrict;
    using CityData = Game.City.City;
    using CityOption = Game.City.CityOption;
    using CitySystem = Game.Simulation.CitySystem;
    using CityUtils = Game.City.CityUtils;
    using DistrictModifier = Game.Areas.DistrictModifier;
    using DistrictModifierType = Game.Areas.DistrictModifierType;
    using PersistentSpeedLimitStorage = RoadRailSpeeds.Data.PersistentSpeedLimitStorage;
    using SpeedLimitEntry = RoadRailSpeeds.Data.SpeedLimitEntry;

    public partial class SegmentSpeedToolUISystem
    {
        private const int kPersistenceAuditSamplesPerState = 4;
        private const int kSelectedPersistenceAuditMaxEdges = 32;

        private enum PersistenceAuditState
        {
            Healthy,
            StoredOverrideNoMarker,
            RuntimeModifiedNoMarker,
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
            public int StoredOverrideNoMarker;
            public int RuntimeModifiedNoMarker;
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
                    case PersistenceAuditState.StoredOverrideNoMarker:
                        StoredOverrideNoMarker++;
                        break;
                    case PersistenceAuditState.RuntimeModifiedNoMarker:
                        RuntimeModifiedNoMarker++;
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
            public int HighwayCarLaneCount;
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

            public readonly bool HasMixedCarDefaultSpeeds =>
                CarDefaultSpeedCount > 1 &&
                MaxCarDefaultSpeedKmh - MinCarDefaultSpeedKmh > kSpeedComparisonTolerance;

            public void AddCar(float currentSpeedKmh, float defaultSpeedKmh, bool isHighway)
            {
                CarLaneCount++;
                if (isHighway)
                {
                    HighwayCarLaneCount++;
                }

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
            bool cityUnlimitedHighwaySpeed = false;
            CitySystem? citySystem = World.GetExistingSystemManaged<CitySystem>();
            if (citySystem != null &&
                citySystem.City != Entity.Null &&
                EntityManager.Exists(citySystem.City) &&
                EntityManager.HasComponent<CityData>(citySystem.City))
            {
                CityData city = EntityManager.GetComponentData<CityData>(citySystem.City);
                cityUnlimitedHighwaySpeed = CityUtils.CheckOption(
                    city,
                    CityOption.UnlimitedHighwaySpeed);
            }

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
                () => $"{Mod.ModTag} Speed persistence audit BEGIN readOnly=true effectiveCarSpeed=CarLane.m_SpeedLimit carDefaultSpeed=CarLane.m_DefaultSpeedLimit trackSpeed=TrackLane.m_SpeedLimit runtimeToNominalKmh=1.8 runtimeToSimulationKmh=3.6 cityUnlimitedHighwaySpeed={cityUnlimitedHighwaySpeed} toleranceKmh={kSpeedComparisonTolerance:0.##}");
            LogPersistenceAuditCounts("all", totals);

            foreach (KeyValuePair<string, PersistenceAuditCounts> item in countsByKind)
            {
                LogPersistenceAuditCounts(item.Key, item.Value);
            }

            LogSelectedSpeedDiagnosticsToLog(prefabSystem);

            for (int i = 0; i < samples.Count; i++)
            {
                string sample = samples[i];
                LogUtils.Info(() => $"{Mod.ModTag} Speed persistence audit sample: {sample}");
            }

            LogUtils.Info(() => $"{Mod.ModTag} Speed persistence audit END samples={samples.Count}");
        }

        private void LogSelectedSpeedDiagnosticsToLog(PrefabSystem prefabSystem)
        {
            if (m_SelectedEdges.Count == 0)
            {
                LogUtils.Info(
                    () => $"{Mod.ModTag} Selected speed diagnostic: no ASL segments are currently selected.");
                return;
            }

            HashSet<Entity> loggedEdges = new HashSet<Entity>();
            int logged = 0;
            for (int i = 0;
                i < m_SelectedEdges.Count && logged < kSelectedPersistenceAuditMaxEdges;
                i++)
            {
                Entity edge = GetBaseEdge(m_SelectedEdges[i]);
                if (edge == Entity.Null ||
                    !EntityManager.Exists(edge) ||
                    !loggedEdges.Add(edge))
                {
                    continue;
                }

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
                SpeedLimitEntry? recoveryEntry =
                    PersistentSpeedLimitStorage.GetSpeedLimit(edge.Index);
                string recovery = recoveryEntry == null
                    ? "<none>"
                    : $"{recoveryEntry.DefaultSpeedKmh.ToString("0.##", CultureInfo.InvariantCulture)}->{recoveryEntry.CurrentSpeedKmh.ToString("0.##", CultureInfo.InvariantCulture)}";
                string diagnostic = BuildPersistenceAuditSample(
                    state,
                    edge,
                    kind,
                    prefabName,
                    prefabSpeedKmh,
                    hasCustomSpeed,
                    customSpeedKmh,
                    laneInfo);

                LogUtils.Info(
                    () => $"{Mod.ModTag} Selected speed diagnostic: {diagnostic} recoveryDefaultToCurrentKmh={recovery}");
                logged++;
            }

            if (m_SelectedEdges.Count > logged)
            {
                LogUtils.Info(
                    () => $"{Mod.ModTag} Selected speed diagnostic: logged={logged} selected={m_SelectedEdges.Count} max={kSelectedPersistenceAuditMaxEdges}");
            }
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

            if (laneInfo.HasMixedSpeeds || laneInfo.HasMixedCarDefaultSpeeds)
            {
                return PersistenceAuditState.MixedLanes;
            }

            float laneSpeedKmh = laneInfo.AverageSpeedKmh;
            if (!hasCustomSpeed)
            {
                if (System.Math.Abs(laneSpeedKmh - prefabSpeedKmh) <=
                    kSpeedComparisonTolerance)
                {
                    return PersistenceAuditState.DefaultNoMarker;
                }

                // CarLane.m_SpeedLimit may legitimately differ because of a district modifier or
                // the city's unlimited-highway option. Only a changed persistent default is safe
                // migration evidence for roads; track lanes have no separate default field.
                bool storedRoadSpeedDiffers =
                    laneInfo.CarDefaultSpeedCount > 0 &&
                    System.Math.Abs(laneInfo.AverageCarDefaultSpeedKmh - prefabSpeedKmh) >
                    kSpeedComparisonTolerance;

                return storedRoadSpeedDiffers || laneInfo.CarLaneCount == 0
                    ? PersistenceAuditState.StoredOverrideNoMarker
                    : PersistenceAuditState.RuntimeModifiedNoMarker;
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
                            carLane.m_DefaultSpeedLimit * 1.8f,
                            (carLane.m_Flags & CarLaneFlags.Highway) != 0);
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

        private string BuildPersistenceAuditSample(
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

            return $"state={GetPersistenceAuditStateLabel(state)} entity={edge.Index}:{edge.Version} kind={kind} prefab=\"{prefabName}\" prefabKmh={prefab} customKmh={custom} effectiveLaneAverageKmh={laneAverage} effectiveLaneRangeKmh={laneRange} carDefaultAverageKmh={carDefaultAverage} carDefaultRangeKmh={carDefaultRange} carLanes={laneInfo.CarLaneCount} highwayCarLanes={laneInfo.HighwayCarLaneCount} trackLanes={laneInfo.TrackLaneCount} {BuildDistrictSpeedModifierSummary(edge)}";
        }

        private string BuildDistrictSpeedModifierSummary(Entity edge)
        {
            if (!EntityManager.HasComponent<BorderDistrict>(edge))
            {
                return "borderDistrict=<none>";
            }

            BorderDistrict borderDistrict = EntityManager.GetComponentData<BorderDistrict>(edge);
            return $"leftDistrict={FormatDistrictSpeedModifier(borderDistrict.m_Left)} rightDistrict={FormatDistrictSpeedModifier(borderDistrict.m_Right)}";
        }

        private string FormatDistrictSpeedModifier(Entity district)
        {
            if (district == Entity.Null)
            {
                return "<none>";
            }

            string id = $"{district.Index}:{district.Version}";
            if (!EntityManager.Exists(district))
            {
                return $"{id}:missing";
            }

            if (!EntityManager.HasBuffer<DistrictModifier>(district))
            {
                return $"{id}:no-modifier-buffer";
            }

            DynamicBuffer<DistrictModifier> modifiers =
                EntityManager.GetBuffer<DistrictModifier>(district, true);
            int streetSpeedIndex = (int)DistrictModifierType.StreetSpeedLimit;
            if (modifiers.Length <= streetSpeedIndex)
            {
                return $"{id}:no-street-speed-slot";
            }

            DistrictModifier modifier = modifiers[streetSpeedIndex];
            float addNominalKmh = modifier.m_Delta.x * 1.8f;
            float multiplyPercent = modifier.m_Delta.y * 100f;
            return $"{id}:addNominalKmh={addNominalKmh.ToString("0.##", CultureInfo.InvariantCulture)},multiplyPercent={multiplyPercent.ToString("0.##", CultureInfo.InvariantCulture)}";
        }

        private static string GetPersistenceAuditStateLabel(PersistenceAuditState state)
        {
            return state switch
            {
                PersistenceAuditState.Healthy => "healthy",
                PersistenceAuditState.StoredOverrideNoMarker => "stored-override-no-marker",
                PersistenceAuditState.RuntimeModifiedNoMarker => "runtime-modified-no-marker",
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
                () => $"{Mod.ModTag} Speed persistence audit kind={kind} total={counts.Total} healthy={counts.Healthy} storedOverrideNoMarker={counts.StoredOverrideNoMarker} runtimeModifiedNoMarker={counts.RuntimeModifiedNoMarker} defaultNoMarker={counts.DefaultNoMarker} componentLaneMismatch={counts.ComponentLaneMismatch} staleMarker={counts.StaleMarker} mixedLanes={counts.MixedLanes} missingPrefab={counts.MissingPrefab} noSupportedLanes={counts.NoSupportedLanes}");
        }
    }
}
#endif
