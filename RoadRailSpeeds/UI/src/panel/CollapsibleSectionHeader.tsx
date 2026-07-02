// File: UI/src/panel/CollapsibleSectionHeader.tsx
// Purpose: Shared collapsible section header used across the speed tool panel.

import type { ReactNode } from "react";

type CollapsibleSectionHeaderProps = {
    label: string;
    expanded: boolean;
    onToggle: () => void;
    leading?: ReactNode;
    // Optional right-aligned content shown on the header line, just left of the chevron.
    // Used by the Stats section to put the Moving/Parked/Total column headers on the title row.
    trailing?: ReactNode;
};

export const CollapsibleSectionHeader = (props: CollapsibleSectionHeaderProps) => {
    const { label, expanded, onToggle, leading, trailing } = props;

    return (
        <button
            onClick={onToggle}
            style={{
                display: "flex",
                alignItems: "center",
                width: "100%",
                minHeight: "18rem",
                paddingTop: "0",
                paddingRight: "0",
                paddingBottom: "3rem",
                paddingLeft: "0",
                // Was 5rem — shared by every section header (Selected segment/Presets/Whole city/
                // Stats), so this tightens the gap to each section's content uniformly.
                marginBottom: expanded ? "2rem" : "0",
                backgroundColor: "transparent",
                borderWidth: "0",
                borderStyle: "solid",
                color: "rgba(226, 236, 241, 0.78)",
                cursor: "pointer",
                fontSize: "12rem",
                fontWeight: 500,
                textAlign: "left"
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
                <span>{label}</span>
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
                    width: "11rem",
                    height: "11rem",
                    marginTop: "2rem",
                    marginLeft: "6rem",
                    filter: "brightness(0) invert(1)",
                    opacity: 0.72
                }}
            />
        </button>
    );
};
