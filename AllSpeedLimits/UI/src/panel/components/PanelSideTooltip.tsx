// <copyright file="PanelSideTooltip.tsx" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: UI/src/panel/components/PanelSideTooltip.tsx
// Purpose: Place tooltips next to the panel without going off-screen.

import { useLayoutEffect, useRef, useState } from "react";
import type { CSSProperties, ReactNode } from "react";

type PanelBounds = { left: number; right: number; top: number };

type PanelSideTooltipProps = {
    visible: boolean;
    panelBounds: PanelBounds | null;
    topOffsetPx: number;
    maxWidth: string;
    fontSize?: string;
    tooltipBaseStyle: CSSProperties;
    content: ReactNode;
};

export const PanelSideTooltip = (props: PanelSideTooltipProps) => {
    const { visible, panelBounds, topOffsetPx, maxWidth, fontSize, tooltipBaseStyle, content } = props;
    const tooltipRef = useRef<HTMLDivElement | null>(null);
    const [placement, setPlacement] = useState<{ x: number; y: number } | null>(null);

    useLayoutEffect(() => {
        if (!visible || panelBounds === null || tooltipRef.current === null) {
            return;
        }

        // Both element bounds are in viewport coordinates. A fixed rem-to-px
        // ratio broke positioning when the CS2 interface was scaled.
        const rect = tooltipRef.current.getBoundingClientRect();
        const margin = 8;
        const gap = 7;
        const rightSpace = window.innerWidth - panelBounds.right - gap - margin;
        const leftSpace = panelBounds.left - gap - margin;
        let x: number;
        if (rect.width <= rightSpace) {
            x = panelBounds.right + gap;
        } else if (rect.width <= leftSpace) {
            x = panelBounds.left - gap - rect.width;
        } else {
            x = rightSpace >= leftSpace
                ? panelBounds.right + gap
                : panelBounds.left - gap - rect.width;
        }

        x = Math.max(margin, Math.min(x, window.innerWidth - rect.width - margin));
        const y = Math.max(
            margin,
            Math.min(panelBounds.top + topOffsetPx, window.innerHeight - rect.height - margin)
        );
        setPlacement(previous => previous &&
            Math.abs(previous.x - x) < 0.5 && Math.abs(previous.y - y) < 0.5
            ? previous
            : { x, y });
    }, [
        visible, panelBounds?.left, panelBounds?.right, panelBounds?.top,
        topOffsetPx, maxWidth, fontSize, content
    ]);

    if (!visible || panelBounds === null) {
        return null;
    }

    return (
        <div
            ref={tooltipRef}
            style={{
                ...tooltipBaseStyle,
                left: `${placement?.x ?? 0}px`,
                top: `${placement?.y ?? 0}px`,
                visibility: placement === null ? "hidden" : "visible",
                maxWidth,
                ...(fontSize ? { fontSize } : {})
            }}
        >
            {content}
        </div>
    );
};
