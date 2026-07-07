// <copyright file="LocalePL.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocalePL.cs
// Purpose: Polish locale Options UI settings.

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

    public sealed class LocalePL : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePL(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "Akcje" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "O modzie" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "Opcje wyświetlania" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "Przywróć domyślne gry" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "Użycie" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "Debug / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "Jednostki prędkości" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "Wybierz jednostki panelu i pływających znaków.\n" +
                    "<AUTO> zależy od typu mapy: EU = KM/H, NA = MPH.\n" +
                    "<KM/H> i <MPH> wymuszają ten widok." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "Synchronizuj suwak z segmentem" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<Zalecane: włączone>\n" +
                    "Włączone: kliknięcie segmentu ustawia suwak na aktualną prędkość pierwszego segmentu.\n" +
                    "Wyłączone: kliknięcie innego segmentu zostawia ostatni cel suwaka.\n" +
                    "Przy wyborze wielu części pierwszy segment nadal ustawia pozycję startową suwaka."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "Krok suwaka panelu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "Ustawia krok suwaka w panelu miasta.\n" +
                    "<Domyślnie = 10>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "Rozmiar tekstu podpowiedzi" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Powiększa okienka moda i tekst pomocy.\n" +
                    "<Domyślnie 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "Pokaż podwojone prędkości gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Wył.> pokazuje prostszą skalę, zwykle bliższą oznaczeniom na drogach.\n" +
                    "<Wł.> panel i tekst pływający pokazują wyższą wewnętrzną skalę gry.\n" +
                    "Przydatne, jeśli inny mod z podpowiedziami pokazuje podwojone wartości wewnętrzne i chcesz je dopasować.\n" +
                    "To tylko wygląd; zapisane prędkości <naprawdę się nie zmieniają>.\n" +
                    "Oznaczenia na drogach są grafiką i mogą nie zgadzać się dokładnie z danymi prefabów.\n" +
                    "Jeśli to myli, zostaw Wył. Samochody będą wyglądać tak samo w ruchu przy Wł. i Wył."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "Przywróć domyślne prędkości gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Opcjonalne czyszczenie przed usunięciem moda.\n" +
                    "Użyj tego <tylko>, jeśli nie chcesz zachować własnych prędkości tego moda.\n" +
                    "Nie jest to wymagane do usunięcia moda. Własne prędkości mogą zostać w mieście bez tego moda.\n" +
                    "<============>\n" +
                    "\n" +
                    "Przywraca znane domyślne wartości gry dla prędkości ustawionych przez ten mod.\n" +
                    "Po zakończeniu zrób **NOWY ZAPIS** przed usunięciem moda.\n" +
                    "Jeśli usuniesz mod bez tej opcji, własne prędkości zostaną, aż zmienisz drogi itd.."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "To przywróci wszystkie obsługiwane własne limity prędkości do znanych domyślnych wartości gry.\n" +
                    "Tej operacji nie da się cofnąć automatycznie.\n" +
                    "Po zakończeniu zapisz miasto jako NOWY zapis przed usunięciem moda."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Pokaż instrukcje" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Pokaż krótkie notatki poniżej." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Panel miasta>\n" +
                    "1. Kliknij albo przeciągnij, aby wybrać segmenty.\n" +
                    "2. Ustaw <Nową prędkość>, potem kliknij <Zastosuj>.\n" +
                    "3. <Reset> przywraca wybrane segmenty.\n" +
                    "4. Przyciski <50%> działają od razu.\n" +
                    "\n" +
                    "<Całe miasto>\n" +
                    "Wybierz grupę dróg i zastosuj do niej <Nową prędkość>.\n" +
                    "Użyj <Drogi>, <Tory>, <Woda> lub <Wszystko>, aby wyczyścić własne prędkości.\n" +
                    "Po zmianach w całym mieście <zapisz miasto>."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "Otwiera stronę autora w Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "Raport debug do logu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<Niepotrzebne do normalnej gry.>\n" +
                    "Zapisuje jednorazowy raport do Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Otwórz log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Otwiera <Logs/AllSpeedLimits.log>. Jeśli plik nie istnieje, otwiera folder Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
