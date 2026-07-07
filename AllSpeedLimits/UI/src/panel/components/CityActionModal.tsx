// File: UI/src/panel/components/CityActionModal.tsx
// Purpose: Confirmation popup for whole-city apply and reset actions.

import type { ReactNode } from "react";
import { Button } from "../../shared/Button";

type CityActionModalProps = {
    visible: boolean;
    position: { x: number; y: number };
    panelWidthRem: number;
    confirmFontSize: string;
    confirmTitleFontSize: string;
    title: string;
    messageContent: ReactNode;
    confirmLabel: string;
    onCancel: () => void;
    onConfirm: () => void;
    confirmVariant: "city" | "danger";
    cancelLabel: string;
    focusKey: unknown;
};

export const CityActionModal = (props: CityActionModalProps) => {
    const {
        visible,
        position,
        panelWidthRem,
        confirmFontSize,
        confirmTitleFontSize,
        title,
        messageContent,
        confirmLabel,
        onCancel,
        onConfirm,
        confirmVariant,
        cancelLabel,
        focusKey
    } = props;

    if (!visible) {
        return null;
    }

    return (
        // Outer wrapper spans the full panel width and centers the inset popup. This avoids mixing
        // px (panel left) with rem (popup width), so the popup stays centered over the panel at any
        // UI scale instead of drifting to the right.
        <div style={{
            position: "fixed",
            left: `${position.x}px`,
            top: `${position.y + 88}px`,
            width: `${panelWidthRem}rem`,
            display: "flex",
            justifyContent: "center",
            zIndex: 1000001,
            pointerEvents: "none"
        }}>
            <div style={{
                width: `${panelWidthRem - 8}rem`,
                backgroundColor: "rgba(28, 32, 38, 0.98)",
                color: "#fff",
                paddingTop: "10rem",
                paddingRight: "10rem",
                paddingBottom: "10rem",
                paddingLeft: "10rem",
                borderRadius: "5rem",
                fontSize: confirmFontSize,
                lineHeight: "1.35",
                borderWidth: "1rem",
                borderStyle: "solid",
                borderColor: "rgba(78, 195, 240, 0.8)",
                pointerEvents: "auto"
            }}>
                <div style={{
                    fontSize: confirmTitleFontSize,
                    fontWeight: "bold",
                    marginBottom: "6rem"
                }}>
                    {title}
                </div>
                <div style={{
                    color: "rgba(255, 255, 255, 0.82)",
                    marginBottom: "10rem"
                }}>
                    {messageContent}
                </div>
                <div style={{
                    display: "flex",
                    justifyContent: "space-between"
                }}>
                    <div style={{ width: "84rem" }}>
                        <Button
                            focusKey={focusKey}
                            onSelect={onCancel}
                            variant="neutral"
                            style={{ width: "100%", paddingTop: "4rem", paddingRight: "0", paddingBottom: "4rem", paddingLeft: "0" }}
                        >
                            {cancelLabel}
                        </Button>
                    </div>
                    <div style={{ width: "104rem" }}>
                        <Button
                            focusKey={focusKey}
                            onSelect={onConfirm}
                            variant={confirmVariant}
                            style={{ width: "100%", paddingTop: "4rem", paddingRight: "0", paddingBottom: "4rem", paddingLeft: "0" }}
                        >
                            {confirmLabel}
                        </Button>
                    </div>
                </div>
            </div>
        </div>
    );
};
