// <copyright file="SpeedLimitData.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Data/SpeedLimitData.cs
// Purpose: Serializable JSON data models for persistent speed-limit backup storage.

namespace RoadRailSpeeds.Data
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public sealed class SpeedLimitEntry
    {
        public int EntityIndex { get; set; }

        // Stored in km/h so UI and JSON remain human-readable.
        public float DefaultSpeedKmh { get; set; }

        public float CurrentSpeedKmh { get; set; }

        public DateTime LastModified { get; set; }
    }

    [Serializable]
    public sealed class MapSpeedLimitData
    {
        public string MapName { get; set; } = string.Empty;

        public string SaveGameId { get; set; } = string.Empty;

        public DateTime LastSaved { get; set; }

        public Dictionary<int, SpeedLimitEntry> SpeedLimits { get; set; } =
            new Dictionary<int, SpeedLimitEntry>();

        // Increment only if the JSON format changes in a breaking way.
        public int Version { get; set; } = 1;
    }
}
