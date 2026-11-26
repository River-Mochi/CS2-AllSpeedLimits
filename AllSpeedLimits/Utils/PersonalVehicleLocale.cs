// <copyright file="PersonalVehicleLocale.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License.
// See THIRD_PARTY_NOTICES.md in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: PersonalVehicleLocale.cs
// Version: 0.1.1

namespace CS2Shared.RiverMochi
{
    using System;
    using System.Globalization;
    using Colossal.Localization;
    using Game.SceneFlow;

    public static class PersonalVehicleLocale
    {
        public static string Localize(string entryId, string fallback)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                return fallback;
            }

            try
            {
                // During early load the active dictionary may be unavailable; fallback keeps UI safe.
                LocalizationDictionary? dict = GameManager.instance.localizationManager.activeDictionary;
                if (dict != null && dict.TryGetValue(entryId, out string value) && !string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            catch
            {
            }

            return fallback;
        }

        public static string SafeFormat(string entryId, string fallbackFormat, params object[] args)
        {
            string format = Localize(entryId, fallbackFormat);

            try
            {
                // Locale strings are hand-edited, so tolerate a bad placeholder count.
                return string.Format(CultureInfo.CurrentCulture, format, args);
            }
            catch (FormatException)
            {
                try
                {
                    return string.Format(CultureInfo.CurrentCulture, fallbackFormat, args);
                }
                catch
                {
                    return fallbackFormat;
                }
            }
            catch
            {
                return fallbackFormat;
            }
        }

        public static string FormatN0(long value)
        {
            // Shared number formatting keeps UI rows and log reports visually consistent.
            return value.ToString("N0", CultureInfo.CurrentCulture);
        }
    }
}
