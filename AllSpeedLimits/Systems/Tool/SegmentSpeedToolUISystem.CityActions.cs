// <copyright file="SegmentSpeedToolUISystem.CityActions.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolUISystem.CityActions.cs
// Purpose: Citywide actions, debug report helpers, and progress bindings for the tool panel.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;       // Dictionary, KeyValuePair
    using System.Globalization;             // CultureInfo
    using System.Linq;                      // OrderByDescending
    using CS2Shared.RiverMochi;             // LogUtils
    using RoadRailSpeeds.Components;        // CustomSpeed
    using Game.Prefabs;                     // PrefabBase, PrefabRef, PrefabSystem, UIObjectData
    using Unity.Collections;                // NativeArray
    using Unity.Entities;                   // Entity

    using PrefabBase = Game.Prefabs.PrefabBase;
    using PrefabRef = Game.Prefabs.PrefabRef;
    using PrefabSystem = Game.Prefabs.PrefabSystem;
    using RoadPrefab = Game.Prefabs.RoadPrefab;
    using TrackPrefab = Game.Prefabs.TrackPrefab;
    using UIObjectData = Game.Prefabs.UIObjectData;
    using WaterwayPrefab = Game.Prefabs.WaterwayPrefab;

    public partial class SegmentSpeedToolUISystem
    {
        private const int kDebugReportMaxRows = 120;

        public void LogDebugReportToLog()
        {
            try
            {
                PrefabSystem prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
                Dictionary<string, int> rows = new Dictionary<string, int>(System.StringComparer.Ordinal);
                int roadOnlyCount = 0;
                int railCount = 0;
                int waterwayCount = 0;
                int customCount = 0;

                using NativeArray<Entity> edges = m_AdjustableEdgeQuery.ToEntityArray(Allocator.Temp);

                for (int i = 0; i < edges.Length; i++)
                {
                    Entity edge = edges[i];
                    Entity baseEdge = GetBaseEdge(edge);

                    if (IsWaterwayType(baseEdge))
                    {
                        waterwayCount++;
                    }
                    else if (IsTrackType(baseEdge))
                    {
                        railCount++;
                    }
                    else if (IsRoadOnly(baseEdge))
                    {
                        roadOnlyCount++;
                    }

                    if (EntityManager.HasComponent<CustomSpeed>(baseEdge))
                    {
                        customCount++;
                    }

                    if (!TryGetPrefab(baseEdge, prefabSystem, out PrefabBase prefabBase, out Entity prefabEntity))
                    {
                        continue;
                    }

                    string row = BuildDebugReportRow(prefabSystem, prefabEntity, prefabBase, baseEdge);
                    rows[row] = rows.TryGetValue(row, out int count) ? count + 1 : 1;
                }

                LogUtils.Info(() => $"{Mod.ModTag} Debug report BEGIN");
                LogUtils.Info(
                    () => $"{Mod.ModTag} Debug report caps: roadMaxKmh={kMaxRoadSpeedKmh:0.##}, railMaxKmh={kMaxTrackSpeedKmh:0.##}, waterMaxKmh={kMaxWaterwaySpeedKmh:0.##}, minKmh={kMinSpeedKmh:0.##}");
                LogUtils.Info(
                    () => $"{Mod.ModTag} Debug report edge counts: totalSupported={edges.Length}, roads={roadOnlyCount}, rails={railCount}, water={waterwayCount}, customSpeeds={customCount}, prefabRows={rows.Count}");

#if DEBUG
                LogSpeedPersistenceAuditToLog(prefabSystem, edges);
#endif

                int logged = 0;
                foreach (KeyValuePair<string, int> row in rows.OrderByDescending(item => item.Value).ThenBy(item => item.Key))
                {
                    if (logged >= kDebugReportMaxRows)
                    {
                        break;
                    }

                    LogUtils.Info(() => $"{Mod.ModTag} Debug report prefab: count={row.Value}, {row.Key}");
                    logged++;
                }

                if (rows.Count > logged)
                {
                    LogUtils.Info(
                        () => $"{Mod.ModTag} Debug report prefab: omitted {rows.Count - logged} additional prefab rows.");
                }

                LogVehicleStatsReportToLog();

                LogUtils.Info(() => $"{Mod.ModTag} Debug report END");
            }
            catch (System.Exception ex)
            {
                LogUtils.Warn(() => $"{Mod.ModTag} Failed to log debug report: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private void ClearCityResetBindings()
        {
            m_CityResetInProgressBinding.Value = false;
            m_CityResetClearedBinding.Value = 0;
            m_CityResetTotalBinding.Value = 0;
        }

        private void ClearCityApplyBindings()
        {
            m_CityApplyInProgressBinding.Value = false;
            m_CityApplyAppliedBinding.Value = 0;
            m_CityApplyTotalBinding.Value = 0;
        }

        private void UpdateCityResetBindings()
        {
            ClearCustomSpeedsSystem? clearSystem = World.GetExistingSystemManaged<ClearCustomSpeedsSystem>();
            bool inProgress = clearSystem?.IsClearInProgress ?? false;
            int cleared = clearSystem?.ClearedCount ?? 0;
            int total = clearSystem?.ClearTotal ?? 0;

            if (m_CityResetInProgressBinding.Value != inProgress)
            {
                m_CityResetInProgressBinding.Value = inProgress;
            }

            if (m_CityResetClearedBinding.Value != cleared)
            {
                m_CityResetClearedBinding.Value = cleared;
            }

            if (m_CityResetTotalBinding.Value != total)
            {
                m_CityResetTotalBinding.Value = total;
            }
        }

        private void UpdateCityApplyBindings()
        {
            CityRoadGroupApplySystem? applySystem = World.GetExistingSystemManaged<CityRoadGroupApplySystem>();
            bool inProgress = applySystem?.IsApplyInProgress ?? false;
            int applied = applySystem?.AppliedCount ?? 0;
            int total = applySystem?.ApplyTotal ?? 0;

            if (m_CityApplyInProgressBinding.Value != inProgress)
            {
                m_CityApplyInProgressBinding.Value = inProgress;
            }

            if (m_CityApplyAppliedBinding.Value != applied)
            {
                m_CityApplyAppliedBinding.Value = applied;
            }

            if (m_CityApplyTotalBinding.Value != total)
            {
                m_CityApplyTotalBinding.Value = total;
            }
        }

        private void HandleApplyCityRoadGroupSpeed(int groupValue, float speedKmh)
        {
            CityRoadGroupApplySystem? applySystem = World.GetExistingSystemManaged<CityRoadGroupApplySystem>();
            if (applySystem == null)
            {
                LogUtils.Error(() => $"{Mod.ModTag} Cannot apply road-group speed: CityRoadGroupApplySystem not found.");
                return;
            }

            if (!System.Enum.IsDefined(typeof(CityRoadGroupApplySystem.RoadGroup), groupValue))
            {
                LogUtils.Warn(() => $"{Mod.ModTag} Ignored unknown road group value {groupValue}.");
                return;
            }

            CityRoadGroupApplySystem.RoadGroup roadGroup = (CityRoadGroupApplySystem.RoadGroup)groupValue;
            applySystem.RequestApplyRoadGroupSpeed(roadGroup, speedKmh);

            LogUtils.Info(
                () => $"{Mod.ModTag} Requested citywide road-group speed apply from panel. group={roadGroup}, targetKmh={speedKmh:0.##}");
        }

        private void HandleApplyCityTrainSpeed(float speedKmh)
        {
            CityRoadGroupApplySystem? applySystem = World.GetExistingSystemManaged<CityRoadGroupApplySystem>();
            if (applySystem == null)
            {
                LogUtils.Error(() => $"{Mod.ModTag} Cannot apply train speed: CityRoadGroupApplySystem not found.");
                return;
            }

            applySystem.RequestApplyTrainSpeed(speedKmh);

            LogUtils.Info(
                () => $"{Mod.ModTag} Requested citywide train speed apply from panel. targetKmh={speedKmh:0.##}");
        }

        private void HandleApplyCitySubwaySpeed(float speedKmh)
        {
            CityRoadGroupApplySystem? applySystem = World.GetExistingSystemManaged<CityRoadGroupApplySystem>();
            if (applySystem == null)
            {
                LogUtils.Error(() => $"{Mod.ModTag} Cannot apply subway speed: CityRoadGroupApplySystem not found.");
                return;
            }

            applySystem.RequestApplySubwaySpeed(speedKmh);

            LogUtils.Info(
                () => $"{Mod.ModTag} Requested citywide subway speed apply from panel. targetKmh={speedKmh:0.##}");
        }

        private bool TryGetPrefab(
            Entity edge,
            PrefabSystem prefabSystem,
            out PrefabBase prefabBase,
            out Entity prefabEntity)
        {
            prefabBase = null!;
            prefabEntity = Entity.Null;

            Entity baseEdge = GetBaseEdge(edge);
            if (!EntityManager.HasComponent<PrefabRef>(baseEdge))
            {
                return false;
            }

            PrefabRef prefabRef = EntityManager.GetComponentData<PrefabRef>(baseEdge);
            prefabEntity = prefabRef.m_Prefab;

            return prefabSystem.TryGetPrefab(prefabRef, out prefabBase) && prefabBase != null;
        }

        private string BuildDebugReportRow(
            PrefabSystem prefabSystem,
            Entity prefabEntity,
            PrefabBase prefabBase,
            Entity edge)
        {
            string groupName = "<no ui group>";
            int itemPriority = -1;
            int groupPriority = -1;

            if (prefabEntity != Entity.Null && EntityManager.HasComponent<UIObjectData>(prefabEntity))
            {
                UIObjectData uiObjectData = EntityManager.GetComponentData<UIObjectData>(prefabEntity);
                itemPriority = uiObjectData.m_Priority;

                if (uiObjectData.m_Group != Entity.Null &&
                    prefabSystem.TryGetPrefab(uiObjectData.m_Group, out PrefabBase? groupPrefab) &&
                    groupPrefab != null)
                {
                    groupName = groupPrefab.name;
                    if (EntityManager.HasComponent<UIObjectData>(uiObjectData.m_Group))
                    {
                        groupPriority = EntityManager.GetComponentData<UIObjectData>(uiObjectData.m_Group).m_Priority;
                    }
                }
            }

            string kind = IsWaterwayType(edge) ? "Water" : IsTrackType(edge) ? "Rail" : IsRoadOnly(edge) ? "Road" : "Other";
            string prefabName = prefabBase.name ?? "<unnamed>";
            string vanillaKmh = GetPrefabSpeedKmh(prefabBase).ToString("0.##", CultureInfo.InvariantCulture);
            string typeInfo = prefabBase switch
            {
                RoadPrefab roadPrefab => $"roadType={roadPrefab.m_RoadType} highwayRules={roadPrefab.m_HighwayRules}",
                TrackPrefab trackPrefab => $"trackType={trackPrefab.m_TrackType}",
                WaterwayPrefab => "waterway=true",
                _ => "typeInfo=<none>"
            };

            return $"kind={kind} group=\"{groupName}\" groupPriority={groupPriority} itemPriority={itemPriority} prefab=\"{prefabName}\" {typeInfo} vanillaKmh={vanillaKmh}";
        }

        private static float GetPrefabSpeedKmh(PrefabBase prefabBase)
        {
            return prefabBase switch
            {
                RoadPrefab roadPrefab => roadPrefab.m_SpeedLimit / 2f,
                TrackPrefab trackPrefab => trackPrefab.m_SpeedLimit / 2f,
                WaterwayPrefab waterwayPrefab => waterwayPrefab.m_SpeedLimit / 2f,
                _ => -1f
            };
        }

        private void HandleResetCityRoads()
        {
            RequestCityReset(ClearCustomSpeedsSystem.ClearScope.Roads, "roads");
        }

        private void HandleResetCityRails()
        {
            RequestCityReset(ClearCustomSpeedsSystem.ClearScope.Rails, "rails");
        }

        private void HandleResetCityWaterways()
        {
            RequestCityReset(ClearCustomSpeedsSystem.ClearScope.Waterways, "waterways");
        }

        private void HandleResetCityAll()
        {
            RequestCityReset(ClearCustomSpeedsSystem.ClearScope.All, "all");
        }

        private void RequestCityReset(ClearCustomSpeedsSystem.ClearScope scope, string label)
        {
            ClearCustomSpeedsSystem? clearSystem = World.GetExistingSystemManaged<ClearCustomSpeedsSystem>();
            if (clearSystem == null)
            {
                LogUtils.Error(() => $"{Mod.ModTag} Cannot reset city speeds: ClearCustomSpeedsSystem not found.");
                return;
            }

            clearSystem.RequestClearCustomSpeeds(scope);
            LogUtils.Info(() => $"{Mod.ModTag} Requested citywide speed reset from panel. scope={label}");
        }
    }
}
