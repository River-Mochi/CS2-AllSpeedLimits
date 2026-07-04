// File: UI/src/panel/components/SpeedToolHeader.tsx
// Purpose: Title-bar UI for the speed tool panel.

import { Tooltip } from "cs2/ui";
import type { Dispatch, MouseEvent, SetStateAction } from "react";
import type { PanelTooltipKind } from "../types";
import speedLimitIcon from "../../images/icon-speedlimit30-classic1.svg";
import advisorInfoIcon from "../../images/AdvisorInfoViewWhite.svg";

type SpeedToolHeaderProps = {
    title: string;
    closeTooltip: string;
    markersTooltip: string;
    panelTooltipsEnabled: boolean;
    speedMarkersHidden: boolean;
    isDragging: boolean;
    isCloseHovered: boolean;
    isGuideHovered: boolean;
    isHelpHovered: boolean;
    isMarkersHovered: boolean;
    setIsCloseHovered: Dispatch<SetStateAction<boolean>>;
    setIsGuideHovered: Dispatch<SetStateAction<boolean>>;
    setIsHelpHovered: Dispatch<SetStateAction<boolean>>;
    setIsMarkersHovered: Dispatch<SetStateAction<boolean>>;
    onMouseDown: (event: MouseEvent) => void;
    onClose: () => void;
    onToggleTooltips: () => void;
    onToggleMarkers: () => void;
    showPanelTitleTooltip: () => void;
    hidePanelTooltip: () => void;
};

export const SpeedToolHeader = (props: SpeedToolHeaderProps) => {
    const {
        title,
        closeTooltip,
        markersTooltip,
        panelTooltipsEnabled,
        speedMarkersHidden,
        isDragging,
        isCloseHovered,
        isGuideHovered,
        isHelpHovered,
        isMarkersHovered,
        setIsCloseHovered,
        setIsGuideHovered,
        setIsHelpHovered,
        setIsMarkersHovered,
        onMouseDown,
        onClose,
        onToggleTooltips,
        onToggleMarkers,
        showPanelTitleTooltip,
        hidePanelTooltip
    } = props;

    const selectionTooltipIconActive = !panelTooltipsEnabled;
    const selectionTooltipIconFilter = selectionTooltipIconActive
        ? "brightness(1.18) sepia(0.95) hue-rotate(-40deg) saturate(4.2)"
        : "none";

    return (
        <div
            onMouseDown={onMouseDown}
            style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                width: "100%",
                minHeight: "24rem",
                cursor: isDragging ? "grabbing" : "grab"
            }}
        >
            <div style={{ display: "flex", alignItems: "center", paddingLeft: "4rem" }}>
                <img
                    src={speedLimitIcon}
                    alt=""
                    onMouseEnter={() => {
                        setIsGuideHovered(true);
                        showPanelTitleTooltip();
                    }}
                    onMouseLeave={() => {
                        setIsGuideHovered(false);
                        hidePanelTooltip();
                    }}
                    style={{
                        width: isGuideHovered ? "25rem" : "23rem",
                        height: isGuideHovered ? "25rem" : "23rem",
                        marginRight: "9rem",
                        marginLeft: "-2rem",
                        opacity: isGuideHovered ? 1 : 0.92
                    }}
                />
                <span>{title}</span>
            </div>

            <div style={{ display: "flex", alignItems: "center" }}>
                <Tooltip tooltip={markersTooltip} direction="up">
                    <button
                        onClick={event => {
                            event.preventDefault();
                            event.stopPropagation();
                            onToggleMarkers();
                        }}
                        onMouseEnter={() => setIsMarkersHovered(true)}
                        onMouseLeave={() => setIsMarkersHovered(false)}
                        style={{
                            backgroundColor: speedMarkersHidden
                                ? (isMarkersHovered ? "rgba(61, 224, 255, 0.22)" : "rgba(61, 224, 255, 0.15)")
                                : (isMarkersHovered ? "rgba(120, 190, 220, 0.14)" : "transparent"),
                            borderWidth: "0",
                            borderStyle: "solid",
                            borderColor: "rgba(255, 255, 255, 0)",
                            borderRadius: "50%",
                            width: "28rem",
                            height: "26rem",
                            boxSizing: "border-box",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            cursor: "pointer",
                            paddingTop: "0",
                            paddingRight: "0",
                            paddingBottom: "0",
                            paddingLeft: "0",
                            marginRight: "1rem"
                        }}
                    >
                        <img
                            src="Media/Tools/Snap Options/All.svg"
                            alt=""
                            style={{
                                width: isMarkersHovered ? "22rem" : "20rem",
                                height: isMarkersHovered ? "22rem" : "20rem",
                                // Normalize the glyph to white, then tint blue-green (like the road numbers)
                                // only when markers are hidden (toggle active). Idle = faded gray-white.
                                filter: speedMarkersHidden
                                    ? "brightness(0) invert(1) sepia(1) saturate(4.5) hue-rotate(150deg) brightness(0.92)"
                                    : "brightness(0) invert(1)",
                                opacity: speedMarkersHidden ? 0.98 : (isMarkersHovered ? 0.95 : 0.5),
                                pointerEvents: "none"
                            }}
                        />
                    </button>
                </Tooltip>

                <button
                    onClick={event => {
                        event.preventDefault();
                        event.stopPropagation();
                        onToggleTooltips();
                    }}
                    onMouseEnter={() => setIsHelpHovered(true)}
                    onMouseLeave={() => setIsHelpHovered(false)}
                    style={{
                        backgroundColor: selectionTooltipIconActive
                            ? (isHelpHovered ? "rgba(245, 188, 64, 0.18)" : "transparent")
                            : (isHelpHovered ? "rgba(120, 190, 220, 0.14)" : "transparent"),
                        borderWidth: "0",
                        borderStyle: "solid",
                        borderColor: "rgba(255, 255, 255, 0)",
                        borderRadius: "50%",
                        width: "28rem",
                        height: "26rem",
                        boxSizing: "border-box",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        cursor: "pointer",
                        paddingTop: "0",
                        paddingRight: "0",
                        paddingBottom: "0",
                        paddingLeft: "0",
                        marginRight: "1rem"
                    }}
                >
                    <img
                        src={advisorInfoIcon}
                        alt=""
                        style={{
                            width: isHelpHovered ? "24rem" : "22rem",
                            height: isHelpHovered ? "24rem" : "22rem",
                            filter: selectionTooltipIconFilter,
                            opacity: selectionTooltipIconActive ? 1 : isHelpHovered ? 0.96 : 0.64,
                            pointerEvents: "none"
                        }}
                    />
                </button>

                <Tooltip tooltip={closeTooltip} direction="up">
                    <button
                        onClick={onClose}
                        onMouseEnter={() => setIsCloseHovered(true)}
                        onMouseLeave={() => setIsCloseHovered(false)}
                        style={{
                            backgroundColor: isCloseHovered ? "rgba(255, 255, 255, 0.12)" : "transparent",
                            borderWidth: "0",
                            borderStyle: "solid",
                            width: "24rem",
                            height: "24rem",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            cursor: "pointer",
                            paddingTop: "0",
                            paddingRight: "0",
                            paddingBottom: "0",
                            paddingLeft: "0",
                            lineHeight: "1",
                            borderRadius: "50%"
                        }}
                    >
                        <img
                            src="Media/Glyphs/Close.svg"
                            alt=""
                            style={{
                                width: isCloseHovered ? "14rem" : "13rem",
                                height: isCloseHovered ? "14rem" : "13rem",
                                filter: "brightness(0) invert(1)",
                                opacity: isCloseHovered ? 1 : 0.45
                            }}
                        />
                    </button>
                </Tooltip>
            </div>
        </div>
    );
};
