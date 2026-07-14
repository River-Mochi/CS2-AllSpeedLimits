// <copyright file="InCityLocalization.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/InCityLocalization.cs
// Purpose: Loads embedded in-city UI JSON translations into the game localization manager.

namespace RoadRailSpeeds
{
    using System;                    // Exception, StringComparison
    using System.Collections.Generic; // Dictionary, KeyValuePair
    using System.IO;                 // Stream, StreamReader
    using System.Linq;               // OrderBy
    using System.Reflection;         // Assembly
    using CS2Shared.RiverMochi;      // LogUtils
    using Colossal.Json;             // JSON, Variant
    using Colossal.Localization;     // LocalizationManager, MemorySource
    using Colossal.Logging;          // ILog
    using Game.SceneFlow;            // GameManager

    internal static class InCityLocalization
    {
        private const string LangMarker = ".lang.";
        private const string JsonSuffix = ".json";

        // Embedding keeps in-city translations independent from local Mods/ vs PDX .cache folder names.
        public static void LoadEmbeddedJsonTranslations(string modId, string modTag, ILog log)
        {
            LocalizationManager? localizationManager = GameManager.instance?.localizationManager;
            if (localizationManager == null)
            {
                LogUtils.Warn(log, $"{modTag} InCityLocalization: no LocalizationManager available.");
                return;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames()
                .Where(IsLangJsonResource)
                .OrderBy(static resourceName => resourceName, StringComparer.Ordinal)
                .ToArray();

            if (resourceNames.Length == 0)
            {
                LogUtils.Warn(log, $"{modTag} InCityLocalization: no embedded lang/*.json resources found.");
                return;
            }

            int registered = 0;
            foreach (string resourceName in resourceNames)
            {
                string localeId = GetLocaleId(resourceName);
                if (string.IsNullOrWhiteSpace(localeId))
                {
                    LogUtils.Warn(log, $"{modTag} InCityLocalization: could not get locale from '{resourceName}'.");
                    continue;
                }

                try
                {
                    Dictionary<string, string> translations = ReadJsonResource(assembly, resourceName);
                    if (translations.Count == 0)
                    {
                        LogUtils.Warn(log, $"{modTag} InCityLocalization: empty translations in '{resourceName}'.");
                        continue;
                    }

                    Dictionary<string, string> prefixed = new Dictionary<string, string>(translations.Count);
                    foreach (KeyValuePair<string, string> entry in translations)
                    {
                        if (!string.IsNullOrEmpty(entry.Value))
                        {
                            prefixed[$"{modId}.UI.{entry.Key}"] = entry.Value;
                        }
                    }

                    localizationManager.AddSource(localeId, new MemorySource(prefixed));
                    registered++;
                }
                catch (Exception ex)
                {
                    LogUtils.Warn(log, $"{modTag} InCityLocalization: failed loading '{resourceName}': {ex.GetType().Name}: {ex.Message}");
                }
            }

#if DEBUG
            // Successful registration is useful while testing translations, but is routine release noise.
            LogUtils.Info(log, $"{modTag} InCityLocalization: registered {registered}/{resourceNames.Length} embedded locale sources.");
#endif
        }

        private static bool IsLangJsonResource(string resourceName)
        {
            return resourceName.IndexOf(LangMarker, StringComparison.Ordinal) >= 0
                && resourceName.EndsWith(JsonSuffix, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetLocaleId(string resourceName)
        {
            int markerIndex = resourceName.LastIndexOf(LangMarker, StringComparison.Ordinal);
            if (markerIndex < 0 || !resourceName.EndsWith(JsonSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            int startIndex = markerIndex + LangMarker.Length;
            int length = resourceName.Length - startIndex - JsonSuffix.Length;
            return length <= 0 ? string.Empty : resourceName.Substring(startIndex, length);
        }

        private static Dictionary<string, string> ReadJsonResource(Assembly assembly, string resourceName)
        {
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return new Dictionary<string, string>();
            }

            using StreamReader reader = new StreamReader(stream);
            string raw = reader.ReadToEnd();
            Variant variant = JSON.Load(raw);
            return variant.Make<Dictionary<string, string>>() ?? new Dictionary<string, string>();
        }
    }
}
