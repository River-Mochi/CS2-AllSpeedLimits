// <copyright file="LocaleEN.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleEN.cs
// Purpose: English locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;
    using Colossal.PSI.Common;
    using Game.Areas;
    using Game.Citizens;
    using Game.City;
    using Game.Objects;
    using Game.UI;

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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Display Options" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Reset Game Defaults" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Usage" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Speed Units" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Choose panel and floating sign units.\n" +
                    "<AUTO> follows the map type:" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "<KM/H or MPH> force that specific display." },
             
                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Sync Slider With Selected Segment" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Enabled is recommended**\n" +
                    "Enabled: clicking a segment moves the slider to the segment's current speed instead of starting everything at 5.\n" +
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
                    "1. Click or drag-select segments.\n" +
                    "2. Set <New speed>, then click <Apply>.\n" +
                    "3. <Reset> restores selected segments.\n" +
                    "4. <Preset> buttons apply instantly.\n\n" +
                    "<Whole City>\n" +
                    "Choose a road group, then apply <New speed> to that group.\n" +
                    "Use <Roads>, <Rails>, <Water>, or <All> to clear custom speeds.\n" +
                    "<Save city> after whole city changes."
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
