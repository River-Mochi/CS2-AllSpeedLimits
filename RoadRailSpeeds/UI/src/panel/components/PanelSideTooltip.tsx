// File: UI/src/panel/components/PanelSideTooltip.tsx
// Purpose: Right-side tooltip layer for compact panel help text.

import type { CSSProperties, ReactNode } from "react";

type PanelSideTooltipProps = {
    visible: boolean;
    position: { x: number; y: number };
    leftOffsetPx: number;
    topOffsetPx: number;
    maxWidth: string;
    fontSize?: string;
    tooltipBaseStyle: CSSProperties;
    content: ReactNode;
};

export const PanelSideTooltip = (props: PanelSideTooltipProps) => {
    const {
        visible,
        position,
        leftOffsetPx,
        topOffsetPx,
        maxWidth,
        fontSize,
        tooltipBaseStyle,
        content
    } = props;

    if (!visible) {
        return null;
    }

    return (
        <div style={{
            ...tooltipBaseStyle,
            left: `${position.x + leftOffsetPx}px`,
            top: `${position.y + topOffsetPx}px`,
            maxWidth,
            ...(fontSize ? { fontSize } : {})
        }}>
            {content}
        </div>
    );
};
