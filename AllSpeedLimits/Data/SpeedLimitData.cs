// <copyright file="SpeedLimitData.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
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
