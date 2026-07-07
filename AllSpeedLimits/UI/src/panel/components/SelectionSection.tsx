// File: UI/src/panel/components/SelectionSection.tsx
// Purpose: Left column of the Selected section — New-speed box (with a click-to-toggle unit) on top,
// Current/Default rows below. The unit text next to the number is the km/h <-> mph toggle.

import { useState } from "react";
import { Button } from "../../shared/Button";

type SelectionSectionProps = {
    focusKey: unknown;
    newSpeedLabel: string;
    newSpeedNumber: string;
    newSpeedUnit: string;
    unitToggleTitle?: string;
    onToggleUnit: () => void;
    currentSpeedTitle?: string;
    gameDefaultTitle?: string;
    currentSpeedLabelText: string;
    currentSpeedValueText: string;
    defaultSpeedLabelText: string;
    defaultSpeedValueText: string;
    onCurrentSpeedMouseEnter: () => void;
    onCurrentSpeedMouseLeave: () => void;
    onGameDefaultMouseEnter: () => void;
    onGameDefaultMouseLeave: () => void;
};

export const SelectionSection = (props: SelectionSectionProps) => {
    const {
        focusKey,
        newSpeedLabel,
        newSpeedNumber,
        newSpeedUnit,
        unitToggleTitle,
        onToggleUnit,
        currentSpeedTitle,
        gameDefaultTitle,
        currentSpeedLabelText,
        currentSpeedValueText,
        defaultSpeedLabelText,
        defaultSpeedValueText,
        onCurrentSpeedMouseEnter,
        onCurrentSpeedMouseLeave,
        onGameDefaultMouseEnter,
        onGameDefaultMouseLeave
    } = props;

    const [unitHovered, setUnitHovered] = useState(false);

    const splitSpeedValue = (value: string) => {
        const parts = value.trim().split(/\s+/);
        if (parts.length >= 2) {
            return {
                number: parts.slice(0, parts.length - 1).join(" "),
                unit: parts[parts.length - 1]
            };
        }

        return {
            number: value,
            unit: ""
        };
    };

    const labelStyle = {
        fontSize: "11.5rem",
        color: "rgba(226, 236, 241, 0.74)",
        width: "42rem",
        minWidth: "42rem",
        marginRight: "4rem",
        whiteSpace: "nowrap" as const,
        textAlign: "left" as const
    };

    const valueStyle = {
        fontSize: "12.2rem",
        fontWeight: 800,
        color: "rgba(255, 255, 255, 0.78)",
        display: "flex" as const,
        justifyContent: "flex-start" as const,
        width: "49rem",
        minWidth: "49rem",
        flexShrink: 0,
        whiteSpace: "nowrap" as const,
        textAlign: "right" as const
    };

    const valueNumberStyle = {
        width: "24rem",
        minWidth: "24rem",
        textAlign: "right" as const
    };

    const valueUnitStyle = {
        width: "22rem",
        minWidth: "22rem",
        marginLeft: "3rem",
        textAlign: "left" as const
    };

    const factRowStyle = {
        display: "flex",
        alignItems: "center" as const,
        justifyContent: "flex-start" as const,
        minHeight: "18rem",
        minWidth: "0"
    };

    const renderSpeedValue = (value: string) => {
        const split = splitSpeedValue(value);

        if (split.unit.length === 0) {
            return (
                <span style={{
                    ...valueStyle,
                    justifyContent: "flex-end" as const
                }}>
                    {split.number}
                </span>
            );
        }

        return (
            <span style={valueStyle}>
                <span style={valueNumberStyle}>{split.number}</span>
                <span style={valueUnitStyle}>{split.unit}</span>
            </span>
        );
    };

    return (
        <div style={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "space-between",
            minHeight: "78rem",
            paddingTop: "4rem",
            paddingRight: "3rem",
            paddingBottom: "3rem",
            paddingLeft: "7rem",
            backgroundColor: "rgba(255, 255, 255, 0.02)",
            borderWidth: "1rem",
            borderStyle: "solid",
            borderColor: "rgba(78, 195, 240, 0.54)",
            borderRadius: "4rem",
            boxSizing: "border-box"
        }}>
            <div style={{
                display: "flex",
                flexDirection: "column",
                justifyContent: "center"
            }}>
                <span style={{
                    fontSize: "11.6rem",
                    fontWeight: 800,
                    color: "rgba(78, 215, 255, 0.98)",
                    whiteSpace: "nowrap",
                    lineHeight: "1.05"
                }}>
                    {newSpeedLabel}
                </span>
                <div style={{ display: "flex", alignItems: "flex-end", marginTop: "1rem" }}>
                    <span style={{
                        fontSize: "20rem",
                        fontWeight: "bold",
                        color: "#fff",
                        whiteSpace: "nowrap",
                        lineHeight: "1.05",
                        minWidth: "0"
                    }}>
                        {newSpeedNumber}
                    </span>
                    {/* The unit text is the km/h <-> mph toggle. */}
                    <Button
                        focusKey={focusKey}
                        variant="neutral"
                        onSelect={onToggleUnit}
                        onMouseEnter={() => setUnitHovered(true)}
                        onMouseLeave={() => setUnitHovered(false)}
                        title={unitToggleTitle}
                        style={{
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            height: "20rem",
                            minHeight: "20rem",
                            minWidth: "41rem",
                            marginLeft: "7rem",
                            marginBottom: "1rem",
                            paddingTop: "0",
                            paddingRight: "4rem",
                            paddingBottom: "0",
                            paddingLeft: "4rem",
                            backgroundColor: unitHovered ? "rgba(255, 255, 255, 0.09)" : "transparent",
                            borderWidth: "1rem",
                            borderStyle: "solid",
                            borderColor: unitHovered ? "rgba(255, 255, 255, 0.24)" : "rgba(255, 255, 255, 0.11)",
                            borderRadius: "3rem",
                            boxSizing: "border-box",
                            fontSize: unitHovered ? "16.9rem" : "16.2rem",
                            fontWeight: 800,
                            color: unitHovered ? "rgba(255, 255, 255, 0.98)" : "rgba(255, 255, 255, 0.78)",
                            lineHeight: "1"
                        }}
                    >
                        {newSpeedUnit}
                    </Button>
                </div>
            </div>

            {/* Current / Default, tied to New speed inside the same outlined group. */}
            <div style={{ marginTop: "8rem", paddingLeft: "0", paddingRight: "1rem" }}>
                <div
                    onMouseEnter={onCurrentSpeedMouseEnter}
                    onMouseLeave={onCurrentSpeedMouseLeave}
                    title={currentSpeedTitle}
                    style={factRowStyle}
                >
                    <span style={labelStyle}>{currentSpeedLabelText}</span>
                    {renderSpeedValue(currentSpeedValueText)}
                </div>
                <div
                    onMouseEnter={onGameDefaultMouseEnter}
                    onMouseLeave={onGameDefaultMouseLeave}
                    title={gameDefaultTitle}
                    style={factRowStyle}
                >
                    <span style={labelStyle}>{defaultSpeedLabelText}</span>
                    {renderSpeedValue(defaultSpeedValueText)}
                </div>
            </div>
        </div>
    );
};
