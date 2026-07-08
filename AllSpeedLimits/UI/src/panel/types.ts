// File: UI/src/panel/types.ts
// Purpose: Shared panel-only types for speed tool sections, tooltips, and city actions.

export type CityActionKind = "applyRoadGroup" | "applyTrain" | "applySubway" | "resetRoads" | "resetRails" | "resetWater" | "resetAll";

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
    | "selectedSegment"
    | "unit"
    | "newSpeedUnit"
    | "wholeCity"
    | "filterRoads"
    | "filterRails"
    | "filterWater"
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
