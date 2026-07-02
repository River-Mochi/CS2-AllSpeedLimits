// File: UI/src/panel/components/SpeedToolOverlays.tsx
// Purpose: Floating side/help/marker tooltips for the speed tool panel.

import type { CSSProperties, ReactNode } from "react";
import { REM_TO_TOOLTIP_PX, RIGHT_TOOLTIP_GAP_PX, RIGHT_TOOLTIP_TOP_OFFSETS } from "../constants";
import type { PanelTooltipKind } from "../types";
import { PanelSideTooltip } from "./PanelSideTooltip";

type PanelPosition = {
    x: number;
    y: number;
};

type RoadGroupTooltip = {
    title: string;
    lines: string[];
};

type SpeedToolOverlaysProps = {
    text: any;
    panelTooltip: PanelTooltipKind | null;
    panelTooltipsEnabled: boolean;
    position: PanelPosition;
    panelWidth: number;
    tooltipBaseStyle: CSSProperties;
    tooltipFontSize: string;
    tooltipFontScale: number;
    markerTooltipText: string;
    markerTooltipX: number;
    markerTooltipY: number;
    markerTooltipFontSize: string;
    isGuideHovered: boolean;
    isHelpHovered: boolean;
    getRoadGroupTooltip: (kind: PanelTooltipKind) => RoadGroupTooltip | null;
};

export const SpeedToolOverlays = (props: SpeedToolOverlaysProps) => {
    const {
        text,
        panelTooltip,
        panelTooltipsEnabled,
        position,
        panelWidth,
        tooltipBaseStyle,
        tooltipFontSize,
        tooltipFontScale,
        markerTooltipText,
        markerTooltipX,
        markerTooltipY,
        markerTooltipFontSize,
        isGuideHovered,
        isHelpHovered,
        getRoadGroupTooltip
    } = props;

    const renderTooltipBlock = (title: string, lines: string[]) => (
        <>
            {title.length > 0 && (
                <div style={{ fontWeight: "bold", marginBottom: "4rem" }}>{title}</div>
            )}
            {lines.map((line, index) => (
                <div
                    key={`${title}-${index}`}
                    style={index === 0 ? undefined : { marginTop: "5rem" }}
                >
                    {line}
                </div>
            ))}
        </>
    );

    const renderSideTooltip = () => {
        if (panelTooltip === null || !panelTooltipsEnabled) {
            return null;
        }

        let topOffset = RIGHT_TOOLTIP_TOP_OFFSETS[panelTooltip];
        let leftOffset = panelWidth * REM_TO_TOOLTIP_PX + RIGHT_TOOLTIP_GAP_PX;
        let maxWidth = "196rem";
        let fontSize = tooltipFontSize;
        let content: ReactNode = null;
        const roadGroupTooltip = getRoadGroupTooltip(panelTooltip);

        if (roadGroupTooltip !== null) {
            maxWidth = "220rem";
            content = renderTooltipBlock(roadGroupTooltip.title, roadGroupTooltip.lines);
        } else if (panelTooltip === "panelTitle") {
            maxWidth = "170rem";
            content = renderTooltipBlock("", [text.tooltips.panelTitle]);
        } else if (panelTooltip === "gameDefault") {
            maxWidth = "210rem";
            fontSize = `${12 * tooltipFontScale / 100}rem`;
            content = renderTooltipBlock(text.tooltips.gameDefault.title, text.tooltips.gameDefault.lines);
        } else if (panelTooltip === "presetUnlimited") {
            maxWidth = "220rem";
            content = renderTooltipBlock(text.tooltips.presetUnlimited.title, text.tooltips.presetUnlimited.lines);
        } else if (panelTooltip === "resetAll") {
            maxWidth = "210rem";
            content = renderTooltipBlock(text.tooltips.resetAll.title, text.tooltips.resetAll.lines);
        } else if (panelTooltip === "resetSelected") {
            maxWidth = "184rem";
            content = renderTooltipBlock(text.buttons.reset, [text.tooltips.reset]);
        } else if (panelTooltip === "roadGroupApply") {
            maxWidth = "210rem";
            content = renderTooltipBlock(text.tooltips.roadGroupApply.title, text.tooltips.roadGroupApply.lines);
        } else if (panelTooltip === "speedSlower") {
            maxWidth = "188rem";
            content = renderTooltipBlock(text.buttons.slower, [text.tooltips.slower]);
        } else if (panelTooltip === "speedFaster") {
            maxWidth = "188rem";
            content = renderTooltipBlock(text.buttons.faster, [text.tooltips.faster]);
        } else if (panelTooltip === "stats") {
            maxWidth = "190rem";
            content = renderTooltipBlock(text.tooltips.stats.title, text.tooltips.stats.lines);
        } else if (panelTooltip === "wholeCity") {
            maxWidth = "210rem";
            content = renderTooltipBlock(text.tooltips.wholeCity.title, text.tooltips.wholeCity.lines);
        } else if (panelTooltip === "filterRoads") {
            maxWidth = "184rem";
            content = renderTooltipBlock(text.panel.filterRoads, [text.tooltips.filterRoads]);
        } else if (panelTooltip === "filterRails") {
            maxWidth = "184rem";
            content = renderTooltipBlock(text.panel.filterRails, [text.tooltips.filterRails]);
        } else if (panelTooltip === "filterWater") {
            maxWidth = "184rem";
            content = renderTooltipBlock(text.panel.filterWater, [text.tooltips.filterWater]);
        } else if (panelTooltip === "apply") {
            maxWidth = "190rem";
            content = renderTooltipBlock(text.buttons.apply, [text.tooltips.apply]);
        } else if (panelTooltip === "applyTrain") {
            maxWidth = "200rem";
            content = renderTooltipBlock(text.buttons.applyTrain, [text.tooltips.applyTrain]);
        } else if (panelTooltip === "applySubway") {
            maxWidth = "200rem";
            content = renderTooltipBlock(text.buttons.applySubway, [text.tooltips.applySubway]);
        } else {
            maxWidth = "120rem";
            content = renderTooltipBlock(text.tooltips.unit.title, text.tooltips.unit.lines);
        }

        return (
            <PanelSideTooltip
                visible={true}
                position={position}
                leftOffsetPx={leftOffset}
                topOffsetPx={topOffset}
                maxWidth={maxWidth}
                fontSize={fontSize}
                tooltipBaseStyle={tooltipBaseStyle}
                content={content}
            />
        );
    };

    const renderMarkerTooltip = () => {
        if (markerTooltipText.length === 0) {
            return null;
        }

        const markerTooltipWidth = 168;
        const markerTooltipLeftShift = 50;
        const left = Math.max(8, Math.min(window.innerWidth - markerTooltipWidth - 8, markerTooltipX - (markerTooltipWidth / 2) - markerTooltipLeftShift));
        const top = Math.max(8, Math.min(window.innerHeight - 44, markerTooltipY + 4));

        return (
            <div style={{
                position: "fixed",
                left: `${left}px`,
                top: `${top}px`,
                zIndex: 1000001,
                pointerEvents: "none",
                backgroundColor: "transparent",
                color: "rgba(255, 255, 255, 1.0)",
                fontSize: markerTooltipFontSize,
                lineHeight: "1.2",
                fontWeight: "900",
                textShadow: "0 0 4rem rgba(0,0,0,0.95), 0 0 2rem rgba(0,0,0,0.95)",
                paddingTop: "5rem",
                paddingRight: "10rem",
                paddingBottom: "5rem",
                paddingLeft: "10rem",
                borderWidth: "1rem",
                borderStyle: "solid",
                borderColor: "rgba(120, 220, 255, 0.7)",
                borderRadius: "4rem",
                minWidth: "126rem",
                textAlign: "center"
            }}>
                {markerTooltipText}
            </div>
        );
    };

    return (
        <>
            {renderSideTooltip()}
            {renderMarkerTooltip()}

            {isGuideHovered && panelTooltipsEnabled && (
                <div style={{
                    ...tooltipBaseStyle,
                    left: `${position.x + panelWidth * REM_TO_TOOLTIP_PX + RIGHT_TOOLTIP_GAP_PX}px`,
                    top: `${position.y + 30}px`,
                    maxWidth: "236rem"
                }}>
                    <div style={{
                        color: "rgba(255, 255, 255, 0.92)",
                        fontWeight: "bold",
                        marginBottom: "7rem"
                    }}>
                        {text.hint.title}
                    </div>
                    {text.help.directions.map((line: string, index: number) => (
                        <div
                            key={`help-${index}`}
                            style={index === 0 ? undefined : { marginTop: "5rem" }}
                        >
                            {line}
                        </div>
                    ))}
                </div>
            )}

            {isHelpHovered && (
                <div style={{
                    ...tooltipBaseStyle,
                    left: `${position.x + panelWidth * REM_TO_TOOLTIP_PX + RIGHT_TOOLTIP_GAP_PX}px`,
                    top: `${position.y + 30}px`,
                    maxWidth: "210rem"
                }}>
                    <div style={{
                        color: panelTooltipsEnabled ? "rgba(255, 255, 255, 0.92)" : "rgba(111, 218, 255, 0.98)",
                        fontWeight: "bold"
                    }}>
                        {panelTooltipsEnabled ? text.help.tooltipsOn : text.help.tooltipsOff}
                    </div>
                </div>
            )}
        </>
    );
};
