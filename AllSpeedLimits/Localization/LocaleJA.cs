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
    using Colossal.PSI.Common;
    using Game.Areas;
    using Game.Citizens;
    using Game.City;
    using Game.Objects;
    using Game.UI;

    public sealed class LocaleJA : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleJA(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            // Version still appears on the About tab through VersionText.
            string title = $"{Mod.ModName} (全速度制限)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "表示オプション" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "ゲーム既定に戻す" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "使い方" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "デバッグ / ログ" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "速度単位" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "パネルと浮動標識の単位を選びます。\n" +
                    "<AUTO> はマップ種別に従います:\n" +
                    "- EU = KM/H、NA = MPH。\n" +
                    "<KM/H または MPH> を選ぶと表示を固定します。"
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "選択セグメントにスライダーを同期" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] 有効を推奨**\n" +
                    "有効: セグメントをクリックすると、5 からではなくその現在速度にスライダーを合わせます。\n" +
                    "- 複数選択時も、最初のセグメントがスライダーの開始位置になります。\n" +
                    "無効: 別のセグメントをクリックしても、最後の目標値を保ちます。"
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "パネルのスライダー刻み" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "都市の速度制限パネルで、スライダーの刻み幅を設定します。\n" +
                    "<既定 = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "ヘルプ文字サイズ" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "速度制限機能にカーソルを合わせた時のヘルプ文字を大きくできます。\n" +
                    "<既定 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "ゲーム内の倍速表示を使う" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<オフ> はよりシンプルな目盛りで、道路表示に近いことが多いです。\n" +
                    "<オン> はパネルと浮動テキストにゲーム内部の高い速度目盛りを表示します。\n" +
                    "別のツールチップ Mod や Scene Explorer がゲーム内部の2倍値を表示していて、それに合わせたい場合に便利です。\n" +
                    "**変わるのは表示だけです。** 保存される速度は<実際には変わりません>。\n" +
                    "分かりにくい場合はオフのままで大丈夫です。オン/オフで車の動きは変わりません。\n" +
                    "注: <道路の表示は見た目用>で、ゲームの実際の速度データと一致しないことがあります。35 mph の標識が実際には ~31 mph の場合もあります。ゲームは道路速度を先にメートル法で定義し、その後に換算します。"
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "ゲーム既定の速度に戻す" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Mod を削除する前の任意のクリーンアップです。\n" +
                    "この Mod のカスタム速度を<残したくない場合だけ>使ってください。\n" +
                    "Mod の削除に必須ではありません。カスタム速度は、この Mod がなくても都市に残せます。\n" +
                    "<============>\n" +
                    "\n" +
                    "このボタンは、この Mod がカスタム速度を適用した場所を、分かっているゲーム既定値に戻します。\n" +
                    "完了後、Mod を削除する前に **新規セーブ** を作ってください。\n" +
                    "これを使わずに Mod を削除すると、道路、鉄道、水路を変更するまでカスタム速度は残ります。"
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "この Mod が適用したすべてのカスタム速度制限を、分かっているゲーム既定値に戻します。\n" +
                    "自動では元に戻せません。\n" +
                    "完了後、Mod を削除する前に都市を新しいセーブデータとして保存してください。"
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "説明を表示" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "下に短い使い方メモを表示します。" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<都市パネル>\n" +
                    "1. セグメントをクリック、またはドラッグ選択します。\n" +
                    "2. <新速度> を設定して <適用> をクリックします。\n" +
                    "3. <リセット> は選択セグメントを戻します。\n" +
                    "4. <プリセット> ボタンはすぐ適用されます。\n" +
                    "\n" +
                    "<都市全体>\n" +
                    "道路グループを選び、そのグループに <新速度> を適用します。\n" +
                    "<道路>、<鉄道>、<水路>、<全て> でカスタム速度を消去します。\n" +
                    "都市全体の変更後は <都市を保存> してください。"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "作者の Paradox Mods ページを開きます。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "デバッグレポートをログへ" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<通常プレイでは不要です。>\n" +
                    "Logs/AllSpeedLimits.log に一度だけレポートを書き込みます。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "ログを開く" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "<Logs/AllSpeedLimits.log> を開きます。ファイルがない場合は Logs フォルダーを開きます。" },
            };
        }

        public void Unload()
        {
        }
    }
}
