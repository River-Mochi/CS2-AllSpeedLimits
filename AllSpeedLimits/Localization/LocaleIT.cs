// <copyright file="LocaleIT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleIT.cs
// Purpose: Italian locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "Biciclette" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Opzioni visualizzazione" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "Scorciatoie" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "Bici e monopattini elettrici" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "Stato veicoli personali" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Ripristina valori del gioco" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Uso" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Log" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Scorciatoia reti invisibili" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Clicca su «non impostato» e scegli un tasto. Funziona anche con ASL chiuso." },

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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "Mostra tooltip ASL" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)),
                    "**Predefinito = ON**  Si consiglia di lasciarlo attivo.\n\n" +
                    "Mostra i tooltip in città. Il pulsante info può riattivarli." },

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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.EnableBicycleSpeedTuning)), "Regola velocità di bici e monopattini" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.EnableBicycleSpeedTuning)),
                    "**OFF** = ASL non modifica queste velocità.\n" +
                    "**ON** = Regola velocità massima, accelerazione e frenata.\n" +
                    "Lascia disattivato se un'altra mod gestisce queste velocità." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "Velocità massima dei veicoli" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)),
                    "**100% = valori del gioco.**\n" +
                    "Modifica la velocità massima di bici e monopattini elettrici. Accelerazione e frenata si adattano automaticamente.\n" +
                    "I limiti dei percorsi restano validi.\n" +
                    "Rimuovi Bikes + Paths se usi ASL. ASL sostituisce tutte le sue funzioni. Le vecchie versioni possono causare conflitti." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Ripristina valori del gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Ripristina velocità, accelerazione e frenata di bici e monopattini al **100% dei valori del gioco**." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "Gruppo bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "Bici e scooter elettrici.\n" +
                    "**Attivi** = hanno una corsia attuale (in movimento).\n" +
                    "**Totale parcheggiati** = include tutti i flag Parked ovunque, non solo nei parcheggi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "Gruppo auto" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "Solo auto personali (esclude il gruppo bici).\n" +
                    "**Attive** = hanno una corsia attuale (in movimento).\n" +
                    "**Parcheggiate** = tutte le **ParkedCar**, non solo i parcheggi.\n" +
                    "La scansione gira solo con il menu Opzioni aperto, non durante il gameplay in città, quindi nessun problema di FPS."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "Auto parcheggiate nascoste" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**Totale al bordo OC** = veicoli del gruppo auto con ParkedCar alla connessione Outside City (OC).\n" +
                    "Alcune città mostrano molte auto bloccate parcheggiate alla connessione Outside City.\n" +
                    "Usa <Log hidden cars> per un esempio di dettaglio delle auto nascoste."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "Registra auto nascoste" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "Scrive un report una tantum in **Logs/AllSpeedLimits.log**.\n" +
                    "Include totale + dettaglio Bucket A/B/C e ID di esempio.\n" +
                    "Usa la mod Scene Explorer per saltare agli ID entità Vehicle elencati e controllare."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "Apri log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "Apre **Logs/AllSpeedLimits.log** se esiste.\n" +
                    "Se il file non esiste ancora, apre invece la cartella Logs."
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "Stato non caricato." },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "Nessuna città... ¯\\_(ツ)_/¯ ...Nessuna statistica" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "fai andare la città per qualche minuto per avere dati." },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} attive | {1} bici | {2} scooter | {3} / {4} parcheggiate/totale" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} attive | {1} parcheggiate | {2} totale | {3} rimorchi" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} nascoste al bordo OC | aggiornato {1}" },

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
                    "▪ Clicca un segmento o trascina per selezionarne molti. Imposta <Nuova velocità>, poi <Applica>.\n" +
                    "▪ I <preset> (numeri cerchiati) si applicano subito, senza Applica.\n" +
                    "▪ <Ripristina> ripristina i segmenti selezionati.\n" +
                    "▪ <Invisibile>: il pulsante tratteggiato mostra/nasconde strade e percorsi nascosti.\n" +
                    "<Intera città>\n" +
                    "▪ Scegli un gruppo stradale e applica la nuova velocità.\n" +
                    "▪ <Pulsanti rossi>: strade, binari, acqua, percorsi o Tutti cancellano i relativi limiti personalizzati.\n" +
                    "▪ <Percorsi>: cursori bici e pedoni per tutta la città. 100% = valore del gioco; 200% raddoppia il limite.\n" +
                    "▪ I <tratti ciclabili personalizzati> mantengono la velocità. I percorsi solo pedonali si regolano in tutta la città.\n" +
                    "▪ <Salva la città> per mantenere le modifiche al caricamento."
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
