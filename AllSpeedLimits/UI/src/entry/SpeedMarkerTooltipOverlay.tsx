// File: UI/src/entry/SpeedMarkerTooltipOverlay.tsx
// Purpose: Always-mounted tooltip layer for floating world speed-number markers.

import { useEffect, useState } from "react";
import {
    MARKER_TOOLTIP_TEXT,
    MARKER_TOOLTIP_X,
    MARKER_TOOLTIP_Y,
    TOOLTIP_FONT_SCALE
} from "../shared/bindings";
import { useSafeBinding } from "../shared/useSafeBinding";

type MousePoint = {
    x: number;
    y: number;
};

const PANEL_TOOLTIP_BLOCK_SELECTOR = "[data-asl-marker-tooltip-block='true']";

const isMouseOverAslPanel = (mousePoint: MousePoint | null): boolean => {
    if (mousePoint === null) {
        return false;
    }

    const element = document.elementFromPoint(mousePoint.x, mousePoint.y);
    return element?.closest(PANEL_TOOLTIP_BLOCK_SELECTOR) !== null;
};

export const SpeedMarkerTooltipOverlay = () => {
    const markerTooltipText = useSafeBinding(MARKER_TOOLTIP_TEXT, "");
    const markerTooltipX = useSafeBinding(MARKER_TOOLTIP_X, 0);
    const markerTooltipY = useSafeBinding(MARKER_TOOLTIP_Y, 0);
    const tooltipFontScale = Math.max(100, Math.min(140, useSafeBinding(TOOLTIP_FONT_SCALE, 110)));
    const [mousePoint, setMousePoint] = useState<MousePoint | null>(null);

    useEffect(() => {
        const handleMouseMove = (event: MouseEvent) => {
            setMousePoint({ x: event.clientX, y: event.clientY });
        };

        window.addEventListener("mousemove", handleMouseMove);
        return () => window.removeEventListener("mousemove", handleMouseMove);
    }, []);

    if (markerTooltipText.length === 0 || isMouseOverAslPanel(mousePoint)) {
        return null;
    }

    // Floating speed-number tooltips are NOT panel tooltips.
    // Do not render this with PanelSideTooltip: that component adds the dark panel-help
    // background/border and treats offsets as panel-relative. markerTooltipX/Y are screen
    // coordinates from C#, and X is the marker center, so this renderer centers itself.
    const markerTooltipWidth = 320;
    const left = Math.max(8, Math.min(window.innerWidth - markerTooltipWidth - 8, markerTooltipX - (markerTooltipWidth / 2)));
    const top = Math.max(8, Math.min(window.innerHeight - 44, markerTooltipY + 4));
    const markerTooltipFontSize = `${20 * tooltipFontScale / 100}rem`;

    return (
        <div style={{
            position: "fixed",
            left: `${left}px`,
            top: `${top}px`,
            width: `${markerTooltipWidth}px`,
            zIndex: 1000001,
            pointerEvents: "none",
            backgroundColor: "transparent",
            color: "rgba(255, 255, 255, 1)",
            fontSize: markerTooltipFontSize,
            lineHeight: "1.2",
            fontWeight: 900,
            textShadow: "0 0 4rem rgba(0,0,0,0.95), 0 0 2rem rgba(0,0,0,0.95)",
            paddingTop: "0",
            paddingRight: "0",
            paddingBottom: "0",
            paddingLeft: "0",
            borderWidth: "0",
            borderStyle: "solid",
            borderColor: "transparent",
            textAlign: "center",
            whiteSpace: "nowrap",
            boxSizing: "border-box"
        }}>
            {markerTooltipText}
        </div>
    );
};
