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
    using System;
    using System.Reflection;
    using System.Diagnostics;
    using Game.Simulation;
    using Colossal.UI.Binding;
    using Game.UI.InGame;
    using Game.UI.Menu;
    using CS2Shared.RiverMochi;   // PersonalVehicleLocale
    using Game;                   // IsGame()
    using Game.SceneFlow;         // GameManager
    using Unity.Entities;         // World

    public static class PersonalVehicleStatus
    {
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

        // Native Options has no public selected-page/section accessor. Resolve the
        // existing read-only bindings once; the public screen guard is always required.
        private static readonly FieldInfo? s_PageField = typeof(OptionsUISystem).GetField(
            "m_ActivePageBinding", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo? s_SectionField = typeof(OptionsUISystem).GetField(
            "m_ActiveSectionBinding", BindingFlags.Instance | BindingFlags.NonPublic);
        private static bool s_HasSnapshot;
        private static int s_VisitNumber;
        private static uint? s_SnapshotFrame;
        private static long s_VisitStartTimestamp;
        private static World? s_World;
        private static ValueBinding<string>? s_PageBinding;
        private static ValueBinding<string>? s_SectionBinding;
        private static string? s_Page;
        private static string? s_Section;

        public static void InvalidateCache()
        {
            if (s_HasSnapshot)
            {
                uint? currentFrame = s_World != null && s_World.IsCreated
                    ? s_World.GetExistingSystemManaged<SimulationSystem>()?.frameIndex
                    : null;
                double visitSeconds = (Stopwatch.GetTimestamp() - s_VisitStartTimestamp) / (double)Stopwatch.Frequency;
                LogUtils.Info(() => $"{Mod.ModTag} Options personal status visit={s_VisitNumber} END scans=1 " +
                    $"startFrame={s_SnapshotFrame} endFrame={currentFrame} durationSec={visitSeconds:0.0}.");
            }

            s_HasSnapshot = false;
            s_SnapshotFrame = null;
            s_World = null;
            s_PageBinding = null;
            s_SectionBinding = null;
            s_Page = null;
            s_Section = null;
            BikesRow = PersonalVehicleLocale.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            CarsRow = string.Empty;
            CarsRow3 = string.Empty;
        }

        public static void MarkDirty() => InvalidateCache();

        private static bool IsOptionsOpen(World world)
        {
            GameManager? gm = GameManager.instance;
            if (gm == null || !gm.gameMode.IsGame() || gm.isGameLoading)
            {
                return false;
            }

            GameScreenUISystem? screen = world.GetExistingSystemManaged<GameScreenUISystem>();
            return screen != null && screen.activeScreen == GameScreenUISystem.GameScreen.Options;
        }

        internal static bool IsCurrentVisit(World world)
        {
            return s_HasSnapshot && s_World == world && IsOptionsOpen(world) &&
                s_PageBinding?.value == s_Page && s_SectionBinding?.value == s_Section;
        }

        public static void RefreshIfNeeded()
        {
            World? world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated || !IsOptionsOpen(world))
            {
                if (s_HasSnapshot)
                {
                    InvalidateCache();
                }

                BikesRow = PersonalVehicleLocale.Localize(KeyStatsNotAvail, FallbackStatsNotAvail);
                CarsRow = string.Empty;
                CarsRow3 = string.Empty;
                return;
            }

            if (IsCurrentVisit(world))
            {
                return;
            }

            InvalidateCache();
            s_World = world;
            OptionsUISystem? options = world.GetExistingSystemManaged<OptionsUISystem>();
            if (options != null)
            {
                try
                {
                    s_PageBinding = s_PageField?.GetValue(options) as ValueBinding<string>;
                    s_SectionBinding = s_SectionField?.GetValue(options) as ValueBinding<string>;
                    s_Page = s_PageBinding?.value;
                    s_Section = s_SectionBinding?.value;
                }
                catch (Exception ex)
                {
                    LogUtils.WarnOnce("PersonalStatusBindings", () => $"{Mod.ModTag} Options tab tracking unavailable: {ex.Message}");
                }
            }

            // If a future game update changes these private bindings, retain the
            // screen-level guard and refresh on reopening Options, not on a timer.
            if (s_PageBinding != null && s_SectionBinding != null)
            {
                // ModSetting registers the page by id; AutomaticSettings prefixes tab ids.
                // Search results and background getters must not start a city scan.
                string? pageId = Mod.Settings?.id;
                if (pageId == null || s_Page != pageId ||
                    s_Section != pageId + "." + SpeedLimitsSetting.kBicycleTab)
                {
                    return;
                }
            }
            else
            {
                // Drop partially available bindings so fallback tracks Options only.
                s_PageBinding = null;
                s_SectionBinding = null;
                s_Page = null;
                s_Section = null;
                LogUtils.WarnOnce("PersonalStatusBindingMissing", () => $"{Mod.ModTag} Options tab tracking unavailable; refresh on reopening Options.");
            }

            s_HasSnapshot = true;
            s_VisitNumber++;
            s_SnapshotFrame = world.GetExistingSystemManaged<SimulationSystem>()?.frameIndex;
            s_VisitStartTimestamp = Stopwatch.GetTimestamp();
            BuildSnapshotSafe(world);
            // Only observe the active Options visit; never schedule another scan.
            world.GetOrCreateSystemManaged<PersonalVehicleStatusSystem>().Enabled = true;
        }

        private static void BuildSnapshotSafe(World world)
        {
            try
            {
                BuildAndApplySnapshot(world);
                double elapsedMs = (Stopwatch.GetTimestamp() - s_VisitStartTimestamp) * 1000.0 / Stopwatch.Frequency;
                LogUtils.Info(() => $"{Mod.ModTag} Options personal status visit={s_VisitNumber} scan=1 " +
                    $"screen=Options frame={s_SnapshotFrame} durationMs={elapsedMs:0.00} page={s_Page ?? "fallback"} section={s_Section ?? "fallback"}.");
            }
            catch (Exception exception)
            {
                LogUtils.WarnOnce("PersonalStatusSnapshot", () => $"{Mod.ModTag} Personal vehicle status scan failed: {exception.Message}");
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
