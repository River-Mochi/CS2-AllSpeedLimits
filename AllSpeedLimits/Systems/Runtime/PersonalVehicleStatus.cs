// <copyright file="PersonalVehicleStatus.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Runtime/PersonalVehicleStatus.cs
// Purpose: UI-facing cached status lines for Options UI.

namespace RoadRailSpeeds.Systems
{
    using System;                 // DateTime, TimeSpan
    using CS2Shared.RiverMochi;   // PersonalVehicleLocale
    using Game;                   // IsGame()
    using Game.SceneFlow;         // GameManager
    using Unity.Entities;         // World
    using UnityEngine;            // Time.frameCount

    public static class PersonalVehicleStatus
    {
        private const int kRefreshIntervalSeconds = 10;

        internal const string KeyStatusNotLoaded = "ASL_PERSONAL_STATUS_NOT_LOADED";
        internal const string KeyStatsNotAvail = "ASL_PERSONAL_STATS_NOT_AVAIL";
        internal const string KeyCarsNotAvail = "ASL_PERSONAL_STATS_CARS_NOT_AVAIL";

        internal const string KeyBikesRow = "ASL_PERSONAL_STATS_BIKES_ROW1";
        internal const string KeyCarsRow = "ASL_PERSONAL_STATS_CARS_ROW2";
        internal const string KeyCarsRow3 = "ASL_PERSONAL_STATS_CARS_ROW3";

        private const string FallbackStatusNotLoaded = "Status not loaded.";
        private const string FallbackStatsNotAvail = "Load a city to see statistics.";
        private const string FallbackCarsNotAvail = "Load a city to see statistics.";

        private const string FallbackBikesRow =
            "{0} active | {1} bikes | {2} e-scooter | {3} / {4} parked/total";

        private const string FallbackCarsRow =
            "{0} active | {1} parked | {2} total | {3} trailers";

        private const string FallbackCarsRow3 =
            "Hidden parked cars {0} total at OC border | updated {1}";

        public static string BikesRow { get; private set; } = string.Empty;
        public static string CarsRow { get; private set; } = string.Empty;
        public static string CarsRow3 { get; private set; } = string.Empty;

        private static bool s_WasInGame;
        private static bool s_HasSnapshotThisCity;
        private static long s_LastRefreshTicksUtc;
        private static int s_LastUiFrame = -1;

        public static void InvalidateCache( )
        {
            s_HasSnapshotThisCity = false;
            s_LastRefreshTicksUtc = 0;
            s_LastUiFrame = -1;

            BikesRow = PersonalVehicleLocale.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            CarsRow = PersonalVehicleLocale.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            CarsRow3 = PersonalVehicleLocale.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
        }

        public static void MarkDirty( )
        {
            s_HasSnapshotThisCity = false;
            s_LastRefreshTicksUtc = 0;
        }

        public static void RefreshIfNeeded( )
        {
            World world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return;
            }

            int frame = Time.frameCount;
            if (frame == s_LastUiFrame)
            {
                return;
            }

            s_LastUiFrame = frame;

            if (string.IsNullOrEmpty(BikesRow))
            {
                BikesRow = PersonalVehicleLocale.SafeFormat(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            }

            if (string.IsNullOrEmpty(CarsRow))
            {
                CarsRow = PersonalVehicleLocale.SafeFormat(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            }

            if (string.IsNullOrEmpty(CarsRow3))
            {
                CarsRow3 = PersonalVehicleLocale.SafeFormat(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            }

            GameManager gm = GameManager.instance;
            bool isGame = (gm != null && gm.gameMode.IsGame());

            if (isGame != s_WasInGame)
            {
                s_WasInGame = isGame;
                InvalidateCache();
            }

            if (!isGame)
            {
                BikesRow = PersonalVehicleLocale.SafeFormat(KeyStatsNotAvail, FallbackStatsNotAvail);
                CarsRow = PersonalVehicleLocale.SafeFormat(KeyCarsNotAvail, FallbackCarsNotAvail);
                CarsRow3 = string.Empty;
                return;
            }

            long nowUtc = DateTime.UtcNow.Ticks;

            if (!s_HasSnapshotThisCity)
            {
                BuildSnapshotSafe(world);
                s_HasSnapshotThisCity = true;
                s_LastRefreshTicksUtc = nowUtc;
                return;
            }

            int interval = kRefreshIntervalSeconds;
            if (interval < 1)
            {
                interval = 5;
            }

            long nextAllowed = s_LastRefreshTicksUtc + TimeSpan.FromSeconds(interval).Ticks;
            if (nowUtc < nextAllowed)
            {
                return;
            }

            BuildSnapshotSafe(world);
            s_LastRefreshTicksUtc = nowUtc;
        }

        private static void BuildSnapshotSafe(World world)
        {
            try
            {
                BuildAndApplySnapshot(world);
            }
            catch
            {
                BikesRow = PersonalVehicleLocale.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
                CarsRow = PersonalVehicleLocale.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
                CarsRow3 = PersonalVehicleLocale.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            }
        }

        private static void BuildAndApplySnapshot(World world)
        {
            PersonalVehicleStatusSystem sys = world.GetOrCreateSystemManaged<PersonalVehicleStatusSystem>();
            PersonalVehicleStatusSystem.Snapshot snap = sys.BuildSnapshot();

            BikesRow = PersonalVehicleLocale.SafeFormat(
                KeyBikesRow,
                fallbackFormat: FallbackBikesRow,
                PersonalVehicleLocale.FormatN0(snap.BikeGroupActive),    // {0}
                PersonalVehicleLocale.FormatN0(snap.BikeOnlyTotal),      // {1}
                PersonalVehicleLocale.FormatN0(snap.ScooterTotal),       // {2}
                PersonalVehicleLocale.FormatN0(snap.BikeGroupParked),    // {3}
                PersonalVehicleLocale.FormatN0(snap.BikeGroupTotal)      // {4}
            );

            CarsRow = PersonalVehicleLocale.SafeFormat(
                KeyCarsRow,
                fallbackFormat: FallbackCarsRow,
                PersonalVehicleLocale.FormatN0(snap.CarGroupActive),     // {0}
                PersonalVehicleLocale.FormatN0(snap.CarGroupParked),     // {1}
                PersonalVehicleLocale.FormatN0(snap.CarGroupTotal),      // {2}
                PersonalVehicleLocale.FormatN0(snap.TrailerTotal)        // {3}
            );

            string updated = snap.SnapshotTimeLocal.ToString("HH:mm:ss");

            CarsRow3 = PersonalVehicleLocale.SafeFormat(
                KeyCarsRow3,
                fallbackFormat: FallbackCarsRow3,
                PersonalVehicleLocale.FormatN0(snap.CarHiddenAtBorder),   // {0} TOTAL OC hidden
                updated                                         // {1}
            );
        }
    }
}
