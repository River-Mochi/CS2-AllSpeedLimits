// File: UI/src/panel/types.ts
// Purpose: Shared panel-only types for speed tool sections, tooltips, and city actions.

export type CityActionKind = "applyRoadGroup" | "applyTrain" | "applySubway" | "resetRoads" | "resetRails" | "resetWater" | "resetAll";

export type RoadGroupKind = "small" | "medium" | "large" | "highway";

export type PanelTooltipKind =
    | "gameDefault"
    | "panelTitle"
    | "presetUnlimited"
    | "resetAll"
    | "resetSelected"
    | "roadGroupApply"
    | "roadGroupSmall"
    | "roadGroupMedium"
    | "roadGroupLarge"
    | "roadGroupHighway"
    | "speedFaster"
    | "speedSlower"
    | "stats"
    | "statsBikes"
    | "statsCars"
    | "statsIndustry"
    | "unit"
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
