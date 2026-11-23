// File: UI/src/panel/components/AllPathsSection.tsx
// A citywide pathway multiplier; individual ASL segment overrides take priority.

import { Button } from "../../shared/Button";
import { Slider } from "../../slider/slider";
import type { PanelTooltipKind } from "../types";

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
        apply: string;
        reset: string;
        working: string;
        sliderTooltip: string;
        resetTooltip: string;
        applyTooltip: string;
    };
    onChange: (percent: number) => void;
    onApply: () => void;
    onReset: () => void;
    onShowTip: (kind: PanelTooltipKind) => void;
    onHideTip: () => void;
};

export const AllPathsSection = ({
    percent, savedPercent, busy, processed, total, focusKey, text,
    onChange, onApply, onReset, onShowTip, onHideTip
}: Props) => (
    <div style={{
        paddingTop: "5rem",
        paddingBottom: "9rem",
        color: "rgba(235, 244, 250, 0.94)",
        fontSize: "12rem"
    }}>
        <div style={{
            display: "flex", alignItems: "center", justifyContent: "space-between",
            fontWeight: 700, fontSize: "14rem", marginBottom: "10rem"
        }}>
            <span>{text.heading}</span>
            <span>{percent}%</span>
        </div>
        <div
            onMouseEnter={() => onShowTip("pathSlider")}
            onMouseLeave={onHideTip}
        >
            <Slider
                start={30}
                end={500}
                step={10}
                value={percent}
                onChange={onChange}
            />
        </div>
        <div style={{
            marginTop: "6rem", marginBottom: "12rem",
            fontSize: "11rem", opacity: 0.78
        }}>
            {text.vanilla}
        </div>
        <div style={{ display: "flex", justifyContent: "flex-end" }}>
            <div
                onMouseEnter={() => onShowTip("pathReset")}
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
                    {text.reset}
                </Button>
            </div>
            <div
                onMouseEnter={() => onShowTip("pathApply")}
                onMouseLeave={onHideTip}
            >
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
                    {text.apply}
                </Button>
            </div>
        </div>
        {busy && (
            <div style={{ fontSize: "11rem", marginTop: "8rem", opacity: 0.84 }}>
                {text.working} {total > 0 ? `${processed}/${total}` : ""}
            </div>
        )}
        <div style={{
            marginTop: "12rem",
            paddingTop: "8rem",
            fontSize: "11rem",
            lineHeight: "1.35",
            borderTopWidth: "1rem",
            borderTopStyle: "solid",
            borderTopColor: "rgba(255,255,255,0.16)",
            opacity: 0.78
        }}>
            {text.description}
        </div>
    </div>
);
