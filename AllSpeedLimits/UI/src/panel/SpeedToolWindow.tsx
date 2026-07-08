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
    HIDE_SPEED_MARKERS,
    UNIT_MODE,
    CITY_CAR_TOTAL,
    CITY_CAR_ACTIVE,
    CITY_CAR_PARKED,
    CITY_BIKE_TOTAL,
    CITY_BIKE_ACTIVE,
    CITY_BIKE_PARKED,
    CITY_INDUSTRY_TOTAL,
    CITY_INDUSTRY_ACTIVE,
    CITY_INDUSTRY_PARKED,
    CITY_RESET_IN_PROGRESS,
    CITY_RESET_CLEARED,
    CITY_RESET_TOTAL,
    CITY_APPLY_IN_PROGRESS,
    CITY_APPLY_APPLIED,
    CITY_APPLY_TOTAL,
    SELECTION_CLICK_X,
    SELECTION_CLICK_Y,
    MOD_ID,
    ApplySpeed,
    ApplyCityRoadGroupSpeed,
    ApplyCityTrainSpeed,
    ApplyCitySubwaySpeed,
    ResetSpeed,
    ResetCityRoadDefaults,
    ResetCityRailDefaults,
    ResetCityWaterwayDefaults,
    ResetCityAllDefaults,
    ToggleUnit,
    SetPanelSpeedUnit,
    SetPanelTooltipsEnabled,
    SetHideSpeedMarkers,
    SetStatsExpanded,
    SetToolActive
} from "../shared/bindings";
import { VanillaComponentResolver } from "../utils/vanilla/VanillaComponentResolver";
import { useText } from "../shared/localization";
import { Button } from "../shared/Button";
import expandCollapseAllIcon from "../images/icon-expandCollapseAll.svg";
import { useSafeBinding } from "../shared/useSafeBinding";
import { shouldIgnorePanelDragTarget, useToolPanelPosition } from "../shared/useSharedPanelPosition";
import { SaveToolPanelPosition, TOOL_PANEL_X, TOOL_PANEL_Y } from "../shared/bindings";
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
    const panelSliderIncrement = Math.max(5, Math.min(25, useSafeBinding(PANEL_SLIDER_INCREMENT, 10)));
    const tooltipFontScale = Math.max(100, Math.min(140, useSafeBinding(TOOLTIP_FONT_SCALE, 110)));
    const panelTooltipsEnabled = useSafeBinding(PANEL_TOOLTIPS_ENABLED, true);
    const hideSpeedMarkers = useSafeBinding(HIDE_SPEED_MARKERS, false);
    const unitMode = useSafeBinding(UNIT_MODE, 0);
    const cityCarTotal = useSafeBinding(CITY_CAR_TOTAL, 0);
    const cityCarActive = useSafeBinding(CITY_CAR_ACTIVE, 0);
    const cityCarParked = useSafeBinding(CITY_CAR_PARKED, 0);
    const cityBikeTotal = useSafeBinding(CITY_BIKE_TOTAL, 0);
    const cityBikeActive = useSafeBinding(CITY_BIKE_ACTIVE, 0);
    const cityBikeParked = useSafeBinding(CITY_BIKE_PARKED, 0);
    const cityIndustryTotal = useSafeBinding(CITY_INDUSTRY_TOTAL, 0);
    const cityIndustryActive = useSafeBinding(CITY_INDUSTRY_ACTIVE, 0);
    const cityIndustryParked = useSafeBinding(CITY_INDUSTRY_PARKED, 0);
    const cityResetInProgress = useSafeBinding(CITY_RESET_IN_PROGRESS, false);
    const cityResetCleared = useSafeBinding(CITY_RESET_CLEARED, 0);
    const cityResetTotal = useSafeBinding(CITY_RESET_TOTAL, 0);
    const cityApplyInProgress = useSafeBinding(CITY_APPLY_IN_PROGRESS, false);
    const cityApplyApplied = useSafeBinding(CITY_APPLY_APPLIED, 0);
    const cityApplyTotal = useSafeBinding(CITY_APPLY_TOTAL, 0);
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
    // Shown after a citywide reset so the player remembers the change is only kept if they save.
    const [showSaveReminder, setShowSaveReminder] = useState(false);
    const [selectedRoadGroup, setSelectedRoadGroup] = useState<RoadGroupKind | null>(null);
    const lastSelectionCounter = useRef(0);
    const preciseStepHoldDelayRef = useRef<number | null>(null);
    const preciseStepRepeatRef = useRef<number | null>(null);
    const preciseStepMouseDownRef = useRef(false);

    const savedToolPanelX = useSafeBinding(TOOL_PANEL_X, -1);
    const savedToolPanelY = useSafeBinding(TOOL_PANEL_Y, -1);
    const { panelRef, position, isDragging, startDragging, snapTo } = useToolPanelPosition(
        { x: savedToolPanelX, y: savedToolPanelY },
        nextPosition => SaveToolPanelPosition(nextPosition.x, nextPosition.y)
    );

    const [isCloseHovered, setIsCloseHovered] = useState(false);
    const [isGuideHovered, setIsGuideHovered] = useState(false);
    const [isHelpHovered, setIsHelpHovered] = useState(false);
    const [isMarkersHovered, setIsMarkersHovered] = useState(false);
    const [isExpandAllHovered, setIsExpandAllHovered] = useState(false);
    const [isTargetUnitHovered, setIsTargetUnitHovered] = useState(false);
    const [panelTooltip, setPanelTooltip] = useState<PanelTooltipKind | null>(null);
    // Default OPEN on first install so the "First segment selected" summary is discoverable (its
    // label sits tight on the slider and players missed that it expands). Once the player toggles
    // it, the stored preference wins on every later open, same as the other sections.
    const [selectionInfoExpanded, setSelectionInfoExpanded] = useState(() => readPanelExpanded("selectionInfoExpanded", true));
    const [sliderExpanded, setSliderExpanded] = useState(() => readPanelExpanded("sliderExpanded", true));
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
        const snapped = value <= min
            ? min
            : Math.round(value / panelSliderIncrement) * panelSliderIncrement;
        return Math.max(min, Math.min(max, snapped));
    };

    const formatSpeed = (speedKmh: number): string => {
        return `${Math.round(getDisplaySpeed(speedKmh))} ${showMetric ? "kmh" : "mph"}`;
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

    useEffect(() => {
        SetStatsExpanded(statsExpanded);
    }, [statsExpanded]);

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
        if (action === "resetRoads" || action === "resetRails" || action === "resetWater" || action === "resetAll") {
            setSelectedRoadGroup(null);
        }

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
            setShowSaveReminder(true);
        } else if (action === "resetRails") {
            ResetCityRailDefaults();
            setShowSaveReminder(true);
        } else if (action === "resetWater") {
            ResetCityWaterwayDefaults();
            setShowSaveReminder(true);
        } else {
            ResetCityAllDefaults();
            setShowSaveReminder(true);
        }

        setTimeout(() => setCityActionApplying(null), 900);
    };

    const handleClose = () => {
        setSelectedRoadGroup(null);
        SetToolActive(false);
    };

    const toggleWholeCityExpanded = () => {
        const nextValue = !wholeCityExpanded;
        setWholeCityExpanded(nextValue);
        writePanelExpanded("wholeCityExpanded", nextValue);
        if (!nextValue) {
            setSelectedRoadGroup(null);
        }
    };

    const toggleSelectionInfoExpanded = () => {
        const nextValue = !selectionInfoExpanded;
        setSelectionInfoExpanded(nextValue);
        writePanelExpanded("selectionInfoExpanded", nextValue);
    };

    const toggleSliderExpanded = () => {
        const nextValue = !sliderExpanded;
        setSliderExpanded(nextValue);
        writePanelExpanded("sliderExpanded", nextValue);
    };

    // Expand-or-collapse-all button in the filter row: if anything is open, close everything; else open all.
    const collapseAllSections = () => {
        const nextValue = !(selectionInfoExpanded || sliderExpanded || wholeCityExpanded || statsExpanded);
        setSelectionInfoExpanded(nextValue);
        writePanelExpanded("selectionInfoExpanded", nextValue);
        setSliderExpanded(nextValue);
        writePanelExpanded("sliderExpanded", nextValue);
        setWholeCityExpanded(nextValue);
        writePanelExpanded("wholeCityExpanded", nextValue);
        setStatsExpanded(nextValue);
        writePanelExpanded("statsExpanded", nextValue);
        if (!nextValue) {
            setSelectedRoadGroup(null);
        }
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

        event.preventDefault();
        event.stopPropagation();
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

    const unitLabel = showMetric ? "kmh" : "mph";
    const targetSpeedUnitLabel = unitMode === 0 ? "a" : showMetric ? "km" : "mi";
    const displaySpeed = getDisplaySpeed(pendingSpeedKmh);
    const sliderValue = displaySpeed;
    const sliderMin = getDisplayMin();
    const sliderMax = getDisplayMax();
    const sliderStep = panelSliderIncrement;
    const currentSpeedLabel = formatSelectionSpeed(selectedSpeedKmh, currentSpeedMixed);
    const vanillaSpeedLabel = formatSelectionSpeed(vanillaSpeedKmh, vanillaSpeedMixed);
    const targetSpeedLabel = `${Math.round(displaySpeed)} ${unitLabel}`;
    // Preset grid: Unlimited is the last circle (sentinel -1), same size as the numbers.
    // mph drops 90 to make room for it; km/h keeps all values and adds Unlimited after 160.
    const presetSpeeds = showMetric
        ? [...METRIC_PRESET_SPEEDS, -1]
        : [...IMPERIAL_PRESET_SPEEDS.filter(speed => speed !== 90), -1];
    const presetRows = [presetSpeeds.slice(0, 6), presetSpeeds.slice(6, 12), presetSpeeds.slice(12)];
    const cityActionInfo = pendingCityAction !== null ? getCityActionInfo(pendingCityAction) : null;
    const cityActionBusy = cityActionApplying !== null || cityResetInProgress || cityApplyInProgress;
    const anySectionExpanded = selectionInfoExpanded || sliderExpanded || wholeCityExpanded || statsExpanded;
    const expandAllTooltipText = anySectionExpanded ? TEXT.buttons.collapseAll : TEXT.buttons.expandAll;
    const speedMarkersTooltipText = hideSpeedMarkers ? TEXT.tooltips.markersShow : TEXT.tooltips.markersHide;
    const panelWidth = PANEL_WIDTH_REM;
    const tooltipFontSize = `${10 * tooltipFontScale / 100}rem`;
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
                // Flex row that wraps: the sentence takes its space and the value box + unit stay
                // together and drop to the next line when they don't fit. cohtml has no inline-block,
                // so this flex-wrap is the supported way to avoid overflow in long locales (ES, PT-BR).
                <div style={{
                    display: "flex",
                    flexWrap: "wrap",
                    alignItems: "center",
                    color: "rgba(255, 255, 255, 0.88)",
                    fontSize: confirmFontSize,
                    lineHeight: "1.6"
                }}>
                    <span style={{ marginRight: "6rem" }}>
                        {sentence}
                    </span>
                    <span style={{
                        display: "flex",
                        alignItems: "center",
                        flexShrink: 0
                    }}>
                        <span style={{
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
                            borderRadius: "3rem",
                            flexShrink: 0
                        }}>
                            {Math.round(displaySpeed)}
                        </span>
                        <span>{unitLabel}</span>
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
            <div
                ref={panelRef}
                // The world marker tooltip overlay is mounted outside this panel. This attribute
                // keeps marker tooltips from appearing over ASL controls when a road is behind them.
                data-asl-marker-tooltip-block="true"
                style={{
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
                            markersTooltip={hideSpeedMarkers ? TEXT.tooltips.markersShow : TEXT.tooltips.markersHide}
                            panelTooltipsEnabled={panelTooltipsEnabled}
                            speedMarkersHidden={hideSpeedMarkers}
                            isDragging={isDragging}
                            isCloseHovered={isCloseHovered}
                            isGuideHovered={isGuideHovered}
                            isHelpHovered={isHelpHovered}
                            isMarkersHovered={isMarkersHovered}
                            setIsCloseHovered={setIsCloseHovered}
                            setIsGuideHovered={setIsGuideHovered}
                            setIsHelpHovered={setIsHelpHovered}
                            setIsMarkersHovered={setIsMarkersHovered}
                            onMouseDown={handleMouseDown}
                            onClose={handleClose}
                            onToggleTooltips={() => SetPanelTooltipsEnabled(!panelTooltipsEnabled)}
                            onToggleMarkers={() => SetHideSpeedMarkers(!hideSpeedMarkers)}
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
                        {showSaveReminder && (
                            <div style={{
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "space-between",
                                marginBottom: "8rem",
                                paddingTop: "6rem",
                                paddingRight: "8rem",
                                paddingBottom: "6rem",
                                paddingLeft: "10rem",
                                backgroundColor: "rgba(240, 176, 64, 0.10)",
                                borderWidth: "1rem",
                                borderStyle: "solid",
                                borderColor: "rgba(240, 176, 64, 0.75)",
                                borderRadius: "4rem"
                            }}>
                                <div style={{
                                    display: "flex",
                                    alignItems: "center",
                                    minWidth: "0",
                                    marginRight: "8rem"
                                }}>
                                    <img
                                        src="Media/Glyphs/Checkmark.svg"
                                        alt=""
                                        style={{
                                            width: "13rem",
                                            height: "13rem",
                                            flexShrink: 0,
                                            marginRight: "6rem",
                                            // Tint the black glyph to the banner's warm amber.
                                            filter: "brightness(0) invert(1) sepia(1) saturate(3) hue-rotate(2deg) brightness(1.05)",
                                            pointerEvents: "none"
                                        }}
                                    />
                                    <span style={{
                                        fontSize: "13rem",
                                        fontWeight: "bold",
                                        color: "rgba(255, 238, 184, 1)",
                                        lineHeight: "1.3"
                                    }}>
                                        {TEXT.reminder.saveAfterReset}
                                    </span>
                                </div>
                                <Button
                                    variant="neutral"
                                    focusKey={FOCUS_DISABLED}
                                    onSelect={() => setShowSaveReminder(false)}
                                    title={TEXT.reminder.dismiss}
                                    style={{
                                        flexShrink: 0,
                                        minHeight: "22rem",
                                        width: "22rem",
                                        paddingTop: "0",
                                        paddingRight: "0",
                                        paddingBottom: "0",
                                        paddingLeft: "0"
                                    }}
                                >
                                    <img
                                        src="Media/Glyphs/Close.svg"
                                        alt=""
                                        style={{
                                            width: "12rem",
                                            height: "12rem",
                                            filter: "brightness(0) invert(1)",
                                            opacity: 0.75,
                                            pointerEvents: "none"
                                        }}
                                    />
                                </Button>
                            </div>
                        )}
                        <div style={{
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "space-between",
                            marginBottom: "13rem"
                        }}>
                            <SelectionFilterControls
                                label={TEXT.panel.selectFilter}
                                roadsLabel={TEXT.panel.filterRoads}
                                railsLabel={TEXT.panel.filterRails}
                                waterLabel={TEXT.panel.filterWater}
                                showTip={showPanelTooltip}
                                hideTip={hidePanelTooltip}
                            />
                            <Button
                                focusKey={FOCUS_DISABLED}
                                variant="neutral"
                                onSelect={collapseAllSections}
                                onMouseEnter={() => {
                                    setIsExpandAllHovered(true);
                                    showPanelTooltip("expandAll");
                                }}
                                onMouseLeave={() => {
                                    setIsExpandAllHovered(false);
                                    hidePanelTooltip();
                                }}
                                title={panelTitle(expandAllTooltipText)}
                                style={{
                                    flexShrink: 0,
                                    width: "30rem",
                                    height: "30rem",
                                    minHeight: "30rem",
                                    marginLeft: "6rem",
                                    marginRight: "-7rem",
                                    display: "flex",
                                    alignItems: "center",
                                    justifyContent: "center",
                                    backgroundColor: "transparent",
                                    borderWidth: "0",
                                    borderStyle: "solid",
                                    borderColor: "rgba(255, 255, 255, 0)",
                                    borderRadius: "50%",
                                    boxSizing: "border-box",
                                    cursor: "pointer",
                                    paddingTop: "0",
                                    paddingRight: "0",
                                    paddingBottom: "0",
                                    paddingLeft: "0"
                                }}
                            >
                                <img
                                    src={expandCollapseAllIcon}
                                    alt=""
                                    style={{
                                        width: "22rem",
                                        height: "22rem",
                                        filter: "brightness(0) invert(1)",
                                        opacity: isExpandAllHovered ? 1 : 0.68,
                                        pointerEvents: "none"
                                    }}
                                />
                            </Button>
                        </div>

                        {/* Selected: New-speed column (left) + presets (right). */}
                        <CollapsibleSectionHeader
                            label={TEXT.panel.selectedSegment}
                            expanded={selectionInfoExpanded}
                            onToggle={toggleSelectionInfoExpanded}
                            focusKey={FOCUS_DISABLED}
                            labelTitle={panelTitle(TEXT.tooltips.selectedSegment)}
                            onLabelMouseEnter={() => showPanelTooltip("selectedSegment")}
                            onLabelMouseLeave={hidePanelTooltip}
                            trailing={selectionInfoExpanded ? (
                                <div
                                    onMouseEnter={() => showPanelTooltip("presets")}
                                    onMouseLeave={hidePanelTooltip}
                                    title={panelTitle(TEXT.tooltips.presets)}
                                    style={{
                                        width: "139rem",
                                        minWidth: "139rem",
                                        textAlign: "left",
                                        fontSize: "11rem",
                                        fontWeight: 500,
                                        color: "rgba(255, 255, 255, 0.58)",
                                        lineHeight: "1"
                                    }}
                                >
                                    {TEXT.panel.presets}
                                </div>
                            ) : undefined}
                        />
                        {selectionInfoExpanded && (
                            <div style={{ display: "flex", marginBottom: "8rem" }}>
                                <div style={{ width: "116rem", minWidth: "116rem", flexShrink: 0 }}>
                                    <SelectionSection
                                        focusKey={FOCUS_DISABLED}
                                        newSpeedLabel={TEXT.panel.newSpeedLimit}
                                        newSpeedNumber={`${Math.round(displaySpeed)}`}
                                        newSpeedUnit={unitLabel}
                                        unitToggleTitle={panelTitle("kmh / mph")}
                                        onToggleUnit={() => SetPanelSpeedUnit(!showMetric)}
                                        currentSpeedTitle={panelTitle(TEXT.tooltips.currentSpeed)}
                                        gameDefaultTitle={panelTitle(TEXT.panel.gameDefault)}
                                        currentSpeedLabelText={TEXT.panel.currentSpeed}
                                        currentSpeedValueText={currentSpeedLabel}
                                        defaultSpeedLabelText={TEXT.panel.gameDefault}
                                        defaultSpeedValueText={vanillaSpeedLabel}
                                        onCurrentSpeedMouseEnter={() => showPanelTooltip("currentSpeed")}
                                        onCurrentSpeedMouseLeave={hidePanelTooltip}
                                        onGameDefaultMouseEnter={() => showPanelTooltip("gameDefault")}
                                        onGameDefaultMouseLeave={hidePanelTooltip}
                                    />
                                </div>
                                <div style={{ flex: 1, minWidth: "0", marginLeft: "8rem" }}>
                                    <PresetControls
                                        presetRows={presetRows}
                                        displayValue={Math.round(displaySpeed)}
                                        minDisplay={sliderMin}
                                        maxDisplay={sliderMax}
                                        unlimitedValue={Math.round(getDisplaySpeed(getMaxSpeedKmh()))}
                                        unitLabel={unitLabel}
                                        onPresetSelect={handlePresetSpeed}
                                        getPresetTitle={preset => panelTitle(`${preset} ${unitLabel}`)}
                                        unlimitedTitle={panelTitle(TEXT.tooltips.presetUnlimited.title)}
                                        onUnlimitedMouseEnter={() => showPanelTooltip("presetUnlimited")}
                                        onUnlimitedMouseLeave={hidePanelTooltip}
                                        focusKey={FOCUS_DISABLED}
                                    />
                                </div>
                            </div>
                        )}

                        {/* Custom: slider + stepper + reset on one row, Apply below. TODO(stage2): localize title. */}
                        <CollapsibleSectionHeader
                            label={TEXT.panel.custom}
                            expanded={sliderExpanded}
                            onToggle={toggleSliderExpanded}
                            focusKey={FOCUS_DISABLED}
                        />
                        {sliderExpanded && (
                            <MainSpeedSection
                                focusKey={FOCUS_DISABLED}
                                resetTitle={panelTitle(TEXT.tooltips.reset)}
                                applyButtonText={isApplying ? TEXT.buttons.applied : TEXT.buttons.apply}
                                sliderMin={sliderMin}
                                sliderMax={sliderMax}
                                sliderStep={sliderStep}
                                sliderValue={sliderValue}
                                unitLabel={unitLabel}
                                isApplying={isApplying}
                                isResetting={isResetting}
                                stepper={
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
                                }
                                onApplyMouseEnter={() => showPanelTooltip("apply")}
                                onResetMouseEnter={() => showPanelTooltip("resetSelected")}
                                onControlMouseLeave={hidePanelTooltip}
                                onSliderChange={handleSliderChange}
                                onApply={handleApply}
                                onReset={handleReset}
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
                            cityIndustryActive={cityIndustryActive}
                            cityIndustryParked={cityIndustryParked}
                            cityIndustryTotal={cityIndustryTotal}
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
                markersTooltipText={speedMarkersTooltipText}
                expandAllTooltipText={expandAllTooltipText}
                isGuideHovered={isGuideHovered}
                isHelpHovered={isHelpHovered}
                isMarkersHovered={isMarkersHovered}
                getRoadGroupTooltip={getRoadGroupTooltip}
            />
        </>
    );
};
