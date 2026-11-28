// <copyright file="LocaleTR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleTR.cs
// Purpose: Turkish locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "Bisikletler" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Hakkında" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Görüntü seçenekleri" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "Kısayollar" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "Bisiklet ve elektrikli scooter" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "Kişisel araç durumu" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Oyun varsayılanlarına dön" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Kullanım" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Hata ayıklama / Günlük" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Görünmez ağlar kısayolu" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "“Ayarlanmadı” alanına tıklayıp tuş atayın. ASL kapalıyken de çalışır." },

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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.EnableBicycleSpeedTuning)), "Bisiklet ve scooter hızını ayarla" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.EnableBicycleSpeedTuning)),
                    "**KAPALI** = ASL bu araçların hızına dokunmaz.\n" +
                    "**AÇIK** = Azami hız, ivme ve frenlemeyi ayarlar.\n" +
                    "Başka bir mod bu hızları ayarlıyorsa kapalı tut." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "Azami araç hızı" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)),
                    "**%100 = oyun varsayılanı.**\n" +
                    "Bisiklet ve elektrikli scooterların azami hızını değiştirir. Hızlanma ve frenleme otomatik ayarlanır.\n" +
                    "Yol hız sınırları geçerliliğini korur.\n" +
                    "ASL kullanıyorsan Bikes + Paths modunu kaldır. ASL tüm özelliklerini içerir. Eski sürümler çakışabilir." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Oyun varsayılanına dön" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Bisiklet ve scooter hızını, hızlanmasını ve frenlemesini **%100 oyun varsayılanına** döndürür." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "Bisiklet grubu" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "Bisikletler ve elektrikli scooterlar.\n" +
                    "**Aktif** = mevcut şeridi var (hareket ediyor).\n" +
                    "**Toplam park** = yalnız otoparklar değil, her yerdeki tüm Parked durumlarını içerir."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "Araba grubu" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "Sadece kişisel arabalar (Bisiklet grubu hariç).\n" +
                    "**Aktif** = mevcut şeridi var (hareket ediyor).\n" +
                    "**Park** = sadece otoparklar değil, tüm **ParkedCar**.\n" +
                    "Tarama sadece Options menüsü açıkken çalışır, şehir oynanışı sırasında değil; FPS derdi yok."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "Gizli park etmiş arabalar" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**OC sınırındaki toplam** = Şehir Dışı (OC) bağlantısında ParkedCar olan araba grubu araçları.\n" +
                    "Bazı şehirlerde Şehir Dışı bağlantısında parkta takılmış çok sayıda araba görülür.\n" +
                    "Gizli arabaların örnek dökümü için <Log hidden cars> kullan."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "Gizli arabaları günlüğe yaz" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "**Logs/AllSpeedLimits.log** içine tek seferlik rapor yazar.\n" +
                    "Toplam + Bucket A/B/C dökümü ve örnek ID numaralarını içerir.\n" +
                    "Scene Explorer moduyla listelenen Vehicle varlık ID'lerine atlayıp incele."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "Günlüğü aç" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "Varsa **Logs/AllSpeedLimits.log** dosyasını açar.\n" +
                    "Dosya henüz bulunamazsa onun yerine Logs klasörünü açar."
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "Durum yüklenmedi." },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "Şehir yok... ¯\\_(ツ)_/¯ ...İstatistik yok" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "Veri için şehri birkaç dakika çalıştır." },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} aktif | {1} bisiklet | {2} scooter | {3} / {4} park/toplam" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} aktif | {1} park | {2} toplam | {3} römork" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} OC sınırında gizli | güncellendi {1}" },

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
                    "▪ Bir kesime tıklayın veya sürükleyerek birden çok kesim seçin. <Yeni hız> belirleyip <Uygula> düğmesine basın.\n" +
                    "▪ <Hazır ayarlar> (daire içindeki sayılar) anında uygulanır; Uygula gerekmez.\n" +
                    "▪ <Sıfırla> seçili kesimleri geri yükler.\n" +
                    "▪ <Görünmez ağlar>: kesikli düğme gizli yolları gösterir/gizler.\n" +
                    "<Tüm şehir>\n" +
                    "▪ Yol grubunu seçin ve yeni hızı uygulayın.\n" +
                    "▪ <Kırmızı sıfırlama düğmeleri>: yollar, raylar, su, patikalar veya tümü ilgili özel hızları temizler.\n" +
                    "▪ <Yollar>: bisiklet ve yaya ölçekleri şehir genelinde geçerli. %100 = oyun varsayılanı; %200 sınırı ikiye katlar.\n" +
                    "▪ <Tekil bisiklet yolu ayarları> önceliklidir. Yalnız yaya yolları şehir genelinde ayarlanır.\n" +
                    "▪ Yüklemede değişiklikleri korumak için <şehri kaydedin>."
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
