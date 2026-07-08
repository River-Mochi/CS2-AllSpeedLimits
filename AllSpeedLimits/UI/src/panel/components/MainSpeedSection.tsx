// File: UI/src/panel/components/MainSpeedSection.tsx
// Purpose: Collapsible "Slider" section — slider + precise stepper, with Apply + reset below.
// Presets auto-apply, so Apply lives here with the slider/stepper (the only controls that need it).

import { useState } from "react";
import type { ReactNode } from "react";
import { Button } from "../../shared/Button";
import { Slider } from "../../slider/slider";
// Game's own reset-arrow icon. Its arrow curls clockwise as-shipped; mirrored horizontally below
// (scaleX(-1)) so it reads as counter-clockwise, the more common "revert/reset" convention.
import resetIcon from "../../images/Reset_Button.svg";

type MainSpeedSectionProps = {
    focusKey: unknown;
    resetTitle?: string;
    applyButtonText: string;
    sliderMin: number;
    sliderMax: number;
    sliderStep: number;
    sliderValue: number;
    unitLabel: string;
    isApplying: boolean;
    isResetting: boolean;
    stepper: ReactNode;
    onApplyMouseEnter?: () => void;
    onResetMouseEnter?: () => void;
    onControlMouseLeave?: () => void;
    onSliderChange: (value: number) => void;
    onApply: () => void;
    onReset: () => void;
};

export const MainSpeedSection = (props: MainSpeedSectionProps) => {
    const {
        focusKey,
        resetTitle,
        applyButtonText,
        sliderMin,
        sliderMax,
        sliderStep,
        sliderValue,
        unitLabel,
        isApplying,
        isResetting,
        stepper,
        onApplyMouseEnter,
        onResetMouseEnter,
        onControlMouseLeave,
        onSliderChange,
        onApply,
        onReset
    } = props;

    const [resetHovered, setResetHovered] = useState(false);
    const controlHeight = "29rem";

    return (
        <div style={{ marginBottom: "8rem" }}>
            {/* Row 1: slider (with min/max labels) + stepper, all on one line. */}
            <div style={{ display: "flex", alignItems: "center", marginBottom: "8rem" }}>
                <div style={{ flex: 1, minWidth: "0", marginRight: "8rem" }}>
                    <Slider
                        start={sliderMin}
                        end={sliderMax}
                        step={sliderStep}
                        value={sliderValue}
                        onChange={onSliderChange}
                    />
                    <div style={{
                        display: "flex",
                        justifyContent: "space-between",
                        fontSize: "11rem",
                        color: "rgba(255, 255, 255, 0.78)",
                        marginTop: "3rem"
                    }}>
                        <span>{sliderMin} {unitLabel}</span>
                        <span>{sliderMax} {unitLabel}</span>
                    </div>
                </div>
                {stepper}
            </div>
            {/* Row 2: Apply + reset, right-aligned under the stepper area. */}
            <div style={{ display: "flex", justifyContent: "flex-end" }}>
                <div style={{ display: "flex", alignItems: "center" }}>
                    <Button
                        focusKey={focusKey}
                        selected={isResetting}
                        disabled={isResetting}
                        onSelect={onReset}
                        variant="neutral"
                        title={resetTitle}
                        onMouseEnter={() => { setResetHovered(true); onResetMouseEnter?.(); }}
                        onMouseLeave={() => { setResetHovered(false); onControlMouseLeave?.(); }}
                        style={{
                            width: "28rem",
                            minWidth: "28rem",
                            minHeight: controlHeight,
                            height: controlHeight,
                            marginRight: "4rem",
                            paddingTop: "0",
                            paddingRight: "0",
                            paddingBottom: "0",
                            paddingLeft: "0",
                            overflow: "visible"
                        }}
                    >
                        {isResetting ? (
                            <span style={{ fontSize: "15rem", fontWeight: "bold", color: "#fff" }}>✓</span>
                        ) : (
                            <img
                                src={resetIcon}
                                alt=""
                                style={{
                                    width: "18rem",
                                    height: "18rem",
                                    transform: resetHovered ? "scaleX(-1) scale(1.14)" : "scaleX(-1) scale(1)",
                                    transformOrigin: "50% 50%",
                                    transitionProperty: "transform",
                                    transitionDuration: "120ms",
                                    transitionTimingFunction: "ease-out",
                                    pointerEvents: "none"
                                }}
                            />
                        )}
                    </Button>
                    <Button
                        focusKey={focusKey}
                        selected={isApplying}
                        disabled={isApplying}
                        onSelect={onApply}
                        onMouseEnter={onApplyMouseEnter}
                        onMouseLeave={onControlMouseLeave}
                        style={{
                            width: "76rem",
                            minWidth: "76rem",
                            minHeight: controlHeight,
                            height: controlHeight,
                            paddingTop: "0",
                            paddingRight: "0",
                            paddingBottom: "0",
                            paddingLeft: "0",
                            fontSize: "15rem",
                            fontWeight: 800
                        }}
                    >
                        {applyButtonText}
                    </Button>
                </div>
            </div>
        </div>
    );
};
