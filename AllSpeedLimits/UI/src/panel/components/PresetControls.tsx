// File: UI/src/panel/components/PresetControls.tsx
// Purpose: Renders preset speed circles plus the unlimited preset badge at the far right.

import { memo, useState } from "react";
import { Button } from "../../shared/Button";
import unlimitedIcon from "../../images/icon-Unlimited.svg";
import unlimitedSelectedIcon from "../../images/icon-Unlimited-Selected.svg";

type PresetControlsProps = {
    presetRows: number[][];
    displayValue: number;
    minDisplay: number;
    maxDisplay: number;
    unlimitedValue: number;
    unitLabel: string;
    onPresetSelect: (value: number) => void;
    getPresetTitle: (value: number) => string | undefined;
    unlimitedTitle?: string;
    onUnlimitedMouseEnter?: () => void;
    onUnlimitedMouseLeave?: () => void;
    focusKey: unknown;
};

export const PresetControls = memo((props: PresetControlsProps) => {
    const {
        presetRows,
        displayValue,
        minDisplay,
        maxDisplay,
        unlimitedValue,
        unitLabel,
        onPresetSelect,
        getPresetTitle,
        unlimitedTitle,
        onUnlimitedMouseEnter,
        onUnlimitedMouseLeave,
        focusKey
    } = props;

    // Unlimited now rides in the preset grid as a normal-size circle (sentinel value -1 in a row).
    const unlimitedSelected = Math.abs(displayValue - unlimitedValue) <= 0.5;
    const [hoveredPresetKey, setHoveredPresetKey] = useState<string | null>(null);

    return (
        <div style={{
            marginTop: "2rem",
            marginBottom: "7rem"
        }}>
            <div style={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between"
            }}>
                <div style={{ flex: 1, minWidth: "0" }}>
                    {presetRows.map((row, rowIndex) => (
                        <div
                            key={`preset-row-${rowIndex}`}
                            style={{
                                display: "flex",
                                alignItems: "center",
                                marginBottom: rowIndex === presetRows.length - 1 ? "0" : "3rem"
                            }}
                        >
                            {row.map((preset, index) => {
                                const isUnlimited = preset === -1;
                                const disabled = !isUnlimited && (preset < minDisplay || preset > maxDisplay);
                                const presetKey = isUnlimited ? "unlimited" : `preset-${preset}`;
                                const numberHovered = !isUnlimited && !disabled && hoveredPresetKey === presetKey;
                                const unlimitedHovered = isUnlimited && hoveredPresetKey === presetKey;
                                const selected = isUnlimited
                                    ? unlimitedSelected
                                    : (!disabled && Math.abs(displayValue - preset) <= 0.5);

                                return (
                                    <div
                                        key={`preset-${preset}`}
                                        onMouseEnter={() => {
                                            if (!disabled) {
                                                setHoveredPresetKey(presetKey);
                                            }
                                            if (isUnlimited) {
                                                onUnlimitedMouseEnter?.();
                                            }
                                        }}
                                        onMouseLeave={() => {
                                            setHoveredPresetKey(null);
                                            if (isUnlimited) {
                                                onUnlimitedMouseLeave?.();
                                            }
                                        }}
                                        style={{
                                            width: "25rem",
                                            minWidth: "25rem",
                                            marginRight: index === row.length - 1 ? "0" : "2rem"
                                        }}
                                    >
                                        <Button
                                            focusKey={focusKey}
                                            selected={selected}
                                            disabled={disabled}
                                            onSelect={() => onPresetSelect(isUnlimited ? unlimitedValue : preset)}
                                            variant="neutral"
                                            style={{
                                                width: "100%",
                                                minHeight: "25rem",
                                                height: "25rem",
                                                paddingTop: "0",
                                                paddingRight: "0",
                                                paddingBottom: "0",
                                                paddingLeft: "0",
                                                borderRadius: "50%",
                                                borderWidth: isUnlimited ? "0" : "1.5rem",
                                                borderColor: isUnlimited
                                                    ? "rgba(255, 255, 255, 0)"
                                                    : selected ? "rgba(255, 255, 255, 0.82)"
                                                    : disabled ? "rgba(180, 72, 72, 0.22)" : "rgba(210, 42, 42, 0.82)",
                                                backgroundColor: isUnlimited
                                                    ? "rgba(255, 255, 255, 0)"
                                                    : selected ? "rgba(194, 36, 36, 0.92)"
                                                    : disabled ? "rgba(255, 255, 255, 0.08)" : "rgba(245, 245, 245, 0.92)",
                                                color: selected ? "#fff" : "rgba(8, 10, 12, 1)",
                                                fontSize: preset >= 100
                                                    ? (numberHovered ? "12.4rem" : "10.6rem")
                                                    : (numberHovered ? "13.4rem" : "11.6rem"),
                                                fontWeight: 900,
                                                letterSpacing: "0",
                                                opacity: disabled ? 0.40 : 1
                                            }}
                                            title={isUnlimited ? unlimitedTitle : (getPresetTitle(preset) ?? `${preset} ${unitLabel}`)}
                                        >
                                            {isUnlimited ? (
                                                <img
                                                    src={unlimitedSelected ? unlimitedSelectedIcon : unlimitedIcon}
                                                    alt=""
                                                    style={{
                                                        width: unlimitedHovered ? "27rem" : "25rem",
                                                        height: unlimitedHovered ? "27rem" : "25rem",
                                                        display: "block"
                                                    }}
                                                />
                                            ) : (
                                                <span style={{ position: "relative", top: "1rem" }}>{preset}</span>
                                            )}
                                        </Button>
                                    </div>
                                );
                            })}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
});
