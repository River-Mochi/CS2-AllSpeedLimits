// File: UI/src/panel/components/CityResetControls.tsx
// Purpose: Whole-city reset buttons for roads, rails, waterways, and all custom speeds.

import { Button } from "../../shared/Button";

type ResetActionKind = "resetRoads" | "resetRails" | "resetWater" | "resetAll";

type CityResetControlsProps = {
    cityActionBusy: boolean;
    cityActionApplying: ResetActionKind | null | string;
    resettingLabel: string;
    resetAllLabel: string;
    resetRoadsLabel: string;
    resetRailsLabel: string;
    resetWaterLabel: string;
    resetAllTypesLabel: string;
    focusKey: unknown;
    onOpenResetRoads: () => void;
    onOpenResetRails: () => void;
    onOpenResetWater: () => void;
    onOpenResetAll: () => void;
};

export const CityResetControls = (props: CityResetControlsProps) => {
    const {
        cityActionBusy,
        cityActionApplying,
        resettingLabel,
        resetAllLabel,
        resetRoadsLabel,
        resetRailsLabel,
        resetWaterLabel,
        resetAllTypesLabel,
        focusKey,
        onOpenResetRoads,
        onOpenResetRails,
        onOpenResetWater,
        onOpenResetAll
    } = props;

    const renderResetButton = (
        action: ResetActionKind,
        label: string,
        onSelect: () => void,
        addRightMargin: boolean
    ) => (
        <div style={{
            flex: 1,
            minWidth: "0",
            marginRight: addRightMargin ? "3rem" : "0"
        }}>
            <Button
                focusKey={focusKey}
                selected={cityActionApplying === action}
                disabled={cityActionBusy}
                onSelect={onSelect}
                variant="danger"
                style={{
                    width: "100%",
                    paddingTop: "4rem",
                    paddingRight: "0",
                    paddingBottom: "4rem",
                    paddingLeft: "0"
                }}
            >
                {cityActionApplying === action ? resettingLabel : label}
            </Button>
        </div>
    );

    return (
        <div>
            <div style={{
                fontSize: "10rem",
                color: "rgba(226, 236, 241, 0.70)",
                marginBottom: "4rem"
            }}>
                {resetAllLabel}
            </div>
            <div style={{
                display: "flex",
                justifyContent: "space-between"
            }}>
                {renderResetButton("resetRoads", resetRoadsLabel, onOpenResetRoads, true)}
                {renderResetButton("resetRails", resetRailsLabel, onOpenResetRails, true)}
                {renderResetButton("resetWater", resetWaterLabel, onOpenResetWater, true)}
                {renderResetButton("resetAll", resetAllTypesLabel, onOpenResetAll, false)}
            </div>
        </div>
    );
};
