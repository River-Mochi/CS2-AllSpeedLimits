// File: UI/src/shared/bindings.ts
// Purpose: React value bindings and triggers for the in-game speed panel.

import { bindValue, trigger } from "cs2/api";
import mod from "mod.json";

export const MOD_ID = mod.id;

export const INITIAL_SPEED = bindValue<number>(
  MOD_ID,
  "BINDING:INFOPANEL_ROAD_SPEED"
);
export const CURRENT_SPEED_MIXED = bindValue<boolean>(
  MOD_ID,
  "BINDING:CURRENT_SPEED_MIXED"
);
export const VANILLA_SPEED = bindValue<number>(
  MOD_ID,
  "BINDING:VANILLA_SPEED"
);
export const VANILLA_SPEED_MIXED = bindValue<boolean>(
  MOD_ID,
  "BINDING:VANILLA_SPEED_MIXED"
);
export const TOOL_ACTIVE = bindValue<boolean>(
  MOD_ID,
  "BINDING:TOOL_ACTIVE"
);
export const SELECTION_COUNTER = bindValue<number>(
  MOD_ID,
  "BINDING:SELECTION_COUNTER"
);
export const SHOW_METRIC = bindValue<boolean>(
  MOD_ID,
  "BINDING:SHOW_METRIC"
);
export const IS_TRACK_TYPE = bindValue<boolean>(
  MOD_ID,
  "BINDING:IS_TRACK_TYPE"
);
export const IS_WATERWAY_TYPE = bindValue<boolean>(
  MOD_ID,
  "BINDING:IS_WATERWAY_TYPE"
);
export const UNIT_MODE = bindValue<number>(
  MOD_ID,
  "BINDING:UNIT_MODE"
);
export const DOUBLE_SPEED_DISPLAY = bindValue<boolean>(
  MOD_ID,
  "BINDING:DOUBLE_SPEED_DISPLAY"
);
export const SYNC_SLIDER_WITH_SELECTION = bindValue<boolean>(
  MOD_ID,
  "BINDING:SYNC_SLIDER_WITH_SELECTION"
);
export const PANEL_SLIDER_INCREMENT = bindValue<number>(
  MOD_ID,
  "BINDING:PANEL_SLIDER_INCREMENT"
);
export const TOOLTIP_FONT_SCALE = bindValue<number>(
  MOD_ID,
  "BINDING:TOOLTIP_FONT_SCALE"
);
export const PANEL_TOOLTIPS_ENABLED = bindValue<boolean>(
  MOD_ID,
  "BINDING:PANEL_TOOLTIPS_ENABLED"
);
export const HIDE_SPEED_MARKERS = bindValue<boolean>(
  MOD_ID,
  "BINDING:HIDE_SPEED_MARKERS"
);
export const CITY_CAR_TOTAL = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_CAR_TOTAL"
);
export const CITY_CAR_ACTIVE = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_CAR_ACTIVE"
);
export const CITY_CAR_PARKED = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_CAR_PARKED"
);
export const CITY_BIKE_TOTAL = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_BIKE_TOTAL"
);
export const CITY_BIKE_ACTIVE = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_BIKE_ACTIVE"
);
export const CITY_BIKE_PARKED = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_BIKE_PARKED"
);
export const CITY_INDUSTRY_TOTAL = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_INDUSTRY_TOTAL"
);
export const CITY_INDUSTRY_ACTIVE = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_INDUSTRY_ACTIVE"
);
export const CITY_INDUSTRY_PARKED = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_INDUSTRY_PARKED"
);
export const CITY_RESET_IN_PROGRESS = bindValue<boolean>(
  MOD_ID,
  "BINDING:CITY_RESET_IN_PROGRESS"
);
export const CITY_RESET_CLEARED = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_RESET_CLEARED"
);
export const CITY_RESET_TOTAL = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_RESET_TOTAL"
);
export const CITY_APPLY_IN_PROGRESS = bindValue<boolean>(
  MOD_ID,
  "BINDING:CITY_APPLY_IN_PROGRESS"
);
export const CITY_APPLY_APPLIED = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_APPLY_APPLIED"
);
export const CITY_APPLY_TOTAL = bindValue<number>(
  MOD_ID,
  "BINDING:CITY_APPLY_TOTAL"
);
export const MARKER_TOOLTIP_TEXT = bindValue<string>(
  MOD_ID,
  "BINDING:MARKER_TOOLTIP_TEXT"
);
export const MARKER_TOOLTIP_X = bindValue<number>(
  MOD_ID,
  "BINDING:MARKER_TOOLTIP_X"
);
export const MARKER_TOOLTIP_Y = bindValue<number>(
  MOD_ID,
  "BINDING:MARKER_TOOLTIP_Y"
);
export const SELECTION_CLICK_X = bindValue<number>(
  MOD_ID,
  "BINDING:SELECTION_CLICK_X"
);
export const SELECTION_CLICK_Y = bindValue<number>(
  MOD_ID,
  "BINDING:SELECTION_CLICK_Y"
);
export const TOOL_PANEL_X = bindValue<number>(
  MOD_ID,
  "BINDING:TOOL_PANEL_X"
);
export const TOOL_PANEL_Y = bindValue<number>(
  MOD_ID,
  "BINDING:TOOL_PANEL_Y"
);

export function ApplySpeed(speed: number) {
  trigger(MOD_ID, "TRIGGER:APPLY_SPEED", speed);
}

export function ApplySelectionMultiplier(multiplier: number) {
  trigger(MOD_ID, "TRIGGER:APPLY_SELECTION_MULTIPLIER", multiplier);
}

export function ApplyCityRoadGroupSpeed(group: number, speed: number) {
  trigger(MOD_ID, "TRIGGER:APPLY_CITY_ROAD_GROUP_SPEED", group, speed);
}

export function ApplyCityTrainSpeed(speed: number) {
  trigger(MOD_ID, "TRIGGER:APPLY_CITY_TRAIN_SPEED", speed);
}

export function ApplyCitySubwaySpeed(speed: number) {
  trigger(MOD_ID, "TRIGGER:APPLY_CITY_SUBWAY_SPEED", speed);
}

export function ResetSpeed() {
  trigger(MOD_ID, "TRIGGER:RESET_SPEED");
}

export function ResetCityRoadDefaults() {
  trigger(MOD_ID, "TRIGGER:RESET_CITY_ROADS");
}

export function ResetCityRailDefaults() {
  trigger(MOD_ID, "TRIGGER:RESET_CITY_RAILS");
}

export function ResetCityWaterwayDefaults() {
  trigger(MOD_ID, "TRIGGER:RESET_CITY_WATERWAYS");
}

export function ResetCityAllDefaults() {
  trigger(MOD_ID, "TRIGGER:RESET_CITY_ALL");
}

export function ToggleUnit() {
  trigger(MOD_ID, "TRIGGER:TOGGLE_UNIT");
}

export function SetPanelSpeedUnit(showMetric: boolean) {
  trigger(MOD_ID, "TRIGGER:SET_PANEL_SPEED_UNIT", showMetric);
}

export function SetPanelTooltipsEnabled(enabled: boolean) {
  trigger(MOD_ID, "TRIGGER:SET_PANEL_TOOLTIPS_ENABLED", enabled);
}

export function SetHideSpeedMarkers(hidden: boolean) {
  trigger(MOD_ID, "TRIGGER:SET_HIDE_SPEED_MARKERS", hidden);
}

export function SetStatsExpanded(expanded: boolean) {
  trigger(MOD_ID, "TRIGGER:SET_STATS_EXPANDED", expanded);
}

export function SaveToolPanelPosition(x: number, y: number) {
  trigger(MOD_ID, "TRIGGER:SET_TOOL_PANEL_POSITION", Math.round(x), Math.round(y));
}

export function ActivateTool() {
  SetToolActive(true);
}

export function SetToolActive(active: boolean) {
  trigger(MOD_ID, "TRIGGER:ACTIVATE_TOOL", active);
}

// Selection type filter (roads / rails / water). Default all on = current behavior.
export const SELECT_ROADS = bindValue<boolean>(MOD_ID, "BINDING:SELECT_ROADS");
export const SELECT_RAILS = bindValue<boolean>(MOD_ID, "BINDING:SELECT_RAILS");
export const SELECT_WATER = bindValue<boolean>(MOD_ID, "BINDING:SELECT_WATER");

export function SetSelectRoads(enabled: boolean) {
  trigger(MOD_ID, "TRIGGER:SET_SELECT_ROADS", enabled);
}

export function SetSelectRails(enabled: boolean) {
  trigger(MOD_ID, "TRIGGER:SET_SELECT_RAILS", enabled);
}

export function SetSelectWater(enabled: boolean) {
  trigger(MOD_ID, "TRIGGER:SET_SELECT_WATER", enabled);
}

// Independent draggable screen positions for the hint panel and the tool panel. The hook clamps
// against the real rendered panel size; this stored-position clamp only keeps the top-left anchor
// reachable across resolution changes before the panel has mounted and measured itself.
const PANEL_LEFT_MARGIN_PX = 10;
const PANEL_TOP_MARGIN_PX = 0;
const PANEL_EDGE_MARGIN_PX = 10;

function clampToViewport(position: { x: number; y: number }) {
  const maxX = Math.max(PANEL_LEFT_MARGIN_PX, window.innerWidth - PANEL_EDGE_MARGIN_PX);
  const maxY = Math.max(PANEL_TOP_MARGIN_PX, window.innerHeight - PANEL_EDGE_MARGIN_PX);
  return {
    x: Math.min(Math.max(PANEL_LEFT_MARGIN_PX, position.x), maxX),
    y: Math.min(Math.max(PANEL_TOP_MARGIN_PX, position.y), maxY)
  };
}

function defaultPanelPosition() {
  return { x: window.innerWidth - 600, y: 100 };
}

let hintPanelPosition = clampToViewport(defaultPanelPosition());

export function getHintPanelPosition() {
  hintPanelPosition = clampToViewport(hintPanelPosition);
  return { ...hintPanelPosition };
}

export function setHintPanelPosition(position: { x: number; y: number }) {
  hintPanelPosition = clampToViewport(position);
}

let toolPanelPosition = clampToViewport(defaultPanelPosition());

export function getToolPanelPosition() {
  toolPanelPosition = clampToViewport(toolPanelPosition);
  return { ...toolPanelPosition };
}

export function setToolPanelPosition(position: { x: number; y: number }) {
  toolPanelPosition = clampToViewport(position);
}
