// <copyright file="LocaleVI.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleVI.cs
// Purpose: Vietnamese locale Options UI settings.

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

    public sealed class LocaleVI : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleVI(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            string title = $"{Mod.ModName} (Mọi giới hạn tốc độ)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "Thao tác" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "Giới thiệu" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "Tùy chọn hiển thị" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "Khôi phục mặc định game" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "Cách dùng" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "Gỡ lỗi / Nhật ký" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "Đơn vị tốc độ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "Chọn đơn vị cho bảng và biển nổi.\n" +
                    "<AUTO> theo loại bản đồ: EU = KM/H, NA = MPH.\n" +
                    "<KM/H> và <MPH> ép kiểu hiển thị đó." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "Đồng bộ thanh trượt với đoạn chọn" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<Nên bật>\n" +
                    "Bật: bấm một đoạn sẽ đưa thanh trượt về tốc độ hiện tại của đoạn đầu tiên được chọn.\n" +
                    "Tắt: bấm đoạn khác vẫn giữ mục tiêu thanh trượt trước đó.\n" +
                    "Nếu chọn nhiều phần, đoạn đầu tiên vẫn đặt vị trí bắt đầu của thanh trượt."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "Bước thanh trượt" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "Đặt bước nhảy của thanh trượt trong bảng thành phố.\n" +
                    "<Mặc định = 5>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "Cỡ chữ trợ giúp" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Mod này có thể tăng cỡ chữ trong ô trợ giúp khi trỏ chuột lên các mục của mod.\n" +
                    "<Mặc định 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "Hiện tốc độ nhân đôi của game" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Tắt> hiển thị thang đơn giản hơn, thường gần với ký hiệu trên đường.\n" +
                    "<Bật> bảng và chữ nổi hiển thị thang tốc độ nội bộ cao hơn của game.\n" +
                    "Hữu ích nếu mod chú thích khác hiện giá trị nội bộ nhân đôi của game và bạn muốn khớp.\n" +
                    "**Chỉ phần hiển thị thay đổi;** tốc độ đã lưu <không thật sự thay đổi>.\n" +
                    "Nếu thấy khó hiểu, cứ để Tắt. Xe vẫn di chuyển như nhau dù Bật hay Tắt.\n" +
                    "Lưu ý: ký hiệu và biển trên đường là hình trang trí, nên có thể không khớp dữ liệu tốc độ thật của mẫu game. Biển 35 mph có thể thực ra là 31 mph. Game tính đường theo hệ mét trước rồi mới chuyển đổi."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "Khôi phục tốc độ mặc định của game" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Dọn dẹp tùy chọn trước khi gỡ mod.\n" +
                    "<Chỉ> dùng mục này nếu bạn không muốn giữ tốc độ tùy chỉnh của mod này.\n" +
                    "Không bắt buộc để gỡ mod. Tốc độ đường tùy chỉnh vẫn có thể ở lại trong thành phố khi không có mod này.\n" +
                    "<============>\n" +
                    "\n" +
                    "Mục này khôi phục các tốc độ tùy chỉnh do mod áp dụng về mặc định game đã biết.\n" +
                    "Sau khi xong, hãy tạo **BẢN LƯU MỚI** trước khi gỡ mod.\n" +
                    "Nếu gỡ mod mà không dùng mục này, tốc độ tùy chỉnh sẽ còn lại cho đến khi bạn đổi đường, v.v."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Mục này sẽ khôi phục mọi giới hạn tốc độ tùy chỉnh được hỗ trợ về mặc định game đã biết.\n" +
                    "Không thể tự động hoàn tác.\n" +
                    "Sau khi xong, hãy lưu thành phố thành một bản lưu MỚI trước khi gỡ mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Hiện hướng dẫn" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Hiện ghi chú nhanh bên dưới." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Bảng thành phố>\n" +
                    "1. Bấm hoặc kéo chọn các đoạn.\n" +
                    "2. Đặt <Tốc độ mới>, rồi bấm <Áp dụng>.\n" +
                    "3. <Đặt lại> khôi phục các đoạn đã chọn.\n" +
                    "4. Các nút cài đặt sẵn áp dụng ngay.\n" +
                    "\n" +
                    "<Toàn thành phố>\n" +
                    "Chọn nhóm đường, rồi áp dụng <Tốc độ mới> cho nhóm đó.\n" +
                    "Dùng <Đường>, <Ray>, <Nước> hoặc <Tất cả> để xóa tốc độ tùy chỉnh.\n" +
                    "<Lưu thành phố> sau thay đổi toàn thành phố."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Phiên bản" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "Mở trang Paradox Mods của tác giả." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "Ghi báo cáo gỡ lỗi vào nhật ký" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<Không cần cho chơi bình thường.>\n" +
                    "Ghi một báo cáo một lần vào Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Mở nhật ký" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Mở <Logs/AllSpeedLimits.log>. Nếu tệp không tồn tại, sẽ mở thư mục Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
