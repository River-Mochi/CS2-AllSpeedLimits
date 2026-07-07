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
        private readonly Setting m_Setting;

        public LocaleIT(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "Azioni" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "Opzioni visualizzazione" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "Ripristina valori del gioco" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "Uso" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "Debug / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "Unità velocità" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "Scegli le unità per pannello e cartelli flottanti.\n" +
                    "<AUTO> segue il tipo di mappa: EU = KM/H, NA = MPH.\n" +
                    "<KM/H> e <MPH> forzano quella visualizzazione." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "Sincronizza slider col segmento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<Consigliato attivo>\n" +
                    "Attivo: cliccare un segmento porta lo slider alla velocità attuale del primo segmento selezionato.\n" +
                    "Disattivo: cliccare un altro segmento mantiene l’ultimo valore scelto.\n" +
                    "Se selezioni più parti, il primo segmento imposta comunque la posizione iniziale."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "Passo dello slider" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "Imposta il passo dello slider nel pannello città.\n" +
                    "<Predefinito = 10>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "Dimensione testo aiuti" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Questo mod può avere testo più grande nelle caselle di aiuto mentre passi sugli elementi del mod.\n" +
                    "<Predefinito 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "Mostra velocità doppie del gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Disattivo> mostra una scala più semplice, di solito più vicina ai segni stradali.\n" +
                    "<Attivo> mostra nel pannello e nel testo flottante la scala interna più alta del gioco.\n" +
                    "Utile se un altro mod tooltip mostra i valori interni raddoppiati e vuoi farli combaciare.\n" +
                    "È solo visuale; le velocità salvate <non cambiano davvero>.\n" +
                    "I segni stradali sono grafica e possono non corrispondere ai dati prefab.\n" +
                    "Se crea confusione, lascia Disattivo. Le auto si muoveranno allo stesso modo con Attivo o Disattivo."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "Ripristina velocità predefinite del gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Pulizia opzionale prima di rimuovere il mod.\n" +
                    "Usalo <solo> se non vuoi tenere le velocità personalizzate di questo mod.\n" +
                    "Non è necessario per rimuovere il mod. Le velocità personalizzate delle strade possono restare in città senza questo mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Ripristina ai valori di gioco noti le velocità personalizzate applicate da questo mod.\n" +
                    "Quando finisce, fai un **NUOVO SALVATAGGIO** prima di rimuovere il mod.\n" +
                    "Se rimuovi il mod senza usarlo, le velocità personalizzate restano finché non cambi le strade, ecc."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Ripristinerà tutti i limiti di velocità personalizzati supportati ai valori di gioco noti.\n" +
                    "Non si può annullare automaticamente.\n" +
                    "Quando finisce, salva la città come NUOVO salvataggio prima di rimuovere il mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Mostra istruzioni" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Mostra brevi note d’uso sotto." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Pannello città>\n" +
                    "1. Clicca o trascina per selezionare segmenti.\n" +
                    "2. Imposta <Nuova velocità>, poi clicca <Applica>.\n" +
                    "3. <Reset> ripristina i segmenti selezionati.\n" +
                    "4. I pulsanti <50%> applicano subito.\n" +
                    "\n" +
                    "<Tutta la città>\n" +
                    "Scegli un gruppo di strade, poi applica <Nuova velocità> a quel gruppo.\n" +
                    "Usa <Strade>, <Binari>, <Acqua> o <Tutto> per cancellare le velocità personalizzate.\n" +
                    "<Salva città> dopo modifiche globali."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Versione" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "Apre la pagina Paradox Mods dell’autore." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "Report debug nel log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<Non serve per il gioco normale.>\n" +
                    "Scrive un rapporto una sola volta in Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Apri log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Apre <Logs/AllSpeedLimits.log>. Se il file non esiste, apre la cartella Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
