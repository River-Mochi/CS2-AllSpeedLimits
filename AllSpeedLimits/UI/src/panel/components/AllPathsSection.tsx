// <copyright file="AllPathsSection.tsx" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: UI/src/panel/components/AllPathsSection.tsx
// Two independent citywide pathway speed-limit scales. Mixed paths belong to Bike.

import { Button } from "../../shared/Button";
import { Slider } from "../../slider/slider";
import type { PanelTooltipKind } from "../types";

type ScaleText = {
    heading: string;
    description: string;
    sliderTooltip: string;
    resetTooltip: string;
    applyTooltip: string;
};

type ScaleProps = {
    percent: number;
    savedPercent: number;
    busy: boolean;
    focusKey: unknown;
    text: ScaleText;
    showDescription?: boolean;
    apply: string;
    reset: string;
    sliderTip: PanelTooltipKind;
    applyTip: PanelTooltipKind;
    resetTip: PanelTooltipKind;
    onChange: (percent: number) => void;
    onApply: () => void;
    onReset: () => void;
    onShowTip: (kind: PanelTooltipKind) => void;
    onHideTip: () => void;
};

const PathScaleRow = (props: ScaleProps) => {
    const {
        percent, savedPercent, busy, focusKey, text, showDescription = false, apply, reset,
        sliderTip, applyTip, resetTip, onChange, onApply, onReset, onShowTip, onHideTip
    } = props;

    return (
        <div>
            <div style={{
                display: "flex", alignItems: "center", justifyContent: "space-between",
                fontWeight: 700, fontSize: "13rem", marginBottom: "6rem"
            }}>
                <span onMouseEnter={() => onShowTip(sliderTip)} onMouseLeave={onHideTip}
                    style={{ flex: 1, minWidth: "0" }}>{text.heading}</span>
            </div>
            <div style={{ display: "flex", alignItems: "center", marginBottom: "8rem" }}>
                <div style={{ flex: 1, minWidth: "0", marginRight: "8rem" }}
                    onMouseEnter={() => onShowTip(sliderTip)} onMouseLeave={onHideTip}>
                    <Slider start={30} end={300} step={10} value={percent}
                        onChange={(value: number) => { if (!busy) onChange(value); }} />
                    <div style={{ display: "flex", justifyContent: "space-between", fontSize: "11rem",
                        color: "rgba(255,255,255,0.78)", marginTop: "3rem" }}>
                        <span>30%</span><span>300%</span>
                    </div>
                </div>
                <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between",
                    width: "112rem", minWidth: "112rem", flexShrink: 0, position: "relative", top: "-4rem" }}
                    onMouseEnter={() => onShowTip(sliderTip)} onMouseLeave={onHideTip}>
                    <Button focusKey={focusKey} variant="neutral" disabled={busy || percent <= 30}
                        onSelect={() => onChange(Math.max(30, percent - 10))}
                        style={{ width: "28rem", minWidth: "28rem", height: "26rem", minHeight: "26rem", paddingLeft: "0", paddingRight: "0" }}>
                        −
                    </Button>
                    <div style={{ width: "46rem", height: "26rem", boxSizing: "border-box", display: "flex",
                        alignItems: "center", justifyContent: "center", fontWeight: 700, fontSize: "14rem",
                        backgroundColor: "rgba(5,10,14,0.42)", borderWidth: "1rem", borderStyle: "solid",
                        borderColor: "rgba(78,195,240,0.48)", borderRadius: "3rem" }}>
                        <span>{percent}%</span>
                    </div>
                    <Button focusKey={focusKey} variant="neutral" disabled={busy || percent >= 300}
                        onSelect={() => onChange(Math.min(300, percent + 10))}
                        style={{ width: "28rem", minWidth: "28rem", height: "26rem", minHeight: "26rem", paddingLeft: "0", paddingRight: "0" }}>
                        +
                    </Button>
                </div>
            </div>
            <div style={{ display: "flex", justifyContent: "flex-end" }}>
                <div style={{ display: "flex", alignItems: "center", minWidth: "112rem", justifyContent: "flex-end" }}>
                    <div onMouseEnter={() => onShowTip(applyTip)} onMouseLeave={onHideTip} style={{ marginRight: "6rem" }}>
                        <Button focusKey={focusKey} disabled={busy || percent === savedPercent}
                            onSelect={onApply} style={{ minWidth: "53rem", height: "26rem", minHeight: "26rem", paddingLeft: "5rem", paddingRight: "5rem" }}>
                            {apply}
                        </Button>
                    </div>
                    <div onMouseEnter={() => onShowTip(resetTip)} onMouseLeave={onHideTip}>
                        <Button focusKey={focusKey} variant="neutral" disabled={busy || (percent === 100 && savedPercent === 100)}
                            onSelect={onReset} style={{ minWidth: "53rem", height: "26rem", minHeight: "26rem", paddingLeft: "5rem", paddingRight: "5rem" }}>
                            {reset}
                        </Button>
                    </div>
                </div>
            </div>
            {showDescription && (
                <div style={{ marginTop: "6rem", fontSize: "11rem", lineHeight: "1.3",
                    color: "rgba(226,236,241,0.78)" }}>
                    {text.description}
                </div>
            )}
        </div>
    );
};

type Props = {
    percent: number;
    savedPercent: number;
    pedPercent: number;
    pedSavedPercent: number;
    busy: boolean;
    processed: number;
    total: number;
    focusKey: unknown;
    text: ScaleText & {
        vanilla: string;
        apply: string;
        reset: string;
        working: string;
    };
    pedText: ScaleText;
    wholeCityLabel: string;
    onChange: (percent: number) => void;
    onApply: () => void;
    onReset: () => void;
    onPedChange: (percent: number) => void;
    onPedApply: () => void;
    onPedReset: () => void;
    onShowTip: (kind: PanelTooltipKind) => void;
    onHideTip: () => void;
};

export const AllPathsSection = (props: Props) => {
    const { percent, savedPercent, pedPercent, pedSavedPercent, busy, processed,
        total, focusKey, text, pedText, wholeCityLabel, onChange, onApply, onReset,
        onPedChange, onPedApply, onPedReset, onShowTip, onHideTip } = props;

    return (
        <div style={{
            paddingTop: "3rem",
            paddingBottom: "6rem",
            color: "rgba(235, 244, 250, 0.94)",
            fontSize: "12rem"
        }}>
            <div style={{ fontSize: "13rem", color: "rgba(226,236,241,0.74)", marginBottom: "10rem" }}>{wholeCityLabel}</div>
            <PathScaleRow
                percent={percent}
                savedPercent={savedPercent}
                busy={busy}
                focusKey={focusKey}
                text={text}
                showDescription
                apply={text.apply}
                reset={text.reset}
                sliderTip="pathSlider"
                applyTip="pathApply"
                resetTip="pathReset"
                onChange={onChange}
                onApply={onApply}
                onReset={onReset}
                onShowTip={onShowTip}
                onHideTip={onHideTip}
            />
            <div style={{
                marginTop: "10rem",
                marginBottom: "10rem",
                borderTopWidth: "1rem",
                borderTopStyle: "solid",
                borderTopColor: "rgba(255,255,255,0.18)"
            }} />
            <PathScaleRow
                percent={pedPercent}
                savedPercent={pedSavedPercent}
                busy={busy}
                focusKey={focusKey}
                text={pedText}
                apply={text.apply}
                reset={text.reset}
                sliderTip="pedPathSlider"
                applyTip="pedPathApply"
                resetTip="pedPathReset"
                onChange={onPedChange}
                onApply={onPedApply}
                onReset={onPedReset}
                onShowTip={onShowTip}
                onHideTip={onHideTip}
            />
            {busy && (
                <div style={{ fontSize: "11rem", marginTop: "9rem", opacity: 0.84 }}>
                    {text.working} {total > 0 ? `${processed}/${total}` : ""}
                </div>
            )}
        </div>
    );
};
