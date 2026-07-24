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
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleES(SpeedLimitsSetting setting)
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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "Acciones" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Acerca de" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Opciones de pantalla" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Restaurar valores del juego" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Uso" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Depuración / Log" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "Unidades de velocidad" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "Elige las unidades del panel y los letreros flotantes.\n" +
                    "<AUTO> sigue el tipo de mapa:\n" +
                    "- EU = KM/H, NA = MPH.\n" +
                    "Elegir <KM/H o MPH> fuerza esa vista."
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "Sincronizar deslizador con el segmento" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] Se recomienda activarlo**\n" +
                    "Activado: al hacer clic en un segmento, el deslizador usa su velocidad actual en vez de empezar en 5.\n" +
                    "- Si seleccionas varios segmentos, el primero sigue fijando la posición inicial del deslizador.\n" +
                    "Desactivado: al elegir otro segmento, se mantiene tu último objetivo."
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "Paso del deslizador" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "Define el tamaño de paso del deslizador en el panel de límites de velocidad de la ciudad.\n" +
                    "<Predeterminado = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "Tamaño del texto de ayuda" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "Este mod permite texto más grande en los cuadros de ayuda al pasar el cursor sobre las funciones de límite de velocidad.\n" +
                    "<Predeterminado 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "Mostrar velocidades dobles del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<Desactivado> muestra una escala más simple, normalmente más cercana a las marcas de carretera.\n" +
                    "<Activado> muestra en el panel y el texto flotante la escala interna más alta del juego.\n" +
                    "Útil si otro mod de ayuda o Scene Explorer muestra los valores internos duplicados del juego y quieres que coincidan.\n" +
                    "**Esto solo cambia la visualización;** las velocidades guardadas <no cambian realmente>.\n" +
                    "Si te confunde, déjalo desactivado. Los coches se mueven igual esté activado o no.\n" +
                    "Nota: <las marcas de carretera son elementos gráficos> y pueden no coincidir con los datos reales de velocidad de la plantilla del juego. Una señal de 35 mph puede representar ~31 mph. El juego define primero las velocidades de las carreteras en el sistema métrico y luego las convierte."
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "Restaurar velocidades predeterminadas del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Limpieza opcional antes de quitar el mod.\n" +
                    "Usa esto <solo> si no quieres conservar las velocidades personalizadas de este mod.\n" +
                    "No es obligatorio para quitar el mod. Las velocidades personalizadas pueden quedarse en la ciudad sin este mod.\n" +
                    "<============>\n" +
                    "\n" +
                    "Este botón restaura los valores conocidos del juego donde este mod aplicó velocidades personalizadas.\n" +
                    "Cuando termine, haz un **NUEVO GUARDADO** antes de quitar el mod.\n" +
                    "Si quitas el mod sin usar esto, las velocidades personalizadas quedan hasta que cambies carreteras, vías férreas o vías navegables."
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "Esto restaura todos los límites de velocidad personalizados aplicados por este mod a valores conocidos del juego.\n" +
                    "No se puede deshacer automáticamente.\n" +
                    "Cuando termine, guarda la ciudad como un NUEVO guardado antes de quitar el mod."
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Mostrar instrucciones" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "Muestra notas rápidas debajo." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<Panel de ciudad>\n" +
                    "1. Haz clic o arrastra para seleccionar segmentos.\n" +
                    "2. Ajusta <Nueva velocidad> y pulsa <Aplicar>.\n" +
                    "3. <Restablecer> restaura los segmentos elegidos.\n" +
                    "4. Los botones <Preajustes> se aplican al instante.\n" +
                    "\n" +
                    "<Toda la ciudad>\n" +
                    "Elige un grupo de carreteras y aplica <Nueva velocidad> a ese grupo.\n" +
                    "Usa <Carreteras>, <Vías>, <Agua> o <Todo> para borrar velocidades personalizadas.\n" +
                    "<Guarda la ciudad> tras cambios en toda la ciudad."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Abre la página de Paradox Mods del autor." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "Informe de depuración al log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<No hace falta para jugar normalmente.>\n" +
                    "Escribe un informe único en Logs/AllSpeedLimits.log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "Abre <Logs/AllSpeedLimits.log>. Si el archivo no existe, abre la carpeta Logs." },
            };
        }

        public void Unload()
        {
        }
    }
}
