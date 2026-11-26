// <copyright file="LocaleFR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleFR.cs
// Purpose: French locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "Vélos" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "À propos" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Options d’affichage" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "Raccourcis clavier" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "Vélos et trottinettes électriques" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "État des véhicules personnels" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Réinitialiser les défauts du jeu" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Utilisation" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Debug / Journal" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Raccourci des réseaux invisibles" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Cliquez sur « non défini » puis choisissez une touche. Fonctionne même sans ouvrir ASL." },

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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "Afficher les infobulles ASL" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)),
                    "**Par défaut = ACTIVÉ**  Il est vivement conseillé de le laisser activé.\n\n" +
                    "Affiche les infobulles en ville. Le bouton info peut les réactiver." },

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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.EnableBicycleSpeedTuning)), "Régler la vitesse des vélos et trottinettes" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.EnableBicycleSpeedTuning)),
                    "**OFF** = ASL ne modifie pas ces vitesses.\n" +
                    "**ON** = Règle vitesse maximale, accélération et freinage.\n" +
                    "Laisse désactivé si un autre mod gère ces vitesses." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "Vitesse maximale des véhicules" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)),
                    "**100% = valeurs du jeu.**\n" +
                    "Modifie la vitesse maximale des vélos et trottinettes électriques. Accélération et freinage ajustés automatiquement.\n" +
                    "Les limites des chemins restent applicables.\n" +
                    "Retire Bikes + Paths si tu utilises ASL. ASL reprend toutes ses fonctions. Les anciennes versions peuvent causer des conflits." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Rétablir les valeurs du jeu" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Rétablit vitesse, accélération et freinage des vélos et trottinettes à **100% des valeurs du jeu**." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "Groupe vélo" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "Vélos et scooters électriques.\n" +
                    "**Actif** = a une voie actuelle (en mouvement).\n" +
                    "**Total stationné** = inclut tous les états Parked, partout, pas seulement dans les parkings."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "Groupe voiture" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "Voitures personnelles seulement (hors groupe vélo).\n" +
                    "**Actif** = a une voie actuelle (en mouvement).\n" +
                    "**Stationné** = tous les **ParkedCar**, pas seulement les parkings.\n" +
                    "Le scan ne tourne que lorsque le menu Options est ouvert, pas pendant le gameplay en ville, donc pas de souci de FPS."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "Voitures stationnées cachées" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**Total à la frontière OC** = véhicules du groupe voiture avec ParkedCar à la connexion Outside City (OC).\n" +
                    "Certaines villes ont beaucoup de voitures bloquées en stationnement à la connexion Outside City.\n" +
                    "Utilisez <Log hidden cars> pour un exemple de détail des voitures cachées."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "Journaliser les voitures cachées" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "Écrit un rapport unique dans **Logs/AllSpeedLimits.log**.\n" +
                    "Inclut le total + le détail Bucket A/B/C et des exemples d’ID.\n" +
                    "Utilisez le mod Scene Explorer pour aller aux ID d’entité Vehicle listés et enquêter."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "Ouvrir le journal" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "Ouvre **Logs/AllSpeedLimits.log** s’il existe.\n" +
                    "Si le fichier n’existe pas encore, ouvre le dossier Logs à la place."
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "État non chargé." },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "Pas de ville... ¯\\_(ツ)_/¯ ...Pas de stats" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "faites tourner la ville quelques minutes pour avoir des données." },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} actifs | {1} vélos | {2} scooters | {3} / {4} stationnés/total" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} actifs | {1} stationnés | {2} total | {3} remorques" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} cachés à la frontière OC | mis à jour {1}" },

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
                    "<Panneau en ville>\n" +
                    "▪ Cliquez sur un segment ou glissez pour en sélectionner plusieurs. Réglez <Nouvelle vitesse>, puis <Appliquer>.\n" +
                    "▪ Les <préréglages> (nombres cerclés) s'appliquent aussitôt, sans Appliquer.\n" +
                    "▪ <Réinitialiser> restaure les segments sélectionnés.\n" +
                    "▪ <Invisible> : le bouton pointillé affiche/masque les routes et chemins cachés.\n" +
                    "<Toute la ville>\n" +
                    "▪ Choisissez un groupe de routes et appliquez la nouvelle vitesse.\n" +
                    "▪ <Boutons rouges> : routes, rails, eau, chemins ou Tout effacent les vitesses personnalisées correspondantes.\n" +
                    "▪ <Chemins> : réglages vélo et piéton pour toute la ville. 100% = jeu par défaut ; 200% double la limite.\n" +
                    "▪ Les <pistes cyclables personnalisées> gardent leur vitesse. Les chemins piétons seuls se règlent à l'échelle de la ville.\n" +
                    "▪ <Sauvegardez la ville> pour conserver les changements au chargement."
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
