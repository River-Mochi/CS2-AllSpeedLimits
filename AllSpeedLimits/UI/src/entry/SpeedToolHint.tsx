// File: UI/src/entry/SpeedToolHint.tsx
// Purpose: First-use hint panel shown before a road/rail segment is selected.

import type { MouseEvent } from "react";
import { useState } from "react";
import { VanillaComponentResolver } from "../utils/vanilla/VanillaComponentResolver";
import { useText } from "../shared/localization";
import { SELECTION_COUNTER, SetToolActive, TOOL_ACTIVE } from "../shared/bindings";
import { useSafeBinding } from "../shared/useSafeBinding";
import { shouldIgnorePanelDragTarget, useHintPanelPosition } from "../shared/useSharedPanelPosition";

export const SpeedToolHint = () => {
    const TEXT = useText().inCity;
    const toolActive = useSafeBinding(TOOL_ACTIVE, false);
    const selectionCounter = useSafeBinding(SELECTION_COUNTER, 0);

    // Only show when tool is active but no selection has been made yet
    const shouldShow = toolActive && selectionCounter === 0;

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

    return (
        <div
            ref={panelRef}
            // The world marker tooltip overlay is mounted outside this hint panel. This attribute
            // keeps marker tooltips from appearing while the player is using ASL panel UI.
            data-asl-marker-tooltip-block="true"
            style={{
                position: "absolute",
                left: `${position.x}px`,
                top: `${position.y}px`,
                width: "350rem",
                pointerEvents: "auto"
            }}>
            <Panel
                header={
                    <div 
                        onMouseDown={handleMouseDown}
                        style={{ 
                            display: "flex", 
                            justifyContent: "space-between", 
                            alignItems: "center",
                            width: "100%",
                            cursor: isDragging ? "grabbing" : "grab"
                        }}
                    >
                        <span style={{ paddingLeft: "10rem" }}>{TEXT.hint.title}</span>
                        <button 
                            onClick={handleClose}
                            onMouseEnter={() => setIsCloseHovered(true)}
                            onMouseLeave={() => setIsCloseHovered(false)}
                            title={TEXT.buttons.close}
                            style={{
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
                                    opacity: isCloseHovered ? 1 : 0.84
                                }}
                            />
                        </button>
                    </div>
                }
                className={PanelTheme.panel}
            >
                <div
                    onMouseDown={handleMouseDown}
                    style={{ 
                        paddingTop: "6rem",
                        paddingRight: "16rem",
                        paddingBottom: "10rem",
                        paddingLeft: "16rem",
                        textAlign: "center",
                        cursor: isDragging ? "grabbing" : "grab"
                }}>
                    <div style={{
                        fontSize: "16rem",
                        color: "rgba(226, 236, 241, 0.82)",
                        lineHeight: "1.2"
                    }}>
                        {TEXT.hint.instruction}
                    </div>
                </div>
            </Panel>
        </div>
    );
};
