// <copyright file="LocaleKO.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleKO.cs
// Purpose: Korean locale Options UI settings.

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

    public sealed class LocaleKO : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleKO(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            // Version still appears on the About tab through VersionText.
            string title = $"{Mod.ModName} (모든 속도 제한)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "동작" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "정보" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "표시 옵션" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "게임 기본값 복원" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "사용법" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "디버그 / 로그" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "속도 단위" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "패널과 떠 있는 표지의 단위를 선택합니다.\n" +
                    "<AUTO>는 맵 종류를 따릅니다: EU = KM/H, NA = MPH.\n" +
                    "<KM/H>와 <MPH>는 표시를 고정합니다." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "선택 구간과 슬라이더 동기화" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<켜짐 권장>\n" +
                    "켜짐: 구간을 클릭하면 첫 번째 선택 구간의 현재 속도로 슬라이더가 이동합니다.\n" +
                    "꺼짐: 다른 구간을 클릭해도 마지막 목표값을 유지합니다.\n" +
                    "여러 구간을 선택해도 첫 번째 구간이 슬라이더 시작 위치를 정합니다."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "패널 슬라이더 간격" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "도시 패널 슬라이더의 단계 크기를 정합니다.\n" +
                    "<기본값 = 5>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "도움말 글자 크기" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "모드 항목에 마우스를 올릴 때 나오는 도움말 글자를 더 크게 할 수 있습니다.\n" +
                    "<기본값 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "게임의 2배 속도 표시" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<끄기>는 더 단순한 눈금을 보여 주며 보통 도로 표시와 더 가깝습니다.\n" +
                    "<켜기>는 패널과 떠 있는 텍스트에 게임 내부의 더 높은 속도 눈금을 표시합니다.\n" +
                    "다른 툴팁 모드가 게임 내부의 두 배 값을 보여 주고 있어 그 표시와 맞추고 싶다면 유용합니다.\n" +
                    "**표시만 바뀝니다.** 저장된 속도는 <실제로 바뀌지 않습니다>.\n" +
                    "헷갈리면 끄기로 두세요. 켜도 꺼도 차량의 실제 움직임은 똑같습니다.\n" +
                    "참고: 도로의 숫자와 표지는 장식이므로 게임의 실제 속도 데이터와 다를 수 있습니다. 35 mph 표지가 실제로는 31 mph일 수 있습니다. 게임은 도로 속도를 먼저 미터법으로 계산한 뒤 변환합니다."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "게임 기본 속도 복원" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "모드를 제거하기 전 선택적으로 정리하는 기능입니다.\n" +
                    "이 모드의 사용자 지정 속도를<유지하고 싶지 않을 때만> 사용하세요.\n" +
                    "모드를 제거하는 데 필수는 아닙니다. 사용자 지정 도로 속도는 이 모드 없이도 도시에 남을 수 있습니다.\n" +
                    "<============>\n" +
                    "\n" +
                    "이 모드가 적용한 사용자 지정 속도를 알려진 게임 기본값으로 되돌립니다.\n" +
                    "완료 후 모드를 제거하기 전에 **새 저장**을 만드세요.\n" +
                    "이 기능을 쓰지 않고 모드를 제거하면 도로를 바꾸기 전까지 사용자 지정 속도는 남습니다."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "지원되는 모든 사용자 지정 속도 제한을 알려진 게임 기본값으로 되돌립니다.\n" +
                    "자동으로 되돌릴 수 없습니다.\n" +
                    "완료 후 모드를 제거하기 전에 도시를 새 저장으로 저장하세요."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "설명 표시" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "아래에 짧은 사용법을 표시합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<도시 패널>\n" +
                    "1. 구간을 클릭하거나 드래그 선택합니다.\n" +
                    "2. <새 속도>를 정한 뒤 <적용>을 누릅니다.\n" +
                    "3. <초기화>는 선택 구간을 복원합니다.\n" +
                    "4. 프리셋 버튼은 즉시 적용됩니다.\n" +
                    "\n" +
                    "<도시 전체>\n" +
                    "도로 그룹을 선택한 뒤 그 그룹에 <새 속도>를 적용합니다.\n" +
                    "<도로>, <철도>, <수로>, <전체>로 사용자 속도를 지웁니다.\n" +
                    "도시 전체 변경 후에는 <도시 저장>을 하세요."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "모드" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "제작자의 Paradox Mods 페이지를 엽니다." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "디버그 보고서를 로그에 기록" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<일반 플레이에는 필요 없습니다.>\n" +
                    "Logs/AllSpeedLimits.log에 한 번 보고서를 씁니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "로그 열기" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "<Logs/AllSpeedLimits.log>를 엽니다. 파일이 없으면 Logs 폴더를 엽니다." },
            };
        }

        public void Unload()
        {
        }
    }
}
