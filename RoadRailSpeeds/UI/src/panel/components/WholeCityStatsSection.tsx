// File: UI/src/panel/components/WholeCityStatsSection.tsx
// Purpose: Whole-city speed actions and vehicle stats sections for the speed tool panel.

import type { Dispatch, SetStateAction } from "react";
import { Button } from "../../shared/Button";
import { CollapsibleSectionHeader } from "../CollapsibleSectionHeader";
import { VehicleStatsTable } from "../VehicleStatsTable";
import { ROAD_GROUPS, ROAD_GROUP_TOOLTIP_KIND, STATS_COLUMN_WIDTH_REM } from "../constants";
import type { CityActionKind, PanelTooltipKind, RoadGroupKind } from "../types";
import { CityResetControls } from "./CityResetControls";
import { RoadGroupApplyControls } from "./RoadGroupApplyControls";

type WholeCityStatsSectionProps = {
    text: any;
    wholeCityExpanded: boolean;
    statsExpanded: boolean;
    selectedRoadGroup: RoadGroupKind | null;
    cityActionBusy: boolean;
    cityActionApplying: CityActionKind | null;
    cityApplyInProgress: boolean;
    cityApplyApplied: number;
    cityApplyTotal: number;
    cityResetInProgress: boolean;
    cityResetCleared: number;
    cityResetTotal: number;
    cityCarActive: number;
    cityCarParked: number;
    cityCarTotal: number;
    cityBikeActive: number;
    cityBikeParked: number;
    cityBikeTotal: number;
    setSelectedRoadGroup: Dispatch<SetStateAction<RoadGroupKind | null>>;
    setPendingCityAction: Dispatch<SetStateAction<CityActionKind | null>>;
    toggleWholeCityExpanded: () => void;
    toggleStatsExpanded: () => void;
    getRoadGroupLabel: (group: RoadGroupKind) => string;
    getRoadGroupShortLabel: (group: RoadGroupKind) => string;
    showPanelTooltip: (kind: PanelTooltipKind) => void;
    hidePanelTooltip: () => void;
    panelTitle: (value: string) => string | undefined;
    formatCount: (value: number) => string;
    focusKey: unknown;
};

export const WholeCityStatsSection = (props: WholeCityStatsSectionProps) => {
    const {
        text,
        wholeCityExpanded,
        statsExpanded,
        selectedRoadGroup,
        cityActionBusy,
        cityActionApplying,
        cityApplyInProgress,
        cityApplyApplied,
        cityApplyTotal,
        cityResetInProgress,
        cityResetCleared,
        cityResetTotal,
        cityCarActive,
        cityCarParked,
        cityCarTotal,
        cityBikeActive,
        cityBikeParked,
        cityBikeTotal,
        setSelectedRoadGroup,
        setPendingCityAction,
        toggleWholeCityExpanded,
        toggleStatsExpanded,
        getRoadGroupLabel,
        getRoadGroupShortLabel,
        showPanelTooltip,
        hidePanelTooltip,
        panelTitle,
        formatCount,
        focusKey
    } = props;

    return (
        <>
            <div style={{
                height: "1rem",
                backgroundColor: "rgba(255, 255, 255, 0.08)",
                marginBottom: "8rem"
            }} />
            <div
                onMouseEnter={() => showPanelTooltip("wholeCity")}
                onMouseLeave={hidePanelTooltip}
            >
                <CollapsibleSectionHeader
                    label={text.panel.wholeCity}
                    expanded={wholeCityExpanded}
                    onToggle={toggleWholeCityExpanded}
                />
            </div>
            {wholeCityExpanded && (
                <>
                    <RoadGroupApplyControls
                        roadGroups={ROAD_GROUPS}
                        roadGroupTooltipKind={ROAD_GROUP_TOOLTIP_KIND}
                        selectedRoadGroup={selectedRoadGroup}
                        cityActionBusy={cityActionBusy}
                        cityActionApplying={cityActionApplying}
                        cityApplyInProgress={cityApplyInProgress}
                        applyingLabel={text.panel.applying}
                        chooseRoadGroupLabel={text.buttons.chooseRoadGroup}
                        applyRoadGroupLabel={text.buttons.applyRoadGroup}
                        getRoadGroupLabel={getRoadGroupLabel}
                        getRoadGroupShortLabel={getRoadGroupShortLabel}
                        setSelectedRoadGroup={setSelectedRoadGroup}
                        setPendingCityAction={setPendingCityAction}
                        showPanelTooltip={showPanelTooltip}
                        hidePanelTooltip={hidePanelTooltip}
                        panelTitle={panelTitle}
                        formatCount={formatCount}
                        cityApplyApplied={cityApplyApplied}
                        cityApplyTotal={cityApplyTotal}
                        focusKey={focusKey}
                    />
                    <div style={{ display: "flex", marginBottom: "8rem" }}>
                        <div
                            onMouseEnter={() => showPanelTooltip("applyTrain")}
                            onMouseLeave={hidePanelTooltip}
                            style={{ flex: 1, minWidth: "0" }}
                        >
                            <Button
                                focusKey={focusKey}
                                selected={cityActionApplying === "applyTrain"}
                                disabled={cityActionBusy}
                                onSelect={() => setPendingCityAction("applyTrain")}
                                variant="city"
                                style={{
                                    width: "100%",
                                    paddingTop: "4rem",
                                    paddingRight: "0",
                                    paddingBottom: "4rem",
                                    paddingLeft: "0",
                                    fontSize: "11rem"
                                }}
                            >
                                {cityActionApplying === "applyTrain" || cityApplyInProgress
                                    ? text.panel.applying
                                    : text.buttons.applyTrain}
                            </Button>
                        </div>
                        <div
                            onMouseEnter={() => showPanelTooltip("applySubway")}
                            onMouseLeave={hidePanelTooltip}
                            style={{ flex: 1, minWidth: "0", marginLeft: "6rem" }}
                        >
                            <Button
                                focusKey={focusKey}
                                selected={cityActionApplying === "applySubway"}
                                disabled={cityActionBusy}
                                onSelect={() => setPendingCityAction("applySubway")}
                                variant="city"
                                style={{
                                    width: "100%",
                                    paddingTop: "4rem",
                                    paddingRight: "0",
                                    paddingBottom: "4rem",
                                    paddingLeft: "0",
                                    fontSize: "11rem"
                                }}
                            >
                                {cityActionApplying === "applySubway" || cityApplyInProgress
                                    ? text.panel.applying
                                    : text.buttons.applySubway}
                            </Button>
                        </div>
                    </div>
                    <div
                        onMouseEnter={() => showPanelTooltip("resetAll")}
                        onMouseLeave={hidePanelTooltip}
                    >
                        <CityResetControls
                            cityActionBusy={cityActionBusy}
                            cityActionApplying={cityActionApplying}
                            resettingLabel={text.panel.resetting}
                            resetAllLabel={text.buttons.resetAll}
                            resetRoadsLabel={text.buttons.resetRoads}
                            resetRailsLabel={text.buttons.resetRails}
                            resetWaterLabel={text.buttons.resetWater}
                            resetAllTypesLabel={text.buttons.resetAllTypes}
                            focusKey={focusKey}
                            onOpenResetRoads={() => setPendingCityAction("resetRoads")}
                            onOpenResetRails={() => setPendingCityAction("resetRails")}
                            onOpenResetWater={() => setPendingCityAction("resetWater")}
                            onOpenResetAll={() => setPendingCityAction("resetAll")}
                        />
                    </div>
                    {cityResetInProgress && (
                        <div style={{
                            marginTop: "4rem",
                            fontSize: "9rem",
                            color: "rgba(255, 255, 255, 0.72)",
                            textAlign: "center"
                        }}>
                            {formatCount(cityResetCleared)} / {formatCount(cityResetTotal)}
                        </div>
                    )}
                </>
            )}

            <div
                style={{
                    marginTop: "8rem",
                    paddingTop: "6rem",
                    borderTopWidth: "1rem",
                    borderTopStyle: "solid",
                    borderTopColor: "rgba(255, 255, 255, 0.06)",
                    color: "rgba(255, 255, 255, 0.62)"
                }}
            >
                <div
                    onMouseEnter={() => showPanelTooltip("stats")}
                    onMouseLeave={hidePanelTooltip}
                >
                    <CollapsibleSectionHeader
                        label={text.panel.stats}
                        expanded={statsExpanded}
                        onToggle={toggleStatsExpanded}
                        trailing={statsExpanded ? (
                            <>
                                {[text.stats.moving, text.stats.parked, text.stats.total].map((header: string) => (
                                    <div
                                        key={`stats-col-${header}`}
                                        style={{
                                            width: `${STATS_COLUMN_WIDTH_REM}rem`,
                                            paddingRight: "8rem",
                                            textAlign: "right",
                                            fontSize: "9rem",
                                            fontWeight: 400,
                                            color: "rgba(255, 255, 255, 0.6)",
                                            lineHeight: "1"
                                        }}
                                    >
                                        {header}
                                    </div>
                                ))}
                            </>
                        ) : undefined}
                    />
                </div>
                {statsExpanded && (
                    <VehicleStatsTable
                        carsLabel={text.stats.cars}
                        bikesLabel={text.stats.bikes}
                        movingLabel={text.stats.moving}
                        parkedLabel={text.stats.parked}
                        totalLabel={text.stats.total}
                        cityCarActive={cityCarActive}
                        cityCarParked={cityCarParked}
                        cityCarTotal={cityCarTotal}
                        cityBikeActive={cityBikeActive}
                        cityBikeParked={cityBikeParked}
                        cityBikeTotal={cityBikeTotal}
                        formatCount={formatCount}
                    />
                )}
            </div>
        </>
    );
};
