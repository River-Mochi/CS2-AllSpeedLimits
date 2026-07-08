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
    originLeft: number;
    originTop: number;
    originWidth: number;
    originHeight: number;
};

const PANEL_LEFT_MARGIN_PX = 10;
const PANEL_TOP_MARGIN_PX = 0;
const PANEL_RIGHT_MARGIN_PX = 10;
const PANEL_BOTTOM_MARGIN_PX = 10;

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

const nearlyEqual = (a: number, b: number) => Math.abs(a - b) < 0.5;

const clampToViewport = (position: PanelPosition): PanelPosition => {
    const maxX = Math.max(PANEL_LEFT_MARGIN_PX, window.innerWidth - PANEL_RIGHT_MARGIN_PX);
    const maxY = Math.max(PANEL_TOP_MARGIN_PX, window.innerHeight - PANEL_BOTTOM_MARGIN_PX);

    return {
        x: Math.min(Math.max(PANEL_LEFT_MARGIN_PX, position.x), maxX),
        y: Math.min(Math.max(PANEL_TOP_MARGIN_PX, position.y), maxY)
    };
};

const usePanelPosition = (
    getStoredPosition: () => PanelPosition,
    setStoredPosition: (position: PanelPosition) => void,
    savedPosition?: PanelPosition,
    saveStoredPosition?: (position: PanelPosition) => void
) => {
    const getInitialPosition = () => {
        if (savedPosition !== undefined && savedPosition.x >= 0 && savedPosition.y >= 0) {
            const clamped = clampToViewport(savedPosition);
            setStoredPosition(clamped);
            return clamped;
        }

        return getStoredPosition();
    };

    const [position, setPositionState] = useState<PanelPosition>(getInitialPosition);
    const [isDragging, setIsDragging] = useState(false);
    const panelRef = useRef<HTMLDivElement | null>(null);
    const positionRef = useRef<PanelPosition>(position);
    const dragRef = useRef<DragState | null>(null);

    const setPosition = (nextPosition: PanelPosition) => {
        positionRef.current = nextPosition;
        setStoredPosition(nextPosition);
        setPositionState(nextPosition);
    };

    const clampMountedPanel = () => {
        const rect = panelRef.current?.getBoundingClientRect();
        if (rect === undefined) {
            const storedPosition = getStoredPosition();
            positionRef.current = storedPosition;
            setPositionState(storedPosition);
            return;
        }

        let nextX = positionRef.current.x;
        let nextY = positionRef.current.y;
        const rightLimit = window.innerWidth - PANEL_RIGHT_MARGIN_PX;
        const bottomLimit = window.innerHeight - PANEL_BOTTOM_MARGIN_PX;

        if (rect.left < PANEL_LEFT_MARGIN_PX) {
            nextX += PANEL_LEFT_MARGIN_PX - rect.left;
        }
        if (rect.top < PANEL_TOP_MARGIN_PX) {
            nextY += PANEL_TOP_MARGIN_PX - rect.top;
        }
        if (rect.right > rightLimit) {
            nextX -= rect.right - rightLimit;
        }
        if (rect.bottom > bottomLimit) {
            nextY -= rect.bottom - bottomLimit;
        }

        if (nearlyEqual(nextX, positionRef.current.x) && nearlyEqual(nextY, positionRef.current.y)) {
            return;
        }

        const nextPosition = { x: nextX, y: nextY };
        setPosition(nextPosition);
    };

    useEffect(() => {
        clampMountedPanel();

        const handleResize = () => {
            const storedPosition = getStoredPosition();
            positionRef.current = storedPosition;
            setPositionState(storedPosition);
            window.requestAnimationFrame(clampMountedPanel);
        };

        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    useEffect(() => {
        if (savedPosition === undefined || savedPosition.x < 0 || savedPosition.y < 0) {
            return;
        }

        const nextPosition = clampToViewport(savedPosition);
        if (nearlyEqual(nextPosition.x, positionRef.current.x) && nearlyEqual(nextPosition.y, positionRef.current.y)) {
            return;
        }

        positionRef.current = nextPosition;
        setStoredPosition(nextPosition);
        setPositionState(nextPosition);
        window.requestAnimationFrame(clampMountedPanel);
    }, [savedPosition?.x, savedPosition?.y]);

    useEffect(() => {
        if (!isDragging) {
            return;
        }

        const handleMouseMove = (event: MouseEvent) => {
            const dragState = dragRef.current;
            if (dragState === null) {
                return;
            }

            const deltaX = event.clientX - dragState.startX;
            const deltaY = event.clientY - dragState.startY;
            let nextX = dragState.initialX + deltaX;
            let nextY = dragState.initialY + deltaY;
            const nextLeft = dragState.originLeft + deltaX;
            const nextTop = dragState.originTop + deltaY;
            const nextRight = nextLeft + dragState.originWidth;
            const nextBottom = nextTop + dragState.originHeight;
            const rightLimit = window.innerWidth - PANEL_RIGHT_MARGIN_PX;
            const bottomLimit = window.innerHeight - PANEL_BOTTOM_MARGIN_PX;

            if (nextLeft < PANEL_LEFT_MARGIN_PX) {
                nextX += PANEL_LEFT_MARGIN_PX - nextLeft;
            }
            if (nextTop < PANEL_TOP_MARGIN_PX) {
                nextY += PANEL_TOP_MARGIN_PX - nextTop;
            }
            if (nextRight > rightLimit) {
                nextX -= nextRight - rightLimit;
            }
            if (nextBottom > bottomLimit) {
                nextY -= nextBottom - bottomLimit;
            }

            setPosition({ x: nextX, y: nextY });
        };

        const handleMouseUp = () => {
            dragRef.current = null;
            setIsDragging(false);
            saveStoredPosition?.(positionRef.current);
        };

        window.addEventListener("mousemove", handleMouseMove);
        window.addEventListener("mouseup", handleMouseUp);

        return () => {
            window.removeEventListener("mousemove", handleMouseMove);
            window.removeEventListener("mouseup", handleMouseUp);
        };
    }, [isDragging]);

    const startDragging = (clientX: number, clientY: number) => {
        const rect = panelRef.current?.getBoundingClientRect();
        if (rect === undefined) {
            return;
        }

        setIsDragging(true);
        dragRef.current = {
            startX: clientX,
            startY: clientY,
            initialX: positionRef.current.x,
            initialY: positionRef.current.y,
            originLeft: rect.left,
            originTop: rect.top,
            originWidth: rect.width,
            originHeight: rect.height
        };
    };

    // Programmatic jumps used to re-anchor the tool panel next to a fresh segment click. Keep this
    // as a no-op so the panel remembers where the player parked it during the current session.
    const snapTo = (nextPosition: PanelPosition) => {
        const storedPosition = getStoredPosition();
        positionRef.current = storedPosition;
        setPositionState(storedPosition);
        window.requestAnimationFrame(clampMountedPanel);
    };

    return {
        panelRef,
        position,
        isDragging,
        startDragging,
        snapTo
    };
};

export const useHintPanelPosition = () => usePanelPosition(getHintPanelPosition, setHintPanelPosition);

export const useToolPanelPosition = (
    savedPosition?: PanelPosition,
    saveStoredPosition?: (position: PanelPosition) => void
) => usePanelPosition(getToolPanelPosition, setToolPanelPosition, savedPosition, saveStoredPosition);
