// <copyright file="LocaleJA.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleJA.cs
// Purpose: Japanese locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;
    using Game.Areas;
    using Game.Citizens;
    using Game.City;
    using Game.Objects;
    using Game.UI;

    public sealed class LocaleJA : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleJA(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {

            // Options menu title keeps English first for stable sorting.
            string title = $"{Mod.ModName} (全速度制限)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "表示オプション" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "ゲーム既定に戻す" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "使い方" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "デバッグ / ログ" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "速度単位" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "パネルと浮き出し標識の単位を選びます。\n" +
                    "<AUTO> はマップ種別に従います: EU = KM/H、NA = MPH。\n" +
                    "<KM/H> と <MPH> は表示を固定します。" },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "選択セグメントにスライダーを同期" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<有効を推奨>\n" +
                    "有効: セグメントをクリックすると、最初の選択セグメントの現在速度にスライダーを合わせます。\n" +
                    "無効: 別のセグメントをクリックしても、最後の目標値を保ちます。\n" +
                    "複数選択時も、最初のセグメントがスライダーの開始位置になります。"
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "パネルのスライダー刻み" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "都市パネルのスライダー刻み幅を設定します。\n" +
                    "<既定 = 5>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "ヘルプ文字サイズ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Mod のポップアップとヘルプ文字を大きくします。\n" +
                    "<既定 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "ゲーム内の倍速表示を使う" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Off> はよりシンプルな目盛りで、道路表示に近いことが多いです。\\n" +
                    "<On> はパネルと浮動テキストにゲーム内部の高い速度目盛りを表示します。\\n" +
                    "別のツールチップModが内部の2倍値を表示する場合、それに合わせたい時に便利です。\\n" +
                    "これは表示だけです。保存される速度は<実際には変わりません>。\\n" +
                    "道路の数字は見た目用で、Prefabの速度データと完全には一致しない場合があります。\\n" +
                    "迷う場合は Off のままで大丈夫です。On/Off で車の動きの見た目は同じです。" },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "全カスタム速度を消去" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "この都市の対応する道路、線路、水路を<ゲーム既定値>に戻します。\\n" +
                    "<リセットを残すには、その後セーブしてください。>\\n" +
                    "- カスタム速度を残したくない場合、Mod削除前に便利です。\\n" +
                    "- 消去せずにModを削除すると、保存済み速度は通常残りますが、リセット/再適用サポートはなくなります。" },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "この都市の対応する道路、線路、水路セグメントから全カスタム速度を消去しますか？\n" +
                    "元に戻せません。"
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "説明を表示" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "下に短い使い方メモを表示します。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<都市パネル>\n" +
                    "1. セグメントをクリック、またはドラッグ選択します。\n" +
                    "2. <新しい速度> を設定して <適用> をクリックします。\n" +
                    "3. <リセット> は選択セグメントを戻します。\n" +
                    "4. <50%> ボタンはすぐ適用されます。\n" +
                    "\n" +
                    "<都市全体>\n" +
                    "道路グループを選び、そのグループに <新しい速度> を適用します。\n" +
                    "<道路>、<線路>、<水路>、<すべて> でカスタム速度を消去します。\n" +
                    "都市全体の変更後は <都市を保存> してください。"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "作者の Paradox Mods ページを開きます。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "デバッグレポートをログへ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<通常プレイでは不要です。>\\n" +
                    "Logs/AllSpeedLimits.log に一度だけレポートを書き込みます。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "ログを開く" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)), "<Logs/AllSpeedLimits.log> を開きます。ファイルがない場合は Logs フォルダーを開きます。" },
            };
        }

        public void Unload()
        {
        }
    }
}
