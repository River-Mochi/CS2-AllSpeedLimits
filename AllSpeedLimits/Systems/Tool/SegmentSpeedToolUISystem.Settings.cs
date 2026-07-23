// <copyright file="SegmentSpeedToolUISystem.Settings.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolUISystem.Settings.cs
// Purpose: Settings, unit preference, panel layout, selection-filter, and activation handlers.

namespace RoadRailSpeeds.Systems
{
    using CS2Shared.RiverMochi;            // LogUtils
    using Game.Prefabs;                    // PrefabSystem, ThemePrefab
    using Unity.Entities;                  // Entity

    public partial class SegmentSpeedToolUISystem
    {
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

        private void HandleSetToolPanelPosition(int x, int y)
        {
            m_Settings ??= Mod.Settings;
            if (m_Settings == null)
            {
                return;
            }

            x = ClampPanelPosition(x);
            y = ClampPanelPosition(y);
            if (m_Settings.ToolPanelPositionX == x && m_Settings.ToolPanelPositionY == y)
            {
                return;
            }

            m_Settings.ToolPanelPositionX = x;
            m_Settings.ToolPanelPositionY = y;
            m_Settings.ApplyAndSave();
            m_ToolPanelXBinding.Value = x;
            m_ToolPanelYBinding.Value = y;
            RequestUpdate();
        }

        private void HandleSetSelectionInfoExpanded(bool expanded)
        {
            m_Settings ??= Mod.Settings;
            if (m_Settings == null || m_Settings.SelectionInfoExpanded == expanded)
            {
                return;
            }

            m_Settings.SelectionInfoExpanded = expanded;
            m_Settings.ApplyAndSave();
            m_SelectionInfoExpandedBinding.Value = expanded;
            RequestUpdate();
        }

        private void HandleSetSliderExpanded(bool expanded)
        {
            m_Settings ??= Mod.Settings;
            if (m_Settings == null || m_Settings.SliderExpanded == expanded)
            {
                return;
            }

            m_Settings.SliderExpanded = expanded;
            m_Settings.ApplyAndSave();
            m_SliderExpandedBinding.Value = expanded;
            RequestUpdate();
        }

        private void HandleSetWholeCityExpanded(bool expanded)
        {
            m_Settings ??= Mod.Settings;
            if (m_Settings == null || m_Settings.WholeCityExpanded == expanded)
            {
                return;
            }

            m_Settings.WholeCityExpanded = expanded;
            m_Settings.ApplyAndSave();
            m_WholeCityExpandedBinding.Value = expanded;
            RequestUpdate();
        }

        private void HandleSetStatsExpanded(bool expanded)
        {
            m_Settings ??= Mod.Settings;
            if (m_Settings == null || m_Settings.StatsExpanded == expanded)
            {
                return;
            }

            m_Settings.StatsExpanded = expanded;
            m_Settings.ApplyAndSave();
            m_StatsExpandedBinding.Value = expanded;
            m_LastVehicleStatsRefreshTime = -1f;
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

        private static int ClampPanelPosition(int value)
        {
            const int limit = 20000;
            if (value < 0)
            {
                return -1;
            }

            return value > limit ? limit : value;
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
            int increment = m_Settings?.PanelSliderIncrement ?? 10;
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
                SpeedLimitsSetting.SpeedUnit.Auto => SpeedLimitsSetting.SpeedUnit.Metric,
                SpeedLimitsSetting.SpeedUnit.Metric => SpeedLimitsSetting.SpeedUnit.Imperial,
                _ => SpeedLimitsSetting.SpeedUnit.Auto
            };

            m_Settings.ApplyAndSave();

            m_ShowMetricBinding.Value = ShouldShowMetric();
            m_UnitModeBinding.Value = (int)m_Settings.SpeedUnitPreference;

            RequestUpdate();
        }

        private void HandleSetPanelSpeedUnit(bool showMetric)
        {
            if (m_Settings == null)
            {
                return;
            }

            SpeedLimitsSetting.SpeedUnit nextPreference = showMetric
                ? SpeedLimitsSetting.SpeedUnit.Metric
                : SpeedLimitsSetting.SpeedUnit.Imperial;

            if (m_Settings.SpeedUnitPreference != nextPreference)
            {
                m_Settings.SpeedUnitPreference = nextPreference;
                m_Settings.ApplyAndSave();
            }

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
                m_LastVehicleStatsRefreshTime = -1f;
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

    }
}
