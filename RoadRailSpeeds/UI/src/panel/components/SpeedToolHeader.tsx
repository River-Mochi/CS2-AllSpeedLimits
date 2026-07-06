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
    showMarkersTooltip: () => void;
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
        showMarkersTooltip,
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
                <Button
                    as="button"
                    focusKey={focusDisabled}
                    theme={{ button: "" }}
                    aria-label={markersTooltip}
                    onSelect={onToggleMarkers}
                    onMouseEnter={() => {
                        setIsMarkersHovered(true);
                        showMarkersTooltip();
                    }}
                    onMouseLeave={() => {
                        setIsMarkersHovered(false);
                        hidePanelTooltip();
                    }}
                    style={{
                        position: "relative",
                        backgroundColor: isMarkersHovered ? "rgba(120, 190, 220, 0.14)" : "transparent",
                        borderWidth: "0",
                        borderStyle: "solid",
                        borderColor: "rgba(255, 255, 255, 0)",
                        borderRadius: "50%",
                        width: "26rem",
                        height: "26rem",
                        minHeight: "26rem",
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
                    <div style={{
                        position: "absolute",
                        top: "3rem",
                        left: "3rem",
                        width: "20rem",
                        height: "20rem",
                        borderRadius: "50%",
                        borderWidth: speedMarkersHidden ? "2rem" : "1rem",
                        borderStyle: "solid",
                        borderColor: speedMarkersHidden
                            ? "rgba(110, 200, 235, 0.88)"
                            : (isMarkersHovered ? "rgba(255, 255, 255, 0.86)" : "rgba(255, 255, 255, 0.34)"),
                        boxSizing: "border-box",
                        pointerEvents: "none"
                    }} />
                    <img
                        src={speedMarkersHidden ? starBlueGreenIcon : "Media/Tools/Snap Options/All.svg"}
                        alt=""
                        style={{
                            width: isMarkersHovered ? "15rem" : "13.5rem",
                            height: isMarkersHovered ? "15rem" : "13.5rem",
                            filter: speedMarkersHidden ? "none" : "brightness(0) invert(1)",
                            opacity: speedMarkersHidden ? 1 : (isMarkersHovered ? 1 : 0.58),
                            transform: isMarkersHovered ? "scale(1.05)" : "none",
                            pointerEvents: "none"
                        }}
                    />
                </Button>

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
