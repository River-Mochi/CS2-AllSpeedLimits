// File: UI/src/panel/components/AllPathsSection.tsx
// Global pathway speed limits, not per-segment pedestrian walking speed.

import { Button } from "../../shared/Button";
import { Slider } from "../../slider/slider";

type Props = {
    percent: number;
    savedPercent: number;
    busy: boolean;
    processed: number;
    total: number;
    focusKey: unknown;
    text: {
        heading: string;
        description: string;
        slider: string;
        vanilla: string;
        saveNote: string;
        apply: string;
        reset: string;
        working: string;
        customWins: string;
    };
    onChange: (percent: number) => void;
    onApply: () => void;
    onReset: () => void;
};

export const AllPathsSection = ({
    percent, savedPercent, busy, processed, total, focusKey, text,
    onChange, onApply, onReset
}: Props) => (
    <div style={{
        paddingTop: "5rem",
        paddingBottom: "10rem",
        color: "rgba(255, 255, 255, 0.90)",
        fontSize: "12rem"
    }}>
        <div style={{ fontWeight: 700, fontSize: "14rem", marginBottom: "7rem" }}>
            {text.heading}
        </div>
        <div style={{ lineHeight: "1.35", marginBottom: "10rem", opacity: 0.85 }}>
            {text.description}
        </div>
        <div style={{
            display: "flex", alignItems: "center", justifyContent: "space-between",
            marginBottom: "6rem", fontWeight: 650
        }}>
            <span>{text.slider}</span>
            <span>{percent}%</span>
        </div>
        <Slider
            start={100}
            end={500}
            step={25}
            value={percent}
            onChange={onChange}
        />
        <div style={{
            display: "flex", justifyContent: "space-between", marginTop: "6rem",
            marginBottom: "12rem", fontSize: "11rem", opacity: 0.78
        }}>
            <span>{text.vanilla}</span>
            <span>{savedPercent}%</span>
        </div>
        <div style={{ display: "flex", justifyContent: "flex-end" }}>
            <Button
                focusKey={focusKey}
                variant="neutral"
                disabled={busy}
                onSelect={onReset}
                style={{ marginRight: "7rem", minHeight: "27rem", paddingLeft: "9rem", paddingRight: "9rem" }}
            >
                {text.reset}
            </Button>
            <Button
                focusKey={focusKey}
                disabled={busy || percent === savedPercent}
                onSelect={onApply}
                style={{ minHeight: "27rem", paddingLeft: "10rem", paddingRight: "10rem" }}
            >
                {busy ? text.working : text.apply}
            </Button>
        </div>
        {busy && (
            <div style={{ fontSize: "11rem", marginTop: "8rem" }}>
                {text.working} {total > 0 ? `${processed}/${total}` : ""}
            </div>
        )}
        <div style={{
            marginTop: "10rem", paddingTop: "7rem", fontSize: "11rem",
            lineHeight: "1.35", borderTopWidth: "1rem", borderTopStyle: "solid",
            borderTopColor: "rgba(255,255,255,0.16)", opacity: 0.75
        }}>
            {text.customWins}
            <div style={{ marginTop: "4rem" }}>{text.saveNote}</div>
        </div>
    </div>
);
