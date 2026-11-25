// <copyright file="LocaleES.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleES.cs
// Purpose: Spanish locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "Bicicletas" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "Acerca de" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "Opciones de pantalla" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "Atajos de teclado" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "Bicicletas y patinetes eléctricos" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "Estado de vehículos personales" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "Restaurar valores del juego" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "Uso" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "Depuración / Log" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Atajo de redes invisibles" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "Haz clic en «sin asignar» y elige una tecla. Funciona también con ASL cerrado." },

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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "Mostrar ayudas de ASL" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)),
                    "**Predeterminado = ACTIVADO**  Se recomienda dejarlo activado.\n\n" +
                    "Muestra ayudas en la ciudad. El botón de información puede reactivarlas." },

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
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "Velocidad máxima del vehículo" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)),
                    "**100% = valores del juego.**\n" +
                    "Cambia la velocidad máxima de bicicletas y patinetes eléctricos. La aceleración y el frenado se ajustan automáticamente.\n" +
                    "Los límites de las vías siguen vigentes.\n" +
                    "Una versión antigua de Bikes + Paths controla la velocidad en su lugar. Desactívala o usa su versión compatible con ASL." },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Restaurar valores del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "Restaura velocidad, aceleración y frenado de bicicletas y patinetes al **100% del juego**." },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "Grupo bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "Bicis y scooters eléctricos.\n" +
                    "**Activo** = tiene un carril actual (en movimiento).\n" +
                    "**Total aparcado** = incluye todos los estados Parked de cualquier lugar, no solo aparcamientos."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "Grupo coche" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "Solo coches personales (excluye el grupo bici).\n" +
                    "**Activo** = tiene un carril actual (en movimiento).\n" +
                    "**Aparcado** = todos los **ParkedCar**, no solo aparcamientos.\n" +
                    "El escaneo solo se ejecuta con el menú Opciones abierto, no durante el juego normal en la ciudad, así que no afecta a los FPS."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "Coches aparcados ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**Total en borde OC** = vehículos del grupo coche con ParkedCar en la conexión Outside City (OC).\n" +
                    "Algunas ciudades muestran muchos coches atrapados aparcados en la conexión Outside City.\n" +
                    "Usa <Log hidden cars> para ver un desglose de ejemplo de coches ocultos."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "Registrar coches ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "Escribe un informe único en **Logs/AllSpeedLimits.log**.\n" +
                    "Incluye total + desglose Bucket A/B/C y números ID de ejemplo.\n" +
                    "Usa el mod Scene Explorer para saltar a los ID de entidad Vehicle listados e investigar."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "Abre **Logs/AllSpeedLimits.log** si existe.\n" +
                    "Si el archivo aún no existe, abre la carpeta Logs."
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "Estado no cargado." },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "Sin ciudad... ¯\\_(ツ)_/¯ ...Sin estadísticas" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "ejecuta la ciudad unos minutos para obtener datos." },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} activos | {1} bicis | {2} scooters | {3} / {4} aparcados/total" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} activos | {1} aparcados | {2} total | {3} remolques" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} ocultos en borde OC | actualizado {1}" },

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
                    "▪ Haz clic en un tramo o arrastra para elegir varios. Ajusta <Nueva velocidad> y pulsa <Aplicar>.\n" +
                    "▪ Los <preajustes> (números en círculos) se aplican al instante, sin Aplicar.\n" +
                    "▪ <Restablecer> restaura los tramos seleccionados.\n" +
                    "▪ <Invisible>: botón discontinuo muestra/oculta calles y caminos ocultos.\n" +
                    "<Toda la ciudad>\n" +
                    "▪ Elige un grupo de carreteras y aplica la nueva velocidad.\n" +
                    "▪ <Botones rojos>: carreteras, vías, agua, caminos o Todo eliminan sus límites personalizados.\n" +
                    "▪ <Caminos>: controles de bici y peatones para toda la ciudad. 100% = valor del juego; 200% duplica el límite.\n" +
                    "▪ <Tramos ciclistas personalizados> conservan su velocidad. Las vías solo peatonales se ajustan en toda la ciudad.\n" +
                    "▪ <Guarda la ciudad> para conservar los cambios al cargar."
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
