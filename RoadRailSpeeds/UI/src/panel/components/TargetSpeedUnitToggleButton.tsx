// File: UI/src/panel/components/TargetSpeedUnitToggleButton.tsx
// Purpose: Small A/km/mi target-speed unit toggle shown in the selected segment summary.

import { Button } from "cs2/ui";
import type { Dispatch, SetStateAction } from "react";
import { VanillaComponentResolver } from "../../utils/vanilla/VanillaComponentResolver";
import unitAutoIcon from "../../images/icon-auto-white.svg";
import unitKmIcon from "../../images/icon-km-white.svg";
import unitMiIcon from "../../images/icon-mi-white.svg";

type TargetSpeedUnitToggleButtonProps = {
    targetSpeedUnitLabel: string;
    isHovered: boolean;
    setHovered: Dispatch<SetStateAction<boolean>>;
    onToggle: () => void;
    showUnitTooltip: () => void;
    hideTooltip: () => void;
};

export const TargetSpeedUnitToggleButton = (props: TargetSpeedUnitToggleButtonProps) => {
    const {
        targetSpeedUnitLabel,
        isHovered,
        setHovered,
        onToggle,
        showUnitTooltip,
        hideTooltip
    } = props;

    const unitIcon = targetSpeedUnitLabel === "a"
        ? unitAutoIcon
        : targetSpeedUnitLabel === "km" ? unitKmIcon : unitMiIcon;
    const focusDisabled = VanillaComponentResolver.instance.FOCUS_DISABLED;

    return (
        <Button
            as="button"
            focusKey={focusDisabled}
            theme={{ button: "" }}
            onSelect={onToggle}
            onMouseEnter={() => {
                setHovered(true);
                showUnitTooltip();
            }}
            onMouseLeave={() => {
                setHovered(false);
                hideTooltip();
            }}
            style={{
                width: "27rem",
                minWidth: "27rem",
                height: "27rem",
                minHeight: "27rem",
                backgroundColor: "transparent",
                borderWidth: "0",
                borderStyle: "solid",
                borderColor: "rgba(255, 255, 255, 0)",
                borderRadius: "50%",
                boxSizing: "border-box",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                cursor: "pointer",
                paddingTop: "0",
                paddingRight: "0",
                paddingBottom: "0",
                paddingLeft: "0"
            }}
        >
            <img
                src={unitIcon}
                alt=""
                style={{
                    width: isHovered ? "27rem" : "23.5rem",
                    height: isHovered ? "27rem" : "23.5rem",
                    opacity: isHovered ? 0.96 : 0.68,
                    pointerEvents: "none"
                }}
            />
        </Button>
    );
};
