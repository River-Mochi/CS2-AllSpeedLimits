// File: UI/src/slider/slider.tsx
// Purpose: Thin wrapper around the vanilla CS2 slider component — no custom overlay on the thumb,
// just CSS variable overrides for the track colors/size.

import { VanillaComponentResolver } from "../utils/vanilla/VanillaComponentResolver";

export const Slider = (props: any) => {
    const VSlider = VanillaComponentResolver.instance.Slider;

    const sliderStyle = {
        ...(props.style ?? {}),
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
            {...props}
            style={sliderStyle}
            noFill={props.noFill ?? false}
            step={props.step}
            onChange={(v: number) => props.onChange(v)}
        />
    );
};
