// File: UI/src/shared/useSharedPanelPosition.ts
// Purpose: Draggable screen position for a panel, backed by a pluggable get/set position store so
// the hint panel and the tool panel can each keep their own independent position.

import { useEffect, useRef, useState } from "react";
import {
    getHintPanelPosition,
    setHintPanelPosition,
    getToolPanelPosition,
    setToolPanelPosition
} from "./bindings";

type PanelPosition = {
    x: number;
    y: number;
};

type DragState = {
    startX: number;
    startY: number;
    initialX: number;
    initialY: number;
};

export const shouldIgnorePanelDragTarget = (target: EventTarget | null): boolean => {
    let element = target as HTMLElement | null;

    while (element !== null) {
        if (element.tagName === "BUTTON" || element.tagName === "INPUT") {
            return true;
        }

        element = element.parentElement;
    }

    return false;
};

const usePanelPosition = (
    getStoredPosition: () => PanelPosition,
    setStoredPosition: (position: PanelPosition) => void
) => {
    const [position, setPosition] = useState<PanelPosition>(getStoredPosition());
    const [isDragging, setIsDragging] = useState(false);
    const dragRef = useRef<DragState>({
        startX: 0,
        startY: 0,
        initialX: 0,
        initialY: 0
    });

    // Re-clamp if the window resizes while the panel is already open (e.g. a live resolution change
    // mid-session, no restart) — the store only re-clamps on the next read, which covers reopening
    // the tool, but not a panel that's already mounted and visible when the resolution changes.
    useEffect(() => {
        const handleResize = () => {
            setPosition(getStoredPosition());
        };

        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    useEffect(() => {
        if (!isDragging) {
            return;
        }

        const handleMouseMove = (event: MouseEvent) => {
            const deltaX = event.clientX - dragRef.current.startX;
            const deltaY = event.clientY - dragRef.current.startY;

            const nextPosition = {
                x: dragRef.current.initialX + deltaX,
                y: dragRef.current.initialY + deltaY
            };

            setStoredPosition(nextPosition);
            setPosition(getStoredPosition());
        };

        const handleMouseUp = () => {
            setIsDragging(false);
        };

        document.addEventListener("mousemove", handleMouseMove);
        document.addEventListener("mouseup", handleMouseUp);

        return () => {
            document.removeEventListener("mousemove", handleMouseMove);
            document.removeEventListener("mouseup", handleMouseUp);
        };
    }, [isDragging]);

    const startDragging = (clientX: number, clientY: number) => {
        setIsDragging(true);
        dragRef.current = {
            startX: clientX,
            startY: clientY,
            initialX: position.x,
            initialY: position.y
        };
    };

    // Programmatic jumps used to re-anchor the tool panel next to a fresh segment click. Keep this
    // as a no-op so the panel remembers where the player parked it during the current session.
    const snapTo = (nextPosition: PanelPosition) => {
        setPosition(getStoredPosition());
    };

    return {
        position,
        isDragging,
        startDragging,
        snapTo
    };
};

export const useHintPanelPosition = () => usePanelPosition(getHintPanelPosition, setHintPanelPosition);

export const useToolPanelPosition = () => usePanelPosition(getToolPanelPosition, setToolPanelPosition);
