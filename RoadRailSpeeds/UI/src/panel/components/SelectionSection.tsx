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
        onGameDefaultMouseEnter,
        onGameDefaultMouseLeave
    } = props;

    const [unitHovered, setUnitHovered] = useState(false);

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
        flex: 1,
        whiteSpace: "nowrap" as const,
        textAlign: "right" as const
    };

    const factRowStyle = {
        display: "flex",
        alignItems: "center" as const,
        justifyContent: "flex-start" as const,
        minHeight: "18rem",
        minWidth: "0"
    };

    return (
        <div style={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "space-between",
            minHeight: "78rem",
            paddingTop: "4rem",
            paddingRight: "6rem",
            paddingBottom: "5rem",
            paddingLeft: "8rem",
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
                    fontSize: "11rem",
                    fontWeight: 800,
                    color: "rgba(78, 215, 255, 0.98)",
                    whiteSpace: "nowrap",
                    lineHeight: "1.05"
                }}>
                    {newSpeedLabel}
                </span>
                <div style={{ display: "flex", alignItems: "flex-end", marginTop: "1rem" }}>
                    <span style={{
                        fontSize: "18.5rem",
                        fontWeight: "bold",
                        color: "#fff",
                        whiteSpace: "nowrap",
                        lineHeight: "1.1"
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
                            height: "19rem",
                            minHeight: "19rem",
                            minWidth: "39rem",
                            marginLeft: "5rem",
                            marginBottom: "2rem",
                            paddingTop: "0",
                            paddingRight: "4rem",
                            paddingBottom: "0",
                            paddingLeft: "4rem",
                            backgroundColor: unitHovered ? "rgba(255, 255, 255, 0.09)" : "rgba(255, 255, 255, 0.045)",
                            borderWidth: "1rem",
                            borderStyle: "solid",
                            borderColor: unitHovered ? "rgba(255, 255, 255, 0.36)" : "rgba(255, 255, 255, 0.20)",
                            borderRadius: "3rem",
                            boxSizing: "border-box",
                            fontSize: unitHovered ? "16rem" : "15.5rem",
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
            <div style={{ marginTop: "5rem", paddingLeft: "0", paddingRight: "1rem" }}>
                <div title={currentSpeedTitle} style={factRowStyle}>
                    <span style={labelStyle}>{currentSpeedLabelText}</span>
                    <span style={valueStyle}>{currentSpeedValueText}</span>
                </div>
                <div
                    onMouseEnter={onGameDefaultMouseEnter}
                    onMouseLeave={onGameDefaultMouseLeave}
                    title={gameDefaultTitle}
                    style={factRowStyle}
                >
                    <span style={labelStyle}>{defaultSpeedLabelText}</span>
                    <span style={valueStyle}>{defaultSpeedValueText}</span>
                </div>
            </div>
        </div>
    );
};
