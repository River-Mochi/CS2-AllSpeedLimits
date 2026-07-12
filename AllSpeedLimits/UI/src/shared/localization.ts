// File: UI/src/shared/localization.ts
// Purpose: In-city UI localization hook backed by CS2 translate() with English JSON fallback.

import enUS from "../../../lang/en-US.json";
import { useLocalization } from "cs2/l10n";
import mod from "mod.json";

const KEY_PREFIX = `${mod.id}.UI`;
const enFallback = enUS as Record<string, string | null>;
type TextLookup = (key: string) => string | null;

const fallbackOptional = (key: string): string | null => enFallback[key] ?? null;

const text = (lookup: TextLookup, key: string): string => {
    return lookup(key) ?? fallbackOptional(key) ?? key;
};

const optionalText = (lookup: TextLookup, key: string): string | null => {
    const value = lookup(key) ?? fallbackOptional(key);
    return value === undefined || value === null || value === "" ? null : value;
};

const lines = (lookup: TextLookup, prefix: string): string[] => {
    const value = optionalText(lookup, `${prefix}.text`);
    return value === null ? [] : value.split("\n");
};

const splitLines = (value: string | null): string[] => value === null ? [] : value.split("\n");

const buildText = (lookup: TextLookup) => ({
    inCity: {
        toolbar: {
            tooltip: text(lookup, "toolbar.tooltip")
        },
        hint: {
            title: text(lookup, "hint.title"),
            instruction: text(lookup, "hint.instruction")
        },
        panel: {
            title: text(lookup, "panel.title"),
            selectedSegment: text(lookup, "panel.selectedSegment"),
            firstSegment: text(lookup, "panel.firstSegment"),
            currentSpeed: text(lookup, "panel.currentSpeed"),
            gameDefault: text(lookup, "panel.gameDefault"),
            newSpeedLimit: text(lookup, "panel.newSpeedLimit"),
            notAvailable: text(lookup, "panel.notAvailable"),
            mixed: text(lookup, "panel.mixed"),
            presets: text(lookup, "panel.presets"),
            custom: text(lookup, "panel.custom"),
            wholeCity: text(lookup, "panel.wholeCity"),
            stats: text(lookup, "panel.stats"),
            selectFilter: text(lookup, "panel.selectFilter"),
            filterRoads: text(lookup, "panel.filterRoads"),
            filterRails: text(lookup, "panel.filterRails"),
            filterWater: text(lookup, "panel.filterWater"),
            working: text(lookup, "panel.working"),
            resetting: text(lookup, "panel.resetting"),
            applying: text(lookup, "panel.applying"),
            unitAuto: text(lookup, "panel.unitAuto"),
            unitMetric: text(lookup, "panel.unitMetric"),
            unitImperial: text(lookup, "panel.unitImperial")
        },
        reminder: {
            saveAfterReset: text(lookup, "reminder.saveAfterReset"),
            dismiss: text(lookup, "reminder.dismiss")
        },
        stats: {
            cars: text(lookup, "stats.cars.label"),
            bikes: text(lookup, "stats.bikes.label"),
            industry: text(lookup, "stats.industry.label"),
            buses: text(lookup, "stats.buses.label"),
            taxis: text(lookup, "stats.taxis.label"),
            total: text(lookup, "stats.total"),
            moving: text(lookup, "stats.moving"),
            parked: text(lookup, "stats.parked")
        },
        buttons: {
            apply: text(lookup, "buttons.apply"),
            applied: text(lookup, "buttons.applied"),
            reset: text(lookup, "buttons.reset"),
            resetDone: text(lookup, "buttons.resetDone"),
            resetAll: text(lookup, "buttons.resetAll"),
            resetAllTypes: text(lookup, "buttons.resetAllTypes"),
            chooseRoadGroup: text(lookup, "buttons.chooseRoadGroup"),
            applyRoadGroup: text(lookup, "buttons.applyRoadGroup"),
            applyTrain: text(lookup, "buttons.applyTrain"),
            applySubway: text(lookup, "buttons.applySubway"),
            resetRoads: text(lookup, "buttons.resetRoads"),
            resetRails: text(lookup, "buttons.resetRails"),
            resetWater: text(lookup, "buttons.resetWater"),
            expandAll: text(lookup, "buttons.expandAll"),
            collapseAll: text(lookup, "buttons.collapseAll"),
            cancel: text(lookup, "buttons.cancel"),
            close: text(lookup, "buttons.close")
        },
        tooltips: {
            panelTitle: text(lookup, "tooltips.panelTitle"),
            draggable: text(lookup, "tooltips.draggable"),
            selectedSegment: text(lookup, "tooltips.selectedSegment"),
            currentSpeed: text(lookup, "tooltips.currentSpeed"),
            presets: text(lookup, "tooltips.presets"),
            filterRoads: text(lookup, "tooltips.filterRoads"),
            filterRails: text(lookup, "tooltips.filterRails"),
            filterWater: text(lookup, "tooltips.filterWater"),
            gameDefault: {
                title: text(lookup, "tooltips.gameDefault.title"),
                lines: lines(lookup, "tooltips.gameDefault")
            },
            presetUnlimited: {
                title: text(lookup, "tooltips.presetUnlimited.title"),
                lines: lines(lookup, "tooltips.presetUnlimited")
            },
            resetAll: {
                title: text(lookup, "tooltips.resetAll.title"),
                lines: lines(lookup, "tooltips.resetAll")
            },
            roadGroupApply: {
                title: text(lookup, "tooltips.roadGroupApply.title"),
                lines: lines(lookup, "tooltips.roadGroupApply")
            },
            roadGroups: {
                small: {
                    title: text(lookup, "tooltips.roadGroups.small.title"),
                    lines: lines(lookup, "tooltips.roadGroups.small")
                },
                medium: {
                    title: text(lookup, "tooltips.roadGroups.medium.title"),
                    lines: lines(lookup, "tooltips.roadGroups.medium")
                },
                large: {
                    title: text(lookup, "tooltips.roadGroups.large.title"),
                    lines: lines(lookup, "tooltips.roadGroups.large")
                },
                highway: {
                    title: text(lookup, "tooltips.roadGroups.highway.title"),
                    lines: lines(lookup, "tooltips.roadGroups.highway")
                }
            },
            stats: {
                title: text(lookup, "tooltips.stats.title"),
                lines: lines(lookup, "tooltips.stats")
            },
            statsRows: {
                bikes: text(lookup, "stats.bikes.tooltip"),
                cars: text(lookup, "stats.cars.tooltip"),
                industry: text(lookup, "stats.industry.tooltip"),
                buses: text(lookup, "stats.buses.tooltip"),
                taxis: text(lookup, "stats.taxis.tooltip")
            },
            wholeCity: {
                title: text(lookup, "tooltips.wholeCity.title"),
                lines: lines(lookup, "tooltips.wholeCity")
            },
            unit: {
                title: text(lookup, "tooltips.unit.title"),
                lines: lines(lookup, "tooltips.unit")
            },
            newSpeedUnit: text(lookup, "tooltips.newSpeedUnit"),
            applyTrain: text(lookup, "tooltips.applyTrain"),
            applySubway: text(lookup, "tooltips.applySubway"),
            preciseStepper: text(lookup, "tooltips.preciseStepper"),
            decreaseTarget: text(lookup, "tooltips.decreaseTarget"),
            increaseTarget: text(lookup, "tooltips.increaseTarget"),
            apply: text(lookup, "tooltips.apply"),
            reset: text(lookup, "tooltips.reset"),
            markersHide: text(lookup, "tooltips.markersHide"),
            markersShow: text(lookup, "tooltips.markersShow")
        },
        help: {
            // Guide directions stay together in one JSON value; preset buttons have their own tooltips.
            directions: splitLines(optionalText(lookup, "help.directions")),
            firstSegmentHint: text(lookup, "help.firstSegmentHint"),
            tooltipsOn: text(lookup, "help.tooltipsOn"),
            tooltipsOff: text(lookup, "help.tooltipsOff"),
            unitTitle: text(lookup, "help.unitTitle"),
            unitLine: text(lookup, "help.unitLine")
        },
        confirm: {
            applyRoadGroup: {
                title: text(lookup, "confirm.applyRoadGroup.title"),
                messageStart: text(lookup, "confirm.applyRoadGroup.messageStart"),
                messageMiddle: text(lookup, "confirm.applyRoadGroup.messageMiddle"),
                confirmLabel: text(lookup, "confirm.applyRoadGroup.confirmLabel")
            },
            applyTrain: {
                title: text(lookup, "confirm.applyTrain.title"),
                message: text(lookup, "confirm.applyTrain.message"),
                confirmLabel: text(lookup, "confirm.applyTrain.confirmLabel")
            },
            applySubway: {
                title: text(lookup, "confirm.applySubway.title"),
                message: text(lookup, "confirm.applySubway.message"),
                confirmLabel: text(lookup, "confirm.applySubway.confirmLabel")
            },
            resetRoads: {
                title: text(lookup, "confirm.resetRoads.title"),
                message: text(lookup, "confirm.resetRoads.message"),
                confirmLabel: text(lookup, "confirm.resetRoads.confirmLabel")
            },
            resetRails: {
                title: text(lookup, "confirm.resetRails.title"),
                message: text(lookup, "confirm.resetRails.message"),
                confirmLabel: text(lookup, "confirm.resetRails.confirmLabel")
            },
            resetWater: {
                title: text(lookup, "confirm.resetWater.title"),
                message: text(lookup, "confirm.resetWater.message"),
                confirmLabel: text(lookup, "confirm.resetWater.confirmLabel")
            },
            resetAll: {
                title: text(lookup, "confirm.resetAll.title"),
                message: text(lookup, "confirm.resetAll.message"),
                confirmLabel: text(lookup, "confirm.resetAll.confirmLabel")
            }
        },
        roadGroups: {
            small: text(lookup, "roadGroups.small"),
            medium: text(lookup, "roadGroups.medium"),
            large: text(lookup, "roadGroups.large"),
            highway: text(lookup, "roadGroups.highway"),
            smallShort: text(lookup, "roadGroups.smallShort"),
            mediumShort: text(lookup, "roadGroups.mediumShort"),
            largeShort: text(lookup, "roadGroups.largeShort"),
            highwayShort: text(lookup, "roadGroups.highwayShort")
        }
    }
} as const);

export const UI_TEXT = buildText(() => null);
export type UIText = typeof UI_TEXT;

export const useText = (): UIText => {
    const { translate } = useLocalization();

    return buildText((key: string): string | null => {
        const translated = translate(`${KEY_PREFIX}.${key}`);
        return translated ?? null;
    });
};
