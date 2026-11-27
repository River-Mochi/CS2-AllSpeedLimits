// <copyright file="types.ts" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: UI/src/panel/types.ts
// Purpose: Shared panel-only types for speed tool sections, tooltips, and city actions.

export type CityActionKind = "applyRoadGroup" | "applyTrain" | "applySubway" | "resetRoads" | "resetPaths" | "resetRails" | "resetWater" | "resetAll";

export type RoadGroupKind = "small" | "medium" | "large" | "highway";

export type PanelTooltipKind =
    | "currentSpeed"
    | "gameDefault"
    | "panelTitle"
    | "presets"
    | "presetUnlimited"
    | "resetAll"
    | "resetSelected"
    | "roadGroupApply"
    | "roadGroupSmall"
    | "roadGroupMedium"
    | "roadGroupLarge"
    | "roadGroupHighway"
    | "stats"
    | "statsBikes"
    | "statsCars"
    | "statsIndustry"
    | "statsBuses"
    | "statsTaxis"
    | "selectedSegment"
    | "unit"
    | "newSpeedUnit"
    | "wholeCity"
    | "filterPaths"
    | "showInvisible"
    | "filterRoads"
    | "filterRails"
    | "filterWater"
    | "dragGrip"
    | "pathSlider"
    | "pathApply"
    | "pathReset"
    | "pedPathSlider"
    | "pedPathApply"
    | "pedPathReset"
    | "apply"
    | "applyTrain"
    | "applySubway"
    | "expandAll"
    | "markers";

export type CityActionInfo = {
    title: string;
    message: string;
    confirmLabel: string;
};
