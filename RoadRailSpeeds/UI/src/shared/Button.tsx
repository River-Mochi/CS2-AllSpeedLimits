// File: UI/src/shared/Button.tsx
// Purpose: Shared COHTML-safe button wrapper for the road/rail speed UI.

import React, { CSSProperties, MouseEvent, useState } from "react";

type ButtonVariant = "default" | "neutral" | "city" | "danger";

type ButtonProps = {
    selected?: boolean;
    disabled?: boolean;
    style?: CSSProperties;
    className?: string;
    onSelect?: (event: MouseEvent<HTMLButtonElement>) => void;
    onMouseEnter?: () => void;
    onMouseLeave?: () => void;
    variant?: ButtonVariant;
    children?: React.ReactNode;
    title?: string;
    focusKey?: unknown;
};

const getColors = (variant: ButtonVariant, selected: boolean, hovered: boolean) => {
    if (variant === "danger") {
        return {
            backgroundColor: selected ? "rgba(183, 96, 96, 0.82)" : hovered ? "rgba(169, 90, 90, 0.82)" : "rgba(142, 71, 71, 0.72)",
            borderColor: selected ? "rgba(255, 255, 255, 0.38)" : hovered ? "rgba(255, 229, 229, 0.48)" : "rgba(255, 255, 255, 0)"
        };
    }

    if (variant === "city") {
        return {
            backgroundColor: selected ? "rgba(207, 148, 59, 0.86)" : hovered ? "rgba(200, 137, 47, 0.86)" : "rgba(168, 111, 34, 0.78)",
            borderColor: selected ? "rgba(255, 255, 255, 0.38)" : hovered ? "rgba(255, 241, 214, 0.48)" : "rgba(255, 255, 255, 0)"
        };
    }

    if (variant === "neutral") {
        return {
            backgroundColor: selected ? "rgba(120, 210, 245, 0.16)" : hovered ? "rgba(255, 255, 255, 0.13)" : "rgba(255, 255, 255, 0.08)",
            borderColor: selected ? "rgba(120, 210, 245, 0.38)" : hovered ? "rgba(240, 246, 255, 0.38)" : "rgba(255, 255, 255, 0)"
        };
    }

    return {
        backgroundColor: selected ? "rgba(67, 191, 227, 0.66)" : hovered ? "rgba(56, 181, 216, 0.64)" : "rgba(42, 166, 203, 0.52)",
        borderColor: selected ? "rgba(255, 255, 255, 0.36)" : hovered ? "rgba(243, 251, 255, 0.42)" : "rgba(255, 255, 255, 0)"
    };
};

export const Button = (props: ButtonProps) => {
    const {
        selected = false,
        disabled = false,
        style,
        className,
        onSelect,
        onMouseEnter,
        onMouseLeave,
        variant = "default",
        children,
        title
    } = props;
    const [hovered, setHovered] = useState(false);
    const colors = getColors(variant, selected, hovered && !disabled);

    const handleClick = (event: MouseEvent<HTMLButtonElement>) => {
        if (disabled) {
            event.preventDefault();
            event.stopPropagation();
            return;
        }

        onSelect?.(event);
    };

    return (
        <button
            type="button"
            tabIndex={-1}
            className={className}
            onClick={handleClick}
            onMouseDown={event => event.preventDefault()}
            onMouseEnter={() => {
                setHovered(true);
                onMouseEnter?.();
            }}
            onMouseLeave={() => {
                setHovered(false);
                onMouseLeave?.();
            }}
            title={title}
            style={{
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                minHeight: "24rem",
                paddingTop: "4rem",
                paddingRight: "14rem",
                paddingBottom: "4rem",
                paddingLeft: "14rem",
                borderWidth: "1rem",
                borderStyle: "solid",
                borderColor: colors.borderColor,
                borderRadius: "4rem",
                backgroundColor: colors.backgroundColor,
                color: "#fff",
                fontSize: "12rem",
                fontWeight: 700,
                cursor: disabled ? "default" : "pointer",
                whiteSpace: "nowrap",
                lineHeight: "1",
                opacity: disabled ? 0.5 : 1,
                ...style
            }}
        >
            {children}
        </button>
    );
};
