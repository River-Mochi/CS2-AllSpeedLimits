// <copyright file="LocaleDE.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleDE.cs
// Purpose: German locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "Fahrräder" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Anzeigeoptionen" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "Tastenkürzel" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "Fahrräder + E-Scooter" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "Status persönlicher Fahrzeuge" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Spielstandards zurücksetzen" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Nutzung" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Log" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Tastenkürzel für unsichtbare Netze" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Auf „Nicht belegt“ klicken und Taste wählen. Funktioniert auch ohne ASL-Fenster." },

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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "ASL-Tooltips anzeigen" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)),
                    "**Standard = EIN**  Es wird empfohlen, dies eingeschaltet zu lassen.\n\n" +
                    "Zeigt ASL-Tooltips in der Stadt. Die Info-Schaltfläche schaltet sie wieder ein." },

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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.EnableBicycleSpeedTuning)), "Rad- und Rollertempo ändern" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.EnableBicycleSpeedTuning)),
                    "**AUS** = ASL lässt die Geschwindigkeiten unverändert.\n" +
                    "**AN** = Höchsttempo, Beschleunigung und Bremsen ändern.\n" +
                    "Bei anderen Fahrradtempo-Mods ausgeschaltet lassen." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "Maximale Fahrzeuggeschwindigkeit" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)),
                    "**100% = Spielstandard.**\n" +
                    "Ändert die Höchstgeschwindigkeit von Fahrrädern und E-Scootern. Beschleunigung und Bremsen werden angepasst.\n" +
                    "Weglimits gelten weiterhin.\n" +
                    "Entferne Bikes + Paths, wenn du ASL nutzt. ASL ersetzt alle Funktionen. Ältere Versionen können Konflikte verursachen." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Spielstandard wiederherstellen" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Tempo, Beschleunigung und Bremsen von Fahrrädern und E-Scootern auf **100% Spielstandard** zurücksetzen." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "Fahrradgruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "Fahrräder und E-Scooter.\n" +
                    "**Aktiv** = hat eine aktuelle Spur (in Bewegung).\n" +
                    "**Geparkt gesamt** = enthält alle Parked-Flags überall, nicht nur Parkplätze."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "Autogruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "Nur private Autos (ohne Fahrradgruppe).\n" +
                    "**Aktiv** = hat eine aktuelle Spur (in Bewegung).\n" +
                    "**Geparkt** = alle **ParkedCar**, nicht nur Parkplätze.\n" +
                    "Scan läuft nur, wenn das Optionen-Menü offen ist, nicht während normalem Stadt-Gameplay, also keine FPS-Sorge."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "Versteckt geparkte Autos" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**Gesamt an OC-Grenze** = Autogruppen-Fahrzeuge mit ParkedCar an der Outside-City-Verbindung (OC).\n" +
                    "Manche Städte zeigen viele Autos, die an der Outside-City-Verbindung geparkt festhängen.\n" +
                    "Nutze <Log hidden cars> für eine Beispielaufschlüsselung versteckter Autos."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "Versteckte Autos loggen" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "Schreibt einen einmaligen Bericht nach **Logs/AllSpeedLimits.log**.\n" +
                    "Enthält Gesamtzahl + Bucket-A/B/C-Aufschlüsselung und Beispiel-IDs.\n" +
                    "Nutze den Scene Explorer Mod, um zu den gelisteten Vehicle-Entity-IDs zu springen und zu prüfen."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "Log öffnen" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "Öffnet **Logs/AllSpeedLimits.log**, falls vorhanden.\n" +
                    "Wenn die Datei noch nicht gefunden wird, wird stattdessen der Logs-Ordner geöffnet."
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "Status nicht geladen." },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "Keine Stadt... ¯\\_(ツ)_/¯ ...Keine Stats" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "Stadt ein paar Minuten laufen lassen für Daten." },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} aktiv | {1} Fahrräder | {2} Scooter | {3} / {4} geparkt/gesamt" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} aktiv | {1} geparkt | {2} gesamt | {3} Anhänger" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} an OC-Grenze versteckt | aktualisiert {1}" },

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
                    "<Stadtpanel>\n" +
                    "▪ Ein Segment anklicken oder mehrere per Ziehen wählen. <Neues Tempo> einstellen, dann <Anwenden>.\n" +
                    "▪ <Vorgaben> (eingekreiste Zahlen) gelten sofort, ohne Anwenden.\n" +
                    "▪ <Zurücksetzen> stellt ausgewählte Segmente wieder her.\n" +
                    "▪ <Unsichtbar>: gestrichelte Schaltfläche zeigt/versteckt verborgene Straßen und Wege.\n" +
                    "<Ganze Stadt>\n" +
                    "▪ Straßengruppe wählen und neues Tempo anwenden.\n" +
                    "▪ <Rote Reset-Tasten>: Straßen, Schienen, Wasser, Wege oder Alle löschen zugehörige Tempolimits.\n" +
                    "▪ <Wege>: Rad- und Fußwegregler gelten stadtweit. 100% = Spielstandard; 200% verdoppelt das Limit.\n" +
                    "▪ <Einzelne Radwege> behalten ihr eigenes Tempo. Reine Fußwege sind nur stadtweit einstellbar.\n" +
                    "▪ <Stadt speichern>, damit Änderungen beim Laden erhalten bleiben."
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
