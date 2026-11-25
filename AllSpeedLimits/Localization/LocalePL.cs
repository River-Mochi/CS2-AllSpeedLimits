// <copyright file="LocalePL.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocalePL.cs
// Purpose: Polish locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocalePL : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocalePL(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            string title = $"{Mod.ModName} (Wszystkie limity prędkości)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "Akcje" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "Rowery" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "O modzie" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Opcje wyświetlania" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "Skróty klawiszowe" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "Rowery i hulajnogi elektryczne" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "Status pojazdów osobistych" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Przywróć wartości domyślne gry" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Użycie" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Log" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Skrót ukrytych sieci" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Kliknij „nie ustawiono” i wybierz klawisz. Działa także przy zamkniętym ASL." },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Jednostki prędkości" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Wybierz jednostki panelu i pływających znaków.\n" +
                    "<AUTO> zależy od typu mapy:\n" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "Wybór <KM/H lub MPH> wymusza ten widok."
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Synchronizuj suwak z segmentem" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Zalecane: włączone**\n" +
                    "Włączone: kliknięcie segmentu ustawia suwak na jego aktualną prędkość zamiast zaczynać od 5.\n" +
                    "- Przy wyborze wielu segmentów pierwszy nadal ustawia pozycję startową suwaka.\n" +
                    "Wyłączone: kliknięcie innego segmentu zostawia ostatni cel suwaka."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Krok suwaka panelu" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "Ustawia krok suwaka w panelu limitów prędkości miasta.\n" +
                    "<Domyślnie = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Rozmiar tekstu podpowiedzi" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "Ten mod pozwala powiększyć tekst w polach pomocy po najechaniu na funkcje limitów prędkości.\n" +
                    "<Domyślnie 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Pokaż podwojone prędkości gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Wył.> pokazuje prostszą skalę, zwykle bliższą oznaczeniom na drogach.\n" +
                    "<Wł.> panel i tekst pływający pokazują wyższą wewnętrzną skalę gry.\n" +
                    "Przydatne, jeśli inny mod z podpowiedziami lub Scene Explorer pokazuje podwojone wartości wewnętrzne gry i chcesz je dopasować.\n" +
                    "**Zmienia się tylko wygląd;** zapisane prędkości <naprawdę się nie zmieniają>.\n" +
                    "Jeśli to myli, zostaw Wył. Samochody poruszają się tak samo przy Wł. i Wył.\n" +
                    "Uwaga: <oznaczenia drogowe są grafiką> i mogą nie odpowiadać rzeczywistym danym prędkości modeli gry. Znak 35 mph może oznaczać ~31 mph. Gra najpierw definiuje prędkości dróg w systemie metrycznym, a potem przelicza jednostki."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "Maksymalna prędkość pojazdów" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "100% = ustawienie gry. Przyspieszenie i hamowanie dostosowują się automatycznie. Limity ścieżek nadal obowiązują. Zaktualizuj Bikes + Paths, jeśli używasz." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Przywróć prędkość" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Przywraca prędkość rowerów i hulajnóg do 100%." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "Grupa rowerów" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "Rowery i hulajnogi elektryczne.\n" +
                    "**Aktywne** = mają aktualny pas (poruszają się).\n" +
                    "**Łącznie zaparkowane** = obejmuje wszystkie flagi Parked z każdego miejsca, nie tylko parkingi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "Grupa aut" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "Tylko auta osobiste (bez grupy rowerów).\n" +
                    "**Aktywne** = mają aktualny pas (poruszają się).\n" +
                    "**Zaparkowane** = wszystkie **ParkedCar**, nie tylko parkingi.\n" +
                    "Skan działa tylko przy otwartym menu Opcje, nie podczas normalnej gry w mieście, więc bez obaw o FPS."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "Ukryte zaparkowane auta" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**Łącznie na granicy OC** = pojazdy z grupy aut z ParkedCar przy połączeniu Outside City (OC).\n" +
                    "W niektórych miastach wiele aut może utknąć zaparkowanych przy połączeniu Outside City.\n" +
                    "Użyj <Log hidden cars>, aby zobaczyć przykładowy podział ukrytych aut."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "Zaloguj ukryte auta" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "Zapisuje jednorazowy raport do **Logs/AllSpeedLimits.log**.\n" +
                    "Zawiera łączną liczbę + podział Bucket A/B/C oraz przykładowe numery ID.\n" +
                    "Użyj moda Scene Explorer, aby przejść do podanych ID encji Vehicle i sprawdzić."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "Otwórz log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "Otwiera **Logs/AllSpeedLimits.log**, jeśli istnieje.\n" +
                    "Jeśli plik jeszcze nie istnieje, otwiera folder Logs."
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "Status niezaładowany." },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "Brak miasta... ¯\\_(ツ)_/¯ ...Brak statystyk" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "uruchom miasto na kilka minut, aby zebrać dane." },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} aktywne | {1} rowery | {2} hulajnogi | {3} / {4} zaparkowane/łącznie" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} aktywne | {1} zaparkowane | {2} łącznie | {3} przyczepy" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} ukryte na granicy OC | aktualizacja {1}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "Przywróć domyślne prędkości gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Opcjonalne czyszczenie przed usunięciem moda.\n" +
                    "Użyj tego <tylko>, jeśli nie chcesz zachować własnych prędkości tego moda.\n" +
                    "Nie jest to wymagane do usunięcia moda. Własne prędkości mogą zostać w mieście bez tego moda.\n" +
                    "<============>\n" +
                    "\n" +
                    "Ten przycisk przywraca znane wartości domyślne gry wszędzie, gdzie mod zastosował własne prędkości.\n" +
                    "Po zakończeniu zrób **NOWY ZAPIS** przed usunięciem moda.\n" +
                    "Jeśli usuniesz mod bez tej opcji, własne prędkości zostaną, aż zmienisz drogi, tory lub drogi wodne."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Przywraca wszystkie własne limity prędkości zastosowane przez ten mod do znanych wartości domyślnych gry.\n" +
                    "Tej operacji nie da się cofnąć automatycznie.\n" +
                    "Po zakończeniu zapisz miasto jako NOWY zapis przed usunięciem moda."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Pokaż instrukcje" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Pokaż krótkie notatki poniżej." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<Panel miasta>\n" +
                    "▪ Kliknij albo przeciągnij, aby wybrać segmenty.\n" +
                    "▪ Ustaw <Nową prędkość>, potem kliknij <Zastosuj>.\n" +
                    "▪ <Przywróć> przywraca wybrane segmenty.\n" +
                    "▪ <Ustawienia wstępne> działają od razu.\n" +
                    "<Całe miasto>\n" +
                    "▪ Wybierz grupę dróg i zastosuj do niej <Nową prędkość>.\n" +
                    "▪ Użyj <Drogi>, <Tory>, <Woda> lub <Wszystko>, aby wyczyścić własne prędkości.\n" +
                    "▪ <Ukryte sieci>: przycisk z linią przerywaną pokazuje ukryte drogi i ścieżki.\n" +
                    "▪ <Ścieżki>: skale rowerowa i piesza dla całego miasta. 100% = ustawienie gry; 200% podwaja limit.\n" +
                    "▪ <Zmiany indywidualne> na ścieżkach rowerowych mają priorytet. Reset przywraca aktywną skalę rowerową.\n" +
                    "▪ Po zmianach w całym mieście <zapisz miasto>."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Otwiera stronę autora w Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "Raport debug do logu" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<Niepotrzebne do normalnej gry.>\n" +
                    "Zapisuje jednorazowy raport do Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "Otwórz log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "Otwiera <Logs/AllSpeedLimits.log>. Jeśli plik nie istnieje, otwiera folder Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
