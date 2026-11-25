// <copyright file="LocaleKO.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleKO.cs
// Purpose: Korean locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleKO : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleKO(SpeedLimitsSetting setting)
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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "동작" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "자전거" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "정보" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "표시 옵션" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "단축키" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "자전거 및 전동 킥보드" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "개인 차량 상태" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "게임 기본값 복원" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "사용법" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "디버그 / 로그" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "숨은 네트워크 단축키" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "'설정 안 됨'을 클릭해 키를 지정하세요. ASL을 닫아도 사용할 수 있습니다." },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "속도 단위" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "패널과 떠 있는 표지의 단위를 선택합니다.\n" +
                    "<AUTO>는 맵 종류를 따릅니다:\n" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "<KM/H 또는 MPH>를 선택하면 해당 표시로 고정됩니다."
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "선택 구간과 슬라이더 동기화" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] 켜짐 권장**\n" +
                    "켜짐: 구간을 클릭하면 5에서 시작하지 않고 해당 구간의 현재 속도로 슬라이더가 이동합니다.\n" +
                    "- 여러 구간을 선택해도 첫 번째 구간이 슬라이더 시작 위치를 정합니다.\n" +
                    "꺼짐: 다른 구간을 클릭해도 마지막 목표값을 유지합니다."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "패널 슬라이더 간격" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "도시 속도 제한 패널에서 슬라이더의 단계 크기를 정합니다.\n" +
                    "<기본값 = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "도움말 글자 크기" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "속도 제한 기능에 마우스를 올릴 때 나오는 도움말 글자를 더 크게 할 수 있습니다.\n" +
                    "<기본값 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "게임의 2배 속도 표시" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<끄기>는 더 단순한 눈금을 보여 주며 보통 도로 표시와 더 가깝습니다.\n" +
                    "<켜기>는 패널과 떠 있는 텍스트에 게임 내부의 더 높은 속도 눈금을 표시합니다.\n" +
                    "다른 툴팁 모드나 Scene Explorer가 게임 내부의 두 배 값을 보여 주고 있어 그 표시와 맞추고 싶다면 유용합니다.\n" +
                    "**표시만 바뀝니다.** 저장된 속도는 <실제로 바뀌지 않습니다>.\n" +
                    "헷갈리면 끄기로 두세요. 켜도 꺼도 차량의 실제 움직임은 똑같습니다.\n" +
                    "참고: <도로 표시는 장식>이므로 게임의 실제 프리팹 속도 데이터와 다를 수 있습니다. 35 mph 표지가 실제로는 ~31 mph일 수 있습니다. 게임은 도로 속도를 먼저 미터법으로 정의한 뒤 변환합니다."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "차량 최고 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "100% = 게임 기본값. 가속과 제동도 자동 조정됩니다. 경로 제한 속도는 유지됩니다. Bikes + Paths 사용 시 업데이트하세요." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "속도 초기화" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "자전거와 전동 킥보드 속도를 100%로 복원합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "자전거 그룹" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "자전거와 전동 스쿠터.\n" +
                    "**활성** = 현재 차선이 있음(이동 중).\n" +
                    "**총 주차** = 주차장뿐 아니라 모든 곳의 Parked 플래그를 포함합니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "자동차 그룹" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "개인 자동차만 포함(자전거 그룹 제외).\n" +
                    "**활성** = 현재 차선이 있음(이동 중).\n" +
                    "**주차** = 주차장뿐 아니라 모든 **ParkedCar**.\n" +
                    "스캔은 옵션 메뉴가 열려 있을 때만 실행되며 도시 플레이 중에는 실행되지 않으므로 FPS 걱정이 없습니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "숨겨진 주차 차량" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**OC 경계 총합** = Outside City (OC) 연결 지점에서 ParkedCar 상태인 자동차 그룹 차량.\n" +
                    "일부 도시에서는 Outside City 연결 지점에 주차된 채 멈춘 차량이 많이 보일 수 있습니다.\n" +
                    "숨겨진 차량의 샘플 분류는 <Log hidden cars>를 사용하세요."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "숨겨진 차량 로그" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "**Logs/AllSpeedLimits.log**에 1회 보고서를 작성합니다.\n" +
                    "총합 + Bucket A/B/C 분류와 샘플 ID 번호를 포함합니다.\n" +
                    "Scene Explorer 모드로 나열된 Vehicle 엔티티 ID로 이동해 확인하세요."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "로그 열기" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "**Logs/AllSpeedLimits.log**가 있으면 엽니다.\n" +
                    "파일이 아직 없으면 대신 Logs 폴더를 엽니다."
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "상태를 불러오지 않았습니다." },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "도시 없음... ¯\\_(ツ)_/¯ ...통계 없음" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "데이터를 얻으려면 도시를 몇 분 실행하세요." },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} 활성 | {1} 자전거 | {2} 스쿠터 | {3} / {4} 주차/총합" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} 활성 | {1} 주차 | {2} 총합 | {3} 트레일러" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} OC 경계에서 숨김 | 업데이트 {1}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "게임 기본 속도 복원" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "모드를 제거하기 전 선택적으로 정리하는 기능입니다.\n" +
                    "이 모드의 사용자 지정 속도를<유지하고 싶지 않을 때만> 사용하세요.\n" +
                    "모드를 제거하는 데 필수는 아닙니다. 사용자 지정 속도는 이 모드 없이도 도시에 남을 수 있습니다.\n" +
                    "<============>\n" +
                    "\n" +
                    "이 버튼은 모드가 사용자 지정 속도를 적용한 곳을 알려진 게임 기본값으로 되돌립니다.\n" +
                    "완료 후 모드를 제거하기 전에 **새 저장 파일**을 만드세요.\n" +
                    "이 기능을 쓰지 않고 모드를 제거하면 도로, 철도 또는 수로를 바꾸기 전까지 사용자 지정 속도는 남습니다."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "이 모드가 적용한 모든 사용자 지정 속도 제한을 알려진 게임 기본값으로 되돌립니다.\n" +
                    "자동으로 되돌릴 수 없습니다.\n" +
                    "완료 후 모드를 제거하기 전에 도시를 새 저장 파일로 저장하세요."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "설명 표시" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "아래에 짧은 사용법을 표시합니다." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<도시 패널>\n" +
                    "▪ 구간을 클릭하거나 드래그 선택합니다.\n" +
                    "▪ <새 속도>를 정한 뒤 <적용>을 누릅니다.\n" +
                    "▪ <초기화>는 선택 구간을 복원합니다.\n" +
                    "▪ <프리셋> 버튼은 즉시 적용됩니다.\n" +
                    "<도시 전체>\n" +
                    "▪ 도로 그룹을 선택한 뒤 그 그룹에 <새 속도>를 적용합니다.\n" +
                    "▪ <도로>, <철도>, <수로>, <전체>로 사용자 속도를 지웁니다.\n" +
                    "▪ <숨은 네트워크>: 점선 버튼으로 숨은 도로와 경로를 표시합니다.\n" +
                    "▪ <경로>: 도시 전체 자전거길과 보행로 배율. 100% = 게임 기본값, 200%는 제한 속도 2배.\n" +
                    "▪ 자전거길 <개별 설정>이 우선합니다. 초기화하면 현재 자전거길 배율을 적용합니다.\n" +
                    "▪ 도시 전체 변경 후에는 <도시 저장>을 하세요."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "모드" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "제작자의 Paradox Mods 페이지를 엽니다." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "디버그 보고서를 로그에 기록" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<일반 플레이에는 필요 없습니다.>\n" +
                    "Logs/AllSpeedLimits.log에 한 번 보고서를 씁니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "로그 열기" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "<Logs/AllSpeedLimits.log>를 엽니다. 파일이 없으면 Logs 폴더를 엽니다." },
            };
        }

        public void Unload()
        {
        }
    }
}
