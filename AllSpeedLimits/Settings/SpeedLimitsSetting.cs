// <copyright file="SpeedLimitsSetting.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Settings/SpeedLimitsSetting.cs
// Purpose: Mod settings model and settings UI actions.

namespace RoadRailSpeeds
{
    using System;                    // Exception
    using Colossal.IO.AssetDatabase; // FileLocation
    using Colossal.Json;             // Exclude
    using CS2Shared.RiverMochi;      // LogUtils, ShellOpen
    using Game;                      // GameMode
    using Game.SceneFlow;            // GameManager
    using Game.Input;                // ActionType, BindingKeyboard, ProxyBinding
    using Game.Modding;              // IMod
    using Game.Settings;             // ModSetting, settings attributes
    using Game.UI;                   // Unit
    using RoadRailSpeeds.Systems;    // ClearCustomSpeedsSystem, SegmentSpeedToolUISystem
    using Unity.Entities;            // World

    [FileLocation("ModsSettings/AllSpeedLimits/AllSpeedLimits")]
    [SettingsUITabOrder(kMainTab, kBicycleTab, kAboutTab)]
    [SettingsUIGroupOrder(kKeybindsGroup, kDisplayGroup, kResetGroup, kUsageGroup, kBicycleGroup, kPersonalStatusGroup, kAboutInfoGroup, kAboutLinksGroup, kAboutDebugGroup)]
    [SettingsUIShowGroupName(kKeybindsGroup, kDisplayGroup, kResetGroup, kUsageGroup, kBicycleGroup, kPersonalStatusGroup, kAboutDebugGroup)]
    [SettingsUIKeyboardAction(Mod.kToggleInvisibleActionName, ActionType.Button, usages: new[] { "Game" })]
    public class SpeedLimitsSetting : ModSetting
    {
        public const string kMainTab = "Main";
        public const string kBicycleTab = "Bicycles";
        public const string kAboutTab = "About";

        public const string kDisplayGroup = "Display";
        public const string kKeybindsGroup = "Keybinds";
        public const string kBicycleGroup = "BicycleSpeed";
        public const string kPersonalStatusGroup = "PersonalVehicleStatus";
        public const string kResetGroup = "ResetGameDefaults";
        public const string kUsageGroup = "Usage";
        public const string kAboutInfoGroup = "AboutInfo";
        public const string kAboutLinksGroup = "AboutLinks";
        public const string kAboutDebugGroup = "AboutDebug";

        private const string UsageIconPath = "coui://ui-mods/images/icon-speedlimit30.svg";
        private const string kAboutLinksRow = nameof(kAboutLinksRow);
        private const string kAboutDebugRow = nameof(kAboutDebugRow);
        private const string kUrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        public SpeedLimitsSetting(IMod mod)
            : base(mod)
        {
        }

        [SettingsUISection(kMainTab, kDisplayGroup)]
        public SpeedUnit SpeedUnitPreference { get; set; } = SpeedUnit.Auto;

        [SettingsUISection(kMainTab, kDisplayGroup)]
        [SettingsUISlider(min = 5f, max = 25f, step = 5f, unit = "integer")]
        public int PanelSliderIncrement { get; set; } = 10;

        [SettingsUISection(kMainTab, kDisplayGroup)]
        [SettingsUISlider(min = 100f, max = 140f, step = 5f, unit = Unit.kPercentage)]
        public int TooltipFontScale { get; set; } = 110;

        [SettingsUISection(kMainTab, kDisplayGroup)]
        public bool DoubleSpeedDisplay { get; set; }

        [SettingsUISection(kMainTab, kDisplayGroup)]
        public bool SyncSliderWithSelection { get; set; } = true;

        [SettingsUISection(kMainTab, kDisplayGroup)]
        public bool PanelTooltipsEnabled { get; set; } = true;

        // Toggled from the panel title bar. Hides the floating speed numbers over roads/rails/water.
        [SettingsUIHidden]
        public bool HideSpeedMarkers { get; set; }

        [SettingsUIHidden]
        public int ToolPanelPositionX { get; set; } = -1;

        [SettingsUIHidden]
        public int ToolPanelPositionY { get; set; } = -1;

        [SettingsUIHidden]
        public bool SelectionInfoExpanded { get; set; } = true;

        [SettingsUIHidden]
        public bool SliderExpanded { get; set; } = true;

        [SettingsUIHidden]
        public bool WholeCityExpanded { get; set; } = true;

        [SettingsUIHidden]
        public bool StatsExpanded { get; set; } = true;

        // Stored in Options for persistence. GlobalPathSpeedPercent keeps existing
        // bike/mixed values from older versions; pedestrian-only is independent.
        internal const int kMinPathSpeedPercent = 30;
        internal const int kMaxPathSpeedPercent = 300;
        private int m_GlobalPathSpeedPercent = 100;
        private int m_GlobalPedPathSpeedPercent = 100;

        [SettingsUIHidden]
        public int GlobalPathSpeedPercent
        {
            get => m_GlobalPathSpeedPercent;
            set => m_GlobalPathSpeedPercent = Math.Max(kMinPathSpeedPercent, Math.Min(kMaxPathSpeedPercent, value));
        }

        [SettingsUIHidden]
        public int GlobalPedPathSpeedPercent
        {
            get => m_GlobalPedPathSpeedPercent;
            set => m_GlobalPedPathSpeedPercent = Math.Max(kMinPathSpeedPercent, Math.Min(kMaxPathSpeedPercent, value));
        }

        private bool m_EnableBicycleSpeedTuning;
        private int m_BicycleSpeedPercent = 100;

        [SettingsUISection(kBicycleTab, kBicycleGroup)]
        public bool EnableBicycleSpeedTuning
        {
            get => m_EnableBicycleSpeedTuning;
            set
            {
                if (m_EnableBicycleSpeedTuning != value)
                {
                    m_EnableBicycleSpeedTuning = value;
                    RequestBicycleSpeedApply();
                }
            }
        }

        [SettingsUISection(kBicycleTab, kBicycleGroup)]
        [SettingsUISlider(min = 30f, max = 1000f, step = 10f, unit = Unit.kPercentage)]
        [SettingsUIHideByCondition(typeof(SpeedLimitsSetting), nameof(EnableBicycleSpeedTuning), true)]
        public int BicycleSpeedPercent
        {
            get => m_BicycleSpeedPercent;
            set
            {
                int percent = Math.Max(30, Math.Min(1000, value));
                if (m_BicycleSpeedPercent != percent)
                {
                    m_BicycleSpeedPercent = percent;
                    RequestBicycleSpeedApply();
                }
            }
        }

        [SettingsUIButton]
        [SettingsUISection(kBicycleTab, kBicycleGroup)]
        [SettingsUIHideByCondition(typeof(SpeedLimitsSetting), nameof(EnableBicycleSpeedTuning), true)]
        public bool ResetBicycleSpeed
        {
            set
            {
                if (value && EnableBicycleSpeedTuning)
                {
                    BicycleSpeedPercent = 100;
                    ApplyAndSave();
                    RequestBicycleSpeedApply();
                }
            }
        }

        [Exclude]
        [SettingsUISection(kBicycleTab, kPersonalStatusGroup)]
        public string StatusSummary1
        {
            get
            {
                RefreshPersonalVehicleStatusSafe();
                return PersonalVehicleStatus.BikesRow;
            }
        }

        [Exclude]
        [SettingsUISection(kBicycleTab, kPersonalStatusGroup)]
        public string StatusSummary2
        {
            get
            {
                RefreshPersonalVehicleStatusSafe();
                return PersonalVehicleStatus.CarsRow;
            }
        }

        [Exclude]
        [SettingsUISection(kBicycleTab, kPersonalStatusGroup)]
        public string StatusSummary3
        {
            get
            {
                RefreshPersonalVehicleStatusSafe();
                return PersonalVehicleStatus.CarsRow3;
            }
        }

        [SettingsUIButton]
        [SettingsUIButtonGroup("PersonalStatusButtons")]
        [SettingsUISection(kBicycleTab, kPersonalStatusGroup)]
        public bool LogBorderHiddenCars
        {
            set
            {
                if (!value)
                {
                    return;
                }

                World? world = World.DefaultGameObjectInjectionWorld;
                if (world != null && world.IsCreated &&
                    GameManager.instance?.gameMode == GameMode.Game)
                {
                    world.GetOrCreateSystemManaged<PersonalVehicleStatusSystem>().LogBorderParkedSamples();
                }
            }
        }

        [SettingsUIButton]
        [SettingsUIButtonGroup("PersonalStatusButtons")]
        [SettingsUISection(kBicycleTab, kPersonalStatusGroup)]
        public bool OpenLogFromStatus
        {
            set
            {
                if (value)
                {
                    ShellOpen.OpenModLogOrLogsFolder();
                }
            }
        }

        private static void RefreshPersonalVehicleStatusSafe()
        {
            try
            {
                PersonalVehicleStatus.RefreshIfNeeded();
            }
            catch (Exception exception)
            {
                LogUtils.WarnOnce("PersonalVehicleStatusRefresh", () => $"{Mod.ModTag} Personal vehicle status refresh failed: {exception.Message}");
            }
        }

        private void RequestBicycleSpeedApply()
        {
            // Ignore temporary/default settings objects during deserialization.
            if (Mod.Settings != this)
            {
                return;
            }

            World? world = World.DefaultGameObjectInjectionWorld;
            if (world != null && world.IsCreated)
            {
                world.GetExistingSystemManaged<BicycleSpeedSystem>()?.ScheduleApply();
            }
        }

        // Unassigned by default so ASL cannot collide with Move It (Ctrl+M).
        [SettingsUISection(kMainTab, kKeybindsGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.None, Mod.kToggleInvisibleActionName)]
        public ProxyBinding ToggleInvisibleMarkersBinding { get; set; }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kMainTab, kResetGroup)]
        public bool ClearAllCustomSpeeds
        {
            set
            {
                if (value)
                {
                    ClearAllCustomSpeedsAction();
                }
            }
        }

        [SettingsUISection(kAboutTab, kAboutInfoGroup)]
        public string NameText => Mod.ModName;

        [SettingsUISection(kAboutTab, kAboutInfoGroup)]
        public string VersionText =>
#if DEBUG
            Mod.ModVersion + " (DEBUG)";
#else
            Mod.ModVersion;
#endif

        [SettingsUIButtonGroup(kAboutLinksRow)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutLinksGroup)]
        public bool OpenParadox
        {
            set
            {
                if (value)
                {
                    TryOpenUrl(kUrlParadox);
                }
            }
        }

        [SettingsUIButtonGroup(kAboutDebugRow)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutDebugGroup)]
        public bool DebugReportToLog
        {
            set
            {
                if (value)
                {
                    DebugReportToLogAction();
                }
            }
        }

        [SettingsUIButtonGroup(kAboutDebugRow)]
        [SettingsUIButton]
        [SettingsUISection(kAboutTab, kAboutDebugGroup)]
        public bool OpenLog
        {
            set
            {
                if (value)
                {
                    ShellOpen.OpenModLogOrLogsFolder();
                }
            }
        }

        [SettingsUISection(kMainTab, kUsageGroup)]
        public bool ShowUsage { get; set; }

        [SettingsUIMultilineText(UsageIconPath)]
        [SettingsUIHideByCondition(typeof(SpeedLimitsSetting), nameof(HideUsageText))]
        [SettingsUISection(kMainTab, kUsageGroup)]
        public string UsageText => string.Empty;

        public override void SetDefaults()
        {
            SpeedUnitPreference = SpeedUnit.Auto;
            DoubleSpeedDisplay = false;
            SyncSliderWithSelection = true;
            PanelSliderIncrement = 10;
            TooltipFontScale = 110;
            PanelTooltipsEnabled = true;
            HideSpeedMarkers = false;
            ToolPanelPositionX = -1;
            ToolPanelPositionY = -1;
            SelectionInfoExpanded = true;
            SliderExpanded = true;
            WholeCityExpanded = true;
            StatsExpanded = true;
            GlobalPathSpeedPercent = 100;
            GlobalPedPathSpeedPercent = 100;
            EnableBicycleSpeedTuning = false;
            BicycleSpeedPercent = 100;
            ShowUsage = false;
        }

        public bool ShouldShowMetric(bool isEUMap)
        {
            return SpeedUnitPreference switch
            {
                SpeedUnit.Auto => isEUMap,
                SpeedUnit.Metric => true,
                SpeedUnit.Imperial => false,
                _ => isEUMap
            };
        }

        private static void ClearAllCustomSpeedsAction()
        {
            try
            {
                World? world = World.DefaultGameObjectInjectionWorld;
                if (world == null)
                {
                    LogUtils.Error(() => $"{Mod.ModTag} Cannot clear speeds: World is null.");
                    return;
                }

                ClearCustomSpeedsSystem? clearSystem =
                    world.GetExistingSystemManaged<ClearCustomSpeedsSystem>();

                if (clearSystem == null)
                {
                    LogUtils.Error(() => $"{Mod.ModTag} Cannot clear speeds: ClearCustomSpeedsSystem not found.");
                    return;
                }

                clearSystem.RequestClearAllCustomSpeeds();
                LogUtils.Info(() => $"{Mod.ModTag} Requested clear all custom speeds.");
            }
            catch (Exception ex)
            {
                LogUtils.Error(() => $"{Mod.ModTag} Failed to request clear speeds: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private static void DebugReportToLogAction()
        {
            try
            {
                World? world = World.DefaultGameObjectInjectionWorld;
                if (world == null)
                {
                    LogUtils.Error(() => $"{Mod.ModTag} Cannot write debug report: World is null.");
                    return;
                }

                SegmentSpeedToolUISystem? uiSystem =
                    world.GetExistingSystemManaged<SegmentSpeedToolUISystem>();

                if (uiSystem == null)
                {
                    LogUtils.Error(() => $"{Mod.ModTag} Cannot write debug report: SegmentSpeedToolUISystem not found.");
                    return;
                }

                uiSystem.LogDebugReportToLog();
            }
            catch (Exception ex)
            {
                LogUtils.Error(() => $"{Mod.ModTag} Failed to write debug report: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private static void TryOpenUrl(string url)
        {
            try
            {
                UnityEngine.Application.OpenURL(url);
            }
            catch (Exception ex)
            {
                LogUtils.Warn(() => $"{Mod.ModTag} Failed to open URL: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        private bool HideUsageText()
        {
            return !ShowUsage;
        }

        public enum SpeedUnit
        {
            Auto,
            Metric,
            Imperial
        }
    }
}
