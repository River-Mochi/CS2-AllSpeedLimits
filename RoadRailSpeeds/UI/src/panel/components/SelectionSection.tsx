// File: UI/src/panel/components/SelectionSection.tsx
// Purpose: Left column of the Selected section — New-speed box on top, Current/Default rows below.

import type { ReactNode } from "react";

type SelectionSectionProps = {
    targetSpeedUnitToggle?: ReactNode;
    newSpeedLabel: string;
    newSpeedValue: string;
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
        targetSpeedUnitToggle,
        newSpeedLabel,
        newSpeedValue,
        currentSpeedTitle,
        gameDefaultTitle,
        currentSpeedLabelText,
        currentSpeedValueText,
        defaultSpeedLabelText,
        defaultSpeedValueText,
        onGameDefaultMouseEnter,
        onGameDefaultMouseLeave
    } = props;

    const labelStyle = {
        fontSize: "11.5rem",
        color: "rgba(226, 236, 241, 0.74)",
        marginRight: "8rem",
        whiteSpace: "nowrap" as const
    };

    const valueStyle = {
        fontSize: "12.2rem",
        fontWeight: "bold" as const,
        color: "#fff",
        whiteSpace: "nowrap" as const,
        textAlign: "right" as const
    };

    const factRowStyle = {
        display: "flex",
        alignItems: "center" as const,
        justifyContent: "space-between" as const,
        minHeight: "18rem",
        minWidth: "0"
    };

    return (
        <div style={{ display: "flex", flexDirection: "column" }}>
            {/* New-speed box */}
            <div style={{
                paddingTop: "3rem",
                paddingRight: targetSpeedUnitToggle ? "36rem" : "8rem",
                paddingBottom: "3rem",
                paddingLeft: "8rem",
                backgroundColor: "rgba(255, 255, 255, 0.02)",
                position: "relative",
                borderWidth: "1rem",
                borderStyle: "solid",
                borderColor: "rgba(78, 195, 240, 0.50)",
                borderRadius: "4rem",
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
                <span style={{
                    fontSize: "18.5rem",
                    fontWeight: "bold",
                    color: "#fff",
                    whiteSpace: "nowrap",
                    lineHeight: "1.1",
                    marginTop: "1rem"
                }}>
                    {newSpeedValue}
                </span>
                {targetSpeedUnitToggle && (
                    <div style={{
                        position: "absolute",
                        right: "6rem",
                        top: "6rem",
                        width: "27rem",
                        height: "27rem"
                    }}>
                        {targetSpeedUnitToggle}
                    </div>
                )}
            </div>

            {/* Current / Default, stacked below the box */}
            <div style={{ marginTop: "5rem", paddingLeft: "2rem", paddingRight: "4rem" }}>
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
