// File: UI/src/panel/components/SpeedToolOverlays.tsx
// Purpose: Floating side/help tooltips for the speed tool panel.

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
    markersTooltipText: string;
    expandAllTooltipText: string;
    isGuideHovered: boolean;
    isHelpHovered: boolean;
    isMarkersHovered: boolean;
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
        markersTooltipText,
        expandAllTooltipText,
        isGuideHovered,
        isHelpHovered,
        isMarkersHovered,
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
        } else if (panelTooltip === "currentSpeed") {
            maxWidth = "190rem";
            content = renderTooltipBlock(text.panel.currentSpeed, text.tooltips.currentSpeed.split("\n"));
        } else if (panelTooltip === "panelTitle") {
            maxWidth = "170rem";
            content = renderTooltipBlock("", [text.tooltips.panelTitle]);
        } else if (panelTooltip === "presets") {
            maxWidth = "190rem";
            content = renderTooltipBlock(text.panel.presets, [text.tooltips.presets]);
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
        } else if (panelTooltip === "stats") {
            maxWidth = "190rem";
            content = renderTooltipBlock(text.tooltips.stats.title, text.tooltips.stats.lines);
        } else if (panelTooltip === "statsBikes") {
            maxWidth = "220rem";
            content = renderTooltipBlock(text.stats.bikes, [text.tooltips.statsRows.bikes]);
        } else if (panelTooltip === "statsCars") {
            maxWidth = "220rem";
            content = renderTooltipBlock(text.stats.cars, [text.tooltips.statsRows.cars]);
        } else if (panelTooltip === "statsIndustry") {
            maxWidth = "238rem";
            content = renderTooltipBlock(text.stats.industry, [text.tooltips.statsRows.industry]);
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
        } else if (panelTooltip === "expandAll") {
            maxWidth = "184rem";
            content = renderTooltipBlock("", [expandAllTooltipText]);
        } else if (panelTooltip === "markers") {
            maxWidth = "210rem";
            content = renderTooltipBlock("", [markersTooltipText]);
        } else if (panelTooltip === "selectedSegment") {
            maxWidth = "210rem";
            content = renderTooltipBlock(text.panel.selectedSegment, [text.tooltips.selectedSegment]);
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

    return (
        <>
            {isGuideHovered && (
                <PanelSideTooltip
                    visible={true}
                    position={position}
                    leftOffsetPx={panelWidth * REM_TO_TOOLTIP_PX + RIGHT_TOOLTIP_GAP_PX}
                    topOffsetPx={20}
                    maxWidth="200rem"
                    fontSize={tooltipFontSize}
                    tooltipBaseStyle={tooltipBaseStyle}
                    content={renderTooltipBlock("", text.help.directions)}
                />
            )}
            {isMarkersHovered && (
                <PanelSideTooltip
                    visible={true}
                    position={position}
                    leftOffsetPx={panelWidth * REM_TO_TOOLTIP_PX + RIGHT_TOOLTIP_GAP_PX}
                    topOffsetPx={28}
                    maxWidth="210rem"
                    fontSize={tooltipFontSize}
                    tooltipBaseStyle={tooltipBaseStyle}
                    content={renderTooltipBlock("", [markersTooltipText])}
                />
            )}
            {isHelpHovered && (
                <PanelSideTooltip
                    visible={true}
                    position={position}
                    leftOffsetPx={panelWidth * REM_TO_TOOLTIP_PX + RIGHT_TOOLTIP_GAP_PX}
                    topOffsetPx={30}
                    maxWidth="210rem"
                    fontSize={tooltipFontSize}
                    tooltipBaseStyle={tooltipBaseStyle}
                    content={(
                        <div style={{
                            color: panelTooltipsEnabled ? "rgba(255, 255, 255, 0.92)" : "rgba(111, 218, 255, 0.98)",
                            fontWeight: "bold"
                        }}>
                            {panelTooltipsEnabled ? text.help.tooltipsOn : text.help.tooltipsOff}
                        </div>
                    )}
                />
            )}
            {renderSideTooltip()}
        </>
    );
};
