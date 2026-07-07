// <copyright file="LocaleTR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleTR.cs
// Purpose: Turkish locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;
    using Game.Areas;
    using Game.Citizens;
    using Game.City;
    using Game.Objects;
    using Game.UI;

    public sealed class LocaleTR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleTR(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            string title = $"{Mod.ModName} (Tüm hız sınırları)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "Eylemler" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "Hakkında" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "Görüntü seçenekleri" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "Oyun varsayılanlarına dön" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "Kullanım" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "Hata ayıklama / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "Hız birimleri" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "Panel ve yüzen tabela birimlerini seç.\n" +
                    "<AUTO> harita tipini izler: EU = KM/H, NA = MPH.\n" +
                    "<KM/H> ve <MPH> bu gösterimi zorlar." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "Kaydırıcıyı seçili bölümle eşleştir" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<Açık önerilir>\n" +
                    "Açık: bir bölüme tıklayınca kaydırıcı ilk seçili bölümün mevcut hızına gider.\n" +
                    "Kapalı: başka bölüme tıklayınca son hedefin kalır.\n" +
                    "Birden fazla parça seçersen, kaydırıcı başlangıcını yine ilk bölüm belirler."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "Panel kaydırıcı adımı" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "Şehir panelindeki kaydırıcı adımını ayarlar.\n" +
                    "<Varsayılan = 5>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "Yardım metni boyutu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Mod açılır pencerelerini ve yardım metnini büyütür.\n" +
                    "<Varsayılan 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "Oyunun çift hızlarını göster" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Kapalı> daha basit bir ölçek gösterir, genelde yol işaretlerine daha yakındır.\\n" +
                    "<Açık> panel ve yüzen yazıda oyunun daha yüksek iç hız ölçeğini gösterir.\\n" +
                    "Başka bir tooltip modu içteki iki kat değerleri gösteriyorsa eşleştirmek için yararlıdır.\\n" +
                    "Bu sadece görseldir; kaydedilen hızlar <gerçekte değişmez>.\\n" +
                    "Yol işaretleri görseldir ve prefab hız verisiyle tam örtüşmeyebilir.\\n" +
                    "Kafa karıştırıyorsa Kapalı bırak. Arabalar Açık veya Kapalı aynı hareket ediyor görünür." },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "Tüm özel hızları temizle" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Bu şehirde desteklenen yolları, rayları ve su yollarını <oyun varsayılanlarına> döndürür.\\n" +
                    "<Sıfırlamayı korumak için sonra kaydet.>\\n" +
                    "- Özel hızları istemiyorsan modu kaldırmadan önce yararlıdır.\\n" +
                    "- Modu temizlemeden kaldırırsan kayıtlı hızlar genelde kalır, ama sıfırla/yeniden uygula desteği gider." },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Bu şehirde desteklenen yol, ray ve su yolu bölümlerinden tüm özel hızlar temizlensin mi?\n" +
                    "Bu geri alınamaz."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Talimatları göster" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Aşağıda kısa kullanım notlarını gösterir." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Şehir paneli>\n" +
                    "1. Bölümlere tıkla veya sürükleyerek seç.\n" +
                    "2. <Yeni hız> ayarla, sonra <Uygula> tıkla.\n" +
                    "3. <Reset> seçili bölümleri geri yükler.\n" +
                    "4. <50%> düğmeleri hemen uygulanır.\n" +
                    "\n" +
                    "<Tüm şehir>\n" +
                    "Bir yol grubu seç, sonra o gruba <Yeni hız> uygula.\n" +
                    "Özel hızları temizlemek için <Yollar>, <Raylar>, <Su> veya <Tümü> kullan.\n" +
                    "Şehir geneli değişikliklerden sonra <Şehri kaydet>."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Sürüm" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "Yazarın Paradox Mods sayfasını açar." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "Debug raporunu loga yaz" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<Normal oyun için gerekmez.>\\n" +
                    "Logs/AllSpeedLimits.log içine tek seferlik rapor yazar." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Logu aç" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)), "<Logs/AllSpeedLimits.log> dosyasını açar. Dosya yoksa Logs klasörüne gider." },
            };
        }

        public void Unload()
        {
        }
    }
}
