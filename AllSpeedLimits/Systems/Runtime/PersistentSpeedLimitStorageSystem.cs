// <copyright file="PersistentSpeedLimitStorageSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Runtime/PersistentSpeedLimitStorageSystem.cs
// Purpose: Initializes and flushes the per-city JSON speed backup storage.

namespace RoadRailSpeeds.Systems
{
    using System;                            // Exception handling and fallback naming.
    using System.IO;                         // Invalid filename cleanup for JSON storage file names.
    using Colossal.Serialization.Entities;  // Purpose.
    using CS2Shared.RiverMochi;              // LogUtils for safe custom file logging.
    using Game;                              // GameSystemBase.
    using Game.SceneFlow;                    // GameManager, GameMode, and world-ready state.
    using RoadRailSpeeds.Data;               // PersistentSpeedLimitStorage JSON backup helper.
    using UnityEngine.Scripting;             // Preserve attributes for game systems.

    public partial class PersistentSpeedLimitStorageSystem : GameSystemBase
    {
        private GameManager? m_GameManager;
        private bool m_Initialized;
        private string? m_LastCityName;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();

            m_GameManager = GameManager.instance;
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (m_GameManager == null)
            {
                return;
            }

            if (!m_Initialized && EnsureInitialized())
            {
                return;
            }

            if (m_Initialized && m_GameManager.gameMode == GameMode.MainMenu)
            {
                // Returning to the main menu means the next loaded city should get its own JSON file.
                m_Initialized = false;
                m_LastCityName = null;
            }
        }

        [Preserve]
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (mode == GameMode.Game &&
                (purpose == Purpose.NewGame || purpose == Purpose.LoadGame))
            {
                // Migration/reapply systems need the per-city backup during their own load callback.
                EnsureInitialized();
            }
        }

        internal bool EnsureInitialized()
        {
            if (m_Initialized)
            {
                return true;
            }

            if (!IsWorldReadyForStorage())
            {
                return false;
            }

            InitializePersistentStorage();
            m_Initialized = true;
            return true;
        }

        [Preserve]
        protected override void OnDestroy()
        {
            if (m_Initialized)
            {
                PersistentSpeedLimitStorage.Save();
                // Waiting during world teardown is safe and prevents losing the last queued backup.
                PersistentSpeedLimitStorage.FlushPendingSave();
            }

            base.OnDestroy();
        }

        private bool IsWorldReadyForStorage()
        {
            return m_GameManager != null &&
                   m_GameManager.state >= GameManager.State.WorldReady &&
                   (m_GameManager.gameMode == GameMode.Game ||
                    m_GameManager.gameMode == GameMode.Editor);
        }

        private void InitializePersistentStorage()
        {
            try
            {
                string cityName = SanitizeFileName(GetCityName());

                if (m_LastCityName == cityName)
                {
                    return;
                }

                m_LastCityName = cityName;

                // PersistentSpeedLimitStorage keeps a JSON backup outside the normal component save path.
                // Both values are cityName for now because this code does not expose a stable save id yet.
                PersistentSpeedLimitStorage.Initialize(cityName, cityName);
            }
            catch (Exception ex)
            {
                LogUtils.Error(
                    () => $"{Mod.ModTag} Failed to initialize persistent storage: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }
        }

        private string GetCityName()
        {
            try
            {
                Game.City.CityConfigurationSystem? cityConfigSystem =
                    World.GetExistingSystemManaged<Game.City.CityConfigurationSystem>();

                string cityName = cityConfigSystem?.cityName ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(cityName))
                {
                    return cityName;
                }
            }
            catch (Exception ex)
            {
                LogUtils.Warn(
                    () => $"{Mod.ModTag} Could not read city name: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }

            return m_GameManager?.gameMode == GameMode.Game ? "City" : "EditorCity";
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "UnnamedCity";
            }

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            fileName = fileName.Replace(' ', '_').Replace('.', '_');

            if (fileName.Length > 100)
            {
                fileName = fileName.Substring(0, 100);
            }

            return fileName;
        }
    }
}
