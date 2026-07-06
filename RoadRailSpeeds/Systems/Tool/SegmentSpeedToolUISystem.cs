// <copyright file="SegmentSpeedToolUISystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolUISystem.cs
// Purpose: C# bridge between React bindings and the segment speed tool panel.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;      // List
    using Colossal.UI.Binding;            // IJsonWriter
    using CS2Shared.RiverMochi;            // LogUtils
    using Game.City;                       // CityConfigurationSystem
    using Game.Common;                     // Deleted, Destroyed
    using Game.Input;                      // InputManager
    using Game.Net;                        // Edge, Curve, Road, tracks
    using Game.Prefabs;                    // PrefabRef, ThemePrefab, PrefabSystem
    using Game.UI;                         // SystemUpdatePhase UI base dependencies
    using Game.UI.InGame;                  // Extended info panel base
    using RoadRailSpeeds.Extensions;       // ExtendedInfoSectionBase, ValueBindingHelper
    using Unity.Entities;                  // Entity, EntityQuery
    using UnityEngine;                     // Screen, Vector3
    using Temp = Game.Tools.Temp;          // Temp alias

    [UpdateInGroup(typeof(SelectedInfoUISystem))]
    public partial class SegmentSpeedToolUISystem : ExtendedInfoSectionBase
    {
        private SegmentSpeedToolSystem m_SegmentSpeedTool = null!;
        private SelectedInfoUISystem m_SelectedInfoUISystem = null!;
        private CityConfigurationSystem m_CityConfigurationSystem = null!;
        private SpeedLimitMarkerRenderSystem m_SpeedLimitMarkerRenderSystem = null!;

        private Entity m_SelectedEntity;
        private Entity m_LastCheckedEntity;

        private readonly List<Entity> m_SelectedEdges = new();
        private readonly List<float> m_Speeds = new();
        private EntityQuery m_AdjustableEdgeQuery;
        private EntityQuery m_DeliveryTruckStatsQuery;

        private ValueBindingHelper<float> m_InitialSpeedBinding = null!;
        private ValueBindingHelper<bool> m_ToolActiveBinding = null!;
        private ValueBindingHelper<int> m_SelectionCounterBinding = null!;
        private ValueBindingHelper<bool> m_ShowMetricBinding = null!;
        private ValueBindingHelper<bool> m_IsTrackTypeBinding = null!;
        private ValueBindingHelper<bool> m_IsWaterwayTypeBinding = null!;
        private ValueBindingHelper<bool> m_SelectRoadsBinding = null!;
        private ValueBindingHelper<bool> m_SelectRailsBinding = null!;
        private ValueBindingHelper<bool> m_SelectWaterBinding = null!;
        private ValueBindingHelper<int> m_UnitModeBinding = null!;
        private ValueBindingHelper<bool> m_DoubleSpeedDisplayBinding = null!;
        private ValueBindingHelper<bool> m_CurrentSpeedMixedBinding = null!;
        private ValueBindingHelper<float> m_VanillaSpeedBinding = null!;
        private ValueBindingHelper<bool> m_VanillaSpeedMixedBinding = null!;
        private ValueBindingHelper<bool> m_SyncSliderWithSelectionBinding = null!;
        private ValueBindingHelper<int> m_PanelSliderIncrementBinding = null!;
        private ValueBindingHelper<int> m_TooltipFontScaleBinding = null!;
        private ValueBindingHelper<bool> m_PanelTooltipsEnabledBinding = null!;
        private ValueBindingHelper<bool> m_HideSpeedMarkersBinding = null!;
        private ValueBindingHelper<int> m_CityCarTotalBinding = null!;
        private ValueBindingHelper<int> m_CityCarActiveBinding = null!;
        private ValueBindingHelper<int> m_CityCarParkedBinding = null!;
        private ValueBindingHelper<int> m_CityBikeTotalBinding = null!;
        private ValueBindingHelper<int> m_CityBikeActiveBinding = null!;
        private ValueBindingHelper<int> m_CityBikeParkedBinding = null!;
        private ValueBindingHelper<int> m_CityIndustryTotalBinding = null!;
        private ValueBindingHelper<int> m_CityIndustryActiveBinding = null!;
        private ValueBindingHelper<int> m_CityIndustryParkedBinding = null!;
        private ValueBindingHelper<bool> m_StatsExpandedBinding = null!;
        private ValueBindingHelper<bool> m_CityResetInProgressBinding = null!;
        private ValueBindingHelper<int> m_CityResetClearedBinding = null!;
        private ValueBindingHelper<int> m_CityResetTotalBinding = null!;
        private ValueBindingHelper<bool> m_CityApplyInProgressBinding = null!;
        private ValueBindingHelper<int> m_CityApplyAppliedBinding = null!;
        private ValueBindingHelper<int> m_CityApplyTotalBinding = null!;
        private ValueBindingHelper<string> m_MarkerTooltipTextBinding = null!;
        private ValueBindingHelper<float> m_MarkerTooltipXBinding = null!;
        private ValueBindingHelper<float> m_MarkerTooltipYBinding = null!;
        private ValueBindingHelper<float> m_SelectionClickXBinding = null!;
        private ValueBindingHelper<float> m_SelectionClickYBinding = null!;

        private Setting? m_Settings;
        private int m_LastVehicleStatsFrame = -1;

        protected override string group => Mod.ModId + ".Systems.Tool." + nameof(SegmentSpeedToolUISystem);

        protected override bool displayForUnderConstruction => false;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_InfoUISystem.AddMiddleSection(this);

            m_SelectedInfoUISystem = World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
            m_SegmentSpeedTool = World.GetOrCreateSystemManaged<SegmentSpeedToolSystem>();
            m_CityConfigurationSystem = World.GetOrCreateSystemManaged<CityConfigurationSystem>();
            m_SpeedLimitMarkerRenderSystem = World.GetOrCreateSystemManaged<SpeedLimitMarkerRenderSystem>();
            m_Settings = Mod.Settings;

            m_AdjustableEdgeQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<Edge>(),
                    ComponentType.ReadOnly<Curve>(),
                    ComponentType.ReadOnly<PrefabRef>()
                },
                Any = new[]
                {
                    ComponentType.ReadOnly<Road>(),
                    ComponentType.ReadOnly<TrainTrack>(),
                    ComponentType.ReadOnly<TramTrack>(),
                    ComponentType.ReadOnly<SubwayTrack>(),
                    ComponentType.ReadOnly<Waterway>()
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>()
                }
            });

            m_DeliveryTruckStatsQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<Game.Vehicles.DeliveryTruck>()
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Game.Vehicles.CarTrailer>(),
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Destroyed>(),
                    ComponentType.ReadOnly<Temp>()
                }
            });

            // These keys are read by the React/COHTML UI.
            m_InitialSpeedBinding = CreateBinding("INFOPANEL_ROAD_SPEED", 50f);
            m_ToolActiveBinding = CreateBinding("TOOL_ACTIVE", false);
            m_SelectionCounterBinding = CreateBinding("SELECTION_COUNTER", 0);
            m_ShowMetricBinding = CreateBinding("SHOW_METRIC", true);
            m_IsTrackTypeBinding = CreateBinding("IS_TRACK_TYPE", false);
            m_IsWaterwayTypeBinding = CreateBinding("IS_WATERWAY_TYPE", false);
            m_UnitModeBinding = CreateBinding("UNIT_MODE", 0);
            m_DoubleSpeedDisplayBinding = CreateBinding("DOUBLE_SPEED_DISPLAY", false);
            m_CurrentSpeedMixedBinding = CreateBinding("CURRENT_SPEED_MIXED", false);
            m_VanillaSpeedBinding = CreateBinding("VANILLA_SPEED", -1f);
            m_VanillaSpeedMixedBinding = CreateBinding("VANILLA_SPEED_MIXED", false);
            m_SyncSliderWithSelectionBinding = CreateBinding("SYNC_SLIDER_WITH_SELECTION", true);
            m_PanelSliderIncrementBinding = CreateBinding("PANEL_SLIDER_INCREMENT", 5);
            m_TooltipFontScaleBinding = CreateBinding("TOOLTIP_FONT_SCALE", 110);
            m_PanelTooltipsEnabledBinding = CreateBinding("PANEL_TOOLTIPS_ENABLED", true);
            m_HideSpeedMarkersBinding = CreateBinding("HIDE_SPEED_MARKERS", false);
            m_CityCarTotalBinding = CreateBinding("CITY_CAR_TOTAL", 0);
            m_CityCarActiveBinding = CreateBinding("CITY_CAR_ACTIVE", 0);
            m_CityCarParkedBinding = CreateBinding("CITY_CAR_PARKED", 0);
            m_CityBikeTotalBinding = CreateBinding("CITY_BIKE_TOTAL", 0);
            m_CityBikeActiveBinding = CreateBinding("CITY_BIKE_ACTIVE", 0);
            m_CityBikeParkedBinding = CreateBinding("CITY_BIKE_PARKED", 0);
            m_CityIndustryTotalBinding = CreateBinding("CITY_INDUSTRY_TOTAL", 0);
            m_CityIndustryActiveBinding = CreateBinding("CITY_INDUSTRY_ACTIVE", 0);
            m_CityIndustryParkedBinding = CreateBinding("CITY_INDUSTRY_PARKED", 0);
            m_StatsExpandedBinding = CreateBinding("STATS_EXPANDED", false);
            m_CityResetInProgressBinding = CreateBinding("CITY_RESET_IN_PROGRESS", false);
            m_CityResetClearedBinding = CreateBinding("CITY_RESET_CLEARED", 0);
            m_CityResetTotalBinding = CreateBinding("CITY_RESET_TOTAL", 0);
            m_CityApplyInProgressBinding = CreateBinding("CITY_APPLY_IN_PROGRESS", false);
            m_CityApplyAppliedBinding = CreateBinding("CITY_APPLY_APPLIED", 0);
            m_CityApplyTotalBinding = CreateBinding("CITY_APPLY_TOTAL", 0);
            m_MarkerTooltipTextBinding = CreateBinding("MARKER_TOOLTIP_TEXT", string.Empty);
            m_MarkerTooltipXBinding = CreateBinding("MARKER_TOOLTIP_X", 0f);
            m_MarkerTooltipYBinding = CreateBinding("MARKER_TOOLTIP_Y", 0f);
            m_SelectionClickXBinding = CreateBinding("SELECTION_CLICK_X", 0f);
            m_SelectionClickYBinding = CreateBinding("SELECTION_CLICK_Y", 0f);
            m_SelectRoadsBinding = CreateBinding("SELECT_ROADS", true);
            m_SelectRailsBinding = CreateBinding("SELECT_RAILS", true);
            m_SelectWaterBinding = CreateBinding("SELECT_WATER", true);

            // These triggers are called from React.
            CreateTrigger<float>("APPLY_SPEED", HandleApplySpeed);
            CreateTrigger<float>("APPLY_SELECTION_MULTIPLIER", HandleApplySelectionMultiplier);
            CreateTrigger<int, float>("APPLY_CITY_ROAD_GROUP_SPEED", HandleApplyCityRoadGroupSpeed);
            CreateTrigger<float>("APPLY_CITY_TRAIN_SPEED", HandleApplyCityTrainSpeed);
            CreateTrigger<float>("APPLY_CITY_SUBWAY_SPEED", HandleApplyCitySubwaySpeed);
            CreateTrigger("RESET_SPEED", HandleResetSpeed);
            CreateTrigger("TOGGLE_UNIT", HandleToggleUnit);
            CreateTrigger<bool>("ACTIVATE_TOOL", HandleActivateTool);
            CreateTrigger<bool>("SET_PANEL_TOOLTIPS_ENABLED", HandleSetPanelTooltipsEnabled);
            CreateTrigger<bool>("SET_HIDE_SPEED_MARKERS", HandleSetHideSpeedMarkers);
            CreateTrigger<bool>("SET_STATS_EXPANDED", HandleSetStatsExpanded);
            CreateTrigger("RESET_CITY_ROADS", HandleResetCityRoads);
            CreateTrigger("RESET_CITY_RAILS", HandleResetCityRails);
            CreateTrigger("RESET_CITY_WATERWAYS", HandleResetCityWaterways);
            CreateTrigger("RESET_CITY_ALL", HandleResetCityAll);
            CreateTrigger<bool>("SET_SELECT_ROADS", HandleSetSelectRoads);
            CreateTrigger<bool>("SET_SELECT_RAILS", HandleSetSelectRails);
            CreateTrigger<bool>("SET_SELECT_WATER", HandleSetSelectWater);

            m_InitialSpeedBinding.Value = 50f;
            m_ToolActiveBinding.Value = false;
            m_SelectionCounterBinding.Value = 0;
            m_ShowMetricBinding.Value = ShouldShowMetric();
            m_IsTrackTypeBinding.Value = false;
            m_IsWaterwayTypeBinding.Value = false;
            m_UnitModeBinding.Value = (int)(m_Settings?.SpeedUnitPreference ?? Setting.SpeedUnit.Auto);
            m_DoubleSpeedDisplayBinding.Value = m_Settings?.DoubleSpeedDisplay ?? false;
            m_CurrentSpeedMixedBinding.Value = false;
            m_VanillaSpeedBinding.Value = -1f;
            m_VanillaSpeedMixedBinding.Value = false;
            m_SyncSliderWithSelectionBinding.Value = m_Settings?.SyncSliderWithSelection ?? true;
            m_PanelSliderIncrementBinding.Value = GetPanelSliderIncrement();
            m_TooltipFontScaleBinding.Value = GetTooltipFontScale();
            m_PanelTooltipsEnabledBinding.Value = m_Settings?.PanelTooltipsEnabled ?? true;
            m_HideSpeedMarkersBinding.Value = m_Settings?.HideSpeedMarkers ?? false;
            m_CityCarTotalBinding.Value = 0;
            m_CityCarActiveBinding.Value = 0;
            m_CityCarParkedBinding.Value = 0;
            m_CityBikeTotalBinding.Value = 0;
            m_CityBikeActiveBinding.Value = 0;
            m_CityBikeParkedBinding.Value = 0;
            m_CityIndustryTotalBinding.Value = 0;
            m_CityIndustryActiveBinding.Value = 0;
            m_CityIndustryParkedBinding.Value = 0;
            m_StatsExpandedBinding.Value = false;
            m_CityResetInProgressBinding.Value = false;
            m_CityResetClearedBinding.Value = 0;
            m_CityResetTotalBinding.Value = 0;
            m_CityApplyInProgressBinding.Value = false;
            m_CityApplyAppliedBinding.Value = 0;
            m_CityApplyTotalBinding.Value = 0;
            m_MarkerTooltipTextBinding.Value = string.Empty;
            m_MarkerTooltipXBinding.Value = 0f;
            m_MarkerTooltipYBinding.Value = 0f;
            m_SelectionClickXBinding.Value = 0f;
            m_SelectionClickYBinding.Value = 0f;

            // Push initial binding values so React does not read uninitialized values.
            RequestUpdate();
        }

        protected override void Reset()
        {
            m_SelectedEntity = Entity.Null;
            m_LastCheckedEntity = Entity.Null;
            m_SelectedEdges.Clear();

            m_InitialSpeedBinding.Value = 50f;
            m_ToolActiveBinding.Value = false;
            m_SelectionCounterBinding.Value = 0;
            m_CurrentSpeedMixedBinding.Value = false;
            m_VanillaSpeedBinding.Value = -1f;
            m_VanillaSpeedMixedBinding.Value = false;
            m_IsTrackTypeBinding.Value = false;
            m_IsWaterwayTypeBinding.Value = false;
            m_PanelTooltipsEnabledBinding.Value = m_Settings?.PanelTooltipsEnabled ?? true;
            m_HideSpeedMarkersBinding.Value = m_Settings?.HideSpeedMarkers ?? false;
            m_StatsExpandedBinding.Value = false;
            ClearCityVehicleStatsBindings();
            ClearCityResetBindings();
            ClearCityApplyBindings();
            ClearMarkerTooltipBindings();
            ClearSelectionClickBindings();
            m_LastVehicleStatsFrame = -1;

            visible = false;
        }

        public void OnSegmentsSelectedByTool(Entity aggregate, IReadOnlyList<Entity> edges)
        {
            // aggregate is kept for future street-wide behavior; the current UI uses selected edges.
            _ = aggregate;

            m_SelectedEdges.Clear();
            m_SelectedEdges.AddRange(edges);

            if (edges.Count == 0)
            {
                return;
            }

            m_SelectedEntity = edges[0];

            SelectionSpeedInfo selectionSpeed = GetSelectionSpeedInfo(edges);
            if (!selectionSpeed.HasCurrentSpeed)
            {
                return;
            }

            // Record where to pop the panel up only when this click starts a brand-new selection
            // (counter is still 0, i.e. nothing was selected a moment ago). Extending or replacing
            // an existing selection must not move the panel out from under the player mid-edit.
            if (m_SelectionCounterBinding.Value == 0)
            {
                Vector3 mousePosition = InputManager.instance.mousePosition;
                m_SelectionClickXBinding.Value = mousePosition.x;
                m_SelectionClickYBinding.Value = Screen.height - mousePosition.y;
            }

            m_InitialSpeedBinding.Value = selectionSpeed.CurrentSpeed;
            m_CurrentSpeedMixedBinding.Value = selectionSpeed.CurrentSpeedMixed;
            m_VanillaSpeedBinding.Value = selectionSpeed.HasVanillaSpeed ? selectionSpeed.VanillaSpeed : -1f;
            m_VanillaSpeedMixedBinding.Value = selectionSpeed.VanillaSpeedMixed;
            m_IsTrackTypeBinding.Value = selectionSpeed.ContainsTrackType;
            m_IsWaterwayTypeBinding.Value = selectionSpeed.ContainsWaterwayType;
            visible = true;

            // Force React to treat this as a new selection, even if the speed value did not change.
            m_SelectionCounterBinding.Value++;

            RequestUpdate();
        }

        protected override void OnProcess()
        {
            // This info-section system updates through OnUpdate and explicit RequestUpdate calls.
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            m_Settings ??= Mod.Settings;

            bool toolActive = m_SegmentSpeedTool.IsActive;
            bool toolStateChanged = m_ToolActiveBinding.Value != toolActive;

            m_ToolActiveBinding.Value = toolActive;

            bool showMetric = ShouldShowMetric();
            if (m_ShowMetricBinding.Value != showMetric)
            {
                m_ShowMetricBinding.Value = showMetric;
            }

            bool doubleSpeedDisplay = m_Settings?.DoubleSpeedDisplay ?? false;
            if (m_DoubleSpeedDisplayBinding.Value != doubleSpeedDisplay)
            {
                m_DoubleSpeedDisplayBinding.Value = doubleSpeedDisplay;
            }

            bool syncSliderWithSelection = m_Settings?.SyncSliderWithSelection ?? true;
            if (m_SyncSliderWithSelectionBinding.Value != syncSliderWithSelection)
            {
                m_SyncSliderWithSelectionBinding.Value = syncSliderWithSelection;
            }

            int panelSliderIncrement = GetPanelSliderIncrement();
            if (m_PanelSliderIncrementBinding.Value != panelSliderIncrement)
            {
                m_PanelSliderIncrementBinding.Value = panelSliderIncrement;
            }

            int tooltipFontScale = GetTooltipFontScale();
            if (m_TooltipFontScaleBinding.Value != tooltipFontScale)
            {
                m_TooltipFontScaleBinding.Value = tooltipFontScale;
            }

            bool panelTooltipsEnabled = m_Settings?.PanelTooltipsEnabled ?? true;
            if (m_PanelTooltipsEnabledBinding.Value != panelTooltipsEnabled)
            {
                m_PanelTooltipsEnabledBinding.Value = panelTooltipsEnabled;
            }

            bool hideSpeedMarkers = m_Settings?.HideSpeedMarkers ?? false;
            if (m_HideSpeedMarkersBinding.Value != hideSpeedMarkers)
            {
                m_HideSpeedMarkersBinding.Value = hideSpeedMarkers;
            }

            UpdateCityResetBindings();
            UpdateCityApplyBindings();
            UpdateMarkerTooltipBindings(toolActive);

            if (!toolActive && toolStateChanged)
            {
                // Tool was closed, so clear the visible panel state.
                m_InitialSpeedBinding.Value = 50f;
                m_SelectionCounterBinding.Value = 0;
                m_CurrentSpeedMixedBinding.Value = false;
                m_VanillaSpeedBinding.Value = -1f;
                m_VanillaSpeedMixedBinding.Value = false;
                m_IsTrackTypeBinding.Value = false;
                m_IsWaterwayTypeBinding.Value = false;
                ClearCityVehicleStatsBindings();
                ClearCityApplyBindings();
                ClearMarkerTooltipBindings();
                ClearSelectionClickBindings();
                m_LastVehicleStatsFrame = -1;
                visible = false;
                m_SelectedEdges.Clear();
                m_SelectedEntity = Entity.Null;
                m_LastCheckedEntity = Entity.Null;

                RequestUpdate();
                return;
            }

            if (toolActive && toolStateChanged)
            {
                // Fallback only: HandleActivateTool (the toolbar button path) already does this
                // reset directly, since it overwrites m_ToolActiveBinding.Value itself and would
                // otherwise erase this edge before this OnUpdate ever saw it. This covers any other
                // route that could activate the tool without going through that trigger.
                m_SelectRoadsBinding.Value = true;
                m_SelectRailsBinding.Value = true;
                m_SelectWaterBinding.Value = true;
            }

            if (toolActive && visible && m_StatsExpandedBinding.Value)
            {
                RefreshVehicleStatsIfNeeded(toolStateChanged);
            }

            if (toolStateChanged)
            {
                RequestUpdate();
            }
        }

        private void HandleSetPanelTooltipsEnabled(bool enabled)
        {
            m_Settings ??= Mod.Settings;
            if (m_Settings == null || m_Settings.PanelTooltipsEnabled == enabled)
            {
                return;
            }

            m_Settings.PanelTooltipsEnabled = enabled;
            m_Settings.ApplyAndSave();
            m_PanelTooltipsEnabledBinding.Value = enabled;
            RequestUpdate();
        }

        private void HandleSetHideSpeedMarkers(bool hidden)
        {
            m_Settings ??= Mod.Settings;
            if (m_Settings == null || m_Settings.HideSpeedMarkers == hidden)
            {
                return;
            }

            m_Settings.HideSpeedMarkers = hidden;
            m_Settings.ApplyAndSave();
            m_HideSpeedMarkersBinding.Value = hidden;
            RequestUpdate();
        }

        private void HandleSetStatsExpanded(bool expanded)
        {
            if (m_StatsExpandedBinding.Value == expanded)
            {
                return;
            }

            m_StatsExpandedBinding.Value = expanded;
            m_LastVehicleStatsFrame = -1;
            RequestUpdate();
        }

        // Selection type filter toggles. The tool reads these flags when it builds its raycast mask.
        private void HandleSetSelectRoads(bool enabled)
        {
            m_SegmentSpeedTool.IncludeRoads = enabled;
            m_SelectRoadsBinding.Value = enabled;
        }

        private void HandleSetSelectRails(bool enabled)
        {
            m_SegmentSpeedTool.IncludeRails = enabled;
            m_SelectRailsBinding.Value = enabled;
        }

        private void HandleSetSelectWater(bool enabled)
        {
            m_SegmentSpeedTool.IncludeWater = enabled;
            m_SelectWaterBinding.Value = enabled;
        }

        public override void OnWriteProperties(IJsonWriter writer)
        {
        }

        private bool ShouldShowMetric()
        {
            try
            {
                bool isEUMap = true;

                if (m_CityConfigurationSystem.defaultTheme != Entity.Null)
                {
                    PrefabSystem prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
                    ThemePrefab theme = prefabSystem.GetPrefab<ThemePrefab>(m_CityConfigurationSystem.defaultTheme);
                    string themeName = theme?.name ?? string.Empty;

                    isEUMap = !themeName.Equals("North American", System.StringComparison.Ordinal);
                }

                return m_Settings?.ShouldShowMetric(isEUMap) ?? isEUMap;
            }
            catch (System.Exception ex)
            {
                LogUtils.Warn($"Failed to determine unit preference: {ex.GetType().Name}: {ex.Message}", ex);
                return true;
            }
        }

        private int GetPanelSliderIncrement()
        {
            int increment = m_Settings?.PanelSliderIncrement ?? 5;
            if (increment < 5)
            {
                return 5;
            }

            if (increment > 25)
            {
                return 25;
            }

            return increment;
        }

        private int GetTooltipFontScale()
        {
            int scale = m_Settings?.TooltipFontScale ?? 110;
            if (scale < 100)
            {
                return 100;
            }

            if (scale > 140)
            {
                return 140;
            }

            return scale;
        }

        private void HandleToggleUnit()
        {
            if (m_Settings == null)
            {
                return;
            }

            // Title-bar unit button cycles the same setting used by Options:
            // A -> km -> mi -> A. Keep this as a real three-state control.
            m_Settings.SpeedUnitPreference = m_Settings.SpeedUnitPreference switch
            {
                Setting.SpeedUnit.Auto => Setting.SpeedUnit.Metric,
                Setting.SpeedUnit.Metric => Setting.SpeedUnit.Imperial,
                _ => Setting.SpeedUnit.Auto
            };

            m_Settings.ApplyAndSave();

            m_ShowMetricBinding.Value = ShouldShowMetric();
            m_UnitModeBinding.Value = (int)m_Settings.SpeedUnitPreference;

            RequestUpdate();
        }

        private void HandleActivateTool(bool enable)
        {
            if (!enable)
            {
                m_SelectionCounterBinding.Value = 0;
                m_InitialSpeedBinding.Value = 50f;
                m_CurrentSpeedMixedBinding.Value = false;
                m_VanillaSpeedBinding.Value = -1f;
                m_VanillaSpeedMixedBinding.Value = false;
                m_IsTrackTypeBinding.Value = false;
                m_IsWaterwayTypeBinding.Value = false;
                ClearCityVehicleStatsBindings();
                ClearCityApplyBindings();
                ClearMarkerTooltipBindings();
                ClearSelectionClickBindings();
                m_LastVehicleStatsFrame = -1;
                visible = false;
                m_SelectedEdges.Clear();
                m_SelectedEntity = Entity.Null;
                m_LastCheckedEntity = Entity.Null;
            }
            else
            {
                // Tool is opening. SegmentSpeedToolSystem.OnStartRunning already reset
                // IncludeRoads/Rails/Water to true; re-sync the UI chips to match RIGHT HERE rather
                // than relying on OnUpdate's toolStateChanged edge detection below. This handler
                // overwrites m_ToolActiveBinding.Value a few lines down, which erases that edge
                // before OnUpdate ever observes it, so the OnUpdate-based reset never actually fires
                // for the normal "click the toolbar button" path (it stayed as a fallback for any
                // activation route that bypasses this trigger).
                m_SelectRoadsBinding.Value = true;
                m_SelectRailsBinding.Value = true;
                m_SelectWaterBinding.Value = true;
            }

            m_SegmentSpeedTool.ToggleTool(enable);
            m_ToolActiveBinding.Value = m_SegmentSpeedTool.IsActive;

            RequestUpdate();
        }

        private void UpdateMarkerTooltipBindings(bool toolActive)
        {
            if (!toolActive ||
                !m_SpeedLimitMarkerRenderSystem.TryGetMarkerTooltip(out string text, out float x, out float y))
            {
                ClearMarkerTooltipBindings();
                return;
            }

            if (m_MarkerTooltipTextBinding.Value == text &&
                System.Math.Abs(m_MarkerTooltipXBinding.Value - x) < 1f &&
                System.Math.Abs(m_MarkerTooltipYBinding.Value - y) < 1f)
            {
                return;
            }

            m_MarkerTooltipTextBinding.Value = text;
            m_MarkerTooltipXBinding.Value = x;
            m_MarkerTooltipYBinding.Value = y;
            RequestUpdate();
        }

        private void ClearMarkerTooltipBindings()
        {
            if (string.IsNullOrEmpty(m_MarkerTooltipTextBinding.Value) &&
                m_MarkerTooltipXBinding.Value == 0f &&
                m_MarkerTooltipYBinding.Value == 0f)
            {
                return;
            }

            m_MarkerTooltipTextBinding.Value = string.Empty;
            m_MarkerTooltipXBinding.Value = 0f;
            m_MarkerTooltipYBinding.Value = 0f;
            RequestUpdate();
        }

        private void ClearSelectionClickBindings()
        {
            m_SelectionClickXBinding.Value = 0f;
            m_SelectionClickYBinding.Value = 0f;
        }
    }
}
