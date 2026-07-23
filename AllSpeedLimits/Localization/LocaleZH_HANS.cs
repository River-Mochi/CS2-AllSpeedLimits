// <copyright file="LocaleZH_HANS.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleZH_HANS.cs
// Purpose: Simplified Chinese locale Options UI settings.

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

    public sealed class LocaleZH_HANS : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleZH_HANS(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            string title = $"{Mod.ModName} (所有限速)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "关于" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "显示选项" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "恢复游戏默认值" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "使用方法" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "调试 / 日志" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "速度单位" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "选择面板和浮动标志的单位。\n" +
                    "<AUTO> 按地图类型：EU = KM/H，NA = MPH。\n" +
                    "<KM/H> 和 <MPH> 强制使用该显示。" },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "滑块跟随所选区段" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "<建议开启>\n" +
                    "开启：点击区段时，滑块会跳到第一个所选区段的当前速度。\n" +
                    "关闭：点击其他区段时，保留上一次滑块目标值。\n" +
                    "选择多个部分时，仍由第一个区段决定滑块起点。"
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "面板滑块步长" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "设置城市面板里的滑块步长。\n" +
                    "<默认 = 10>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "提示文字大小" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "本模组可放大鼠标悬停在模组项目上时显示的提示框文字。\n" +
                    "<默认 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "显示游戏双倍速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<关> 显示更简单的刻度，通常更接近道路标线。\n" +
                    "<开> 面板和悬浮文字会显示游戏较高的内部速度刻度。\n" +
                    "如果其他提示模组显示游戏内部的双倍值，而你想让两者一致，此选项会很有用。\n" +
                    "**这只改变视觉显示；**保存的速度<不会真的改变>。\n" +
                    "如果觉得混乱，保持关闭即可。无论开启还是关闭，车辆的实际移动都一样。\n" +
                    "注意：道路标线和标志只是美术表现，可能与游戏模板中的实际速度数据不同。标为 35 mph 的道路，实际值可能是 31 mph。游戏会先按公制计算道路速度，再转换单位。"
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "恢复游戏默认速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "卸载模组前的可选清理。\n" +
                    "<只在>不想保留本模组的自定义速度时使用。\n" +
                    "卸载模组不一定需要它。没有本模组，自定义道路速度也可以留在城市里。\n" +
                    "<============>\n" +
                    "\n" +
                    "这会把本模组应用的自定义速度恢复为已知的游戏默认值。\n" +
                    "完成后，在卸载模组前请做一个**新存档**。\n" +
                    "如果不使用它就卸载模组，自定义速度会保留，直到你修改道路等。"
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "这会把所有支持的自定义限速恢复为已知的游戏默认值。\n" +
                    "此操作无法自动撤销。\n" +
                    "完成后，在卸载模组前请将城市保存为新存档。"
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "显示说明" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "在下方显示简短说明。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<城市面板>\n" +
                    "1. 点击或拖动选择区段。\n" +
                    "2. 设置 <新速度>，然后点击 <应用>。\n" +
                    "3. <重置> 会恢复所选区段。\n" +
                    "4. 预设按钮会立即生效。\n" +
                    "\n" +
                    "<全城>\n" +
                    "选择一个道路组，然后把 <新速度> 应用到该组。\n" +
                    "用 <道路>、<轨道>、<水路> 或 <全部> 清除自定义速度。\n" +
                    "全城修改后请 <保存城市>。"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "模组" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "打开作者的 Paradox Mods 页面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "调试报告写入日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<正常游戏不需要。>\n" +
                    "向 Logs/AllSpeedLimits.log 写入一次性报告。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "打开日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "打开 <Logs/AllSpeedLimits.log>。如果文件不存在，则打开 Logs 文件夹。" },
            };
        }

        public void Unload()
        {
        }
    }
}
