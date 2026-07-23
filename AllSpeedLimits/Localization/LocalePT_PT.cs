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
                    "<AUTO> segue o tipo de mapa: EU = KM/H, NA = MPH.\n" +
                    "<KM/H> e <MPH> forçam essa apresentação." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Sincronizar cursor com o segmento" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "<Recomendado ativado>\n" +
                    "Ativado: clicar num segmento move o cursor para a velocidade atual do primeiro segmento.\n" +
                    "Desativado: clicar noutro segmento mantém o último alvo.\n" +
                    "Se escolheres várias partes, o primeiro segmento continua a definir o início do cursor."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Passo do cursor no painel" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "Define o passo do cursor no painel da cidade.\n" +
                    "<Predefinição = 10>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Tamanho do texto de ajuda" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "Este mod pode aumentar o texto nas caixas de ajuda ao passar o cursor sobre os itens do mod.\n" +
                    "<Predefinição 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Mostrar velocidades duplicadas do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Desligado> mostra uma escala mais simples, normalmente mais próxima das marcações da estrada.\n" +
                    "<Ligado> painel e texto flutuante mostram a escala interna mais alta do jogo.\n" +
                    "Útil se outro mod de dicas mostrar os valores internos duplicados do jogo e quiseres igualar.\n" +
                    "**Isto só altera a apresentação;** as velocidades guardadas <não mudam realmente>.\n" +
                    "Se for confuso, deixa Desligado. Os carros movem-se da mesma forma com Ligado ou Desligado.\n" +
                    "Nota: as marcações e os sinais da estrada são elementos gráficos e podem não corresponder aos dados reais de velocidade do modelo do jogo. Um sinal de 35 mph pode corresponder realmente a 31 mph. O jogo calcula primeiro as estradas no sistema métrico e só depois converte."
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
                    "Não é necessário para remover o mod. As velocidades personalizadas das estradas podem ficar na cidade sem este mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Isto repõe para predefinições conhecidas do jogo as velocidades personalizadas aplicadas por este mod.\n" +
                    "Quando terminar, guarda num **NOVO FICHEIRO** antes de remover o mod.\n" +
                    "Se removeres o mod sem usar isto, as velocidades personalizadas ficam até mudares as estradas, etc."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Isto vai repor todos os limites de velocidade personalizados suportados para predefinições conhecidas do jogo.\n" +
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
                    "4. Botões de predefinição aplicam logo.\n" +
                    "\n" +
                    "<Cidade inteira>\n" +
                    "Escolhe um grupo de estradas e aplica <Nova velocidade> a esse grupo.\n" +
                    "Usa <Estradas>, <Carris>, <Água> ou <Tudo> para limpar velocidades personalizadas.\n" +
                    "<Guarda a cidade> após alterações globais."
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
