// <copyright file="Mod.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
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

        public static Setting? Settings { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            // ShellOpen.Configure also calls LogUtils.Configure(LogFileName, s_Log).
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

            Setting setting = new Setting(this);
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
                    // Register localization before settings/options UI reads the dictionary.
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
                AssetDatabase.global.LoadSettings(ModId, setting, new Setting(this));
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
                updateSystem.UpdateAt<CustomSpeedReapplySystem>(SystemUpdatePhase.ModificationEnd);

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
