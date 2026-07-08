// File: UI/src/panel/CollapsibleSectionHeader.tsx
// Purpose: Shared collapsible section header used across the speed tool panel.

import type { ReactNode } from "react";
import { useState } from "react";
import { Button } from "../shared/Button";

type CollapsibleSectionHeaderProps = {
    label: string;
    expanded: boolean;
    onToggle: () => void;
    leading?: ReactNode;
    // Optional right-aligned content shown on the header line, just left of the chevron.
    // Used by the Stats section to put the Moving/Parked/Total column headers on the title row.
    trailing?: ReactNode;
    compactCollapsed?: boolean;
    focusKey?: unknown;
    labelTitle?: string;
    onLabelMouseEnter?: () => void;
    onLabelMouseLeave?: () => void;
};

export const CollapsibleSectionHeader = (props: CollapsibleSectionHeaderProps) => {
    const {
        label,
        expanded,
        onToggle,
        leading,
        trailing,
        compactCollapsed = false,
        focusKey,
        labelTitle,
        onLabelMouseEnter,
        onLabelMouseLeave
    } = props;
    const [hovered, setHovered] = useState(false);
    const compact = compactCollapsed && !expanded;

    return (
        <Button
            focusKey={focusKey}
            onSelect={onToggle}
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
            style={{
                display: "flex",
                alignItems: "center",
                justifyContent: "flex-start",
                width: "100%",
                minHeight: compact ? "14rem" : "18rem",
                paddingTop: "0",
                paddingRight: "0",
                paddingBottom: compact ? "1rem" : "3rem",
                paddingLeft: "0",
                // Was 5rem — shared by every section header (Selected segment/Presets/Whole city/
                // Stats), so this tightens the gap to each section's content uniformly.
                marginBottom: expanded ? "2rem" : "0",
                backgroundColor: "transparent",
                borderWidth: "0",
                borderStyle: "solid",
                color: hovered ? "rgba(255, 255, 255, 0.94)" : "rgba(226, 236, 241, 0.78)",
                cursor: "pointer",
                fontSize: "12.5rem",
                fontWeight: 500,
                textAlign: "left",
                lineHeight: "1",
                boxShadow: "none"
            }}
        >
            <span style={{
                flex: 1,
                display: "flex",
                alignItems: "center",
                minWidth: "0"
            }}>
                {leading && (
                    <span style={{
                        display: "flex",
                        alignItems: "center",
                        marginRight: "6rem"
                    }}>
                        {leading}
                    </span>
                )}
                <span
                    title={labelTitle}
                    onMouseEnter={onLabelMouseEnter}
                    onMouseLeave={onLabelMouseLeave}
                >
                    {label}
                </span>
            </span>
            {trailing && (
                <span style={{
                    display: "flex",
                    alignItems: "center",
                    flexShrink: 0
                }}>
                    {trailing}
                </span>
            )}
            <img
                src={`Media/Glyphs/${expanded ? "ThickStrokeArrowDown.svg" : "ThickStrokeArrowRight.svg"}`}
                alt=""
                style={{
                    display: "block",
                    width: "14.5rem",
                    height: "14.5rem",
                    marginTop: "2rem",
                    marginLeft: "6rem",
                    filter: "brightness(0) invert(1)",
                    opacity: hovered ? 1 : (expanded ? 0.78 : 0.58),
                    pointerEvents: "none"
                }}
            />
        </Button>
    );
};
