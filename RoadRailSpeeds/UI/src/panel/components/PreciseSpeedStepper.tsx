// File: UI/src/panel/components/PreciseSpeedStepper.tsx
// Purpose: Compact up/down stepper beside the slider for precise speed changes.

import { Button, FOCUS_DISABLED } from "cs2/ui";
import { memo, useState } from "react";

type PreciseSpeedStepperProps = {
    widthRem: number;
    buttonWidthRem: number;
    numberWidthRem: number;
    displaySpeed: number;
    sliderMin: number;
    sliderMax: number;
    decreaseTitle?: string;
    increaseTitle?: string;
    groupTitle?: string;
    onStep: (direction: number) => void;
    onStepHoldStart: (direction: number) => void;
    onStepHoldStop: () => void;
    onStepClickFallback: (direction: number, disabled: boolean) => void;
    preciseStepMouseDownRef: { current: boolean };
};

export const PreciseSpeedStepper = memo((props: PreciseSpeedStepperProps) => {
    const {
        widthRem,
        buttonWidthRem,
        numberWidthRem,
        displaySpeed,
        sliderMin,
        sliderMax,
        decreaseTitle,
        increaseTitle,
        groupTitle,
        onStep,
        onStepHoldStart,
        onStepHoldStop,
        onStepClickFallback,
        preciseStepMouseDownRef
    } = props;

    const controlHeight = "26rem";
    const [hoveredDirection, setHoveredDirection] = useState<number | null>(null);

    const renderSpeedStepButton = (direction: number, label: string | undefined) => {
        const icon = direction < 0 ? "ThickStrokeArrowDown.svg" : "ThickStrokeArrowUp.svg";
        const disabled = direction < 0
            ? displaySpeed <= sliderMin + 0.01
            : displaySpeed >= sliderMax - 0.01;
        const hovered = !disabled && hoveredDirection === direction;

        return (
            <Button
                as="button"
                focusKey={FOCUS_DISABLED}
                theme={{ button: "" }}
                onMouseEnter={() => {
                    if (!disabled) {
                        setHoveredDirection(direction);
                    }
                }}
                onMouseDown={event => {
                    if (disabled) {
                        return;
                    }

                    preciseStepMouseDownRef.current = true;
                    event.preventDefault();
                    onStepHoldStart(direction);
                }}
                onMouseUp={onStepHoldStop}
                onMouseLeave={() => {
                    setHoveredDirection(null);
                    preciseStepMouseDownRef.current = false;
                    onStepHoldStop();
                }}
                onSelect={() => onStepClickFallback(direction, disabled)}
                onKeyDown={event => {
                    if (!disabled && (event.key === "Enter" || event.key === " ")) {
                        event.preventDefault();
                        onStep(direction);
                    }
                }}
                title={label}
                disabled={disabled}
                style={{
                    width: `${buttonWidthRem}rem`,
                    height: controlHeight,
                    minHeight: controlHeight,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    backgroundColor: disabled ? "rgba(255, 255, 255, 0.035)" : hovered ? "rgba(255, 255, 255, 0.16)" : "rgba(255, 255, 255, 0.08)",
                    borderWidth: "1rem",
                    borderStyle: "solid",
                    borderColor: disabled ? "rgba(255, 255, 255, 0.04)" : hovered ? "rgba(255, 255, 255, 0.34)" : "rgba(255, 255, 255, 0.14)",
                    borderRadius: "4rem",
                    cursor: disabled ? "default" : "pointer",
                    paddingTop: "0",
                    paddingRight: "0",
                    paddingBottom: "0",
                    paddingLeft: "0",
                    opacity: disabled ? 0.5 : hovered ? 1 : 0.9
                }}
            >
                <img
                    src={`Media/Glyphs/${icon}`}
                    alt=""
                    style={{
                        width: "16rem",
                        height: "16rem",
                        filter: "brightness(0) invert(1)",
                        opacity: disabled ? 0.40 : hovered ? 0.96 : 0.76
                    }}
                />
            </Button>
        );
    };

    return (
        <div
            title={groupTitle}
            style={{
                width: `${widthRem}rem`,
                marginLeft: "0",
                display: "flex",
                flexDirection: "row",
                alignItems: "center",
                justifyContent: "space-between"
            }}
        >
            {renderSpeedStepButton(-1, decreaseTitle)}
            <div
                style={{
                    width: `${numberWidthRem}rem`,
                    height: controlHeight,
                    minHeight: controlHeight,
                    backgroundColor: "rgba(5, 10, 14, 0.42)",
                    borderWidth: "1rem",
                    borderStyle: "solid",
                    borderColor: "rgba(78, 195, 240, 0.48)",
                    borderRadius: "3rem",
                    color: "#fff",
                    fontSize: "13rem",
                    fontWeight: "bold",
                    lineHeight: controlHeight,
                    textAlign: "center"
                }}
            >
                {Math.round(displaySpeed)}
            </div>
            {renderSpeedStepButton(1, increaseTitle)}
        </div>
    );
});
