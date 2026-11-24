// <copyright file="LocaleZH_HANS.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleZH_HANS.cs
// Purpose: Simplified Chinese locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;


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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "自行车" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "关于" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "显示选项" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "快捷键" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "自行车和电动滑板车" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "个人车辆状态" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "恢复游戏默认值" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "使用方法" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "调试 / 日志" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "隐藏路网快捷键" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "点击“未设置”并指定按键。关闭 ASL 面板后仍可使用。" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "速度单位" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "选择面板和浮动标志的单位。\n" +
                    "<AUTO> 按地图类型：\n" +
                    "- EU = KM/H，NA = MPH。\n" +
                    "选择 <KM/H 或 MPH> 可强制使用该显示。"
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "滑块跟随所选区段" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] 建议开启**\n" +
                    "开启：点击区段时，滑块会跳到该区段的当前速度，而不是从 5 开始。\n" +
                    "- 选择多个区段时，仍由第一个区段决定滑块起点。\n" +
                    "关闭：点击其他区段时，保留上一次滑块目标值。"
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "面板滑块步长" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "设置城市限速面板里的滑块步长。\n" +
                    "<默认 = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "提示文字大小" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "本模组可放大鼠标悬停在限速功能上时显示的提示框文字。\n" +
                    "<默认 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "显示游戏双倍速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<关> 显示更简单的刻度，通常更接近道路标线。\n" +
                    "<开> 面板和悬浮文字会显示游戏较高的内部速度刻度。\n" +
                    "如果其他提示模组或 Scene Explorer 显示游戏内部的双倍值，而你想让两者一致，此选项会很有用。\n" +
                    "**这只改变视觉显示；**保存的速度<不会真的改变>。\n" +
                    "如果觉得混乱，保持关闭即可。无论开启还是关闭，车辆的实际移动都一样。\n" +
                    "注意：<道路标线只是美术表现>，可能与游戏预制件中的真实速度数据不同。标为 35 mph 的道路实际可能为 ~31 mph。游戏先用公制定义道路速度，再进行转换。"
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "车辆最高速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "100% = 游戏默认。自动调整加速和制动。仍受道路和路径限速影响。如使用 Bikes + Paths，请更新。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "重置车速" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "将自行车和电动滑板车车速恢复为 100%。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "自行车组" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "自行车和电动滑板车。\n" +
                    "**活动** = 当前有车道（正在移动）。\n" +
                    "**总停放** = 包含所有位置的 Parked 标记，不只是停车场。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "汽车组" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "仅个人汽车（不包括自行车组）。\n" +
                    "**活动** = 当前有车道（正在移动）。\n" +
                    "**停放** = 所有 **ParkedCar**，不只是停车场。\n" +
                    "扫描只在选项菜单打开时运行，不在城市正常游玩时运行，因此不用担心 FPS。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "隐藏停放汽车" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**OC 边界总数** = 在 Outside City (OC) 连接处带 ParkedCar 的汽车组车辆。\n" +
                    "有些城市会在 Outside City 连接处显示大量卡住的停放汽车。\n" +
                    "使用 <Log hidden cars> 查看隐藏汽车的示例分类。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "记录隐藏汽车" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "向 **Logs/AllSpeedLimits.log** 写入一次性报告。\n" +
                    "包含总数 + Bucket A/B/C 分类和示例 ID。\n" +
                    "使用 Scene Explorer 模组跳转到列出的 Vehicle 实体 ID 并检查。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "打开日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "如果存在，则打开 **Logs/AllSpeedLimits.log**。\n" +
                    "如果尚未找到文件，则改为打开 Logs 文件夹。"
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "状态未加载。" },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "没有城市... ¯\\_(ツ)_/¯ ...没有统计" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "运行城市几分钟以获取数据。" },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} 活动 | {1} 自行车 | {2} 滑板车 | {3} / {4} 停放/总数" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} 活动 | {1} 停放 | {2} 总数 | {3} 拖车" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} 在 OC 边界隐藏 | 更新 {1}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "恢复游戏默认速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "卸载模组前的可选清理。\n" +
                    "<只在>不想保留本模组的自定义速度时使用。\n" +
                    "卸载模组不一定需要它。没有本模组，自定义速度也可以留在城市里。\n" +
                    "<============>\n" +
                    "\n" +
                    "此按钮会在本模组应用过自定义速度的地方恢复已知的游戏默认值。\n" +
                    "完成后，在卸载模组前请做一个**新存档**。\n" +
                    "如果不使用它就卸载模组，自定义速度会保留，直到你修改道路、轨道或水路。"
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "这会把本模组应用的所有自定义限速恢复为已知的游戏默认值。\n" +
                    "此操作无法自动撤销。\n" +
                    "完成后，在卸载模组前请将城市保存为新存档。"
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "显示说明" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "在下方显示简短说明。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "点击或拖选路段。设置速度后应用；预设按钮立即生效。\n" +
                    "虚线按钮显示隐藏路网。\n" +
                    "路径：200% 使默认限速翻倍，单独修改的路段优先。\n" +
                    "修改后保存城市。"
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
