// File: UI/src/panel/components/SelectionSection.tsx
// Purpose: Left column of the Selected section — New-speed box (with a click-to-toggle unit) on top,
// Current/Default rows below. The unit text next to the number is the km/h <-> mph toggle.

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

    const labelStyle = {
        fontSize: "11.5rem",
        color: "rgba(226, 236, 241, 0.74)",
        marginRight: "6rem",
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
                paddingRight: "7rem",
                paddingBottom: "4rem",
                paddingLeft: "8rem",
                backgroundColor: "rgba(255, 255, 255, 0.02)",
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
                        title={unitToggleTitle}
                        style={{
                            minHeight: "0",
                            marginLeft: "5rem",
                            marginBottom: "2rem",
                            paddingTop: "1rem",
                            paddingRight: "5rem",
                            paddingBottom: "1rem",
                            paddingLeft: "5rem",
                            fontSize: "12rem",
                            fontWeight: 700,
                            color: "rgba(120, 210, 245, 0.98)"
                        }}
                    >
                        {newSpeedUnit}
                    </Button>
                </div>
            </div>

            {/* Current / Default, stacked below the box */}
            <div style={{ marginTop: "5rem", paddingLeft: "2rem", paddingRight: "3rem" }}>
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
