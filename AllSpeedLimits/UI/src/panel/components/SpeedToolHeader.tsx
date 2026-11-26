// File: UI/src/panel/components/SpeedToolHeader.tsx
// Purpose: Title-bar UI for the speed tool panel.

import { Button, Tooltip } from "cs2/ui";
import type { Dispatch, MouseEvent, SetStateAction } from "react";
import { VanillaComponentResolver } from "../../utils/vanilla/VanillaComponentResolver";
import speedLimitIcon from "../../images/icon-speedlimit30-classic1.svg";
import advisorInfoIcon from "../../images/AdvisorInfoViewWhite.svg";
import advisorInfoDisabledIcon from "../../images/AdvisorInfoViewGold.svg";

type SpeedToolHeaderProps = {
    title: string;
    speedLimitsTab: string;
    allPathsTab: string;
    activeView: "limits" | "paths";
    onViewChange: (view: "limits" | "paths") => void;
    closeTooltip: string;
    draggableTooltip: string;
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
    hidePanelTooltip: () => void;
};

export const SpeedToolHeader = (props: SpeedToolHeaderProps) => {
    const {
        title,
        speedLimitsTab,
        allPathsTab,
        activeView,
        onViewChange,
        closeTooltip,
        draggableTooltip,
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
        hidePanelTooltip
    } = props;

    const focusDisabled = VanillaComponentResolver.instance.FOCUS_DISABLED;
    const selectionTooltipIconActive = !panelTooltipsEnabled;
    const disabledIconColor = "#FFD264";

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
            <div style={{ display: "flex", alignItems: "center", paddingLeft: "4rem", flex: 1, minWidth: "0", overflow: "hidden" }}>
                <img
                    src={speedLimitIcon}
                    alt=""
                    onMouseEnter={() => {
                        hidePanelTooltip();
                        setIsGuideHovered(true);
                    }}
                    onMouseLeave={() => {
                        setIsGuideHovered(false);
                    }}
                    style={{
                        width: isGuideHovered ? "25rem" : "23rem",
                        height: isGuideHovered ? "25rem" : "23rem",
                        marginRight: "5rem",
                        flexShrink: 0,
                        marginLeft: "-2rem",
                        opacity: isGuideHovered ? 1 : 0.92
                    }}
                />
                <div style={{ display: "flex", alignItems: "center", flex: 1, minWidth: "0", overflow: "hidden" }} title={title} role="tablist">
                    {([
                        ["limits", speedLimitsTab],
                        ["paths", allPathsTab]
                    ] as const).map(([view, label]) => (
                        <button
                            key={view}
                            type="button"
                            role="tab"
                            aria-selected={activeView === view}
                            title={label}
                            onMouseDown={(event: MouseEvent) => event.stopPropagation()}
                            onClick={() => onViewChange(view)}
                            style={{
                                color: activeView === view ? "#EFFAFF" : "rgba(218, 235, 244, 0.77)",
                                fontSize: "12rem",
                                fontWeight: activeView === view ? 700 : 550,
                                backgroundColor: activeView === view
                                    ? "rgba(92, 161, 192, 0.22)" : "rgba(92, 161, 192, 0.08)",
                                borderWidth: "0",
                                borderStyle: "solid",
                                borderBottomWidth: "2rem",
                                borderBottomColor: activeView === view ? "#79D4EE" : "transparent",
                                borderTopLeftRadius: "5rem",
                                borderTopRightRadius: "5rem",
                                borderBottomLeftRadius: "0",
                                borderBottomRightRadius: "0",
                                minHeight: "25rem",
                                paddingTop: "3rem",
                                paddingBottom: "2rem",
                                paddingLeft: "4rem",
                                paddingRight: "4rem",
                                marginRight: view === "limits" ? "3rem" : "0",
                                flexShrink: 1,
                                minWidth: "0",
                                maxWidth: "135rem",
                                overflow: "hidden",
                                textOverflow: "ellipsis",
                                whiteSpace: "nowrap",
                                cursor: "pointer"
                            }}
                        >
                            {label}
                        </button>
                    ))}
                </div>
            </div>

            <div style={{ display: "flex", alignItems: "center", flexShrink: 0, minWidth: "80rem" }}>
                <Button
                    as="button"
                    focusKey={focusDisabled}
                    theme={{ button: "" }}
                    aria-label={markersTooltip}
                    onSelect={onToggleMarkers}
                    onMouseEnter={() => {
                        hidePanelTooltip();
                        setIsMarkersHovered(true);
                    }}
                    onMouseLeave={() => setIsMarkersHovered(false)}
                    style={{
                        position: "relative",
                        backgroundColor: speedMarkersHidden && isMarkersHovered ? "rgba(245, 188, 64, 0.18)" : isMarkersHovered ? "rgba(120, 190, 220, 0.14)" : "transparent",
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
                    <span style={{
                        fontSize: isMarkersHovered ? "19rem" : "17rem",
                        fontWeight: 700,
                        lineHeight: "1",
                        color: speedMarkersHidden ? disabledIconColor : "#FFFFFF",
                        opacity: speedMarkersHidden ? 1 : isMarkersHovered ? 0.96 : 0.64,
                        pointerEvents: "none"
                    }}>№</span>
                </Button>

                <Button
                    as="button"
                    focusKey={focusDisabled}
                    theme={{ button: "" }}
                    onSelect={onToggleTooltips}
                    aria-disabled={panelTooltipsEnabled}
                    onMouseEnter={() => {
                        hidePanelTooltip();
                        setIsHelpHovered(true);
                    }}
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
                        src={selectionTooltipIconActive ? advisorInfoDisabledIcon : advisorInfoIcon}
                        alt=""
                        style={{
                            width: isHelpHovered ? "26rem" : "24.5rem",
                            height: isHelpHovered ? "26rem" : "24.5rem",
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
