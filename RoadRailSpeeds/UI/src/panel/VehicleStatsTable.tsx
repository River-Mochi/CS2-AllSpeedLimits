// File: UI/src/panel/VehicleStatsTable.tsx
// Purpose: Compact moving/parked/total stats table for bikes, personal cars, and road work vehicles.

import { useState } from "react";
// Prefer stock game icons so the stats rows match the rest of CS2. Bundled icons stay as fallbacks
// in case a game update moves the media path.
import carIconFallback from "../images/GenericVehicle_max.svg";
import bikeIconFallback from "../images/Bicycle_max.svg";
import { STATS_COLUMN_WIDTH_REM, STATS_LABEL_WIDTH_REM } from "./constants";
import type { PanelTooltipKind } from "./types";

const bikeIcon = "Media/Game/Icons/Bicycle.svg";
const carIcon = "Media/Game/Icons/GenericVehicle.svg";
const cargoTruckIcon = "Media/Game/Icons/CargoTruck.svg";

type VehicleStatsTableProps = {
    carsLabel: string;
    bikesLabel: string;
    movingLabel: string;
    parkedLabel: string;
    totalLabel: string;
    cityCarActive: number;
    cityCarParked: number;
    cityCarTotal: number;
    cityBikeActive: number;
    cityBikeParked: number;
    cityBikeTotal: number;
    industryLabel: string;
    cityIndustryActive: number;
    cityIndustryParked: number;
    cityIndustryTotal: number;
    showPanelTooltip: (kind: PanelTooltipKind) => void;
    hidePanelTooltip: () => void;
    formatCount: (value: number) => string;
};

export const VehicleStatsTable = (props: VehicleStatsTableProps) => {
    const {
        carsLabel,
        bikesLabel,
        movingLabel,
        parkedLabel,
        totalLabel,
        cityCarActive,
        cityCarParked,
        cityCarTotal,
        cityBikeActive,
        cityBikeParked,
        cityBikeTotal,
        industryLabel,
        cityIndustryActive,
        cityIndustryParked,
        cityIndustryTotal,
        showPanelTooltip,
        hidePanelTooltip,
        formatCount
    } = props;

    const [hoveredRowKey, setHoveredRowKey] = useState<string | null>(null);
    // Column geometry is shared with the Stats section header so the Moving/Parked/Total labels
    // (now rendered on the section title line) line up with the number columns here.
    const labelWidth = STATS_LABEL_WIDTH_REM;
    const statColumnWidth = STATS_COLUMN_WIDTH_REM;
    const columns = [
        { label: movingLabel, cars: cityCarActive, bikes: cityBikeActive, industry: cityIndustryActive },
        { label: parkedLabel, cars: cityCarParked, bikes: cityBikeParked, industry: cityIndustryParked },
        { label: totalLabel, cars: cityCarTotal, bikes: cityBikeTotal, industry: cityIndustryTotal }
    ];

    // iconIdleRem/iconHoverRem are per-row: the bicycle art is thinner than the chunky car, so the
    // bike gets a slightly larger box to read as the same visual size. The number font is identical
    // for both rows (see fontSize below), so any size difference you see is the icon, not the text.
    const renderStatsRow = (
        iconSrc: string,
        fallbackIconSrc: string | undefined,
        label: string,
        values: number[],
        rowKey: string,
        tooltipKind: PanelTooltipKind,
        iconSizeRem: number
    ) => {
        const isHovered = hoveredRowKey === rowKey;

        return (
            <div
                onMouseEnter={() => {
                    setHoveredRowKey(rowKey);
                    showPanelTooltip(tooltipKind);
                }}
                onMouseLeave={() => {
                    setHoveredRowKey(null);
                    hidePanelTooltip();
                }}
                style={{
                    display: "flex",
                    alignItems: "center",
                    minHeight: "23rem"
                }}
            >
                <div
                    title={label}
                    style={{
                        width: `${labelWidth}rem`,
                        minWidth: `${labelWidth}rem`,
                        display: "flex",
                        alignItems: "center"
                    }}
                >
                    <img
                        src={iconSrc}
                        alt={label}
                        onError={(event) => {
                            if (fallbackIconSrc !== undefined && event.currentTarget.getAttribute("src") !== fallbackIconSrc) {
                                event.currentTarget.setAttribute("src", fallbackIconSrc);
                            }
                        }}
                        style={{
                            width: `${iconSizeRem}rem`,
                            height: `${iconSizeRem}rem`,
                            opacity: isHovered ? 1 : 0.85
                        }}
                    />
                </div>
                {/* Spacer pushes the number columns to the right edge so they line up with the
                    Moving/Parked/Total headers on the section title line (which sit left of the chevron). */}
                <div style={{ flex: 1, minWidth: "0" }} />
                <div style={{
                    display: "flex",
                    alignItems: "center",
                    width: `${statColumnWidth * columns.length}rem`,
                    minWidth: `${statColumnWidth * columns.length}rem`,
                    minHeight: "23rem",
                    backgroundColor: isHovered ? "rgba(255, 255, 255, 0.105)" : "rgba(255, 255, 255, 0.045)",
                    borderRadius: "3rem"
                }}>
                    {values.map((value, index) => (
                        <div
                            key={`${rowKey}-${columns[index].label}`}
                            style={{
                                width: `${statColumnWidth}rem`,
                                minWidth: `${statColumnWidth}rem`,
                                paddingTop: "2rem",
                                paddingRight: "8rem",
                                paddingBottom: "2rem",
                                paddingLeft: "0",
                                textAlign: "right",
                                fontSize: "12.5rem",
                                fontWeight: "bold",
                                color: "rgba(255, 255, 255, 0.92)",
                                whiteSpace: "nowrap"
                            }}
                        >
                            {formatCount(value)}
                        </div>
                    ))}
                </div>
            </div>
        );
    };

    return (
        // paddingRight matches the section header's chevron area (chevron 11rem + 6rem gap) so the
        // right-aligned number columns end on the same vertical line as the column headers above.
        <div style={{ marginTop: "0", paddingRight: "17rem" }}>
            <div style={{
                display: "flex",
                flexDirection: "column"
            }}>
                {renderStatsRow(bikeIcon, bikeIconFallback, bikesLabel, columns.map(column => column.bikes), "bikes", "statsBikes", 23)}
                <div style={{ height: "2rem" }} />
                {renderStatsRow(carIcon, carIconFallback, carsLabel, columns.map(column => column.cars), "cars", "statsCars", 22)}
                <div style={{ height: "2rem" }} />
                {renderStatsRow(cargoTruckIcon, undefined, industryLabel, columns.map(column => column.industry), "industry", "statsIndustry", 24)}
            </div>
        </div>
    );
};
