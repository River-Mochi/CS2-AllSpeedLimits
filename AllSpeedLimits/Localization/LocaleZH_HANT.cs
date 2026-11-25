// <copyright file="LocaleZH_HANT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleZH_HANT.cs
// Purpose: Traditional Chinese locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "自行車" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "關於" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "顯示選項" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "快捷鍵" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "自行車和電動滑板車" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "個人車輛狀態" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "還原遊戲預設值" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "使用方法" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "除錯 / 日誌" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "隱藏路網快捷鍵" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "點擊「未設定」並指定按鍵。關閉 ASL 面板後仍可使用。" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "速度單位" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "選擇面板和浮動標誌的單位。\n" +
                    "<AUTO> 依地圖類型：\n" +
                    "- EU = KM/H，NA = MPH。\n" +
                    "選擇 <KM/H 或 MPH> 可強制使用該顯示。"
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "滑桿跟隨所選區段" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] 建議開啟**\n" +
                    "開啟：點擊區段時，滑桿會移到該區段的目前速度，而不是從 5 開始。\n" +
                    "- 選取多個區段時，仍由第一個區段決定滑桿起點。\n" +
                    "關閉：點擊其他區段時，保留上一次滑桿目標值。"
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "面板滑桿步長" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "設定城市速限面板中的滑桿步長。\n" +
                    "<預設 = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "提示文字大小" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "本模組可放大滑鼠停留在速限功能上時顯示的提示框文字。\n" +
                    "<預設 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "顯示 ASL 提示" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "顯示城市內的 ASL 提示。資訊按鈕可重新開啟。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "顯示遊戲雙倍速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<關> 顯示較簡單的刻度，通常更接近道路標線。\n" +
                    "<開> 面板和浮動文字會顯示遊戲較高的內部速度刻度。\n" +
                    "如果其他提示模組或 Scene Explorer 顯示遊戲內部的雙倍值，而你想讓兩者一致，此選項會很有用。\n" +
                    "**這只改變視覺顯示；**儲存的速度<不會真的改變>。\n" +
                    "如果覺得混亂，保持關閉即可。無論開啟還是關閉，車輛的實際移動都一樣。\n" +
                    "注意：<道路標線只是美術表現>，可能與遊戲預製件中的真實速度資料不同。標示為 35 mph 的道路實際可能為 ~31 mph。遊戲先用公制定義道路速度，再進行轉換。"
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "車輛最高速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "100% = 遊戲預設。自動調整加速和煞車。仍受道路及路徑速限影響。如使用 Bikes + Paths，請更新。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "重設車速" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "將自行車和電動滑板車車速恢復為 100%。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "自行車群組" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "自行車與電動滑板車。\n" +
                    "**活動** = 目前有車道（正在移動）。\n" +
                    "**總停放** = 包含所有位置的 Parked 標記，不只是停車場。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "汽車群組" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "僅個人汽車（不包含自行車群組）。\n" +
                    "**活動** = 目前有車道（正在移動）。\n" +
                    "**停放** = 所有 **ParkedCar**，不只是停車場。\n" +
                    "掃描只在選項選單開啟時執行，不在城市正常遊玩時執行，因此不用擔心 FPS。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "隱藏停放汽車" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**OC 邊界總數** = 在 Outside City (OC) 連接處帶 ParkedCar 的汽車群組車輛。\n" +
                    "有些城市會在 Outside City 連接處出現大量卡住的停放汽車。\n" +
                    "使用 <Log hidden cars> 查看隱藏汽車的範例分類。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "記錄隱藏汽車" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "向 **Logs/AllSpeedLimits.log** 寫入一次性報告。\n" +
                    "包含總數 + Bucket A/B/C 分類與範例 ID。\n" +
                    "使用 Scene Explorer 模組跳到列出的 Vehicle 實體 ID 並檢查。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "開啟日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "如果存在，開啟 **Logs/AllSpeedLimits.log**。\n" +
                    "如果尚未找到檔案，則改為開啟 Logs 資料夾。"
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "狀態未載入。" },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "沒有城市... ¯\\_(ツ)_/¯ ...沒有統計" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "執行城市幾分鐘以取得資料。" },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} 活動 | {1} 自行車 | {2} 滑板車 | {3} / {4} 停放/總數" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} 活動 | {1} 停放 | {2} 總數 | {3} 拖車" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} 在 OC 邊界隱藏 | 更新 {1}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "還原遊戲預設速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "移除模組前的可選清理。\n" +
                    "<只在>不想保留本模組的自訂速度時使用。\n" +
                    "移除模組不一定需要它。沒有本模組，自訂速度也可以留在城市裡。\n" +
                    "<============>\n" +
                    "\n" +
                    "此按鈕會在本模組套用過自訂速度的地方還原已知的遊戲預設值。\n" +
                    "完成後，在移除模組前請做一個**新存檔**。\n" +
                    "如果不使用它就移除模組，自訂速度會保留，直到你修改道路、軌道或水路。"
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "這會把本模組套用的所有自訂速限還原為已知的遊戲預設值。\n" +
                    "此操作無法自動復原。\n" +
                    "完成後，在移除模組前請將城市儲存為新存檔。"
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "顯示說明" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "在下方顯示簡短說明。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<城市面板>\n" +
                    "▪ 點擊或拖曳選取路段。設定<新速度>，再點擊<套用>。\n" +
                    "▪ <預設按鈕>（圓圈數字）立即生效，無需點擊套用。\n" +
                    "▪ <重設>恢復選取路段。\n" +
                    "▪ <隱藏路網>：虛線按鈕顯示/隱藏道路及路徑。\n" +
                    "<全城>\n" +
                    "▪ 選擇道路組並套用新速度。\n" +
                    "▪ <紅色重設按鈕>：道路、軌道、水路、路徑或全部，清除相應自訂速限。\n" +
                    "▪ <路徑分頁>：自行車道及步行路徑滑桿全城生效。100% = 遊戲預設；200% 速限加倍。\n" +
                    "▪ <自行車道路段個別修改>優先，保留自己的速限。純步行路徑僅支援全城調整。\n" +
                    "▪ <儲存城市>，重新載入時保留修改。"
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
