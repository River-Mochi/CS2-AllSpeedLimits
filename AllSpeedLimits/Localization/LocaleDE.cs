// <copyright file="LocaleDE.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleDE.cs
// Purpose: German locale Options UI settings.

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

    public sealed class LocaleDE : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleDE(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            // Version still appears on the About tab through VersionText.
            string title = $"{Mod.ModName} (Alle Tempolimits)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "Aktionen" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Anzeigeoptionen" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Spielstandards zurücksetzen" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Nutzung" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Geschwindigkeitseinheiten" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Einheiten für Panel und schwebende Schilder wählen.\n" +
                    "<AUTO> folgt dem Kartentyp:\n" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "Mit <KM/H oder MPH> wird diese Anzeige festgelegt."
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Regler mit ausgewähltem Segment abgleichen" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Aktivierung empfohlen**\n" +
                    "Aktiviert: Beim Klick auf ein Segment springt der Regler auf dessen aktuelle Geschwindigkeit, statt bei 5 zu starten.\n" +
                    "- Bei Mehrfachauswahl bestimmt weiterhin das erste Segment die Startposition des Reglers.\n" +
                    "Deaktiviert: Beim Klick auf ein anderes Segment bleibt der letzte Zielwert erhalten."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Regler-Schrittweite" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "Legt die Schrittweite des Reglers im Tempolimit-Panel der Stadt fest.\n" +
                    "<Standard = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Tooltip-Textgröße" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "Dieser Mod erlaubt größeren Text in den Hinweisfeldern, wenn du mit der Maus über Tempolimit-Funktionen fährst.\n" +
                    "<Standard 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Verdoppelte Spielwerte anzeigen" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Aus> zeigt eine einfachere Skala, meist näher an Straßenmarkierungen.\n" +
                    "<Ein> zeigt im Panel und schwebenden Text die höheren internen Spielwerte.\n" +
                    "Nützlich, wenn ein anderes Tooltip-Mod oder Scene Explorer die internen doppelten Spielwerte zeigt und du die Anzeigen angleichen willst.\n" +
                    "**Dies ändert nur die Anzeige;** gespeicherte Geschwindigkeiten <ändern sich nicht wirklich>.\n" +
                    "Wenn das verwirrend ist, lass es einfach Aus. Autos bewegen sich gleich, egal ob Ein oder Aus.\n" +
                    "Hinweis: <Straßenmarkierungen sind Grafik> und stimmen möglicherweise nicht mit den echten Geschwindigkeitsdaten der Spielvorlage überein. Ein 35-mph-Schild kann etwa 31 mph bedeuten. Das Spiel definiert Straßengeschwindigkeiten zuerst metrisch und rechnet sie dann um."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "Spielstandard-Geschwindigkeiten wiederherstellen" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Optionaler Aufräum-Schritt vor dem Entfernen des Mods.\n" +
                    "Nutze das <nur>, wenn du die eigenen Geschwindigkeiten dieses Mods nicht behalten willst.\n" +
                    "Zum Entfernen des Mods ist das nicht erforderlich. Eigene Geschwindigkeiten können auch ohne diesen Mod in der Stadt bleiben.\n" +
                    "<============>\n" +
                    "\n" +
                    "Diese Schaltfläche stellt bekannte Spielstandards überall dort wieder her, wo der Mod eigene Geschwindigkeiten angewendet hat.\n" +
                    "Nach Abschluss als **NEUEN SPIELSTAND** speichern, bevor du den Mod entfernst.\n" +
                    "Wenn du den Mod ohne diesen Schritt entfernst, bleiben eigene Geschwindigkeiten erhalten, bis du Straßen, Schienen oder Wasserwege änderst."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Dies setzt alle von diesem Mod angewendeten eigenen Tempolimits auf bekannte Spielstandards zurück.\n" +
                    "Das kann nicht automatisch rückgängig gemacht werden.\n" +
                    "Nach Abschluss die Stadt als NEUEN Spielstand speichern, bevor du den Mod entfernst."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Anleitung anzeigen" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Zeigt kurze Hinweise unten an." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<Stadt-Panel>\n" +
                    "1. Segmente anklicken oder per Ziehen auswählen.\n" +
                    "2. <Neues Tempo> setzen, dann <Anwenden> klicken.\n" +
                    "3. <Zurücksetzen> stellt ausgewählte Segmente wieder her.\n" +
                    "4. <Vorgaben> werden sofort angewendet.\n" +
                    "\n" +
                    "<Ganze Stadt>\n" +
                    "Eine Straßengruppe wählen, dann <Neues Tempo> darauf anwenden.\n" +
                    "Mit <Straßen>, <Schienen>, <Wasser> oder <Alle> eigene Werte löschen.\n" +
                    "Nach Änderungen in der ganzen Stadt <Stadt speichern>."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Öffnet die Paradox-Mods-Seite des Autors." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "Debug-Bericht ins Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<Für normales Spielen nicht nötig.>\n" +
                    "Schreibt einmalig einen Bericht nach Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "Log öffnen" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "Öffnet <Logs/AllSpeedLimits.log>. Fällt auf den Logs-Ordner zurück, wenn die Datei nicht existiert." },
            };
        }

        public void Unload()
        {
        }
    }
}
