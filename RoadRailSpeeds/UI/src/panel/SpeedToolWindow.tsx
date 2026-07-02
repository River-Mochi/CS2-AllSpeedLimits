// File: UI/src/panel/SpeedToolWindow.tsx
// Purpose: In-game panel for selecting and applying road/rail/waterway speed changes.

import { useState, useEffect, useRef } from "react";
import {
    INITIAL_SPEED,
    CURRENT_SPEED_MIXED,
    VANILLA_SPEED,
    VANILLA_SPEED_MIXED,
    TOOL_ACTIVE,
    SHOW_METRIC,
    SELECTION_COUNTER,
    IS_TRACK_TYPE,
    IS_WATERWAY_TYPE,
    DOUBLE_SPEED_DISPLAY,
    SYNC_SLIDER_WITH_SELECTION,
    PANEL_SLIDER_INCREMENT,
    TOOLTIP_FONT_SCALE,
    PANEL_TOOLTIPS_ENABLED,
    UNIT_MODE,
    CITY_CAR_TOTAL,
    CITY_CAR_ACTIVE,
    CITY_CAR_PARKED,
    CITY_BIKE_TOTAL,
    CITY_BIKE_ACTIVE,
    CITY_BIKE_PARKED,
    CITY_RESET_IN_PROGRESS,
    CITY_RESET_CLEARED,
    CITY_RESET_TOTAL,
    CITY_APPLY_IN_PROGRESS,
    CITY_APPLY_APPLIED,
    CITY_APPLY_TOTAL,
    MARKER_TOOLTIP_TEXT,
    MARKER_TOOLTIP_X,
    MARKER_TOOLTIP_Y,
    SELECTION_CLICK_X,
    SELECTION_CLICK_Y,
    MOD_ID,
    ApplySpeed,
    ApplySelectionMultiplier,
    ApplyCityRoadGroupSpeed,
    ApplyCityTrainSpeed,
    ApplyCitySubwaySpeed,
    ResetSpeed,
    ResetCityRoadDefaults,
    ResetCityRailDefaults,
    ResetCityWaterwayDefaults,
    ResetCityAllDefaults,
    ToggleUnit,
    SetPanelTooltipsEnabled,
    SetToolActive
} from "../shared/bindings";
import { VanillaComponentResolver } from "../utils/vanilla/VanillaComponentResolver";
import { useText } from "../shared/localization";
import { useSafeBinding } from "../shared/useSafeBinding";
import { shouldIgnorePanelDragTarget, useToolPanelPosition } from "../shared/useSharedPanelPosition";
import { CollapsibleSectionHeader } from "./CollapsibleSectionHeader";
import { PANEL_CLICK_OFFSET_PX, PANEL_WIDTH_REM, PRECISE_STEP_HOLD_DELAY_MS, PRECISE_STEP_REPEAT_MS, ROAD_GROUPS, METRIC_PRESET_SPEEDS, IMPERIAL_PRESET_SPEEDS, SPEED_STEPPER_BUTTON_WIDTH_REM, SPEED_STEPPER_NUMBER_WIDTH_REM, SPEED_STEPPER_WIDTH_REM } from "./constants";
import { CityActionModal } from "./components/CityActionModal";
import { PresetControls } from "./components/PresetControls";
import { MainSpeedSection } from "./components/MainSpeedSection";
import { PreciseSpeedStepper } from "./components/PreciseSpeedStepper";
import { SelectionSection } from "./components/SelectionSection";
import { SelectionFilterControls } from "./components/SelectionFilterControls";
import { SpeedToolHeader } from "./components/SpeedToolHeader";
import { TargetSpeedUnitToggleButton } from "./components/TargetSpeedUnitToggleButton";
import { SpeedToolOverlays } from "./components/SpeedToolOverlays";
import { WholeCityStatsSection } from "./components/WholeCityStatsSection";
import type { CityActionInfo, CityActionKind, PanelTooltipKind, RoadGroupKind } from "./types";

export const SpeedToolWindow = () => {
    const TEXT = useText().inCity;
    const toolActive = useSafeBinding(TOOL_ACTIVE, false);
    const showMetric = useSafeBinding(SHOW_METRIC, true);
    const isTrackType = useSafeBinding(IS_TRACK_TYPE, false);
    const isWaterwayType = useSafeBinding(IS_WATERWAY_TYPE, false);
    const doubleSpeedDisplay = useSafeBinding(DOUBLE_SPEED_DISPLAY, false);
    const selectionCounter = useSafeBinding(SELECTION_COUNTER, 0);
    const selectedSpeedKmh = useSafeBinding(INITIAL_SPEED, 50);
    const currentSpeedMixed = useSafeBinding(CURRENT_SPEED_MIXED, false);
    const vanillaSpeedKmh = useSafeBinding(VANILLA_SPEED, -1);
    const vanillaSpeedMixed = useSafeBinding(VANILLA_SPEED_MIXED, false);
    const syncSliderWithSelection = useSafeBinding(SYNC_SLIDER_WITH_SELECTION, true);
    const panelSliderIncrement = Math.max(5, Math.min(25, useSafeBinding(PANEL_SLIDER_INCREMENT, 5)));
    const tooltipFontScale = Math.max(100, Math.min(130, useSafeBinding(TOOLTIP_FONT_SCALE, 110)));
    const panelTooltipsEnabled = useSafeBinding(PANEL_TOOLTIPS_ENABLED, true);
    const unitMode = useSafeBinding(UNIT_MODE, 0);
    const cityCarTotal = useSafeBinding(CITY_CAR_TOTAL, 0);
    const cityCarActive = useSafeBinding(CITY_CAR_ACTIVE, 0);
    const cityCarParked = useSafeBinding(CITY_CAR_PARKED, 0);
    const cityBikeTotal = useSafeBinding(CITY_BIKE_TOTAL, 0);
    const cityBikeActive = useSafeBinding(CITY_BIKE_ACTIVE, 0);
    const cityBikeParked = useSafeBinding(CITY_BIKE_PARKED, 0);
    const cityResetInProgress = useSafeBinding(CITY_RESET_IN_PROGRESS, false);
    const cityResetCleared = useSafeBinding(CITY_RESET_CLEARED, 0);
    const cityResetTotal = useSafeBinding(CITY_RESET_TOTAL, 0);
    const cityApplyInProgress = useSafeBinding(CITY_APPLY_IN_PROGRESS, false);
    const cityApplyApplied = useSafeBinding(CITY_APPLY_APPLIED, 0);
    const cityApplyTotal = useSafeBinding(CITY_APPLY_TOTAL, 0);
    const markerTooltipText = useSafeBinding(MARKER_TOOLTIP_TEXT, "");
    const markerTooltipX = useSafeBinding(MARKER_TOOLTIP_X, 0);
    const markerTooltipY = useSafeBinding(MARKER_TOOLTIP_Y, 0);
    const selectionClickX = useSafeBinding(SELECTION_CLICK_X, 0);
    const selectionClickY = useSafeBinding(SELECTION_CLICK_Y, 0);

    const readPanelExpanded = (key: string, defaultValue: boolean): boolean => {
        try {
            const value = window.localStorage?.getItem(`${MOD_ID}.${key}`);
            if (value === "true") {
                return true;
            }

            if (value === "false") {
                return false;
            }
        } catch (e) {
            return defaultValue;
        }

        return defaultValue;
    };

    const writePanelExpanded = (key: string, value: boolean) => {
        try {
            window.localStorage?.setItem(`${MOD_ID}.${key}`, value ? "true" : "false");
        } catch (e) {
            // Preference persistence is best-effort only.
        }
    };

    const [visible, setVisible] = useState(false);
    const [pendingSpeedKmh, setPendingSpeedKmh] = useState(5);
    const [isApplying, setIsApplying] = useState(false);
    const [isResetting, setIsResetting] = useState(false);
    const [pendingCityAction, setPendingCityAction] = useState<CityActionKind | null>(null);
    const [cityActionApplying, setCityActionApplying] = useState<CityActionKind | null>(null);
    const [selectedRoadGroup, setSelectedRoadGroup] = useState<RoadGroupKind | null>(null);
    const lastSelectionCounter = useRef(0);
    const preciseStepHoldDelayRef = useRef<number | null>(null);
    const preciseStepRepeatRef = useRef<number | null>(null);
    const preciseStepMouseDownRef = useRef(false);

    const { position, isDragging, startDragging, snapTo } = useToolPanelPosition();

    const [isCloseHovered, setIsCloseHovered] = useState(false);
    const [isGuideHovered, setIsGuideHovered] = useState(false);
    const [isHelpHovered, setIsHelpHovered] = useState(false);
    const [isTargetUnitHovered, setIsTargetUnitHovered] = useState(false);
    const [hoveredPresetSpeed, setHoveredPresetSpeed] = useState<number | null>(null);
    const [panelTooltip, setPanelTooltip] = useState<PanelTooltipKind | null>(null);
    // Default OPEN on first install so the "First segment selected" summary is discoverable (its
    // label sits tight on the slider and players missed that it expands). Once the player toggles
    // it, the stored preference wins on every later open, same as the other sections.
    const [selectionInfoExpanded, setSelectionInfoExpanded] = useState(() => readPanelExpanded("selectionInfoExpanded", true));
    const [presetsExpanded, setPresetsExpanded] = useState(() => readPanelExpanded("presetsExpanded", false));
    const [wholeCityExpanded, setWholeCityExpanded] = useState(() => readPanelExpanded("wholeCityExpanded", false));
    const [statsExpanded, setStatsExpanded] = useState(() => readPanelExpanded("statsExpanded", false));

    const resolver = VanillaComponentResolver.instance;
    const Panel = resolver.Panel;
    const PanelTheme = resolver.panelTheme;
    const FOCUS_DISABLED = resolver.FOCUS_DISABLED;

    const kmhToMph = (kmh: number): number => {
        return Math.round(kmh * 0.621371);
    };

    const mphToKmh = (mph: number): number => {
        return Math.round(mph / 0.621371);
    };

    const getDefaultSpeed = (): number => {
        return showMetric ? 5 : mphToKmh(5);
    };

    const getMaxSpeedKmh = (): number => {
        if (isWaterwayType) {
            return 240;
        }

        // Match the game's highway Unlimited policy cap; vehicles still obey their own max speeds.
        return 400;
    };

    const clampSpeedKmh = (speedKmh: number): number => {
        return Math.max(5, Math.min(getMaxSpeedKmh(), speedKmh));
    };

    const getDisplaySpeed = (speedKmh: number): number => {
        const multiplier = doubleSpeedDisplay ? 2 : 1;
        return showMetric ? speedKmh * multiplier : kmhToMph(speedKmh) * multiplier;
    };

    const getDisplayMin = (): number => {
        return 5 * (doubleSpeedDisplay ? 2 : 1);
    };

    const getDisplayMax = (): number => {
        const multiplier = doubleSpeedDisplay ? 2 : 1;
        return showMetric
            ? getMaxSpeedKmh() * multiplier
            : kmhToMph(getMaxSpeedKmh()) * multiplier;
    };

    const snapDisplayToIncrement = (value: number): number => {
        const min = getDisplayMin();
        const max = getDisplayMax();
        const snapped = min + Math.round((value - min) / panelSliderIncrement) * panelSliderIncrement;
        return Math.max(min, Math.min(max, snapped));
    };

    const formatSpeed = (speedKmh: number): string => {
        return `${Math.round(getDisplaySpeed(speedKmh))} ${showMetric ? "km/h" : "mph"}`;
    };

    const formatCount = (value: number): string => {
        const safeValue = Math.max(0, Math.round(value));
        const fallback = safeValue.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

        try {
            if (typeof Intl !== "undefined") {
                const formatted = new Intl.NumberFormat(undefined, { maximumFractionDigits: 0 }).format(safeValue);
                return formatted === safeValue.toString() ? fallback : formatted;
            }
        } catch (e) {
            return fallback;
        }

        return fallback;
    };

    const formatSelectionSpeed = (
        speedKmh: number,
        isMixed: boolean,
        emptyLabel: string = TEXT.panel.notAvailable
    ): string => {
        if (isMixed) {
            return TEXT.panel.mixed;
        }

        return speedKmh > 0 ? formatSpeed(speedKmh) : emptyLabel;
    };

    const showPanelTooltip = (kind: PanelTooltipKind) => {
        if (panelTooltipsEnabled) {
            setPanelTooltip(kind);
        }
    };

    const hidePanelTooltip = () => {
        setPanelTooltip(null);
    };

    const panelTitle = (value: string): string | undefined => {
        return panelTooltipsEnabled ? value : undefined;
    };

    const getRoadGroupLabel = (group: RoadGroupKind): string => {
        return TEXT.roadGroups[group];
    };

    const getRoadGroupShortLabel = (group: RoadGroupKind): string => {
        if (group === "small") {
            return TEXT.roadGroups.smallShort;
        }

        if (group === "medium") {
            return TEXT.roadGroups.mediumShort;
        }

        if (group === "large") {
            return TEXT.roadGroups.largeShort;
        }

        return TEXT.roadGroups.highwayShort;
    };

    const getRoadGroupConfirmTitle = (group: RoadGroupKind | null): string => {
        return TEXT.confirm.applyRoadGroup.title;
    };

    const getRoadGroupConfirmSentence = (group: RoadGroupKind | null): string => {
        if (group !== null) {
            return `${TEXT.confirm.applyRoadGroup.messageStart} ${getRoadGroupLabel(group)} ${TEXT.confirm.applyRoadGroup.messageMiddle}`;
        }

        return TEXT.buttons.chooseRoadGroup;
    };

    const getRoadGroupTooltip = (kind: PanelTooltipKind) => {
        if (kind === "roadGroupSmall") {
            return TEXT.tooltips.roadGroups.small;
        }

        if (kind === "roadGroupMedium") {
            return TEXT.tooltips.roadGroups.medium;
        }

        if (kind === "roadGroupLarge") {
            return TEXT.tooltips.roadGroups.large;
        }

        if (kind === "roadGroupHighway") {
            return TEXT.tooltips.roadGroups.highway;
        }

        return null;
    };

    const getCityActionInfo = (kind: CityActionKind): CityActionInfo => {
        if (kind === "applyRoadGroup") {
            return {
                title: getRoadGroupConfirmTitle(selectedRoadGroup),
                message: "",
                confirmLabel: TEXT.confirm.applyRoadGroup.confirmLabel
            };
        }

        if (kind === "applyTrain") {
            return {
                title: TEXT.confirm.applyTrain.title,
                message: "",
                confirmLabel: TEXT.confirm.applyTrain.confirmLabel
            };
        }

        if (kind === "applySubway") {
            return {
                title: TEXT.confirm.applySubway.title,
                message: "",
                confirmLabel: TEXT.confirm.applySubway.confirmLabel
            };
        }

        if (kind === "resetRoads") {
            return {
                title: TEXT.confirm.resetRoads.title,
                message: TEXT.confirm.resetRoads.message,
                confirmLabel: TEXT.confirm.resetRoads.confirmLabel
            };
        }

        if (kind === "resetRails") {
            return {
                title: TEXT.confirm.resetRails.title,
                message: TEXT.confirm.resetRails.message,
                confirmLabel: TEXT.confirm.resetRails.confirmLabel
            };
        }

        if (kind === "resetWater") {
            return {
                title: TEXT.confirm.resetWater.title,
                message: TEXT.confirm.resetWater.message,
                confirmLabel: TEXT.confirm.resetWater.confirmLabel
            };
        }

        return {
            title: TEXT.confirm.resetAll.title,
            message: TEXT.confirm.resetAll.message,
            confirmLabel: TEXT.confirm.resetAll.confirmLabel
        };
    };

    useEffect(() => {
        if (toolActive && selectionCounter > 0 && selectionCounter !== lastSelectionCounter.current) {
            // Re-anchor near the click only when this is a brand-new selection (nothing was
            // selected right before it) — extending or replacing an existing selection leaves the
            // panel wherever the player put it, so it doesn't jump around mid-edit.
            if (lastSelectionCounter.current === 0) {
                snapTo({
                    x: selectionClickX + PANEL_CLICK_OFFSET_PX,
                    y: selectionClickY + PANEL_CLICK_OFFSET_PX
                });
            }

            setVisible(true);
            if (isTrackType || isWaterwayType) {
                setSelectedRoadGroup(null);
            }
            if (syncSliderWithSelection && selectedSpeedKmh > 0) {
                setPendingSpeedKmh(clampSpeedKmh(selectedSpeedKmh));
            }
            lastSelectionCounter.current = selectionCounter;
        } else if (!toolActive || selectionCounter === 0) {
            setVisible(false);
            setPendingCityAction(null);
            lastSelectionCounter.current = 0;
        }
    }, [selectionCounter, toolActive, syncSliderWithSelection, currentSpeedMixed, selectedSpeedKmh, isTrackType, isWaterwayType, selectionClickX, selectionClickY]);

    const handleSliderChange = (value: number) => {
        const snappedValue = snapDisplayToIncrement(value);
        setPendingSpeedKmh(displayValueToSpeedKmh(snappedValue));
    };

    const displayValueToSpeedKmh = (displayValue: number): number => {
        const boundedDisplayValue = Math.max(getDisplayMin(), Math.min(getDisplayMax(), displayValue));
        if (showMetric) {
            const actualKmh = doubleSpeedDisplay ? boundedDisplayValue / 2 : boundedDisplayValue;
            return clampSpeedKmh(actualKmh);
        }

        const actualMph = doubleSpeedDisplay ? boundedDisplayValue / 2 : boundedDisplayValue;
        return clampSpeedKmh(mphToKmh(actualMph));
    };

    const handlePresetSpeed = (displayValue: number) => {
        const speedKmh = displayValueToSpeedKmh(displayValue);
        setPendingSpeedKmh(speedKmh);

        if (selectionCounter > 0) {
            setIsApplying(true);
            ApplySpeed(speedKmh);
            setTimeout(() => setIsApplying(false), 500);
        }
    };

    const handlePreciseStep = (direction: number) => {
        setPendingSpeedKmh(currentSpeedKmh => {
            const currentDisplayValue = Math.round(getDisplaySpeed(currentSpeedKmh));
            return displayValueToSpeedKmh(currentDisplayValue + direction);
        });
    };

    const stopPreciseStepHold = () => {
        if (preciseStepHoldDelayRef.current !== null) {
            window.clearTimeout(preciseStepHoldDelayRef.current);
            preciseStepHoldDelayRef.current = null;
        }

        if (preciseStepRepeatRef.current !== null) {
            window.clearInterval(preciseStepRepeatRef.current);
            preciseStepRepeatRef.current = null;
        }
    };

    const handlePreciseStepClickFallback = (direction: number, disabled: boolean) => {
        if (disabled) {
            return;
        }

        if (preciseStepMouseDownRef.current) {
            preciseStepMouseDownRef.current = false;
            return;
        }

        handlePreciseStep(direction);
    };

    const startPreciseStepHold = (direction: number) => {
        stopPreciseStepHold();
        handlePreciseStep(direction);

        preciseStepHoldDelayRef.current = window.setTimeout(() => {
            preciseStepHoldDelayRef.current = null;
            preciseStepRepeatRef.current = window.setInterval(() => {
                handlePreciseStep(direction);
            }, PRECISE_STEP_REPEAT_MS);
        }, PRECISE_STEP_HOLD_DELAY_MS);
    };

    const handleHalfSpeed = () => {
        setIsApplying(true);
        ApplySelectionMultiplier(0.5);
        setTimeout(() => setIsApplying(false), 500);
    };

    const handleFasterSpeed = () => {
        setIsApplying(true);
        ApplySelectionMultiplier(1.5);
        setTimeout(() => setIsApplying(false), 500);
    };

    const handleApply = () => {
        setIsApplying(true);
        ApplySpeed(pendingSpeedKmh);
        setTimeout(() => setIsApplying(false), 500);
    };

    const handleReset = () => {
        setIsResetting(true);
        ResetSpeed();
        setTimeout(() => {
            setIsResetting(false);
            if (!vanillaSpeedMixed && vanillaSpeedKmh > 0) {
                setPendingSpeedKmh(clampSpeedKmh(vanillaSpeedKmh));
            } else {
                setPendingSpeedKmh(getDefaultSpeed());
            }
        }, 500);
    };

    const handleConfirmCityAction = () => {
        if (pendingCityAction === null) {
            return;
        }

        const action = pendingCityAction;
        setPendingCityAction(null);
        setCityActionApplying(action);

        if (action === "applyRoadGroup") {
            if (selectedRoadGroup === null) {
                setCityActionApplying(null);
                return;
            }

            const group = ROAD_GROUPS.find(item => item.key === selectedRoadGroup);
            ApplyCityRoadGroupSpeed(group?.value ?? 1, pendingSpeedKmh);
        } else if (action === "applyTrain") {
            ApplyCityTrainSpeed(pendingSpeedKmh);
        } else if (action === "applySubway") {
            ApplyCitySubwaySpeed(pendingSpeedKmh);
        } else if (action === "resetRoads") {
            ResetCityRoadDefaults();
        } else if (action === "resetRails") {
            ResetCityRailDefaults();
        } else if (action === "resetWater") {
            ResetCityWaterwayDefaults();
        } else {
            ResetCityAllDefaults();
        }

        setTimeout(() => setCityActionApplying(null), 900);
    };

    const handleClose = () => {
        SetToolActive(false);
    };

    const toggleWholeCityExpanded = () => {
        const nextValue = !wholeCityExpanded;
        setWholeCityExpanded(nextValue);
        writePanelExpanded("wholeCityExpanded", nextValue);
    };

    const toggleSelectionInfoExpanded = () => {
        const nextValue = !selectionInfoExpanded;
        setSelectionInfoExpanded(nextValue);
        writePanelExpanded("selectionInfoExpanded", nextValue);
    };

    const togglePresetsExpanded = () => {
        const nextValue = !presetsExpanded;
        setPresetsExpanded(nextValue);
        writePanelExpanded("presetsExpanded", nextValue);
    };

    const toggleStatsExpanded = () => {
        const nextValue = !statsExpanded;
        setStatsExpanded(nextValue);
        writePanelExpanded("statsExpanded", nextValue);
    };

    const handleMouseDown = (event: React.MouseEvent) => {
        if (shouldIgnorePanelDragTarget(event.target)) {
            return;
        }

        startDragging(event.clientX, event.clientY);
    };

    useEffect(() => {
        const stopHold = () => stopPreciseStepHold();

        document.addEventListener("mouseup", stopHold);
        window.addEventListener("blur", stopHold);

        return () => {
            stopPreciseStepHold();
            document.removeEventListener("mouseup", stopHold);
            window.removeEventListener("blur", stopHold);
        };
    }, []);

    if (!visible) {
        return null;
    }

    const unitLabel = showMetric ? "km/h" : "mph";
    const targetSpeedUnitLabel = unitMode === 0 ? "a" : showMetric ? "km" : "mi";
    const displaySpeed = getDisplaySpeed(pendingSpeedKmh);
    const sliderValue = displaySpeed;
    const sliderMin = getDisplayMin();
    const sliderMax = getDisplayMax();
    const sliderStep = panelSliderIncrement;
    const currentSpeedLabel = formatSelectionSpeed(selectedSpeedKmh, currentSpeedMixed);
    const vanillaSpeedLabel = formatSelectionSpeed(vanillaSpeedKmh, vanillaSpeedMixed);
    const targetSpeedLabel = `${Math.round(displaySpeed)} ${unitLabel}`;
    const cityActionInfo = pendingCityAction !== null ? getCityActionInfo(pendingCityAction) : null;
    const cityActionBusy = cityActionApplying !== null || cityResetInProgress || cityApplyInProgress;
    const panelWidth = PANEL_WIDTH_REM;
    const tooltipFontSize = `${10 * tooltipFontScale / 100}rem`;
    const markerTooltipFontSize = `${20 * tooltipFontScale / 100}rem`;
    const tooltipLineHeight = `${1.35 * tooltipFontScale / 100}`;
    const confirmFontSize = `${10.5 * tooltipFontScale / 100}rem`;
    const confirmTitleFontSize = `${12.5 * tooltipFontScale / 100}rem`;

    const tooltipBaseStyle = {
        position: "fixed" as const,
        backgroundColor: "rgba(30, 33, 38, 0.97)",
        color: "#fff",
        paddingTop: "10rem",
        paddingRight: "12rem",
        paddingBottom: "10rem",
        paddingLeft: "12rem",
        borderRadius: "4rem",
        fontSize: tooltipFontSize,
        lineHeight: tooltipLineHeight,
        zIndex: 1000000,
        borderWidth: "1rem",
        borderStyle: "solid",
        borderColor: "#606872",
        pointerEvents: "none" as const
    };


    const renderCityActionMessage = () => {
        if (pendingCityAction === "applyRoadGroup" || pendingCityAction === "applyTrain" || pendingCityAction === "applySubway") {
            const sentence = pendingCityAction === "applyTrain"
                ? TEXT.confirm.applyTrain.message
                : pendingCityAction === "applySubway"
                    ? TEXT.confirm.applySubway.message
                    : getRoadGroupConfirmSentence(selectedRoadGroup);
            return (
                // Normal block flow (not a nowrap flex row): the sentence wraps naturally and the
                // value box + unit flow inline after it, dropping to the next line when they don't
                // fit. Fixes the value/unit overflowing off the right edge in long locales (ES, PT-BR).
                <div style={{
                    color: "rgba(255, 255, 255, 0.88)",
                    fontSize: confirmFontSize,
                    lineHeight: "1.6"
                }}>
                    <span style={{ marginRight: "6rem" }}>
                        {sentence}
                    </span>
                    <span style={{
                        display: "inline-block",
                        whiteSpace: "nowrap",
                        verticalAlign: "middle"
                    }}>
                        <span style={{
                            display: "inline-block",
                            verticalAlign: "middle",
                            minWidth: "30rem",
                            paddingTop: "2rem",
                            paddingRight: "7rem",
                            paddingBottom: "2rem",
                            paddingLeft: "7rem",
                            marginRight: "4rem",
                            textAlign: "center",
                            fontWeight: "bold",
                            color: "#fff",
                            backgroundColor: "rgba(78, 195, 240, 0.22)",
                            borderWidth: "1rem",
                            borderStyle: "solid",
                            borderColor: "rgba(78, 195, 240, 0.78)",
                            borderRadius: "3rem"
                        }}>
                            {Math.round(displaySpeed)}
                        </span>
                        <span style={{ verticalAlign: "middle" }}>{unitLabel}</span>
                    </span>
                </div>
            );
        }

        return cityActionInfo?.message;
    };

    const targetSpeedUnitToggleButton = (
        <TargetSpeedUnitToggleButton
            targetSpeedUnitLabel={targetSpeedUnitLabel}
            isHovered={isTargetUnitHovered}
            setHovered={setIsTargetUnitHovered}
            onToggle={ToggleUnit}
            showUnitTooltip={() => showPanelTooltip("unit")}
            hideTooltip={hidePanelTooltip}
        />
    );
    return (
        <>
            <div style={{
                position: "absolute",
                left: `${position.x}px`,
                top: `${position.y}px`,
                width: `${panelWidth}rem`,
                pointerEvents: "auto"
            }}>

                <Panel
                    header={
                        <SpeedToolHeader
                            title={TEXT.panel.title}
                            closeTooltip={TEXT.buttons.close}
                            panelTooltipsEnabled={panelTooltipsEnabled}
                            isDragging={isDragging}
                            isCloseHovered={isCloseHovered}
                            isGuideHovered={isGuideHovered}
                            isHelpHovered={isHelpHovered}
                            setIsCloseHovered={setIsCloseHovered}
                            setIsGuideHovered={setIsGuideHovered}
                            setIsHelpHovered={setIsHelpHovered}
                            onMouseDown={handleMouseDown}
                            onClose={handleClose}
                            onToggleTooltips={() => SetPanelTooltipsEnabled(!panelTooltipsEnabled)}
                            showPanelTitleTooltip={() => showPanelTooltip("panelTitle")}
                            hidePanelTooltip={hidePanelTooltip}
                        />
                    }
                    className={PanelTheme.panel}
                >
                    <div style={{
                        paddingTop: "7.5rem",
                        paddingRight: "9rem",
                        paddingBottom: "10rem",
                        paddingLeft: "9rem",
                        pointerEvents: "auto",
                        backgroundColor: "rgba(20, 24, 28, 0.18)",
                        borderTopWidth: "1rem",
                        borderTopStyle: "solid",
                        borderTopColor: "rgba(255, 255, 255, 0.08)"
                    }}>
                        <div style={{ position: "relative" }}>
                            <SelectionFilterControls
                                label={TEXT.panel.selectFilter}
                                roadsLabel={TEXT.panel.filterRoads}
                                railsLabel={TEXT.panel.filterRails}
                                waterLabel={TEXT.panel.filterWater}
                                showTip={showPanelTooltip}
                                hideTip={hidePanelTooltip}
                            />
                            <div style={{
                                position: "absolute",
                                right: "0",
                                top: "35rem",
                                zIndex: 2
                            }}>
                                <PreciseSpeedStepper
                                    widthRem={SPEED_STEPPER_WIDTH_REM}
                                    buttonWidthRem={SPEED_STEPPER_BUTTON_WIDTH_REM}
                                    numberWidthRem={SPEED_STEPPER_NUMBER_WIDTH_REM}
                                    displaySpeed={displaySpeed}
                                    sliderMin={sliderMin}
                                    sliderMax={sliderMax}
                                    decreaseTitle={panelTitle(TEXT.tooltips.decreaseTarget)}
                                    increaseTitle={panelTitle(TEXT.tooltips.increaseTarget)}
                                    groupTitle={panelTitle(TEXT.tooltips.preciseStepper)}
                                    onStep={handlePreciseStep}
                                    onStepHoldStart={startPreciseStepHold}
                                    onStepHoldStop={stopPreciseStepHold}
                                    onStepClickFallback={handlePreciseStepClickFallback}
                                    preciseStepMouseDownRef={preciseStepMouseDownRef}
                                />
                            </div>
                        </div>
                        <MainSpeedSection
                            focusKey={FOCUS_DISABLED}
                            newSpeedLabelText={TEXT.panel.newSpeedLimit}
                            newSpeedValueText={targetSpeedLabel}
                            slowerTitle={panelTitle(TEXT.tooltips.slower)}
                            fasterTitle={panelTitle(TEXT.tooltips.faster)}
                            resetTitle={panelTitle(TEXT.tooltips.reset)}
                            slowerButtonContent={<span>{`${TEXT.buttons.slower} ↓`}</span>}
                            fasterButtonContent={<span>{`${TEXT.buttons.faster} ↑`}</span>}
                            applyButtonText={isApplying ? TEXT.buttons.applied : TEXT.buttons.apply}
                            sliderMin={sliderMin}
                            sliderMax={sliderMax}
                            sliderStep={sliderStep}
                            sliderValue={sliderValue}
                            unitLabel={unitLabel}
                            isApplying={isApplying}
                            isResetting={isResetting}
                            onHalfSpeed={handleHalfSpeed}
                            onFasterSpeed={handleFasterSpeed}
                            onHalfSpeedMouseEnter={() => showPanelTooltip("speedSlower")}
                            onFasterSpeedMouseEnter={() => showPanelTooltip("speedFaster")}
                            onApplyMouseEnter={() => showPanelTooltip("apply")}
                            onResetMouseEnter={() => showPanelTooltip("resetSelected")}
                            onControlMouseLeave={hidePanelTooltip}
                            onSliderChange={handleSliderChange}
                            onApply={handleApply}
                            onReset={handleReset}
                        />

                        {/* Moved below the New speed / Apply / 50% controls: this is reference info
                            about the selection, not part of the primary edit flow, so it no longer
                            crowds the slider area above. */}
                        <CollapsibleSectionHeader
                            label={TEXT.panel.selectedSegment}
                            expanded={selectionInfoExpanded}
                            onToggle={toggleSelectionInfoExpanded}
                            trailing={selectionInfoExpanded ? (
                                <span style={{
                                    paddingRight: "8rem",
                                    color: "rgba(255, 255, 255, 0.6)",
                                    fontSize: "10rem",
                                    fontWeight: 400,
                                    lineHeight: "1",
                                    whiteSpace: "nowrap"
                                }}>
                                    {TEXT.panel.firstSegment}
                                </span>
                            ) : undefined}
                        />
                        <SelectionSection
                            visible={selectionInfoExpanded}
                            targetSpeedUnitToggle={targetSpeedUnitToggleButton}
                            currentSpeedTitle={panelTitle(TEXT.tooltips.currentSpeed)}
                            gameDefaultTitle={panelTitle(TEXT.panel.gameDefault)}
                            currentSpeedLabelText={TEXT.panel.currentSpeed}
                            currentSpeedValueText={currentSpeedLabel}
                            defaultSpeedLabelText={"Default"}
                            defaultSpeedValueText={vanillaSpeedLabel}
                            onGameDefaultMouseEnter={() => showPanelTooltip("gameDefault")}
                            onGameDefaultMouseLeave={hidePanelTooltip}
                        />

                        {/* Removed the divider that used to sit here: Presets is a close extension of
                            the main edit controls right above, so its own collapsible header (with
                            chevron) already marks the boundary without needing a hard rule too.
                            Spacer also shrunk (was 8rem) — combined with SelectionSection's own
                            margin this was making the gap to Presets too tall. */}
                        <div style={{ marginBottom: "2rem" }} />
                        <CollapsibleSectionHeader
                            label={TEXT.panel.presets}
                            expanded={presetsExpanded}
                            onToggle={togglePresetsExpanded}
                        />
                        {presetsExpanded && (
                            <PresetControls
                                presetRows={showMetric
                                    ? [METRIC_PRESET_SPEEDS.slice(0, 8), METRIC_PRESET_SPEEDS.slice(8)]
                                    : [IMPERIAL_PRESET_SPEEDS.slice(0, 9), IMPERIAL_PRESET_SPEEDS.slice(9)]}
                                displayValue={Math.round(displaySpeed)}
                                minDisplay={sliderMin}
                                maxDisplay={sliderMax}
                                unlimitedValue={Math.round(getDisplaySpeed(getMaxSpeedKmh()))}
                                unitLabel={unitLabel}
                                hoveredPresetSpeed={hoveredPresetSpeed}
                                setHoveredPresetSpeed={setHoveredPresetSpeed}
                                onPresetSelect={handlePresetSpeed}
                                getPresetTitle={preset => panelTitle(`${preset} ${unitLabel}`)}
                                unlimitedTitle={panelTitle(TEXT.tooltips.presetUnlimited.title)}
                                onUnlimitedMouseEnter={() => showPanelTooltip("presetUnlimited")}
                                onUnlimitedMouseLeave={hidePanelTooltip}
                                focusKey={FOCUS_DISABLED}
                            />
                        )}
                        <WholeCityStatsSection
                            text={TEXT}
                            wholeCityExpanded={wholeCityExpanded}
                            statsExpanded={statsExpanded}
                            selectedRoadGroup={selectedRoadGroup}
                            cityActionBusy={cityActionBusy}
                            cityActionApplying={cityActionApplying}
                            cityApplyInProgress={cityApplyInProgress}
                            cityApplyApplied={cityApplyApplied}
                            cityApplyTotal={cityApplyTotal}
                            cityResetInProgress={cityResetInProgress}
                            cityResetCleared={cityResetCleared}
                            cityResetTotal={cityResetTotal}
                            cityCarActive={cityCarActive}
                            cityCarParked={cityCarParked}
                            cityCarTotal={cityCarTotal}
                            cityBikeActive={cityBikeActive}
                            cityBikeParked={cityBikeParked}
                            cityBikeTotal={cityBikeTotal}
                            setSelectedRoadGroup={setSelectedRoadGroup}
                            setPendingCityAction={setPendingCityAction}
                            toggleWholeCityExpanded={toggleWholeCityExpanded}
                            toggleStatsExpanded={toggleStatsExpanded}
                            getRoadGroupLabel={getRoadGroupLabel}
                            getRoadGroupShortLabel={getRoadGroupShortLabel}
                            showPanelTooltip={showPanelTooltip}
                            hidePanelTooltip={hidePanelTooltip}
                            panelTitle={panelTitle}
                            formatCount={formatCount}
                            focusKey={FOCUS_DISABLED}
                        />
                    </div>
                </Panel>
            </div>

            <CityActionModal
                visible={cityActionInfo !== null}
                position={position}
                panelWidthRem={panelWidth}
                confirmFontSize={confirmFontSize}
                confirmTitleFontSize={confirmTitleFontSize}
                title={cityActionInfo?.title ?? ""}
                messageContent={renderCityActionMessage() ?? null}
                confirmLabel={cityActionInfo?.confirmLabel ?? ""}
                onCancel={() => setPendingCityAction(null)}
                onConfirm={handleConfirmCityAction}
                confirmVariant={pendingCityAction === "applyRoadGroup" || pendingCityAction === "applyTrain" || pendingCityAction === "applySubway" ? "city" : "danger"}
                cancelLabel={TEXT.buttons.cancel}
                focusKey={FOCUS_DISABLED}
            />

            <SpeedToolOverlays
                text={TEXT}
                panelTooltip={panelTooltip}
                panelTooltipsEnabled={panelTooltipsEnabled}
                position={position}
                panelWidth={panelWidth}
                tooltipBaseStyle={tooltipBaseStyle}
                tooltipFontSize={tooltipFontSize}
                tooltipFontScale={tooltipFontScale}
                markerTooltipText={markerTooltipText}
                markerTooltipX={markerTooltipX}
                markerTooltipY={markerTooltipY}
                markerTooltipFontSize={markerTooltipFontSize}
                isGuideHovered={isGuideHovered}
                isHelpHovered={isHelpHovered}
                getRoadGroupTooltip={getRoadGroupTooltip}
            />
        </>
    );
};
