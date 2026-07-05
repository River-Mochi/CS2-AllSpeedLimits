// File: UI/src/panel/components/SelectionSection.tsx
// Purpose: Compact first-segment summary shown in the collapsible top section.

import { useEffect, useState } from "react";
import type { ReactNode } from "react";

type SelectionSectionProps = {
    visible: boolean;
    targetSpeedUnitToggle?: ReactNode;
    currentSpeedTitle?: string;
    gameDefaultTitle?: string;
    currentSpeedLabelText: string;
    currentSpeedValueText: string;
    defaultSpeedLabelText: string;
    defaultSpeedValueText: string;
    onGameDefaultMouseEnter: () => void;
    onGameDefaultMouseLeave: () => void;
};

const kTargetSpeedEventName = "RoadRailSpeeds:targetSpeedChanged";

type TargetSpeedSummary = {
    label: string;
    value: string;
};

type TargetSpeedWindow = Window & {
    __rrsTargetSpeedSummary?: TargetSpeedSummary;
};

const readInitialTargetSpeedSummary = (): TargetSpeedSummary => {
    return (window as TargetSpeedWindow).__rrsTargetSpeedSummary ?? { label: "New speed", value: "" };
};

export const SelectionSection = (props: SelectionSectionProps) => {
    const {
        visible,
        targetSpeedUnitToggle,
        currentSpeedTitle,
        gameDefaultTitle,
        currentSpeedLabelText,
        currentSpeedValueText,
        defaultSpeedLabelText,
        defaultSpeedValueText,
        onGameDefaultMouseEnter,
        onGameDefaultMouseLeave
    } = props;

    const [targetSpeedSummary, setTargetSpeedSummary] = useState(readInitialTargetSpeedSummary);

    useEffect(() => {
        const handleTargetSpeedChanged = (event: Event) => {
            const customEvent = event as CustomEvent<TargetSpeedSummary>;
            if (customEvent.detail !== undefined) {
                setTargetSpeedSummary(customEvent.detail);
            }
        };

        window.addEventListener(kTargetSpeedEventName, handleTargetSpeedChanged);
        setTargetSpeedSummary(readInitialTargetSpeedSummary());
        return () => window.removeEventListener(kTargetSpeedEventName, handleTargetSpeedChanged);
    }, []);

    if (!visible) {
        return null;
    }

    const labelStyle = {
        fontSize: "11.5rem",
        color: "rgba(226, 236, 241, 0.74)",
        marginRight: "8rem",
        whiteSpace: "nowrap" as const
    };

    const valueStyle = {
        fontSize: "12.2rem",
        fontWeight: "bold",
        color: "#fff",
        whiteSpace: "nowrap" as const,
        minWidth: "58rem",
        textAlign: "right" as const
    };

    const factRowStyle = {
        display: "flex",
        // cohtml/Gameface has no "baseline"; it warns and ignores it. "center" reads the same
        // for these single-line label/value rows and keeps the UI log clean.
        alignItems: "center",
        justifyContent: "flex-end",
        minHeight: "18rem",
        minWidth: "0"
    };

    return (
        <div style={{ marginBottom: "2rem" }}>
            <div
                style={{
                    minHeight: "43rem",
                    paddingTop: "1rem",
                    paddingRight: "3rem",
                    paddingBottom: "1rem",
                    paddingLeft: "0",
                    backgroundColor: "transparent",
                    display: "flex",
                    alignItems: "stretch",
                    overflow: "hidden"
                }}
            >
                <div style={{
                    width: "150rem",
                    minWidth: "150rem",
                    paddingTop: "3rem",
                    paddingRight: targetSpeedUnitToggle ? "40rem" : "8rem",
                    paddingBottom: "2rem",
                    paddingLeft: "8rem",
                    backgroundColor: "rgba(255, 255, 255, 0.02)",
                    position: "relative",
                    borderWidth: "1rem",
                    borderStyle: "solid",
                    borderColor: "rgba(78, 195, 240, 0.50)",
                    borderRadius: "4rem",
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    flexShrink: 0
                }}>
                    <span style={{
                        fontSize: "11rem",
                        fontWeight: 800,
                        color: "rgba(78, 215, 255, 0.98)",
                        whiteSpace: "nowrap",
                        lineHeight: "1.05"
                    }}>
                        {targetSpeedSummary.label}
                    </span>
                    <span style={{
                        fontSize: "18.5rem",
                        fontWeight: "bold",
                        color: "#fff",
                        whiteSpace: "nowrap",
                        lineHeight: "1.1",
                        marginTop: "1rem"
                    }}>
                        {targetSpeedSummary.value}
                    </span>
                    {targetSpeedUnitToggle && (
                        <div style={{
                            position: "absolute",
                            right: "7rem",
                            top: "7rem",
                            width: "27rem",
                            height: "27rem"
                        }}>
                            {targetSpeedUnitToggle}
                        </div>
                    )}
                </div>

                <div style={{
                    flex: 1,
                    minWidth: "0",
                    marginLeft: "10rem",
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center"
                }}>
                    <div
                        title={currentSpeedTitle}
                        style={factRowStyle}
                    >
                        <span style={labelStyle}>
                            {currentSpeedLabelText}
                        </span>
                        <span style={valueStyle}>
                            {currentSpeedValueText}
                        </span>
                    </div>

                    <div
                        onMouseEnter={onGameDefaultMouseEnter}
                        onMouseLeave={onGameDefaultMouseLeave}
                        title={gameDefaultTitle}
                        style={factRowStyle}
                    >
                        <span style={labelStyle}>
                            {defaultSpeedLabelText}
                        </span>
                        <span style={valueStyle}>
                            {defaultSpeedValueText}
                        </span>
                    </div>
                </div>
            </div>
        </div>
    );
};
