// <copyright file="PersistentSpeedLimitStorage.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Data/PersistentSpeedLimitStorage.cs
// Purpose: JSON backup storage for custom road/rail/waterway speed-limit data outside the normal save component.

namespace RoadRailSpeeds.Data
{
    using System;                       // DateTime, Exception
    using System.Collections.Generic;   // Dictionary, IReadOnlyDictionary
    using System.IO;                    // Path, Directory, File
    using System.Text;                  // UTF8Encoding
    using System.Threading.Tasks;       // Ordered background backup writes
    using Colossal.PSI.Environment;     // EnvPath
    using CS2Shared.RiverMochi;         // LogUtils
    using Newtonsoft.Json;              // JsonConvert, Formatting

    public static class PersistentSpeedLimitStorage
    {
        private const string kModsDataFolderName = "SpeedLimits";
        private const int kMaxFileNameLength = 50;

        private static readonly IReadOnlyDictionary<int, SpeedLimitEntry> s_EmptySpeedLimits =
            new Dictionary<int, SpeedLimitEntry>();

        private static MapSpeedLimitData? s_CurrentMapData;
        private static string? s_CurrentFilePath;
        private static string? s_BaseDirectory;
        private static readonly object s_SaveQueueLock = new object();
        private static Task s_SaveQueue = Task.CompletedTask;

        public static string BaseDirectory
        {
            get
            {
                string? cachedBaseDirectory = s_BaseDirectory;

                if (cachedBaseDirectory != null && cachedBaseDirectory.Length > 0)
                {
                    return cachedBaseDirectory;
                }

                // CS2 resolves this correctly for local builds and PDX-installed copies.
                string baseDirectory = Path.Combine(
                    EnvPath.kUserDataPath,
                    "ModsData",
                    kModsDataFolderName);

                Directory.CreateDirectory(baseDirectory);

                s_BaseDirectory = baseDirectory;
                return baseDirectory;
            }
        }

        public static void Initialize(string saveGameName, string saveGameId)
        {
            try
            {
                // A prior world may still have a queued backup. Finish it before changing paths.
                FlushPendingSave();

                string sanitizedName = SanitizeFileName(saveGameName);
                string safeSaveGameId = SanitizeFileName(saveGameId);
                string fileName = $"{sanitizedName}_{safeSaveGameId}.json";

                s_CurrentFilePath = Path.Combine(BaseDirectory, fileName);

                if (File.Exists(s_CurrentFilePath))
                {
                    LoadFromFile();
                    return;
                }

                s_CurrentMapData = new MapSpeedLimitData
                {
                    MapName = saveGameName,
                    SaveGameId = saveGameId,
                    LastSaved = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                LogUtils.Error(
                    () => $"{Mod.ModTag} Failed to initialize persistent speed storage: {ex.GetType().Name}: {ex.Message}",
                    ex);

                s_CurrentMapData = new MapSpeedLimitData();
                s_CurrentFilePath = null;
            }
        }

        // Performance contract: these per-edge methods only mutate memory. Call Save once after
        // the complete user action; never serialize the full backup from inside an edge loop.
        public static void StoreSpeedLimit(
            int entityIndex,
            float defaultSpeedKmh,
            float currentSpeedKmh)
        {
            if (s_CurrentMapData == null)
            {
                LogUtils.WarnOnce(
                    "PersistentSpeedLimitStorage.StoreBeforeInitialize",
                    () => $"{Mod.ModTag} Persistent speed storage is not initialized; StoreSpeedLimit ignored.");

                return;
            }

            s_CurrentMapData.SpeedLimits[entityIndex] = new SpeedLimitEntry
            {
                EntityIndex = entityIndex,
                DefaultSpeedKmh = defaultSpeedKmh,
                CurrentSpeedKmh = currentSpeedKmh,
                LastModified = DateTime.Now
            };
        }

        public static SpeedLimitEntry? GetSpeedLimit(int entityIndex)
        {
            if (s_CurrentMapData == null)
            {
                return null;
            }

            return s_CurrentMapData.SpeedLimits.TryGetValue(entityIndex, out SpeedLimitEntry entry)
                ? entry
                : null;
        }

        public static float? GetDefaultSpeedLimit(int entityIndex)
        {
            return GetSpeedLimit(entityIndex)?.DefaultSpeedKmh;
        }

        public static float? GetCurrentSpeedLimit(int entityIndex)
        {
            return GetSpeedLimit(entityIndex)?.CurrentSpeedKmh;
        }

        public static IReadOnlyDictionary<int, SpeedLimitEntry> GetAllSpeedLimits()
        {
            return s_CurrentMapData?.SpeedLimits ?? s_EmptySpeedLimits;
        }

        public static void RemoveSpeedLimit(int entityIndex)
        {
            if (s_CurrentMapData == null)
            {
                return;
            }

            s_CurrentMapData.SpeedLimits.Remove(entityIndex);
        }

        public static void Save()
        {
            if (s_CurrentMapData == null || string.IsNullOrEmpty(s_CurrentFilePath))
            {
                return;
            }

            try
            {
                s_CurrentMapData.LastSaved = DateTime.Now;
                MapSpeedLimitData snapshot = CreateSnapshot(s_CurrentMapData);
                string filePath = s_CurrentFilePath;

                // Keep writes ordered so an older snapshot can never finish after a newer one.
                // Only the dictionary snapshot above happens on the game thread; full JSON
                // serialization and disk I/O run on the background queue.
                lock (s_SaveQueueLock)
                {
                    s_SaveQueue = s_SaveQueue.ContinueWith(
                        _ => WriteSnapshot(filePath, snapshot),
                        TaskScheduler.Default);
                }
            }
            catch (Exception ex)
            {
                LogUtils.Error(
                    () => $"{Mod.ModTag} Failed to queue persistent speed data: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }
        }

        public static void FlushPendingSave()
        {
            Task pendingSave;
            lock (s_SaveQueueLock)
            {
                pendingSave = s_SaveQueue;
            }

            try
            {
                pendingSave.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                LogUtils.Error(
                    () => $"{Mod.ModTag} Failed while flushing persistent speed data: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }
        }

        public static void Clear()
        {
            if (s_CurrentMapData == null)
            {
                return;
            }

            s_CurrentMapData.SpeedLimits.Clear();
            Save();
        }

        public static string GetStats()
        {
            if (s_CurrentMapData == null)
            {
                return "Storage not initialized";
            }

            return $"Map: {s_CurrentMapData.MapName}, Speed limits: {s_CurrentMapData.SpeedLimits.Count}, Last saved: {s_CurrentMapData.LastSaved}";
        }

        private static MapSpeedLimitData CreateSnapshot(MapSpeedLimitData source)
        {
            return new MapSpeedLimitData
            {
                MapName = source.MapName,
                SaveGameId = source.SaveGameId,
                LastSaved = source.LastSaved,
                // StoreSpeedLimit replaces entries instead of mutating them, so copying the
                // dictionary is enough to give the background writer a stable snapshot.
                SpeedLimits = new Dictionary<int, SpeedLimitEntry>(source.SpeedLimits),
                Version = source.Version
            };
        }

        private static void WriteSnapshot(string filePath, MapSpeedLimitData snapshot)
        {
            try
            {
                string json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);

                // Explicit UTF-8 without BOM, matching the repo text-file policy.
                File.WriteAllText(
                    filePath,
                    json,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            }
            catch (Exception ex)
            {
                LogUtils.Error(
                    () => $"{Mod.ModTag} Failed to save persistent speed data: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }
        }

        private static void LoadFromFile()
        {
            if (string.IsNullOrEmpty(s_CurrentFilePath))
            {
                s_CurrentMapData = new MapSpeedLimitData();
                return;
            }

            try
            {
                string json = File.ReadAllText(s_CurrentFilePath);
                s_CurrentMapData = JsonConvert.DeserializeObject<MapSpeedLimitData>(json) ?? new MapSpeedLimitData();
            }
            catch (Exception ex)
            {
                LogUtils.Error(
                    () => $"{Mod.ModTag} Failed to load persistent speed data: {ex.GetType().Name}: {ex.Message}",
                    ex);

                s_CurrentMapData = new MapSpeedLimitData();
            }
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "unnamed";
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();

            string sanitized = string.Join(
                    "_",
                    fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
                .Trim()
                .TrimEnd('.');

            if (sanitized.Length > kMaxFileNameLength)
            {
                sanitized = sanitized.Substring(0, kMaxFileNameLength);
            }

            return string.IsNullOrEmpty(sanitized) ? "unnamed" : sanitized;
        }
    }
}
