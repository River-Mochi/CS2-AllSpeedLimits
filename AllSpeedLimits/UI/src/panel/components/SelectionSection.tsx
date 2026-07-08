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
    onUnitMouseEnter: () => void;
    onUnitMouseLeave: () => void;
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
        onUnitMouseEnter,
        onUnitMouseLeave,
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
    const detailValueColor = "rgba(255, 255, 255, 0.74)";
    const unitToggleIdleColor = "rgba(255, 255, 255, 0.86)";

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
        width: "47rem",
        minWidth: "47rem",
        marginRight: "3rem",
        whiteSpace: "nowrap" as const,
        textAlign: "left" as const
    };

    const valueStyle = {
        fontSize: "12.2rem",
        fontWeight: 800,
        color: detailValueColor,
        display: "flex" as const,
        justifyContent: "flex-start" as const,
        width: "49rem",
        minWidth: "49rem",
        flexShrink: 0,
        whiteSpace: "nowrap" as const,
        textAlign: "right" as const
    };

    const valueNumberStyle = {
        width: "25rem",
        minWidth: "25rem",
        color: detailValueColor,
        display: "flex" as const,
        justifyContent: "flex-end" as const,
        textAlign: "right" as const
    };

    const valueUnitStyle = {
        width: "22rem",
        minWidth: "22rem",
        marginLeft: "2rem",
        color: detailValueColor,
        textAlign: "left" as const
    };

    const factRowStyle = {
        display: "flex",
        alignItems: "center" as const,
        justifyContent: "flex-start" as const,
        minHeight: "16rem",
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
            justifyContent: "flex-start",
            height: "84rem",
            minHeight: "84rem",
            paddingTop: "3rem",
            paddingRight: "3rem",
            paddingBottom: "2rem",
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
                <div style={{
                    position: "relative",
                    display: "flex",
                    alignItems: "flex-end",
                    minHeight: "21rem",
                    marginTop: "4rem"
                }}>
                    <span style={{
                        display: "flex",
                        justifyContent: "flex-end",
                        fontSize: "20rem",
                        fontWeight: "bold",
                        color: "#fff",
                        whiteSpace: "nowrap",
                        lineHeight: "1.05",
                        width: "36rem"
                    }}>
                        {newSpeedNumber}
                    </span>
                    {/* The unit text is the km/h <-> mph toggle. */}
                    <Button
                        focusKey={focusKey}
                        variant="neutral"
                        onSelect={onToggleUnit}
                        onMouseEnter={() => {
                            setUnitHovered(true);
                            onUnitMouseEnter();
                        }}
                        onMouseLeave={() => {
                            setUnitHovered(false);
                            onUnitMouseLeave();
                        }}
                        title={unitToggleTitle}
                        style={{
                            display: "flex",
                            position: "absolute",
                            left: "55.5rem",
                            bottom: "0",
                            alignItems: "center",
                            justifyContent: "center",
                            height: "20rem",
                            minHeight: "20rem",
                            width: "45rem",
                            minWidth: "45rem",
                            marginLeft: "0",
                            marginBottom: "0",
                            paddingTop: "0",
                            paddingRight: "0",
                            paddingBottom: "0",
                            paddingLeft: "0",
                            backgroundColor: unitHovered ? "rgba(255, 255, 255, 0.09)" : "transparent",
                            borderWidth: "1rem",
                            borderStyle: "solid",
                            borderColor: unitHovered ? "rgba(255, 255, 255, 0.24)" : "rgba(255, 255, 255, 0.11)",
                            borderRadius: "3rem",
                            boxSizing: "border-box",
                            fontSize: unitHovered ? "16.9rem" : "16.2rem",
                            fontWeight: 800,
                            color: unitHovered ? "rgba(255, 255, 255, 0.98)" : unitToggleIdleColor,
                            lineHeight: "1"
                        }}
                    >
                        {newSpeedUnit}
                    </Button>
                </div>
            </div>

            {/* Current / Default, tied to New speed inside the same outlined group. */}
            <div style={{ marginTop: "4rem", paddingLeft: "0", paddingRight: "1rem" }}>
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
