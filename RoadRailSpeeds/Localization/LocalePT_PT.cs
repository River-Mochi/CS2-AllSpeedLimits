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
    using Game.Areas;
    using Game.Citizens;
    using Game.City;
    using Game.Objects;
    using Game.UI;

    public sealed class LocalePT_PT : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePT_PT(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title intentionally omits version.
            // Version still appears on the About tab through VersionText.
            string title = Mod.ModName;

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "Ações" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "Opções de apresentação" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "Repor predefinições do jogo" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "Utilização" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "Debug / Registo" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "Unidades de velocidade" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "Escolhe as unidades do painel e dos sinais flutuantes.\n" +
                    "<AUTO> segue o tipo de mapa: EU = KM/H, NA = MPH.\n" +
                    "<KM/H> e <MPH> forçam essa apresentação." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "Sincronizar cursor com o segmento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<Recomendado ativado>\n" +
                    "Ativado: clicar num segmento move o cursor para a velocidade atual do primeiro segmento.\n" +
                    "Desativado: clicar noutro segmento mantém o último alvo.\n" +
                    "Se escolheres várias partes, o primeiro segmento continua a definir o início do cursor."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "Passo do cursor no painel" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "Define o passo do cursor no painel da cidade.\n" +
                    "<Predefinição = 5>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "Tamanho do texto de ajuda" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Aumenta os popups do mod e o texto de ajuda.\n" +
                    "<Predefinição 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "Mostrar velocidades duplicadas do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Desligado> mostra uma escala mais simples, normalmente mais próxima das marcações da estrada.\\n" +
                    "<Ligado> mostra no painel e no texto flutuante a escala interna mais alta do jogo.\\n" +
                    "Útil se outro mod de dicas mostrar valores internos duplicados e quiseres igualar.\\n" +
                    "Isto é apenas visual; as velocidades guardadas <não mudam realmente>.\\n" +
                    "As marcações da estrada são arte e podem não corresponder exatamente aos dados do prefab.\\n" +
                    "Se for confuso, deixa Desligado. Os carros parecem iguais em movimento com Ligado ou Desligado." },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "Limpar velocidades personalizadas" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Restaura estradas, carris e vias navegáveis compatíveis nesta cidade para os <padrões do jogo>.\\n" +
                    "<Guarda depois para manter a reposição.>\\n" +
                    "- Útil antes de remover o mod se não quiseres as velocidades personalizadas.\\n" +
                    "- Se removeres o mod sem limpar, as velocidades guardadas normalmente ficam, mas o suporte de repor/reaplicar desaparece." },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Limpar todas as velocidades personalizadas dos segmentos suportados de estrada, carril e água nesta cidade?\n" +
                    "Isto não pode ser anulado."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Mostrar instruções" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Mostra notas rápidas abaixo." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Painel da cidade>\n" +
                    "1. Clica ou arrasta para selecionar segmentos.\n" +
                    "2. Define <Nova velocidade> e clica em <Aplicar>.\n" +
                    "3. <Reset> repõe os segmentos selecionados.\n" +
                    "4. Os botões <50%> aplicam logo.\n" +
                    "\n" +
                    "<Cidade inteira>\n" +
                    "Escolhe um grupo de estradas e aplica <Nova velocidade> a esse grupo.\n" +
                    "Usa <Estradas>, <Carris>, <Água> ou <Tudo> para limpar velocidades personalizadas.\n" +
                    "<Guarda a cidade> após alterações globais."
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "Relatório de debug no registo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<Não é necessário para jogar normalmente.>\\n" +
                    "Escreve um relatório único em Logs/AdjustSpeedLimits.log." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Abrir registo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)), "Abre <Logs/AdjustSpeedLimits.log>. Se o ficheiro não existir, abre a pasta Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
