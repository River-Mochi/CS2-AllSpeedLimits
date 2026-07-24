// <copyright file="LocalePT_PT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocalePT_PT.cs
// Purpose: European Portuguese locale Options UI settings.

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

    public sealed class LocalePT_PT : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocalePT_PT(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            string title = $"{Mod.ModName} (Todos os limites de velocidade)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "Ações" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Opções de apresentação" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Repor predefinições do jogo" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Utilização" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Registo" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Unidades de velocidade" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Escolhe as unidades do painel e dos sinais flutuantes.\n" +
                    "<AUTO> segue o tipo de mapa:\n" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "Selecionar <KM/H ou MPH> força essa apresentação."
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Sincronizar cursor com o segmento" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Recomendado: ativado**\n" +
                    "Ativado: clicar num segmento move o cursor para a velocidade atual dele, em vez de começar em 5.\n" +
                    "- Se escolheres vários segmentos, o primeiro continua a definir a posição inicial do cursor.\n" +
                    "Desativado: clicar noutro segmento mantém o último alvo."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Passo do cursor no painel" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "Define o passo do cursor no painel de limites de velocidade da cidade.\n" +
                    "<Predefinição = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Tamanho do texto de ajuda" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "Este mod permite texto maior nas caixas de ajuda ao passar o cursor sobre funções de limite de velocidade.\n" +
                    "<Predefinição 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Mostrar velocidades duplicadas do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Desligado> mostra uma escala mais simples, normalmente mais próxima das marcações da estrada.\n" +
                    "<Ligado> faz o painel e o texto flutuante mostrarem a escala interna mais alta do jogo.\n" +
                    "Útil se outro mod de dicas ou o Scene Explorer mostrar os valores internos duplicados do jogo e quiseres que coincidam.\n" +
                    "**Isto só altera a apresentação;** as velocidades guardadas <não mudam realmente>.\n" +
                    "Se for confuso, deixa Desligado. Os carros movem-se da mesma forma com Ligado ou Desligado.\n" +
                    "Nota: <as marcações da estrada são elementos gráficos> e podem não corresponder aos dados reais de velocidade do modelo do jogo. Um sinal de 35 mph pode representar ~31 mph. O jogo define primeiro as velocidades das estradas no sistema métrico e depois converte-as."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "Repor velocidades predefinidas do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Limpeza opcional antes de remover o mod.\n" +
                    "Usa isto <apenas> se não quiseres manter as velocidades personalizadas deste mod.\n" +
                    "Não é necessário para remover o mod. As velocidades personalizadas podem ficar na cidade sem este mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Este botão repõe as predefinições conhecidas do jogo onde este mod aplicou velocidades personalizadas.\n" +
                    "Quando terminar, guarda num **NOVO FICHEIRO** antes de remover o mod.\n" +
                    "Se removeres o mod sem usar isto, as velocidades personalizadas ficam até mudares estradas, carris ou vias navegáveis."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Isto repõe todos os limites de velocidade personalizados aplicados por este mod para predefinições conhecidas do jogo.\n" +
                    "Isto não pode ser anulado automaticamente.\n" +
                    "Quando terminar, guarda a cidade num NOVO FICHEIRO antes de remover o mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Mostrar instruções" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Mostra notas rápidas abaixo." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<Painel da cidade>\n" +
                    "1. Clica ou arrasta para selecionar segmentos.\n" +
                    "2. Define <Nova velocidade> e clica em <Aplicar>.\n" +
                    "3. <Repor> repõe os segmentos selecionados.\n" +
                    "4. Botões de <Predefinição> aplicam logo.\n" +
                    "\n" +
                    "<Cidade inteira>\n" +
                    "Escolhe um grupo de estradas e aplica <Nova velocidade> a esse grupo.\n" +
                    "Usa <Estradas>, <Carris>, <Água> ou <Tudo> para limpar velocidades personalizadas.\n" +
                    "<Guarda a cidade> após alterações na cidade inteira."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Abre a página do autor no Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "Relatório de debug no registo" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<Não é necessário para jogar normalmente.>\n" +
                    "Escreve um relatório único em Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "Abrir registo" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "Abre <Logs/AllSpeedLimits.log>. Se o ficheiro não existir, abre a pasta Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
