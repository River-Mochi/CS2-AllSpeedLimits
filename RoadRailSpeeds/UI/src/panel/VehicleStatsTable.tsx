// File: UI/src/panel/VehicleStatsTable.tsx
// Purpose: Compact moving/parked/total stats table for cars, bikes, and industry delivery trucks.

import { useState } from "react";
// Bundled "_max" icons: same game art with the glyph enlarged inside the same square so it reads
// bigger here (the stock game icons have transparent padding we can't trim from CSS). These replaced
// the Coches/Bicis text labels, which overflowed in some languages ("Bisikletler", "Bicicletas").
// Bundled = always present, so no runtime game-path fallback is needed.
import carIcon from "../images/GenericVehicle_max.svg";
import bikeIcon from "../images/Bicycle_max.svg";
import { STATS_COLUMN_WIDTH_REM, STATS_LABEL_WIDTH_REM } from "./constants";

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
        label: string,
        values: number[],
        rowKey: string,
        iconIdleRem: number,
        iconHoverRem: number
    ) => {
        const isHovered = hoveredRowKey === rowKey;

        return (
            <div
                onMouseEnter={() => setHoveredRowKey(rowKey)}
                onMouseLeave={() => setHoveredRowKey(null)}
                style={{
                    display: "flex",
                    alignItems: "center",
                    minHeight: isHovered ? "27rem" : "23rem"
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
                        style={{
                            // Per-row size (car 22/25, bike 23/26) so both icons read the same despite
                            // the bike's thinner art. Kept within the 23/27 row so heights stay even.
                            width: isHovered ? `${iconHoverRem}rem` : `${iconIdleRem}rem`,
                            height: isHovered ? `${iconHoverRem}rem` : `${iconIdleRem}rem`,
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
                    minHeight: isHovered ? "27rem" : "23rem",
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
                                fontSize: isHovered ? "14rem" : "12rem",
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
                {renderStatsRow(carIcon, carsLabel, columns.map(column => column.cars), "cars", 22, 25)}
                <div style={{ height: "2rem" }} />
                {renderStatsRow(bikeIcon, bikesLabel, columns.map(column => column.bikes), "bikes", 23, 26)}
                <div style={{ height: "2rem" }} />
                {renderStatsRow(carIcon, industryLabel, columns.map(column => column.industry), "industry", 22, 25)}
            </div>
        </div>
    );
};
