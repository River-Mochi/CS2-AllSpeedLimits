// <copyright file="LocaleEN.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleEN.cs
// Purpose: English locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleEN : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleEN(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title intentionally omits version.
            // Version still appears on the About tab through VersionText.
            string title = Mod.ModName;

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "Bicycles" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Display Options" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "Keybinds" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "Bicycles + electric scooters" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "Status personal vehicles" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Reset Game Defaults" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Usage" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Log" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Invisible Networks Hotkey" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Click 'not set', then press a shortcut. Works anywhere in the city, even with ASL closed. Leave unassigned to disable." },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Speed Units" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Choose panel and floating sign units.\n" +
                    "<AUTO> follows the map type:\n" +
                    "- EU = KM/H\n" +
                    "- NA = MPH\n" +
                    "Selecting <KM/H or MPH> forces that specific display."
                },
             
                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Sync Slider With Selected Segment" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Enabled is recommended**\n" +
                    "Enabled: clicking a segment moves the slider to the segment's current speed instead of starting the slider at 5.\n" +
                    "- If you select multiple parts, the first segment still sets the start position on the slider.\n"+
                    "Disabled: clicking another segment keeps your last slider target number."          
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Panel Slider Increment" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "This sets the step size for the slider bar in the city speed limit panel.\n" +
                    "<Default = 10>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Tooltip Text Size" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "This mod allows bigger text in the hint boxes while hovering speed limit features.\n" +
                    "<Default 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "Show ASL Tooltips" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)),
                    "**Default = ON**  Highly recommended to keep this ON.\n\n" +
                    "Show in-city ASL tooltips. The info button can turn them back on." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Show Game Doubled Speeds" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Off> shows a simpler scale, usually closer to road decals.\n" +
                    "<On> the panel + floating text show the game's higher internal speed scale.\n" +
                    "Useful if another tooltip mod or Scene Explorer shows the game internal doubled values and you want to match.\n" +
                    "**This is visual Display only;** saved speeds <do not really change>.\n" +
                    "If this is confusing, just keep it Off. Cars will look the same moving if this is on or off.\n" +
                    "Note: <road decals are Art> and may not match the game's real prefab speed data. 35 mph sign may be ~31 mph. Game does roads in metric first, then converts."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "Maximum vehicle speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "100% = game default. Acceleration and braking adjust automatically. Path limits still apply. Update Bikes + Paths if installed." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Reset bicycle speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Restore bicycle and electric scooter vehicle speed to 100%." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "Bike group" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "Bikes and electric scooters.\n" +
                    "**Active** = has a current lane (moving).\n" +
                    "**Total Parked** = includes all Parked flags from anywhere, not just parking lots."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "Car group" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "Personal cars only (excludes the Bike group).\n" +
                    "**Active** = has a current lane (moving).\n" +
                    "**Parked** = all **ParkedCar**, not just parking lots.\n " +
                    "Scan runs only while Options menu is open, not in-city gameplay, so no FPS concern."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "Hidden parked cars" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**Total at OC border** = car-group vehicles with ParkedCar at Outside City (OC) connection.\n" +
                    "Some cities show a large number of cars stuck parked at Outside City connection.\n" +
                    "Use <Log hidden cars> for sample breakdown of hidden cars.\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "Log hidden cars" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "Writes a one-time report to **Logs/AllSpeedLimits.log**.\n" +
                    "Includes Total + Bucket A/B/C breakdown and sample ID numbers.\n" +
                    "Use Scene Explorer mod to Jump To the listed Vehicle entity IDs and investigate."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "Open Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "Opens **Logs/AllSpeedLimits.log** if it exists.\n" +
                    "If the file is not found yet, opens the Logs folder instead."
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "Status not loaded." },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "No city... ¯\\_(ツ)_/¯ ...No stats" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "run the city a few minutes for data." },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} active | {1} bikes | {2} scooters | {3} / {4} parked/total" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} active | {1} parked | {2} total | {3} trailers" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} hidden at OC border | updated {1}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "Restore Game Default Speeds" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Optional cleanup before removing the mod.\n" +
                    "Use this <only> if you do not want to keep this mod's custom speeds.\n" +
                    "This is not required to remove the mod. Custom road speeds can remain in the city without this mod.\n" +
                    "<============>\n\n" +
                    "This button restores any known game defaults over areas with custom speeds applied by this mod.\n" +
                    "After it finishes, do a **NEW SAVE** before removing the mod.\n" +
                    "If you remove the mod without using this, custom speeds remain until you change the roads, rails, water."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "This restores all known game defaults to any custom speed limits applied by this mod.\n" +
                    "This cannot be undone automatically.\n" +
                    "After it finishes, save the city as a NEW save before removing the mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Show Instructions" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Show short how-to notes below." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<City Panel>\n" +
                    "▪ Click one or drag-select many segments. Set <New speed>, then click <Apply>.\n" +
                    "▪ <Preset buttons> (circled numbers) apply instantly; no Apply needed.\n" +
                    "▪ <Reset> restores selected segments.\n" +
                    "▪ <Invisible>: dashed button shows/hides hidden roads and paths.\n" +
                    "<Whole City>\n" +
                    "▪ Choose a road group, then apply New speed to that group.\n" +
                    "▪ <Red reset buttons>: Roads, Rails, Water, Paths or All clear related custom speeds.\n" +
                    "▪ <Paths Tab>: Bike and Pedestrian sliders apply citywide. 100% = game default; 200% doubles the limit.\n" +
                    "▪ <Custom Bike edits>: individual bike segments keep their own speed, overriding the global slider. Pedestrian-only paths are citywide only.\n" +
                    "▪ <Save city> after changes to keep them when reloading."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Open the author's Paradox Mods page." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "Debug Report to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<Not needed for normal gameplay.>\n" +
                    "Writes a one-time report to Logs/AllSpeedLimits.log."
                },
 
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "Open Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "Open <Logs/AllSpeedLimits.log>. Falls back to the Logs folder if the file does not exist." },
            };
        }

        public void Unload()
        {
        }
    }
}
