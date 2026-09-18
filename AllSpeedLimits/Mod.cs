// <copyright file="Mod.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Mod.cs
// Purpose: Mod entrypoint; registers settings, localization, systems, and the popup-safe mod log.

namespace RoadRailSpeeds
{
    using System;                    // Exception
    using System.Reflection;         // Assembly
    using Colossal.IO.AssetDatabase; // AssetDatabase
    using Colossal.Localization;     // LocalizationManager
    using Colossal.Logging;          // ILog, LogManager
    using CS2Shared.RiverMochi;      // LogUtils, ShellOpen
    using Game;                      // UpdateSystem, SystemUpdatePhase
    using Game.Modding;              // IMod
    using Game.Pathfind;             // LaneDataSystem
    using Game.SceneFlow;            // GameManager
    using RoadRailSpeeds.Systems;    // Mod systems
    using Unity.Entities;            // World

    public sealed class Mod : IMod
    {
        public const string ModName = "All Speed Limits";
        public const string ModId = "RoadRailSpeeds";
        public const string LogFileName = "AllSpeedLimits";
        public const string ModTag = "[ASL]";

        public static readonly string ModVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.1.0";

        private static bool s_BannerLogged;

        public static readonly ILog s_Log =
            LogManager.GetLogger(LogFileName).SetShowsErrorsInUI(false);

        public static SpeedLimitsSetting? Settings { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            // Configure file logging first so startup failures still leave a useful mod log.
            ShellOpen.Configure(s_Log, LogFileName, ModTag);

            if (!s_BannerLogged)
            {
                s_BannerLogged = true;

#if DEBUG
                LogUtils.Info($"{ModName} v{ModVersion} DEBUG loaded");
#else
                LogUtils.Info($"{ModName} v{ModVersion} loaded");
#endif
            }

            GameManager? gameManager = GameManager.instance;
            if (gameManager == null)
            {
                LogUtils.Warn($"{ModTag} GameManager.instance is null; {ModName} cannot initialize.");
                return;
            }

            SpeedLimitsSetting setting = new SpeedLimitsSetting(this);
            Settings = setting;

            try
            {
                LocalizationManager? localizationManager = gameManager.localizationManager;
                if (localizationManager == null)
                {
                    LogUtils.Warn($"{ModTag} LocalizationManager is null; locale sources were not registered.");
                }
                else
                {
                    // Load translations first so the Options screen never shows raw localization keys.
                    localizationManager.AddSource("en-US", new LocaleEN(setting));
                    localizationManager.AddSource("fr-FR", new LocaleFR(setting));
                    localizationManager.AddSource("es-ES", new LocaleES(setting));
                    localizationManager.AddSource("de-DE", new LocaleDE(setting));
                    localizationManager.AddSource("it-IT", new LocaleIT(setting));
                    localizationManager.AddSource("ja-JP", new LocaleJA(setting));
                    localizationManager.AddSource("ko-KR", new LocaleKO(setting));
                    localizationManager.AddSource("pl-PL", new LocalePL(setting));
                    localizationManager.AddSource("pt-BR", new LocalePT_BR(setting));
                    localizationManager.AddSource("zh-HANS", new LocaleZH_HANS(setting));
                    localizationManager.AddSource("zh-HANT", new LocaleZH_HANT(setting));
                    localizationManager.AddSource("th-TH", new LocaleTH(setting));
                    localizationManager.AddSource("vi-VN", new LocaleVI(setting));
                    localizationManager.AddSource("tr-TR", new LocaleTR(setting));
                    localizationManager.AddSource("pt-PT", new LocalePT_PT(setting));

                    InCityLocalization.LoadEmbeddedJsonTranslations(ModId, ModTag, s_Log);
                }
            }
            catch (Exception ex)
            {
                LogUtils.Warn($"{ModTag} Localization registration failed: {ex.GetType().Name}: {ex.Message}", ex);
            }

            try
            {
                AssetDatabase.global.LoadSettings(ModId, setting, new SpeedLimitsSetting(this));
            }
            catch (Exception ex)
            {
                LogUtils.Error($"{ModTag} Settings load failed: {ex.GetType().Name}: {ex.Message}", ex);
            }

            try
            {
                setting.RegisterInOptionsUI();
            }
            catch (Exception ex)
            {
                LogUtils.Error($"{ModTag} Options UI registration failed: {ex.GetType().Name}: {ex.Message}", ex);
            }

            try
            {
                World world = World.DefaultGameObjectInjectionWorld;

                world.GetOrCreateSystemManaged<PersistentSpeedLimitStorageSystem>();
                updateSystem.UpdateAt<PersistentSpeedLimitStorageSystem>(SystemUpdatePhase.Deserialize);

                world.GetOrCreateSystemManaged<SegmentSpeedToolSystem>();
                updateSystem.UpdateAt<SegmentSpeedToolSystem>(SystemUpdatePhase.ToolUpdate);

                world.GetOrCreateSystemManaged<CustomSpeedReapplySystem>();
                // CS2 can overwrite lane speeds when roads or policies refresh. Run afterward so
                // the player's custom segment speeds remain in effect without taking over game cleanup.
                updateSystem.UpdateAfter<CustomSpeedReapplySystem, LaneDataSystem>(
                    SystemUpdatePhase.ModificationEnd);

                world.GetOrCreateSystemManaged<ClearCustomSpeedsSystem>();
                updateSystem.UpdateAt<ClearCustomSpeedsSystem>(SystemUpdatePhase.ModificationEnd);

                world.GetOrCreateSystemManaged<CityRoadGroupApplySystem>();
                updateSystem.UpdateAt<CityRoadGroupApplySystem>(SystemUpdatePhase.ModificationEnd);

                world.GetOrCreateSystemManaged<SegmentSpeedToolUISystem>();
                updateSystem.UpdateAt<SegmentSpeedToolUISystem>(SystemUpdatePhase.UIUpdate);

                // Waterway overlay only. Road/rail selection uses the game's Highlighted outline.
                world.GetOrCreateSystemManaged<SegmentSelectionOverlayRenderSystem>();
                updateSystem.UpdateAt<SegmentSelectionOverlayRenderSystem>(SystemUpdatePhase.Rendering);

                // Draw speed numbers after the waterway selection overlay so transparent overlay
                // colors do not wash out waterway speed labels when they overlap on screen.
                world.GetOrCreateSystemManaged<SpeedLimitMarkerRenderSystem>();
                updateSystem.UpdateAt<SpeedLimitMarkerRenderSystem>(SystemUpdatePhase.Rendering);
            }
            catch (Exception ex)
            {
                LogUtils.Error($"{ModTag} System scheduling failed: {ex.GetType().Name}: {ex.Message}", ex);
            }
        }

        public void OnDispose()
        {
            if (Settings != null)
            {
                Settings.UnregisterInOptionsUI();
                Settings = null;
            }
        }
    }
}
