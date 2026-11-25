// <copyright file="LocalePT_BR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocalePT_BR.cs
// Purpose: Brazilian Portuguese locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocalePT_BR : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocalePT_BR(SpeedLimitsSetting setting)
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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "Bicicletas" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Opções de exibição" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "Teclas de atalho" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "Bicicletas e patinetes elétricos" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "Status dos veículos pessoais" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Restaurar padrões do jogo" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Uso" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Log" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Atalho de redes invisíveis" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Clique em 'não definido' e escolha uma tecla. Funciona mesmo com o ASL fechado." },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Unidades de velocidade" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Escolha as unidades do painel e das placas flutuantes.\n" +
                    "<AUTO> segue o tipo do mapa:\n" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "Selecionar <KM/H ou MPH> força essa exibição."
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Sincronizar controle com o segmento" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Recomendado: ativado**\n" +
                    "Ativado: clicar em um segmento move o controle para a velocidade atual dele, em vez de começar em 5.\n" +
                    "- Se selecionar vários segmentos, o primeiro ainda define a posição inicial do controle.\n" +
                    "Desativado: clicar em outro segmento mantém seu último alvo."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Passo do controle no painel" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "Define o tamanho do passo no painel de limite de velocidade da cidade.\n" +
                    "<Padrão = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Tamanho do texto de ajuda" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "Este mod permite texto maior nas caixas de ajuda ao passar o cursor sobre recursos de limite de velocidade.\n" +
                    "<Padrão 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "Mostrar dicas do ASL" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "Mostra dicas na cidade. O botão de informações pode reativá-las." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Mostrar velocidades dobradas do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Desligado> mostra uma escala mais simples, geralmente mais próxima das marcações da via.\n" +
                    "<Ligado> faz o painel e o texto flutuante mostrarem a escala interna mais alta do jogo.\n" +
                    "Útil se outro mod de dicas ou o Scene Explorer mostrar os valores internos dobrados do jogo e você quiser que coincidam.\n" +
                    "**Isto muda apenas a exibição;** as velocidades salvas <não mudam de verdade>.\n" +
                    "Se isso confundir, deixe Desligado. Os carros se movem da mesma forma com Ligado ou Desligado.\n" +
                    "Observação: <as marcações da via são elementos gráficos> e podem não corresponder aos dados reais de velocidade do modelo do jogo. Uma placa de 35 mph pode representar ~31 mph. O jogo define primeiro as velocidades das vias no sistema métrico e depois as converte."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "Velocidade máxima dos veículos" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "100% = padrão do jogo. Aceleração e frenagem se ajustam automaticamente. Limites dos caminhos continuam valendo. Atualize Bikes + Paths se usar." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Restaurar velocidade" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Restaura bicicletas e patinetes elétricos para 100%." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "Grupo das bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "Bicicletas e scooters elétricos.\n" +
                    "**Ativo** = tem uma faixa atual (em movimento).\n" +
                    "**Total estacionado** = inclui todos os flags Parked de qualquer lugar, não só estacionamentos."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "Grupo dos carros" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "Somente carros pessoais (exclui o grupo das bikes).\n" +
                    "**Ativo** = tem uma faixa atual (em movimento).\n" +
                    "**Estacionado** = todos os **ParkedCar**, não só estacionamentos.\n" +
                    "A varredura só roda enquanto o menu Opções está aberto, não durante o gameplay da cidade, então sem preocupação com FPS."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "Carros estacionados ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**Total na borda OC** = veículos do grupo de carros com ParkedCar na conexão Outside City (OC).\n" +
                    "Algumas cidades mostram muitos carros presos estacionados na conexão Outside City.\n" +
                    "Use <Log hidden cars> para um exemplo de detalhamento dos carros ocultos."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "Registrar carros ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "Escreve um relatório único em **Logs/AllSpeedLimits.log**.\n" +
                    "Inclui total + detalhamento Bucket A/B/C e números de ID de exemplo.\n" +
                    "Use o mod Scene Explorer para ir aos IDs de entidade Vehicle listados e investigar."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "Abre **Logs/AllSpeedLimits.log** se existir.\n" +
                    "Se o arquivo ainda não existir, abre a pasta Logs."
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "Status não carregado." },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "Sem cidade... ¯\\_(ツ)_/¯ ...Sem estatísticas" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "rode a cidade por alguns minutos para obter dados." },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} ativos | {1} bikes | {2} scooters | {3} / {4} estacionados/total" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} ativos | {1} estacionados | {2} total | {3} trailers" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} ocultos na borda OC | atualizado {1}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "Restaurar velocidades padrão do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Limpeza opcional antes de remover o mod.\n" +
                    "Use isto <somente> se você não quiser manter as velocidades personalizadas deste mod.\n" +
                    "Não é obrigatório para remover o mod. Velocidades personalizadas podem continuar na cidade sem este mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Este botão restaura os padrões conhecidos do jogo onde este mod aplicou velocidades personalizadas.\n" +
                    "Depois de terminar, faça um **NOVO SALVAMENTO** antes de remover o mod.\n" +
                    "Se remover o mod sem usar isto, as velocidades personalizadas ficam até você mudar estradas, trilhos ou hidrovias."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Isto restaura todos os limites de velocidade personalizados aplicados por este mod aos padrões conhecidos do jogo.\n" +
                    "Isso não pode ser desfeito automaticamente.\n" +
                    "Depois de terminar, salve a cidade em um NOVO ARQUIVO antes de remover o mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Mostrar instruções" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Mostra notas rápidas abaixo." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<Painel da cidade>\n" +
                    "▪ Clique em um trecho ou arraste para selecionar vários. Defina <Nova velocidade> e clique em <Aplicar>.\n" +
                    "▪ <Predefinições> (números em círculos) são imediatas, sem Aplicar.\n" +
                    "▪ <Restaurar> restaura os trechos selecionados.\n" +
                    "▪ <Invisível>: botão tracejado mostra/oculta vias e caminhos ocultos.\n" +
                    "<Cidade inteira>\n" +
                    "▪ Escolha um grupo de vias e aplique a nova velocidade.\n" +
                    "▪ <Botões vermelhos>: vias, trilhos, água, caminhos ou Todos limpam os ajustes correspondentes.\n" +
                    "▪ <Caminhos>: controles de bicicleta e pedestres para toda a cidade. 100% = padrão do jogo; 200% dobra o limite.\n" +
                    "▪ <Ajustes individuais> nas ciclovias têm prioridade. Caminhos só de pedestres são globais.\n" +
                    "▪ <Salve a cidade> para manter as alterações ao carregar."
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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "Relatório de debug no log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<Não é necessário para jogar normalmente.>\n" +
                    "Grava um relatório único em Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "Abre <Logs/AllSpeedLimits.log>. Se o arquivo não existir, abre a pasta Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
