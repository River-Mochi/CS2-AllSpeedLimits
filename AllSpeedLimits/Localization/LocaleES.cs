// <copyright file="LocaleES.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleES.cs
// Purpose: Spanish locale Options UI settings.

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

    public sealed class LocaleES : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleES(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            // Version still appears on the About tab through VersionText.
            string title = $"{Mod.ModName} (Todos los límites)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(Setting.kMainTab), "Acciones" },
                { m_Setting.GetOptionTabLocaleID(Setting.kAboutTab), "Acerca de" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kDisplayGroup), "Opciones de pantalla" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kResetGroup), "Restaurar valores del juego" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kUsageGroup), "Uso" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAboutDebugGroup), "Depuración / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedUnitPreference)), "Unidades de velocidad" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedUnitPreference)),
                    "Elige las unidades del panel y los letreros flotantes.\n" +
                    "<AUTO> sigue el tipo de mapa: EU = KM/H, NA = MPH.\n" +
                    "<KM/H> y <MPH> fuerzan esa vista." },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SyncSliderWithSelection)), "Sincronizar deslizador con el segmento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SyncSliderWithSelection)),
                    "<Se recomienda activarlo>\n" +
                    "Activado: al hacer clic en un segmento, el deslizador usa la velocidad actual del primer segmento seleccionado.\n" +
                    "Desactivado: al elegir otro segmento, se mantiene tu último objetivo.\n" +
                    "Si seleccionas varias partes, el primer segmento sigue fijando el inicio del deslizador."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PanelSliderIncrement)), "Paso del deslizador" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PanelSliderIncrement)),
                    "Define el tamaño de paso en el panel de ciudad.\n" +
                    "<Predeterminado = 5>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TooltipFontScale)), "Tamaño del texto de ayuda" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TooltipFontScale)),
                    "Este mod puede mostrar texto más grande en las ayudas al pasar el cursor sobre elementos del mod.\n" +
                    "<Predeterminado 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoubleSpeedDisplay)), "Mostrar velocidades dobles del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoubleSpeedDisplay)),
                    "<Desactivado> muestra una escala más simple, normalmente más cercana a las marcas de carretera.\n" +
                    "<Activado> muestra en el panel y el texto flotante la escala interna más alta del juego.\n" +
                    "Útil si otro mod de ayuda muestra los valores internos duplicados del juego y quieres que coincidan.\n" +
                    "**Esto solo cambia la visualización;** las velocidades guardadas <no cambian realmente>.\n" +
                    "Si te confunde, déjalo desactivado. Los coches se mueven igual esté activado o no.\n" +
                    "Nota: las marcas y señales de carretera son elementos gráficos y pueden no coincidir con los datos reales de velocidad de la plantilla del juego. Una señal de 35 mph puede corresponder en realidad a 31 mph. El juego calcula primero las carreteras en unidades métricas y luego convierte."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(Setting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ClearAllCustomSpeeds)), "Restaurar velocidades predeterminadas del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Limpieza opcional antes de quitar el mod.\n" +
                    "Usa esto <solo> si no quieres conservar las velocidades personalizadas de este mod.\n" +
                    "No es obligatorio para quitar el mod. Las velocidades personalizadas de carretera pueden quedarse en la ciudad sin este mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Esto restaura a valores conocidos del juego las velocidades personalizadas aplicadas por este mod.\n" +
                    "Cuando termine, haz un **NUEVO GUARDADO** antes de quitar el mod.\n" +
                    "Si quitas el mod sin usar esto, las velocidades personalizadas quedan hasta que cambies las carreteras, etc."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ClearAllCustomSpeeds)),
                    "Esto restaurará todos los límites de velocidad personalizados compatibles a valores conocidos del juego.\n" +
                    "No se puede deshacer automáticamente.\n" +
                    "Cuando termine, guarda la ciudad como NUEVO guardado antes de quitar el mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ShowUsage)), "Mostrar instrucciones" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ShowUsage)), "Muestra notas rápidas debajo." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UsageText)),
                    "<Panel de ciudad>\n" +
                    "1. Haz clic o arrastra para seleccionar segmentos.\n" +
                    "2. Ajusta <Nueva velocidad> y pulsa <Aplicar>.\n" +
                    "3. <Restablecer> restaura los segmentos elegidos.\n" +
                    "4. Los preajustes se aplican al instante.\n" +
                    "\n" +
                    "<Toda la ciudad>\n" +
                    "Elige un grupo de carreteras y aplica <Nueva velocidad> a ese grupo.\n" +
                    "Usa <Carreteras>, <Vías>, <Agua> o <Todo> para borrar velocidades personalizadas.\n" +
                    "<Guarda la ciudad> tras cambios globales."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionText)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadox)), "Abre la página de Paradox Mods del autor." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DebugReportToLog)), "Informe de depuración al log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DebugReportToLog)),
                    "<No hace falta para jugar normalmente.>\n" +
                    "Escribe un informe único en Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLog)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLog)),
                    "Abre <Logs/AllSpeedLimits.log>. Si el archivo no existe, abre la carpeta Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
