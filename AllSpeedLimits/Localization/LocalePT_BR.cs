// <copyright file="LocalePT_BR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocalePT_BR.cs
// Purpose: Brazilian Portuguese locale Options UI settings.

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

    public sealed class LocalePT_BR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePT_BR(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "Ações" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "Opções de exibição" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "Restaurar padrões do jogo" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "Uso" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "Debug / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "Unidades de velocidade" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "Escolha as unidades do painel e das placas flutuantes.\n" +
                    "<AUTO> segue o tipo do mapa: EU = KM/H, NA = MPH.\n" +
                    "<KM/H> e <MPH> forçam essa exibição." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "Sincronizar controle com o segmento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<Recomendado ativado>\n" +
                    "Ativado: clicar em um segmento move o controle para a velocidade atual do primeiro segmento.\n" +
                    "Desativado: clicar em outro segmento mantém seu último alvo.\n" +
                    "Se selecionar várias partes, o primeiro segmento ainda define o início do controle."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "Passo do controle no painel" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "Define o tamanho do passo no painel da cidade.\n" +
                    "<Padrão = 10>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "Tamanho do texto de ajuda" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Este mod pode aumentar o texto nas caixas de ajuda ao passar o cursor sobre os itens do mod.\n" +
                    "<Padrão 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "Mostrar velocidades dobradas do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Desligado> mostra uma escala mais simples, geralmente mais próxima das marcações da via.\n" +
                    "<Ligado> painel e texto flutuante mostram a escala interna mais alta do jogo.\n" +
                    "Útil se outro mod de dicas mostra os valores internos dobrados do jogo e você quer combinar.\n" +
                    "**Isto muda apenas a exibição;** as velocidades salvas <não mudam de verdade>.\n" +
                    "Se isso confundir, deixe Desligado. Os carros se movem da mesma forma com Ligado ou Desligado.\n" +
                    "Observação: marcações e placas na via são elementos gráficos e podem não corresponder aos dados reais de velocidade do modelo do jogo. Uma placa de 35 mph pode representar 31 mph. O jogo calcula as vias primeiro no sistema métrico e depois converte."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "Restaurar velocidades padrão do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Limpeza opcional antes de remover o mod.\n" +
                    "Use isto <somente> se você não quiser manter as velocidades personalizadas deste mod.\n" +
                    "Não é obrigatório para remover o mod. Velocidades personalizadas das vias podem continuar na cidade sem este mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Isto restaura para padrões conhecidos do jogo as velocidades personalizadas aplicadas por este mod.\n" +
                    "Depois de terminar, faça um **NOVO ARQUIVO SALVO** antes de remover o mod.\n" +
                    "Se remover o mod sem usar isto, as velocidades personalizadas ficam até você mudar as vias etc."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Isto vai restaurar todos os limites de velocidade personalizados suportados para padrões conhecidos do jogo.\n" +
                    "Isso não pode ser desfeito automaticamente.\n" +
                    "Depois de terminar, salve a cidade em um NOVO ARQUIVO antes de remover o mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Mostrar instruções" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Mostra notas rápidas abaixo." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Painel da cidade>\n" +
                    "1. Clique ou arraste para selecionar segmentos.\n" +
                    "2. Defina <Nova velocidade> e clique em <Aplicar>.\n" +
                    "3. <Redefinir> restaura os segmentos selecionados.\n" +
                    "4. Botões de predefinição aplicam na hora.\n" +
                    "\n" +
                    "<Cidade inteira>\n" +
                    "Escolha um grupo de estradas e aplique <Nova velocidade> a esse grupo.\n" +
                    "Use <Vias>, <Trilhos>, <Água> ou <Tudo> para limpar velocidades personalizadas.\n" +
                    "<Salve a cidade> após mudanças gerais."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "Abre a página do autor no Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "Relatório de debug no log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<Não é necessário para jogar normalmente.>\n" +
                    "Grava um relatório único em Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Abre <Logs/AllSpeedLimits.log>. Se o arquivo não existir, abre a pasta Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
