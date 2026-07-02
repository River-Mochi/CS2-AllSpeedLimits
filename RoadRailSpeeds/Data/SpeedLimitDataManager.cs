// <copyright file="SpeedLimitDataManager.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Data/SpeedLimitDataManager.cs
// Purpose: In-memory cache for original and custom road/rail/waterway speeds.

namespace RoadRailSpeeds.Data
{
    using System.Collections.Generic;

    public static class SpeedLimitDataManager
    {
        private static readonly Dictionary<int, float> s_OriginalSpeeds = new Dictionary<int, float>();
        private static readonly Dictionary<int, float> s_CustomSpeedLimits = new Dictionary<int, float>();

        public static void StoreOriginalSpeed(int entityIndex, float speedKmh)
        {
            if (!s_OriginalSpeeds.ContainsKey(entityIndex))
            {
                // Store default only once so reset always goes back to the true original speed.
                s_OriginalSpeeds[entityIndex] = speedKmh;
            }
        }

        public static float? GetOriginalSpeed(int entityIndex)
        {
            return s_OriginalSpeeds.TryGetValue(entityIndex, out float speedKmh)
                ? speedKmh
                : null;
        }

        public static void RemoveOriginalSpeed(int entityIndex)
        {
            s_OriginalSpeeds.Remove(entityIndex);
        }

        public static void AddCustomSpeedLimit(int entityIndex, float speedKmh)
        {
            s_CustomSpeedLimits[entityIndex] = speedKmh;
        }

        public static void RemoveCustomSpeedLimit(int entityIndex)
        {
            s_CustomSpeedLimits.Remove(entityIndex);
        }

        public static float? GetCustomSpeed(int entityIndex)
        {
            return s_CustomSpeedLimits.TryGetValue(entityIndex, out float speedKmh)
                ? speedKmh
                : null;
        }

        public static IReadOnlyCollection<int> GetCustomSpeedLimitEntityIndexes()
        {
            return s_CustomSpeedLimits.Keys;
        }

        public static IReadOnlyDictionary<int, float> GetAllCustomSpeedLimitData()
        {
            return s_CustomSpeedLimits;
        }

        public static IReadOnlyDictionary<int, float> GetAllOriginalSpeeds()
        {
            return s_OriginalSpeeds;
        }

        public static void ClearAll()
        {
            s_OriginalSpeeds.Clear();
            s_CustomSpeedLimits.Clear();
        }
    }
}
