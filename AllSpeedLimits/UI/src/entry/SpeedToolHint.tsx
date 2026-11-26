// File: UI/src/entry/SpeedToolHint.tsx
// Purpose: Simple start panel shown before a network segment is selected.

import { Tooltip } from "cs2/ui";
import type { MouseEvent } from "react";
import { useState } from "react";
import { VanillaComponentResolver } from "../utils/vanilla/VanillaComponentResolver";
import { useText } from "../shared/localization";
import { SELECTION_COUNTER, SetToolActive, TOOL_ACTIVE, OPEN_ALL_PATHS, MAIN_PANEL_OPEN, PANEL_TOOLTIPS_ENABLED } from "../shared/bindings";
import { useSafeBinding } from "../shared/useSafeBinding";
import { shouldIgnorePanelDragTarget, useHintPanelPosition } from "../shared/useSharedPanelPosition";

export const SpeedToolHint = () => {
    const TEXT = useText().inCity;
    const toolActive = useSafeBinding(TOOL_ACTIVE, false);
    const selectionCounter = useSafeBinding(SELECTION_COUNTER, 0);
    const allPathsOpen = useSafeBinding(OPEN_ALL_PATHS, false);
    const mainPanelOpen = useSafeBinding(MAIN_PANEL_OPEN, false);
    const panelTooltipsEnabled = useSafeBinding(PANEL_TOOLTIPS_ENABLED, true);

    // Only show when tool is active but no selection has been made yet
    const shouldShow = toolActive && selectionCounter === 0 && !allPathsOpen && !mainPanelOpen;

    const { panelRef, position, isDragging, startDragging } = useHintPanelPosition();
    const [isCloseHovered, setIsCloseHovered] = useState(false);

    const handleClose = () => {
        SetToolActive(false);
    };

    const handleMouseDown = (event: MouseEvent) => {
        if (shouldIgnorePanelDragTarget(event.target)) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();
        startDragging(event.clientX, event.clientY);
    };

    if (!shouldShow) return null;

    const resolver = VanillaComponentResolver.instance;
    const Panel = resolver.Panel;
    const PanelTheme = resolver.panelTheme;

    const [firstGuideLine, ...otherGuideLines] = TEXT.hint.guide.split("\n");
    const guideLines = [firstGuideLine, TEXT.hint.drag, ...otherGuideLines];
    const instructions = (
        <div style={{
            paddingTop: "8rem",
            paddingRight: "8rem",
            paddingBottom: "9rem",
            paddingLeft: "8rem",
            textAlign: "center"
        }}>
            <div style={{
                paddingLeft: "22rem",
                paddingRight: "22rem",
                fontSize: "18rem",
                fontWeight: 700,
                color: "rgba(229, 244, 252, 0.90)",
                lineHeight: "1.2"
            }}>
                {TEXT.hint.instruction}
            </div>
            <div style={{
                marginTop: "4rem",
                fontSize: "14rem",
                color: "rgba(226, 236, 241, 0.90)",
                lineHeight: "1.3"
            }}>
                {TEXT.hint.alternatives}
            </div>
        </div>
    );

    return (
        <div
            ref={panelRef}
            aria-label={TEXT.hint.title}
            data-asl-marker-tooltip-block="true"
            style={{
                position: "absolute",
                left: `${position.x}px`,
                top: `${position.y}px`,
                width: "295rem",
                pointerEvents: "auto"
            }}
        >
            <Panel className={PanelTheme.panel}>
                <div
                    onMouseDown={handleMouseDown}
                    style={{
                        position: "relative",
                        width: "100%",
                        cursor: isDragging ? "grabbing" : "grab"
                    }}
                >
                    <Tooltip
                        disabled={!panelTooltipsEnabled || isDragging}
                        direction="up"
                        tooltip={
                            <div>
                                {guideLines.map((line, index) => (
                                    <div key={index}>{line}</div>
                                ))}
                            </div>
                        }
                    >
                        {instructions}
                    </Tooltip>
                    <button
                        onClick={handleClose}
                        onMouseDown={event => event.stopPropagation()}
                        onMouseEnter={() => setIsCloseHovered(true)}
                        onMouseLeave={() => setIsCloseHovered(false)}
                        aria-label={TEXT.buttons.close}
                        title={TEXT.buttons.close}
                        style={{
                            position: "absolute",
                            top: "2rem",
                            right: "2rem",
                            backgroundColor: isCloseHovered ? "rgba(255, 255, 255, 0.12)" : "transparent",
                            borderWidth: "0",
                            borderStyle: "solid",
                            width: "24rem",
                            height: "24rem",
                            cursor: "pointer",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
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
                                width: isCloseHovered ? "14rem" : "12rem",
                                height: isCloseHovered ? "14rem" : "12rem",
                                filter: "brightness(0) invert(1)",
                                opacity: isCloseHovered ? 1 : 0.84,
                                pointerEvents: "none"
                            }}
                        />
                    </button>
                </div>
            </Panel>
        </div>
    );
};
