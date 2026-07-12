// File: UI/src/panel/constants.ts
// Purpose: Shared panel constants so layout and preset tweaks live in one obvious place.

import type { PanelTooltipKind, RoadGroupKind } from "./types";

export const ROAD_GROUPS: Array<{ key: RoadGroupKind; value: number }> = [
    { key: "small", value: 0 },
    { key: "medium", value: 1 },
    { key: "large", value: 2 },
    { key: "highway", value: 3 }
];

export const METRIC_PRESET_SPEEDS = [
    10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160
];

export const IMPERIAL_PRESET_SPEEDS = [
    5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90
];

export const PRECISE_STEP_HOLD_DELAY_MS = 350;
export const PRECISE_STEP_REPEAT_MS = 150;

// Main panel sizing. Smaller values leave more city visible behind the tool.
export const PANEL_WIDTH_REM = 299;

// On a brand-new selection (nothing was selected before this click), the panel re-anchors near the
// cursor instead of opening wherever it was left. This nudges it down-right of the click so the
// title bar isn't pinned exactly under the cursor and the clicked segment stays visible.
export const PANEL_CLICK_OFFSET_PX = 16;
export const PRIMARY_ACTION_BUTTON_WIDTH_REM = 58;
export const SPEED_STEPPER_WIDTH_REM = 76;
export const SPEED_STEPPER_BUTTON_WIDTH_REM = 22;
export const SPEED_STEPPER_NUMBER_WIDTH_REM = 32;

// Stats table column geometry. Shared so the column headers (rendered on the "Stats" section
// line) line up with the number columns in the table below them.
export const STATS_COLUMN_WIDTH_REM = 76;
export const STATS_LABEL_WIDTH_REM = 30;

// Tooltip layout knobs.
// REM_TO_TOOLTIP_PX converts the panel's rem width into COHTML tooltip pixels.
// RIGHT_TOOLTIP_GAP_PX: smaller = closer to panel; larger = farther right.
export const REM_TO_TOOLTIP_PX = 1.34;
export const RIGHT_TOOLTIP_GAP_PX = 10;

// Right-side tooltip vertical positions, measured from the panel top.
export const RIGHT_TOOLTIP_TOP_OFFSETS: Record<PanelTooltipKind, number> = {
    currentSpeed: 142,
    gameDefault: 82,
    panelTitle: 28,
    presets: 118,
    presetUnlimited: 244,
    resetAll: 300,
    resetSelected: 142,
    roadGroupApply: 300,
    roadGroupSmall: 300,
    roadGroupMedium: 300,
    roadGroupLarge: 300,
    roadGroupHighway: 300,
    stats: 340,
    statsBikes: 340,
    statsCars: 340,
    statsIndustry: 340,
    statsBuses: 340,
    statsTaxis: 340,
    selectedSegment: 118,
    unit: 28,
    newSpeedUnit: 128,
    wholeCity: 300,
    // Filter chips sit just under the title bar; Apply is in the main edit block.
    filterRoads: 58,
    filterRails: 58,
    filterWater: 58,
    apply: 196,
    applyTrain: 300,
    applySubway: 300,
    expandAll: 58,
    markers: 28
};

export const ROAD_GROUP_TOOLTIP_KIND: Record<RoadGroupKind, PanelTooltipKind> = {
    small: "roadGroupSmall",
    medium: "roadGroupMedium",
    large: "roadGroupLarge",
    highway: "roadGroupHighway"
};
