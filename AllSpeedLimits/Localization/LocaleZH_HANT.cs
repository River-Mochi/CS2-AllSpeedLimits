// <copyright file="LocaleZH_HANT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleZH_HANT.cs
// Purpose: Traditional Chinese locale Options UI settings.

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

    public sealed class LocaleZH_HANT : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleZH_HANT(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            string title = $"{Mod.ModName} (所有速限)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "關於" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "顯示選項" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "還原遊戲預設值" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "使用方法" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "除錯 / 日誌" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "速度單位" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "選擇面板和浮動標誌的單位。\n" +
                    "<AUTO> 依地圖類型：EU = KM/H，NA = MPH。\n" +
                    "<KM/H> 和 <MPH> 會強制使用該顯示。" },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "滑桿跟隨所選區段" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "<建議開啟>\n" +
                    "開啟：點擊區段時，滑桿會移到第一個所選區段的目前速度。\n" +
                    "關閉：點擊其他區段時，保留上一次滑桿目標值。\n" +
                    "選取多個部分時，仍由第一個區段決定滑桿起點。"
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "面板滑桿步長" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "設定城市面板中的滑桿步長。\n" +
                    "<預設 = 10>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "提示文字大小" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "本模組可放大滑鼠停留在模組項目上時顯示的提示框文字。\n" +
                    "<預設 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "顯示遊戲雙倍速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<關> 顯示較簡單的刻度，通常更接近道路標線。\n" +
                    "<開> 面板和浮動文字會顯示遊戲較高的內部速度刻度。\n" +
                    "如果其他提示模組顯示遊戲內部的雙倍值，而你想讓兩者一致，此選項會很有用。\n" +
                    "**這只改變視覺顯示；**儲存的速度<不會真的改變>。\n" +
                    "如果覺得混亂，保持關閉即可。無論開啟還是關閉，車輛的實際移動都一樣。\n" +
                    "注意：道路標線和標誌只是美術表現，可能與遊戲範本中的實際速度資料不同。標示為 35 mph 的道路，實際值可能是 31 mph。遊戲會先按公制計算道路速度，再轉換單位。"
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "還原遊戲預設速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "移除模組前的可選清理。\n" +
                    "<只在>不想保留本模組的自訂速度時使用。\n" +
                    "移除模組不一定需要它。沒有本模組，自訂道路速度也可以留在城市裡。\n" +
                    "<============>\n" +
                    "\n" +
                    "這會把本模組套用的自訂速度還原為已知的遊戲預設值。\n" +
                    "完成後，在移除模組前請做一個**新存檔**。\n" +
                    "如果不使用它就移除模組，自訂速度會保留，直到你修改道路等。"
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "這會把所有支援的自訂速限還原為已知的遊戲預設值。\n" +
                    "此操作無法自動復原。\n" +
                    "完成後，在移除模組前請將城市儲存為新存檔。"
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "顯示說明" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "在下方顯示簡短說明。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<城市面板>\n" +
                    "1. 點擊或拖曳選取區段。\n" +
                    "2. 設定 <新速度>，然後點擊 <套用>。\n" +
                    "3. <重置> 會還原所選區段。\n" +
                    "4. 預設按鈕會立即生效。\n" +
                    "\n" +
                    "<全城>\n" +
                    "選擇一個道路群組，然後把 <新速度> 套用到該群組。\n" +
                    "用 <道路>、<軌道>、<水路> 或 <全部> 清除自訂速度。\n" +
                    "全城修改後請 <儲存城市>。"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "模組" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "開啟作者的 Paradox Mods 頁面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "除錯報告寫入日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<正常遊戲不需要。>\n" +
                    "向 Logs/AllSpeedLimits.log 寫入一次性報告。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "開啟日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "開啟 <Logs/AllSpeedLimits.log>。如果檔案不存在，則開啟 Logs 資料夾。" },
            };
        }

        public void Unload()
        {
        }
    }
}
