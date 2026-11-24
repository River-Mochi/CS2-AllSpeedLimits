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
    vanilla: string;
    showDefaultHint: boolean;
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
        percent, savedPercent, busy, focusKey, text, vanilla, showDefaultHint, apply, reset,
        sliderTip, applyTip, resetTip, onChange, onApply, onReset, onShowTip, onHideTip
    } = props;

    return (
        <div>
            <div style={{
                display: "flex", alignItems: "center", justifyContent: "space-between",
                fontWeight: 700, fontSize: "13rem", marginBottom: "6rem"
            }}>
                <span>{text.heading}</span>
                {showDefaultHint && (
                    <span style={{ fontWeight: 500, fontSize: "10rem", opacity: 0.74 }}>
                        {vanilla}
                    </span>
                )}
            </div>
            <div style={{ display: "flex", alignItems: "center" }}
                 onMouseEnter={() => onShowTip(sliderTip)} onMouseLeave={onHideTip}>
                <div style={{ flex: 1, minWidth: 0 }}>
                    <Slider
                        start={30}
                        end={500}
                        step={10}
                        value={percent}
                        onChange={onChange}
                    />
                </div>
                <span style={{
                    minWidth: "44rem", textAlign: "right", marginLeft: "8rem",
                    fontWeight: 700, fontSize: "14rem"
                }}>
                    {percent}%
                </span>
            </div>
            <div style={{ display: "flex", justifyContent: "flex-end", marginTop: "8rem" }}>
                <div
                    onMouseEnter={() => onShowTip(resetTip)}
                    onMouseLeave={onHideTip}
                    style={{ marginRight: "7rem" }}
                >
                    <Button
                        focusKey={focusKey}
                        variant="neutral"
                        disabled={busy || (percent === 100 && savedPercent === 100)}
                        onSelect={onReset}
                        style={{
                            minHeight: "27rem",
                            paddingLeft: "13rem",
                            paddingRight: "13rem"
                        }}
                    >
                        {reset}
                    </Button>
                </div>
                <div onMouseEnter={() => onShowTip(applyTip)} onMouseLeave={onHideTip}>
                    <Button
                        focusKey={focusKey}
                        disabled={busy || percent === savedPercent}
                        onSelect={onApply}
                        style={{
                            minHeight: "27rem",
                            paddingLeft: "13rem",
                            paddingRight: "13rem"
                        }}
                    >
                        {apply}
                    </Button>
                </div>
            </div>
            <div style={{
                marginTop: "6rem",
                fontSize: "11rem",
                lineHeight: "1.35",
                opacity: 0.78
            }}>
                {text.description}
            </div>
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
        total, focusKey, text, pedText, onChange, onApply, onReset,
        onPedChange, onPedApply, onPedReset, onShowTip, onHideTip } = props;

    return (
        <div style={{
            paddingTop: "3rem",
            paddingBottom: "6rem",
            color: "rgba(235, 244, 250, 0.94)",
            fontSize: "12rem"
        }}>
            <PathScaleRow
                percent={percent}
                savedPercent={savedPercent}
                busy={busy}
                focusKey={focusKey}
                text={text}
                vanilla={text.vanilla}
                showDefaultHint={true}
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
                vanilla={text.vanilla}
                showDefaultHint={false}
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
