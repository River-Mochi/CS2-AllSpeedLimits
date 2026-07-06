// File: UI/src/panel/components/SpeedToolHeader.tsx
// Purpose: Title-bar UI for the speed tool panel.

import { Button, Tooltip } from "cs2/ui";
import type { Dispatch, MouseEvent, SetStateAction } from "react";
import type { PanelTooltipKind } from "../types";
import { VanillaComponentResolver } from "../../utils/vanilla/VanillaComponentResolver";
import speedLimitIcon from "../../images/icon-speedlimit30-classic1.svg";
import advisorInfoIcon from "../../images/AdvisorInfoViewWhite.svg";
import starBlueGreenIcon from "../../images/star-all-bluegreen.svg";

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

    const focusDisabled = VanillaComponentResolver.instance.FOCUS_DISABLED;
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
                    <Button
                        as="button"
                        focusKey={focusDisabled}
                        theme={{ button: "" }}
                        onSelect={onToggleMarkers}
                        onMouseEnter={() => setIsMarkersHovered(true)}
                        onMouseLeave={() => setIsMarkersHovered(false)}
                        style={{
                            // 24rem button = the hover-wash circle. The active blue ring is a SEPARATE,
                            // smaller circle (below) inset inside it, so the ring reads clearly smaller.
                            position: "relative",
                            backgroundColor: isMarkersHovered ? "rgba(120, 190, 220, 0.14)" : "transparent",
                            borderWidth: "0",
                            borderStyle: "solid",
                            borderColor: "rgba(255, 255, 255, 0)",
                            borderRadius: "50%",
                            width: "24rem",
                            height: "24rem",
                            minHeight: "24rem",
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
                        {speedMarkersHidden && (
                            <div style={{
                                // 20rem ring inset 2rem inside the 24rem button — smaller diameter, not
                                // touching the edge. Same blue as the filter boxes.
                                position: "absolute",
                                top: "2rem",
                                left: "2rem",
                                width: "20rem",
                                height: "20rem",
                                borderRadius: "50%",
                                borderWidth: "2rem",
                                borderStyle: "solid",
                                borderColor: "rgba(110, 200, 235, 0.85)",
                                boxSizing: "border-box",
                                pointerEvents: "none"
                            }} />
                        )}
                        <img
                            // Active: a real blue-green star (bundled SVG with a fill), so the color is
                            // exact instead of an unreliable CSS filter. Idle: the white game glyph.
                            src={speedMarkersHidden ? starBlueGreenIcon : "Media/Tools/Snap Options/All.svg"}
                            alt=""
                            style={{
                                width: isMarkersHovered ? "14rem" : "13rem",
                                height: isMarkersHovered ? "14rem" : "13rem",
                                filter: speedMarkersHidden ? "none" : "brightness(0) invert(1)",
                                opacity: speedMarkersHidden ? 1 : (isMarkersHovered ? 0.95 : 0.5),
                                pointerEvents: "none"
                            }}
                        />
                    </Button>
                </Tooltip>

                <Button
                    as="button"
                    focusKey={focusDisabled}
                    theme={{ button: "" }}
                    onSelect={onToggleTooltips}
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
                        height: "28rem",
                        minHeight: "28rem",
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
                            width: isHelpHovered ? "25rem" : "23rem",
                            height: isHelpHovered ? "25rem" : "23rem",
                            filter: selectionTooltipIconFilter,
                            opacity: selectionTooltipIconActive ? 1 : isHelpHovered ? 0.96 : 0.64,
                            pointerEvents: "none"
                        }}
                    />
                </Button>

                <Tooltip tooltip={closeTooltip} direction="up">
                    <Button
                        as="button"
                        focusKey={focusDisabled}
                        theme={{ button: "" }}
                        onSelect={onClose}
                        onMouseEnter={() => setIsCloseHovered(true)}
                        onMouseLeave={() => setIsCloseHovered(false)}
                        style={{
                            backgroundColor: isCloseHovered ? "rgba(255, 255, 255, 0.12)" : "transparent",
                            borderWidth: "0",
                            borderStyle: "solid",
                            width: "24rem",
                            height: "24rem",
                            minHeight: "24rem",
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
                    </Button>
                </Tooltip>
            </div>
        </div>
    );
};
