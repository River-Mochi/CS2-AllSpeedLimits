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
    using Colossal.PSI.Common;
    using Game.Areas;
    using Game.Citizens;
    using Game.City;
    using Game.Objects;
    using Game.UI;

    public sealed class LocaleFR : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleFR(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            // Version still appears on the About tab through VersionText.
            string title = $"{Mod.ModName} (Toutes les vitesses)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "À propos" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Options d’affichage" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Réinitialiser les défauts du jeu" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Utilisation" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Journal" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Unités de vitesse" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Choisissez les unités du panneau et des indicateurs flottants.\n" +
                    "<AUTO> suit le type de carte :\n" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "Choisir <KM/H ou MPH> force cet affichage."
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Synchroniser le curseur avec le segment" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Activation recommandée**\n" +
                    "Activé : cliquer un segment place le curseur sur sa vitesse actuelle au lieu de commencer à 5.\n" +
                    "- Si plusieurs segments sont sélectionnés, le premier fixe toujours la position initiale du curseur.\n" +
                    "Désactivé : cliquer un autre segment garde votre dernière cible."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Pas du curseur" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "Définit le pas du curseur dans le panneau des limites de vitesse de la ville.\n" +
                    "<Défaut = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Taille du texte d’aide" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "Ce mod permet d’agrandir le texte des bulles d’aide au survol des fonctions de limitation de vitesse.\n" +
                    "<Défaut 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Afficher les vitesses doublées du jeu" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Désactivé> affiche une échelle plus simple, souvent plus proche des marquages routiers.\n" +
                    "<Activé> affiche dans le panneau et le texte flottant l’échelle interne plus élevée du jeu.\n" +
                    "Utile si un autre mod d’infobulles ou Scene Explorer affiche les valeurs internes doublées du jeu et que vous voulez les faire correspondre.\n" +
                    "**Seul l’affichage change ;** les vitesses sauvegardées <ne changent pas réellement>.\n" +
                    "Si c’est confus, gardez Désactivé. Les voitures roulent pareil, que ce soit Activé ou Désactivé.\n" +
                    "Remarque : <les marquages routiers sont décoratifs> et peuvent ne pas correspondre aux données réelles de vitesse des modèles du jeu. Un panneau 35 mph peut représenter ~31 mph. Le jeu définit d’abord les vitesses routières en unités métriques, puis les convertit."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "Restaurer les vitesses par défaut du jeu" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Nettoyage optionnel avant de retirer le mod.\n" +
                    "Utilisez ceci <uniquement> si vous ne voulez pas garder les vitesses personnalisées de ce mod.\n" +
                    "Ce n’est pas obligatoire pour retirer le mod. Les vitesses personnalisées peuvent rester dans la ville sans ce mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Ce bouton rétablit les valeurs par défaut connues du jeu partout où ce mod a appliqué des vitesses personnalisées.\n" +
                    "Une fois terminé, faites une **NOUVELLE SAUVEGARDE** avant de retirer le mod.\n" +
                    "Si vous retirez le mod sans l’utiliser, les vitesses personnalisées restent jusqu’à ce que vous changiez les routes, rails ou voies navigables."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Cela rétablit toutes les limites de vitesse personnalisées appliquées par ce mod aux valeurs par défaut connues du jeu.\n" +
                    "Impossible d’annuler automatiquement.\n" +
                    "Une fois terminé, sauvegardez la ville dans une NOUVELLE sauvegarde avant de retirer le mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Afficher les instructions" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Affiche de courtes notes d’aide ci-dessous." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<Panneau de ville>\n" +
                    "1. Cliquez ou glissez pour sélectionner des segments.\n" +
                    "2. Réglez <Nouvelle vitesse>, puis cliquez sur <Appliquer>.\n" +
                    "3. <Réinit.> restaure les segments sélectionnés.\n" +
                    "4. Les boutons <Préréglages> s’appliquent instantanément.\n" +
                    "\n" +
                    "<Ville entière>\n" +
                    "Choisissez un groupe de routes, puis appliquez <Nouvelle vitesse> à ce groupe.\n" +
                    "Utilisez <Routes>, <Rails>, <Eau> ou <Tout> pour effacer les vitesses personnalisées.\n" +
                    "<Sauvegardez la ville> après les changements dans toute la ville."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Ouvre la page Paradox Mods de l’auteur." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "Rapport debug vers le journal" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<Inutile pour jouer normalement.>\n" +
                    "Écrit un rapport unique dans Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "Ouvrir le journal" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "Ouvre <Logs/AllSpeedLimits.log>. Ouvre le dossier Logs si le fichier n’existe pas." },
            };
        }

        public void Unload()
        {
        }
    }
}
