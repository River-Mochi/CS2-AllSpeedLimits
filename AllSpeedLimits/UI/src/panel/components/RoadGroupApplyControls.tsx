// File: UI/src/panel/components/RoadGroupApplyControls.tsx
// Purpose: Whole-city road group chooser and apply button.

import { memo } from "react";
import { Button } from "../../shared/Button";
import type { PanelTooltipKind, RoadGroupKind } from "../types";

type RoadGroupApplyControlsProps = {
    roadGroups: Array<{ key: RoadGroupKind; value: number }>;
    roadGroupTooltipKind: Record<RoadGroupKind, PanelTooltipKind>;
    selectedRoadGroup: RoadGroupKind | null;
    cityActionBusy: boolean;
    cityActionApplying: string | null;
    cityApplyInProgress: boolean;
    applyingLabel: string;
    chooseRoadGroupLabel: string;
    applyRoadGroupLabel: string;
    getRoadGroupLabel: (group: RoadGroupKind) => string;
    getRoadGroupShortLabel: (group: RoadGroupKind) => string;
    setSelectedRoadGroup: (value: RoadGroupKind | null) => void;
    setPendingCityAction: (value: "applyRoadGroup") => void;
    showPanelTooltip: (value: PanelTooltipKind) => void;
    hidePanelTooltip: () => void;
    panelTitle: (value: string) => string | undefined;
    formatCount: (value: number) => string;
    cityApplyApplied: number;
    cityApplyTotal: number;
    focusKey: unknown;
};

export const RoadGroupApplyControls = memo((props: RoadGroupApplyControlsProps) => {
    const {
        roadGroups,
        roadGroupTooltipKind,
        selectedRoadGroup,
        cityActionBusy,
        cityActionApplying,
        cityApplyInProgress,
        applyingLabel,
        chooseRoadGroupLabel,
        applyRoadGroupLabel,
        getRoadGroupLabel,
        getRoadGroupShortLabel,
        setSelectedRoadGroup,
        setPendingCityAction,
        showPanelTooltip,
        hidePanelTooltip,
        panelTitle,
        formatCount,
        cityApplyApplied,
        cityApplyTotal,
        focusKey
    } = props;

    return (
        <div style={{ marginBottom: "8rem" }}>
            <div style={{
                fontSize: "10rem",
                color: "rgba(226, 236, 241, 0.70)",
                marginBottom: "4rem"
            }}>
                {chooseRoadGroupLabel}
            </div>
            <div style={{
                display: "flex",
                marginBottom: "5rem"
            }}>
                {roadGroups.map((group, index) => {
                    const selected = selectedRoadGroup === group.key;

                    return (
                        <div
                            key={group.key}
                            onMouseEnter={() => showPanelTooltip(roadGroupTooltipKind[group.key])}
                            onMouseLeave={hidePanelTooltip}
                            style={{
                                flex: 1,
                                minWidth: "0",
                                marginLeft: index === 0 ? "0" : "3rem"
                            }}
                        >
                            <Button
                                focusKey={focusKey}
                                selected={selected}
                                disabled={cityActionBusy}
                                onSelect={() => setSelectedRoadGroup(selected ? null : group.key)}
                                variant="neutral"
                                style={{
                                    width: "100%",
                                    minHeight: "22rem",
                                    paddingTop: "3rem",
                                    paddingRight: "0",
                                    paddingBottom: "3rem",
                                    paddingLeft: "0",
                                    fontSize: "12rem"
                                }}
                                title={panelTitle(getRoadGroupLabel(group.key))}
                            >
                                {getRoadGroupShortLabel(group.key)}
                            </Button>
                        </div>
                    );
                })}
            </div>
            <div
                onMouseEnter={() => showPanelTooltip("roadGroupApply")}
                onMouseLeave={hidePanelTooltip}
            >
                <Button
                    focusKey={focusKey}
                    selected={cityActionApplying === "applyRoadGroup"}
                    disabled={cityActionBusy || selectedRoadGroup === null}
                    onSelect={() => setPendingCityAction("applyRoadGroup")}
                    variant="city"
                    style={{
                        width: "100%",
                        paddingTop: "4rem",
                        paddingRight: "0",
                        paddingBottom: "4rem",
                        paddingLeft: "0",
                        fontSize: "13rem"
                    }}
                >
                    {cityActionApplying === "applyRoadGroup" || cityApplyInProgress
                        ? applyingLabel
                        : `${applyRoadGroupLabel}: ${selectedRoadGroup === null ? chooseRoadGroupLabel : getRoadGroupLabel(selectedRoadGroup)}`}
                </Button>
            </div>
            {cityApplyInProgress && (
                <div style={{
                    marginTop: "4rem",
                    fontSize: "9rem",
                    color: "rgba(255, 255, 255, 0.72)",
                    textAlign: "center"
                }}>
                    {formatCount(cityApplyApplied)} / {formatCount(cityApplyTotal)}
                </div>
            )}
        </div>
    );
});
