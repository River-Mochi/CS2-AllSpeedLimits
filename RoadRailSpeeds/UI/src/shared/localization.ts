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

const lines = (lookup: TextLookup, prefix: string, count: number): string[] => {
    const values: string[] = [];

    for (let index = 1; index <= count; index++) {
        const value = optionalText(lookup, `${prefix}.line${index}`);
        if (value !== null) {
            values.push(value);
        }
    }

    return values;
};

const buildText = (lookup: TextLookup) => ({
    inCity: {
        toolbar: {
            tooltip: text(lookup, "inCity.toolbar.tooltip")
        },
        hint: {
            title: text(lookup, "inCity.hint.title"),
            instruction: text(lookup, "inCity.hint.instruction")
        },
        panel: {
            title: text(lookup, "inCity.panel.title"),
            selectedSegment: text(lookup, "inCity.panel.selectedSegment"),
            firstSegment: text(lookup, "inCity.panel.firstSegment"),
            currentSpeed: text(lookup, "inCity.panel.currentSpeed"),
            gameDefault: text(lookup, "inCity.panel.gameDefault"),
            newSpeedLimit: text(lookup, "inCity.panel.newSpeedLimit"),
            notAvailable: text(lookup, "inCity.panel.notAvailable"),
            mixed: text(lookup, "inCity.panel.mixed"),
            presets: text(lookup, "inCity.panel.presets"),
            wholeCity: text(lookup, "inCity.panel.wholeCity"),
            stats: text(lookup, "inCity.panel.stats"),
            selectFilter: text(lookup, "inCity.panel.selectFilter"),
            filterRoads: text(lookup, "inCity.panel.filterRoads"),
            filterRails: text(lookup, "inCity.panel.filterRails"),
            filterWater: text(lookup, "inCity.panel.filterWater"),
            working: text(lookup, "inCity.panel.working"),
            resetting: text(lookup, "inCity.panel.resetting"),
            applying: text(lookup, "inCity.panel.applying"),
            unitAuto: text(lookup, "inCity.panel.unitAuto"),
            unitMetric: text(lookup, "inCity.panel.unitMetric"),
            unitImperial: text(lookup, "inCity.panel.unitImperial")
        },
        reminder: {
            saveAfterReset: text(lookup, "inCity.reminder.saveAfterReset"),
            dismiss: text(lookup, "inCity.reminder.dismiss")
        },
        stats: {
            cars: text(lookup, "inCity.stats.cars"),
            bikes: text(lookup, "inCity.stats.bikes"),
            total: text(lookup, "inCity.stats.total"),
            moving: text(lookup, "inCity.stats.moving"),
            parked: text(lookup, "inCity.stats.parked")
        },
        buttons: {
            slower: text(lookup, "inCity.buttons.slower"),
            faster: text(lookup, "inCity.buttons.faster"),
            apply: text(lookup, "inCity.buttons.apply"),
            applied: text(lookup, "inCity.buttons.applied"),
            reset: text(lookup, "inCity.buttons.reset"),
            resetDone: text(lookup, "inCity.buttons.resetDone"),
            resetAll: text(lookup, "inCity.buttons.resetAll"),
            resetAllTypes: text(lookup, "inCity.buttons.resetAllTypes"),
            chooseRoadGroup: text(lookup, "inCity.buttons.chooseRoadGroup"),
            applyRoadGroup: text(lookup, "inCity.buttons.applyRoadGroup"),
            applyTrain: text(lookup, "inCity.buttons.applyTrain"),
            applySubway: text(lookup, "inCity.buttons.applySubway"),
            resetRoads: text(lookup, "inCity.buttons.resetRoads"),
            resetRails: text(lookup, "inCity.buttons.resetRails"),
            resetWater: text(lookup, "inCity.buttons.resetWater"),
            cancel: text(lookup, "inCity.buttons.cancel"),
            close: text(lookup, "inCity.buttons.close")
        },
        tooltips: {
            panelTitle: text(lookup, "inCity.tooltips.panelTitle"),
            currentSpeed: text(lookup, "inCity.tooltips.currentSpeed"),
            filterRoads: text(lookup, "inCity.tooltips.filterRoads"),
            filterRails: text(lookup, "inCity.tooltips.filterRails"),
            filterWater: text(lookup, "inCity.tooltips.filterWater"),
            gameDefault: {
                title: text(lookup, "inCity.tooltips.gameDefault.title"),
                lines: lines(lookup, "inCity.tooltips.gameDefault", 3)
            },
            presetUnlimited: {
                title: text(lookup, "inCity.tooltips.presetUnlimited.title"),
                lines: lines(lookup, "inCity.tooltips.presetUnlimited", 2)
            },
            resetAll: {
                title: text(lookup, "inCity.tooltips.resetAll.title"),
                lines: lines(lookup, "inCity.tooltips.resetAll", 2)
            },
            roadGroupApply: {
                title: text(lookup, "inCity.tooltips.roadGroupApply.title"),
                lines: lines(lookup, "inCity.tooltips.roadGroupApply", 2)
            },
            roadGroups: {
                small: {
                    title: text(lookup, "inCity.tooltips.roadGroups.small.title"),
                    lines: lines(lookup, "inCity.tooltips.roadGroups.small", 2)
                },
                medium: {
                    title: text(lookup, "inCity.tooltips.roadGroups.medium.title"),
                    lines: lines(lookup, "inCity.tooltips.roadGroups.medium", 2)
                },
                large: {
                    title: text(lookup, "inCity.tooltips.roadGroups.large.title"),
                    lines: lines(lookup, "inCity.tooltips.roadGroups.large", 2)
                },
                highway: {
                    title: text(lookup, "inCity.tooltips.roadGroups.highway.title"),
                    lines: lines(lookup, "inCity.tooltips.roadGroups.highway", 2)
                }
            },
            stats: {
                title: text(lookup, "inCity.tooltips.stats.title"),
                lines: lines(lookup, "inCity.tooltips.stats", 2)
            },
            wholeCity: {
                title: text(lookup, "inCity.tooltips.wholeCity.title"),
                lines: lines(lookup, "inCity.tooltips.wholeCity", 4)
            },
            unit: {
                title: text(lookup, "inCity.tooltips.unit.title"),
                lines: lines(lookup, "inCity.tooltips.unit", 3)
            },
            applyTrain: text(lookup, "inCity.tooltips.applyTrain"),
            applySubway: text(lookup, "inCity.tooltips.applySubway"),
            preciseStepper: text(lookup, "inCity.tooltips.preciseStepper"),
            decreaseTarget: text(lookup, "inCity.tooltips.decreaseTarget"),
            increaseTarget: text(lookup, "inCity.tooltips.increaseTarget"),
            slower: text(lookup, "inCity.tooltips.slower"),
            faster: text(lookup, "inCity.tooltips.faster"),
            apply: text(lookup, "inCity.tooltips.apply"),
            reset: text(lookup, "inCity.tooltips.reset"),
            markersHide: text(lookup, "inCity.tooltips.markersHide"),
            markersShow: text(lookup, "inCity.tooltips.markersShow")
        },
        help: {
            // Guide popup (hover the title-bar speed-limit icon) shows only the action directions.
            // line4 (50% buttons) is intentionally left out: those buttons carry their own tooltips.
            // line5 is shown on the "First segment selected" header (firstSegmentHint), not here.
            directions: [
                optionalText(lookup, "inCity.help.line1"),
                optionalText(lookup, "inCity.help.line2"),
                optionalText(lookup, "inCity.help.line3"),
                optionalText(lookup, "inCity.help.line10")
            ].filter((line): line is string => line !== null),
            firstSegmentHint: text(lookup, "inCity.help.line5"),
            tooltipsOn: text(lookup, "inCity.help.tooltipsOn"),
            tooltipsOff: text(lookup, "inCity.help.tooltipsOff"),
            unitTitle: text(lookup, "inCity.help.unitTitle"),
            unitLine: text(lookup, "inCity.help.unitLine")
        },
        confirm: {
            applyRoadGroup: {
                title: text(lookup, "inCity.confirm.applyRoadGroup.title"),
                messageStart: text(lookup, "inCity.confirm.applyRoadGroup.messageStart"),
                messageMiddle: text(lookup, "inCity.confirm.applyRoadGroup.messageMiddle"),
                confirmLabel: text(lookup, "inCity.confirm.applyRoadGroup.confirmLabel")
            },
            applyTrain: {
                title: text(lookup, "inCity.confirm.applyTrain.title"),
                message: text(lookup, "inCity.confirm.applyTrain.message"),
                confirmLabel: text(lookup, "inCity.confirm.applyTrain.confirmLabel")
            },
            applySubway: {
                title: text(lookup, "inCity.confirm.applySubway.title"),
                message: text(lookup, "inCity.confirm.applySubway.message"),
                confirmLabel: text(lookup, "inCity.confirm.applySubway.confirmLabel")
            },
            resetRoads: {
                title: text(lookup, "inCity.confirm.resetRoads.title"),
                message: text(lookup, "inCity.confirm.resetRoads.message"),
                confirmLabel: text(lookup, "inCity.confirm.resetRoads.confirmLabel")
            },
            resetRails: {
                title: text(lookup, "inCity.confirm.resetRails.title"),
                message: text(lookup, "inCity.confirm.resetRails.message"),
                confirmLabel: text(lookup, "inCity.confirm.resetRails.confirmLabel")
            },
            resetWater: {
                title: text(lookup, "inCity.confirm.resetWater.title"),
                message: text(lookup, "inCity.confirm.resetWater.message"),
                confirmLabel: text(lookup, "inCity.confirm.resetWater.confirmLabel")
            },
            resetAll: {
                title: text(lookup, "inCity.confirm.resetAll.title"),
                message: text(lookup, "inCity.confirm.resetAll.message"),
                confirmLabel: text(lookup, "inCity.confirm.resetAll.confirmLabel")
            }
        },
        roadGroups: {
            small: text(lookup, "inCity.roadGroups.small"),
            medium: text(lookup, "inCity.roadGroups.medium"),
            large: text(lookup, "inCity.roadGroups.large"),
            highway: text(lookup, "inCity.roadGroups.highway"),
            smallShort: text(lookup, "inCity.roadGroups.smallShort"),
            mediumShort: text(lookup, "inCity.roadGroups.mediumShort"),
            largeShort: text(lookup, "inCity.roadGroups.largeShort"),
            highwayShort: text(lookup, "inCity.roadGroups.highwayShort")
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
