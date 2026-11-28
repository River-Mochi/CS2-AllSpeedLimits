// <copyright file="SpeedToolHint.tsx" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: UI/src/entry/SpeedToolHint.tsx
// Purpose: Simple start panel shown before a network segment is selected.

import { Tooltip } from "cs2/ui";
import type { MouseEvent } from "react";
import { useState } from "react";
import { VanillaComponentResolver } from "../utils/vanilla/VanillaComponentResolver";
import { useText } from "../shared/localization";
import { SELECTION_COUNTER, SetToolActive, TOOL_ACTIVE, OPEN_ALL_PATHS, MAIN_PANEL_OPEN } from "../shared/bindings";
import { useSafeBinding } from "../shared/useSafeBinding";
import { CompactInvisibleNetworkButton } from "../panel/components/SelectionFilterControls";
import { shouldIgnorePanelDragTarget, useHintPanelPosition } from "../shared/useSharedPanelPosition";

// Switch to true to compare the bottom-right layout during UI development.
const kInvisibleButtonOnRight = false;

export const SpeedToolHint = () => {
    const TEXT = useText().inCity;
    const toolActive = useSafeBinding(TOOL_ACTIVE, false);
    const selectionCounter = useSafeBinding(SELECTION_COUNTER, 0);
    const allPathsOpen = useSafeBinding(OPEN_ALL_PATHS, false);
    const mainPanelOpen = useSafeBinding(MAIN_PANEL_OPEN, false);

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
            textAlign: kInvisibleButtonOnRight ? "left" : "center"
        }}>
            <div style={{
                paddingLeft: kInvisibleButtonOnRight ? "0" : "34rem",
                paddingRight: kInvisibleButtonOnRight ? "22rem" : "34rem",
                fontSize: "18rem",
                fontWeight: 700,
                color: "rgba(229, 244, 252, 0.90)",
                lineHeight: "1.2"
            }}>
                {TEXT.hint.instruction}
            </div>
            <div style={{
                marginTop: "4rem",
                paddingRight: kInvisibleButtonOnRight ? "34rem" : "0",
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
                width: "auto",
                minWidth: "295rem",
                maxWidth: "365rem",
                pointerEvents: "auto"
            }}
        >
            <Panel className={PanelTheme.panel}>
                <div
                    onMouseDown={handleMouseDown}
                    style={{
                        position: "relative",
                        cursor: isDragging ? "grabbing" : "grab"
                    }}
                >
                    <Tooltip
                        disabled={isDragging}
                        direction="up"
                        tooltip={
                            <div>
                                {guideLines.map((line, index) => (
                                    <div key={index}>{`▪ ${line}`}</div>
                                ))}
                            </div>
                        }
                    >
                        {instructions}
                    </Tooltip>
                    <div style={kInvisibleButtonOnRight
                        ? { position: "absolute", right: "4rem", bottom: "4rem" }
                        : { position: "absolute", left: "2rem", top: "2rem" }}>
                        <CompactInvisibleNetworkButton
                            text={TEXT.panel.showInvisible}
                            tooltip={!isDragging ? TEXT.tooltips.showInvisible : undefined}
                        />
                    </div>
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
