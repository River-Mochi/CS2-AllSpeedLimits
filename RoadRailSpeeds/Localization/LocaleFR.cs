// <copyright file="LocaleFR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleFR.cs
// Purpose: French locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;
    using Game.Areas;
    using Game.Citizens;
    using Game.City;
    using Game.Objects;
    using Game.UI;

    public sealed class LocaleFR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleFR(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title intentionally omits version.
            // Version still appears on the About tab through VersionText.
            string title = "Adjust Speed Limits (Vitesse)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "À propos" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "Options d’affichage" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "Réinitialiser les défauts du jeu" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "Utilisation" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "Debug / Journal" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "Unités de vitesse" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "Choisir les unités du panneau et des panneaux flottants.\n" +
                    "<AUTO> suit le type de carte : EU = KM/H, NA = MPH.\n" +
                    "<KM/H> et <MPH> forcent cet affichage." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "Synchroniser le curseur avec le segment" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<Activé recommandé>\n" +
                    "Activé : cliquer un segment place le curseur sur la vitesse actuelle du premier segment.\n" +
                    "Désactivé : cliquer un autre segment garde votre dernière cible.\n" +
                    "Si plusieurs segments sont sélectionnés, le premier fixe quand même le départ du curseur."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "Pas du curseur" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "Définit le pas du curseur dans le panneau de ville.\n" +
                    "<Défaut = 5>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "Taille du texte d’aide" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Agrandit les popups du mod et le texte d’aide.\n" +
                    "<Défaut 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "Afficher les vitesses doublées du jeu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Off> affiche une échelle plus simple, souvent plus proche des marquages routiers.\\n" +
                    "<On> affiche dans le panneau et le texte flottant l’échelle interne plus élevée du jeu.\\n" +
                    "Utile si un autre mod de tooltip affiche ces valeurs internes doublées et que vous voulez les faire correspondre.\\n" +
                    "C’est seulement visuel ; les vitesses sauvegardées <ne changent pas vraiment>.\\n" +
                    "Les marquages routiers sont décoratifs et peuvent ne pas correspondre aux données du prefab.\\n" +
                    "Si c’est confus, gardez Off. Les voitures auront le même aspect en mouvement, que ce soit On ou Off." },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "Effacer les vitesses personnalisées" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Rétablit les routes, rails et voies d’eau compatibles de cette ville aux <valeurs du jeu>.\\n" +
                    "<Sauvegardez ensuite pour garder la réinitialisation.>\\n" +
                    "- Utile avant de retirer le mod si vous ne voulez pas garder les vitesses personnalisées.\\n" +
                    "- Si vous retirez le mod sans effacer, les vitesses sauvegardées restent souvent, mais le reset/réappliquer n’est plus disponible." },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Effacer toutes les vitesses personnalisées des segments route, rail et eau pris en charge dans cette ville ?\n" +
                    "Impossible d’annuler."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Afficher les instructions" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Affiche de courtes notes d’aide ci-dessous." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Panneau de ville>\n" +
                    "1. Cliquez ou glissez pour sélectionner des segments.\n" +
                    "2. Réglez <Nouvelle vitesse>, puis cliquez <Appliquer>.\n" +
                    "3. <Réinit.> restaure les segments sélectionnés.\n" +
                    "4. Les boutons <50%> s’appliquent tout de suite.\n" +
                    "\n" +
                    "<Ville entière>\n" +
                    "Choisissez un groupe de routes, puis appliquez <Nouvelle vitesse> à ce groupe.\n" +
                    "Utilisez <Routes>, <Rails>, <Eau> ou <Tout> pour effacer les vitesses personnalisées.\n" +
                    "<Sauvegardez la ville> après les changements globaux."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "Ouvre la page Paradox Mods de l’auteur." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "Rapport debug vers le journal" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<Inutile pour jouer normalement.>\\n" +
                    "Écrit un rapport unique dans Logs/AdjustSpeedLimits.log." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Ouvrir le journal" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)), "Ouvre <Logs/AdjustSpeedLimits.log>. Ouvre le dossier Logs si le fichier n’existe pas." },
            };
        }

        public void Unload()
        {
        }
    }
}
