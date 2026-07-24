// <copyright file="LocaleIT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleIT.cs
// Purpose: Italian locale Options UI settings.

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

    public sealed class LocaleIT : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleIT(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            // Version still appears on the About tab through VersionText.
            string title = $"{Mod.ModName} (Tutti i limiti)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "Azioni" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Opzioni visualizzazione" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Ripristina valori del gioco" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Uso" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Unità velocità" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Scegli le unità per il pannello e i cartelli flottanti.\n" +
                    "<AUTO> segue il tipo di mappa:\n" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "Selezionando <KM/H o MPH> si forza quella visualizzazione."
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Sincronizza slider col segmento" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Si consiglia di attivarlo**\n" +
                    "Attivo: cliccare un segmento porta lo slider alla sua velocità attuale invece di partire da 5.\n" +
                    "- Se selezioni più segmenti, il primo imposta comunque la posizione iniziale dello slider.\n" +
                    "Disattivo: cliccare un altro segmento mantiene l’ultimo valore scelto."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Passo dello slider" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "Imposta il passo dello slider nel pannello dei limiti di velocità della città.\n" +
                    "<Predefinito = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Dimensione testo aiuti" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "Questo mod permette un testo più grande nelle caselle di aiuto quando passi sulle funzioni dei limiti di velocità.\n" +
                    "<Predefinito 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Mostra velocità doppie del gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Disattivo> mostra una scala più semplice, di solito più vicina alle marcature stradali.\n" +
                    "<Attivo> mostra nel pannello e nel testo flottante la scala interna più alta del gioco.\n" +
                    "Utile se un altro mod di suggerimenti o Scene Explorer mostra i valori interni raddoppiati del gioco e vuoi farli combaciare.\n" +
                    "**Cambia solo la visualizzazione;** le velocità salvate <non cambiano davvero>.\n" +
                    "Se crea confusione, lascia Disattivo. Le auto si muovono allo stesso modo con Attivo o Disattivo.\n" +
                    "Nota: <le marcature stradali sono elementi grafici> e possono non corrispondere ai dati reali di velocità del modello di gioco. Un cartello da 35 mph può rappresentare ~31 mph. Il gioco definisce prima le velocità stradali in unità metriche e poi le converte."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "Ripristina velocità predefinite del gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Pulizia opzionale prima di rimuovere il mod.\n" +
                    "Usalo <solo> se non vuoi tenere le velocità personalizzate di questo mod.\n" +
                    "Non è necessario per rimuovere il mod. Le velocità personalizzate possono restare in città senza questo mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Questo pulsante ripristina i valori di gioco noti dove il mod ha applicato velocità personalizzate.\n" +
                    "Quando finisce, fai un **NUOVO SALVATAGGIO** prima di rimuovere il mod.\n" +
                    "Se rimuovi il mod senza usarlo, le velocità personalizzate restano finché non cambi strade, rotaie o vie d’acqua."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Ripristina ai valori di gioco noti tutti i limiti di velocità personalizzati applicati da questo mod.\n" +
                    "Non si può annullare automaticamente.\n" +
                    "Quando finisce, salva la città come NUOVO salvataggio prima di rimuovere il mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Mostra istruzioni" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Mostra brevi note d’uso sotto." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<Pannello città>\n" +
                    "1. Clicca o trascina per selezionare segmenti.\n" +
                    "2. Imposta <Nuova velocità>, poi clicca <Applica>.\n" +
                    "3. <Ripristina> ripristina i segmenti selezionati.\n" +
                    "4. I pulsanti <Preset> si applicano subito.\n" +
                    "\n" +
                    "<Tutta la città>\n" +
                    "Scegli un gruppo di strade e applica <Nuova velocità> a quel gruppo.\n" +
                    "Usa <Strade>, <Rotaie>, <Acqua> o <Tutto> per eliminare le velocità personalizzate.\n" +
                    "<Salva la città> dopo le modifiche in tutta la città."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "Versione" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Apre la pagina Paradox Mods dell’autore." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "Report debug nel log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<Non serve per il gioco normale.>\n" +
                    "Scrive un rapporto una sola volta in Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "Apri log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "Apre <Logs/AllSpeedLimits.log>. Se il file non esiste, apre la cartella Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
