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
    using Colossal.PSI.Common;
    using Game.Areas;
    using Game.Citizens;
    using Game.City;
    using Game.Objects;
    using Game.UI;

    public sealed class LocaleTR : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleTR(SpeedLimitsSetting setting)
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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "Eylemler" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Hakkında" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Görüntü seçenekleri" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Oyun varsayılanlarına dön" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Kullanım" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Hata ayıklama / Günlük" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Hız birimleri" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Panel ve yüzen tabela birimlerini seç.\n" +
                    "<AUTO> harita tipini izler:\n" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "<KM/H veya MPH> seçmek bu gösterimi sabitler."
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Kaydırıcıyı seçili bölümle eşleştir" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Açık olması önerilir**\n" +
                    "Açık: Bir bölüme tıklayınca kaydırıcı 5’ten başlamak yerine o bölümün mevcut hızına gider.\n" +
                    "- Birden çok bölüm seçersen ilk bölüm kaydırıcının başlangıç konumunu belirler.\n" +
                    "Kapalı: Başka bir bölüme tıklayınca son hedef değer korunur."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Panel kaydırıcı adımı" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "Şehir hız sınırı panelindeki kaydırıcının adım boyutunu ayarlar.\n" +
                    "<Varsayılan = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Yardım metni boyutu" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "Bu mod, hız sınırı özelliklerinin üzerine gelince açılan yardım kutularındaki yazıyı büyütebilir.\n" +
                    "<Varsayılan 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Oyunun çift hızlarını göster" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Kapalı> daha basit bir ölçek gösterir, genelde yol işaretlerine daha yakındır.\n" +
                    "<Açık> panel ve yüzen yazıda oyunun daha yüksek iç hız ölçeğini gösterir.\n" +
                    "Başka bir ipucu modu veya Scene Explorer oyunun içteki iki kat değerlerini gösteriyorsa ve bunlarla eşleştirmek istiyorsan yararlıdır.\n" +
                    "**Bu yalnızca görünümü değiştirir;** kaydedilen hızlar <gerçekte değişmez>.\n" +
                    "Kafa karıştırıyorsa Kapalı bırak. Arabalar Açık veya Kapalıyken aynı şekilde hareket eder.\n" +
                    "Not: <yol üzerindeki işaretler görsel tasarımdır> ve oyunun gerçek hazır nesne hız verileriyle uyuşmayabilir. 35 mph yazan bir işaret ~31 mph olabilir. Oyun yol hızlarını önce metrik olarak tanımlar, sonra dönüştürür."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "Oyun varsayılan hızlarına dön" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Modu kaldırmadan önce isteğe bağlı temizlik.\n" +
                    "Bunu <yalnızca> bu modun özel hızlarını saklamak istemiyorsan kullan.\n" +
                    "Modu kaldırmak için gerekli değildir. Özel hızlar bu mod olmadan da şehirde kalabilir.\n" +
                    "<============>\n" +
                    "\n" +
                    "Bu düğme, modun özel hız uyguladığı yerlerde bilinen oyun varsayılanlarını geri yükler.\n" +
                    "Bittiğinde modu kaldırmadan önce **YENİ KAYIT** oluştur.\n" +
                    "Bunu kullanmadan modu kaldırırsan özel hızlar yolları, rayları veya su yollarını değiştirene kadar kalır."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Bu modun uyguladığı tüm özel hız sınırlarını bilinen oyun varsayılanlarına döndürür.\n" +
                    "Bu otomatik olarak geri alınamaz.\n" +
                    "Bittiğinde modu kaldırmadan önce şehri YENİ kayıt olarak kaydet."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Talimatları göster" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Aşağıda kısa kullanım notlarını gösterir." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<Şehir paneli>\n" +
                    "1. Bölümlere tıkla veya sürükleyerek seç.\n" +
                    "2. <Yeni hız> ayarla, sonra <Uygula> tıkla.\n" +
                    "3. <Sıfırla> seçili bölümleri geri yükler.\n" +
                    "4. <Ön ayar> düğmeleri anında uygulanır.\n" +
                    "\n" +
                    "<Tüm şehir>\n" +
                    "Bir yol grubu seç, sonra o gruba <Yeni hız> uygula.\n" +
                    "Özel hızları temizlemek için <Yollar>, <Raylar>, <Su> veya <Tümü> kullan.\n" +
                    "Tüm şehir değişikliklerinden sonra <Şehri kaydet>."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "Sürüm" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Yazarın Paradox Mods sayfasını açar." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "Hata ayıklama raporunu günlüğe yaz" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<Normal oyun için gerekmez.>\n" +
                    "Logs/AllSpeedLimits.log içine tek seferlik rapor yazar."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "Günlüğü aç" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "<Logs/AllSpeedLimits.log> dosyasını açar. Dosya yoksa Logs klasörüne gider." },
            };
        }

        public void Unload()
        {
        }
    }
}
