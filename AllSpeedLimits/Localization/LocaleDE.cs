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
        private readonly Setting m_Setting;

        public LocaleDE(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "Aktionen" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "Anzeigeoptionen" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "Spielstandards zurücksetzen" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "Nutzung" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "Debug / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "Geschwindigkeitseinheiten" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "Einheiten für Panel und schwebende Schilder wählen.\n" +
                    "<AUTO> folgt dem Kartentyp: EU = KM/H, NA = MPH.\n" +
                    "<KM/H> und <MPH> erzwingen diese Anzeige." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "Regler mit ausgewähltem Segment abgleichen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<Empfohlen: Ein>\n" +
                    "Ein: Beim Klick auf ein Segment springt der Regler auf die aktuelle Geschwindigkeit des ersten Segments.\n" +
                    "Aus: Beim Klick auf ein anderes Segment bleibt dein letzter Zielwert.\n" +
                    "Bei Mehrfachauswahl setzt weiterhin das erste Segment die Startposition des Reglers."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "Regler-Schrittweite" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "Legt die Schrittweite im Stadt-Panel fest.\n" +
                    "<Standard = 10>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "Tooltip-Textgröße" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Macht Mod-Hinweise größer, wenn du über Mod-Elemente fährst.\n" +
                    "<Standard 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "Verdoppelte Spielwerte anzeigen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Aus> zeigt eine einfachere Skala, meist näher an Straßenmarkierungen.\n" +
                    "<Ein> zeigt im Panel und schwebenden Text die höheren internen Spielwerte.\n" +
                    "Nützlich, wenn ein anderes Tooltip-Mod die internen doppelten Werte des Spiels zeigt und du diese Anzeige angleichen willst.\n" +
                    "**Dies ändert nur die Anzeige;** gespeicherte Geschwindigkeiten <ändern sich nicht wirklich>.\n" +
                    "Wenn das verwirrend ist, lass es einfach Aus. Autos bewegen sich gleich, egal ob Ein oder Aus.\n" +
                    "Hinweis: Straßenmarkierungen sind Grafik und stimmen möglicherweise nicht mit den echten Geschwindigkeitsdaten der Spielvorlage überein. Ein 35-mph-Schild kann tatsächlich 31 mph bedeuten. Das Spiel berechnet Straßen zuerst metrisch und rechnet dann um."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "Spielstandard-Geschwindigkeiten wiederherstellen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Optionaler Aufräum-Schritt vor dem Entfernen des Mods.\n" +
                    "Nutze das <nur>, wenn du die eigenen Geschwindigkeiten dieses Mods nicht behalten willst.\n" +
                    "Zum Entfernen des Mods ist das nicht erforderlich. Eigene Straßengeschwindigkeiten können in der Stadt bleiben, auch ohne diesen Mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Dadurch werden die bekannten Spielstandards für die vom Mod geänderten Geschwindigkeiten wiederhergestellt.\n" +
                    "Nach Abschluss: als **NEUEN SPIELSTAND** speichern, bevor du den Mod entfernst.\n" +
                    "Wenn du den Mod ohne diesen Schritt entfernst, bleiben eigene Geschwindigkeiten erhalten, bis du die Straßen änderst usw."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Dadurch werden alle unterstützten eigenen Tempolimits auf bekannte Spielstandards zurückgesetzt.\n" +
                    "Das kann nicht automatisch rückgängig gemacht werden.\n" +
                    "Nach Abschluss als NEUEN Spielstand speichern, bevor du den Mod entfernst."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Anleitung anzeigen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Zeigt kurze Hinweise unten an." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Stadt-Panel>\n" +
                    "1. Segmente anklicken oder per Ziehen auswählen.\n" +
                    "2. <Neues Tempo> setzen, dann <Anwenden> klicken.\n" +
                    "3. <Zurücksetzen> stellt ausgewählte Segmente wieder her.\n" +
                    "4. Voreinstellungen werden sofort angewendet.\n" +
                    "\n" +
                    "<Ganze Stadt>\n" +
                    "Eine Straßengruppe wählen, dann <Neues Tempo> darauf anwenden.\n" +
                    "Mit <Straßen>, <Schienen>, <Wasser> oder <Alle> eigene Werte löschen.\n" +
                    "Nach stadtweiten Änderungen <Stadt speichern>."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "Öffnet die Paradox-Mods-Seite des Autors." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "Debug-Bericht ins Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<Für normales Spielen nicht nötig.>\n" +
                    "Schreibt einmalig einen Bericht nach Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Log öffnen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Öffnet <Logs/AllSpeedLimits.log>. Fällt auf den Logs-Ordner zurück, wenn die Datei nicht existiert." },
            };
        }

        public void Unload()
        {
        }
    }
}
