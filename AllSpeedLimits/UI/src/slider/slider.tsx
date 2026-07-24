// File: UI/src/slider/slider.tsx
// Purpose: Wrapper around the vanilla CS2 slider with stepping anchored to each drag's starting
// value, plus CSS variable overrides for the track colors/size.

import { useRef } from "react";
import { VanillaComponentResolver } from "../utils/vanilla/VanillaComponentResolver";

export const Slider = (props: any) => {
    const VSlider = VanillaComponentResolver.instance.Slider;
    const draggingRef = useRef(false);
    const dragAnchorRef = useRef(Number(props.value));
    const {
        step: requestedStep = 1,
        onDragStart,
        onDragEnd,
        style,
        noFill,
        ...sliderProps
    } = props;
    const step = Math.max(1, Number(requestedStep) || 1);

    const snapFromDragAnchor = (start: number, end: number, normalized: number): number => {
        const rawValue = start + normalized * (end - start);
        const minimum = Math.min(start, end);
        const maximum = Math.max(start, end);

        if (rawValue <= minimum) {
            return minimum;
        }

        if (rawValue >= maximum) {
            return maximum;
        }

        const anchor = draggingRef.current
            ? dragAnchorRef.current
            : Number(sliderProps.value);
        const steppedValue = anchor + Math.round((rawValue - anchor) / step) * step;
        return Math.max(minimum, Math.min(maximum, steppedValue));
    };

    const sliderStyle = {
        ...(style ?? {}),
        // Vanilla slider variables. Keep fill visible, but use quiet greys instead of game blue.
        // Right (unfilled) made more transparent so it reads as clearly different from the left.
        "--backgroundColor": "rgba(176, 184, 188, 0.20)",
        // Left (filled) nudged back up a hair — the previous pass darkened it too much.
        "--sliderRangeColor": "rgba(140, 148, 154, 0.56)",
        "--sliderRangeColorFocused": "rgba(165, 173, 179, 0.66)",
        "--sliderThumbColor": "rgba(235, 237, 239, 1)",
        "--sliderThumbColorFocused": "rgba(255, 255, 255, 1)",
        "--trackSize": "8rem",
        "--focusedColor": "transparent"
    };

    return (
        <VSlider
            {...sliderProps}
            style={sliderStyle}
            noFill={noFill ?? false}
            gamepadStep={step}
            valueTransformer={snapFromDragAnchor}
            onDragStart={() => {
                draggingRef.current = true;
                dragAnchorRef.current = Number(sliderProps.value);
                onDragStart?.();
            }}
            onDragEnd={() => {
                draggingRef.current = false;
                onDragEnd?.();
            }}
        />
    );
};
