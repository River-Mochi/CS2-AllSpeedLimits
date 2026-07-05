// File: UI/src/panel/components/MainSpeedSection.tsx
// Purpose: Slider row plus the primary speed action row for the speed tool panel.

import { useEffect, useState } from "react";
import type { ReactNode } from "react";
import { Button } from "../../shared/Button";
import { Slider } from "../../slider/slider";
// Game's own reset-arrow icon. Its arrow curls clockwise as-shipped; mirrored horizontally below
// (scaleX(-1)) so it reads as counter-clockwise, the more common "revert/reset" convention.
import resetIcon from "../../images/Reset_Button.svg";

type MainSpeedSectionProps = {
    focusKey: unknown;
    newSpeedLabelText: string;
    newSpeedValueText: string;
    slowerTitle?: string;
    fasterTitle?: string;
    resetTitle?: string;
    slowerButtonContent: ReactNode;
    fasterButtonContent: ReactNode;
    applyButtonText: string;
    sliderMin: number;
    sliderMax: number;
    sliderStep: number;
    sliderValue: number;
    unitLabel: string;
    isApplying: boolean;
    isResetting: boolean;
    onHalfSpeed: () => void;
    onFasterSpeed: () => void;
    onHalfSpeedMouseEnter?: () => void;
    onFasterSpeedMouseEnter?: () => void;
    onApplyMouseEnter?: () => void;
    onResetMouseEnter?: () => void;
    onControlMouseLeave?: () => void;
    onSliderChange: (value: number) => void;
    onApply: () => void;
    onReset: () => void;
};

const kTargetSpeedEventName = "RoadRailSpeeds:targetSpeedChanged";

type TargetSpeedSummary = {
    label: string;
    value: string;
};

type TargetSpeedWindow = Window & {
    __rrsTargetSpeedSummary?: TargetSpeedSummary;
};

export const MainSpeedSection = (props: MainSpeedSectionProps) => {
    const {
        focusKey,
        newSpeedLabelText,
        newSpeedValueText,
        slowerTitle,
        fasterTitle,
        resetTitle,
        slowerButtonContent,
        fasterButtonContent,
        applyButtonText,
        sliderMin,
        sliderMax,
        sliderStep,
        sliderValue,
        unitLabel,
        isApplying,
        isResetting,
        onHalfSpeed,
        onFasterSpeed,
        onHalfSpeedMouseEnter,
        onFasterSpeedMouseEnter,
        onApplyMouseEnter,
        onResetMouseEnter,
        onControlMouseLeave,
        onSliderChange,
        onApply,
        onReset
    } = props;

    const [resetHovered, setResetHovered] = useState(false);

    const sharedButtonHeight = "29rem";
    const resetButtonWidth = "30.7rem";

    useEffect(() => {
        const summary = {
            label: newSpeedLabelText,
            value: newSpeedValueText
        };
        (window as TargetSpeedWindow).__rrsTargetSpeedSummary = summary;
        window.dispatchEvent(new CustomEvent<TargetSpeedSummary>(kTargetSpeedEventName, { detail: summary }));
    }, [newSpeedLabelText, newSpeedValueText]);

    return (
        <div style={{ marginBottom: "8rem" }}>
            {/* Row 1: slider + precise stepper. Keep only a small visual gap between the slider
                and the stepper so the bar reads as one row instead of stopping too early. */}
            <div style={{
                paddingRight: "88rem",
                marginBottom: "8rem"
            }}>
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

            {/* Row 2: 50% down | 50% up | Apply | Reset. Same visual height for every button. */}
            <div style={{
                display: "flex",
                alignItems: "center"
            }}>
                <Button
                    focusKey={focusKey}
                    onSelect={onHalfSpeed}
                    variant="neutral"
                    title={slowerTitle}
                    onMouseEnter={onHalfSpeedMouseEnter}
                    onMouseLeave={onControlMouseLeave}
                    style={{
                        width: "54rem",
                        minHeight: sharedButtonHeight,
                        height: sharedButtonHeight,
                        marginRight: "5rem",
                        paddingTop: "0",
                        paddingRight: "0",
                        paddingBottom: "0",
                        paddingLeft: "0",
                        fontSize: "12rem",
                        fontWeight: 800
                    }}
                >
                    {slowerButtonContent}
                </Button>
                <Button
                    focusKey={focusKey}
                    onSelect={onFasterSpeed}
                    variant="neutral"
                    title={fasterTitle}
                    onMouseEnter={onFasterSpeedMouseEnter}
                    onMouseLeave={onControlMouseLeave}
                    style={{
                        width: "54rem",
                        minHeight: sharedButtonHeight,
                        height: sharedButtonHeight,
                        marginRight: "7rem",
                        paddingTop: "0",
                        paddingRight: "0",
                        paddingBottom: "0",
                        paddingLeft: "0",
                        fontSize: "12rem",
                        fontWeight: 800
                    }}
                >
                    {fasterButtonContent}
                </Button>
                <Button
                    focusKey={focusKey}
                    selected={isApplying}
                    disabled={isApplying}
                    onSelect={onApply}
                    onMouseEnter={onApplyMouseEnter}
                    onMouseLeave={onControlMouseLeave}
                    style={{
                        flex: 1,
                        minWidth: "86rem",
                        minHeight: sharedButtonHeight,
                        height: sharedButtonHeight,
                        marginRight: "8rem",
                        paddingTop: "0",
                        paddingRight: "10rem",
                        paddingBottom: "0",
                        paddingLeft: "10rem",
                        fontSize: "13rem",
                        fontWeight: 800
                    }}
                >
                    {applyButtonText}
                </Button>
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
                        width: resetButtonWidth,
                        minWidth: resetButtonWidth,
                        minHeight: sharedButtonHeight,
                        height: sharedButtonHeight,
                        paddingTop: "0",
                        paddingRight: "0",
                        paddingBottom: "0",
                        paddingLeft: "0",
                        marginRight: "21.3rem",
                        overflow: "visible"
                    }}
                >
                    {isResetting ? (
                        <span style={{ fontSize: "16rem", fontWeight: "bold", color: "#fff" }}>✓</span>
                    ) : (
                        <img
                            src={resetIcon}
                            alt=""
                            style={{
                                width: "20rem",
                                height: "20rem",
                                transform: resetHovered ? "scaleX(-1) scale(1.22)" : "scaleX(-1) scale(1)",
                                transformOrigin: "50% 50%",
                                transitionProperty: "transform",
                                transitionDuration: "120ms",
                                transitionTimingFunction: "ease-out",
                                pointerEvents: "none"
                            }}
                        />
                    )}
                </Button>
            </div>
        </div>
    );
};
