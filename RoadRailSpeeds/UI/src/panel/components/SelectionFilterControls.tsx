// File: UI/src/panel/components/SelectionFilterControls.tsx
// Purpose: Compact toggles limiting which net types the tool can select (roads / rails / water).

import { Button } from "cs2/ui";
import { useState } from "react";
import type { PanelTooltipKind } from "../types";
import { VanillaComponentResolver } from "../../utils/vanilla/VanillaComponentResolver";
import { useSafeBinding } from "../../shared/useSafeBinding";
import {
  SELECT_ROADS,
  SELECT_RAILS,
  SELECT_WATER,
  SetSelectRoads,
  SetSelectRails,
  SetSelectWater,
} from "../../shared/bindings";
import roadIcon from "../../images/HighwayTwoway2lanes_max.svg";
import railIcon from "../../images/TwoWayTrainTrack_max.svg";
import waterIcon from "../../images/NarrowBoatway_max.svg";

const USE_ICONS = true;
const kSelectedAccent = "rgba(110, 200, 235, 0.88)";

type FilterChipProps = {
  active: boolean;
  text: string;
  tipKind: PanelTooltipKind;
  iconSrc: string;
  onToggle: () => void;
  showTip: (kind: PanelTooltipKind) => void;
  hideTip: () => void;
};

const FilterChip = ({ active, text, tipKind, iconSrc, onToggle, showTip, hideTip }: FilterChipProps) => {
  const [hovered, setHovered] = useState(false);
  // Must come from the resolver (game module), not a direct cs2/ui import, or the Button
  // auto-generates a focus key and floods "cannot register second focus key" errors.
  const focusDisabled = VanillaComponentResolver.instance.FOCUS_DISABLED;

  // No lighter wash on hover — it washed out the icon. Hover feedback is the border glow + icon grow.
  const backgroundColor = "rgba(60, 82, 98, 0.42)";
  const borderColor = active
    ? "rgba(110, 200, 235, 0.55)"
    : (hovered ? "rgba(255, 255, 255, 0.45)" : "rgba(255, 255, 255, 0)");
  const boxShadow = active ? "0 0 4rem rgba(110, 200, 235, 0.45)" : "none";
  const iconFilter = hovered
    ? "contrast(1.2) saturate(1.3) brightness(1.12)"
    : "contrast(1.2) saturate(1.3)";

  return (
    <Button
      as="button"
      focusKey={focusDisabled}
      theme={{ button: "" }}
      aria-pressed={active}
      onMouseEnter={() => { setHovered(true); showTip(tipKind); }}
      onMouseLeave={() => { setHovered(false); hideTip(); }}
      onSelect={onToggle}
      style={{
        position: "relative",
        width: USE_ICONS ? "30rem" : "auto",
        minWidth: USE_ICONS ? "30rem" : "46rem",
        height: "30rem",
        marginLeft: "2rem",
        paddingLeft: USE_ICONS ? "0" : "8rem",
        paddingRight: USE_ICONS ? "0" : "8rem",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        borderRadius: "4rem",
        borderWidth: "1rem",
        borderStyle: "solid",
        borderColor,
        backgroundColor,
        boxShadow,
        boxSizing: "border-box",
        cursor: "pointer",
        transitionProperty: "background-color, border-color, box-shadow, transform",
        transitionDuration: "120ms, 120ms, 120ms, 120ms",
        transitionTimingFunction: "ease-out, ease-out, ease-out, ease-out"
      }}
    >
      {USE_ICONS ? (
        <img
          src={iconSrc}
          alt={text}
          style={{
            width: "26rem",
            height: "26rem",
            pointerEvents: "none",
            opacity: 1,
            filter: iconFilter,
            transform: hovered ? "scale(1.05)" : "none"
          }}
        />
      ) : (
        <span style={{ fontSize: "10.5rem", fontWeight: 700, color: "#fff", whiteSpace: "nowrap" }}>
          {text}
        </span>
      )}

      {active && (
        <span style={{
          position: "absolute",
          left: "2rem",
          bottom: "2rem",
          width: "4.2rem",
          height: "4.2rem",
          borderRadius: "50%",
          backgroundColor: kSelectedAccent,
          boxShadow: "0 0 2rem rgba(110, 200, 235, 0.85)",
          pointerEvents: "none"
        }} />
      )}
    </Button>
  );
};

type SelectionFilterControlsProps = {
  label: string;
  roadsLabel: string;
  railsLabel: string;
  waterLabel: string;
  showTip: (kind: PanelTooltipKind) => void;
  hideTip: () => void;
};

export const SelectionFilterControls = (props: SelectionFilterControlsProps) => {
  const {
    label,
    roadsLabel,
    railsLabel,
    waterLabel,
    showTip,
    hideTip
  } = props;

  const roads = useSafeBinding(SELECT_ROADS, true);
  const rails = useSafeBinding(SELECT_RAILS, true);
  const water = useSafeBinding(SELECT_WATER, true);

  const toggle = (current: boolean, others: boolean[], setter: (v: boolean) => void) => () => {
    if (current && !others.some(Boolean)) {
      return;
    }

    setter(!current);
  };

  return (
    <div>
      <div style={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        width: "100%"
      }}>
        <div style={{
          display: "flex",
          alignItems: "center",
          minWidth: "0"
        }}>
          <span style={{
            fontSize: "10.5rem",
            color: "rgba(226, 236, 241, 0.74)",
            marginRight: "4rem",
            whiteSpace: "nowrap",
          }}>
            {label}
          </span>
          <FilterChip
            active={roads}
            text={roadsLabel}
            tipKind="filterRoads"
            iconSrc={roadIcon}
            onToggle={toggle(roads, [rails, water], SetSelectRoads)}
            showTip={showTip}
            hideTip={hideTip}
          />
          <FilterChip
            active={rails}
            text={railsLabel}
            tipKind="filterRails"
            iconSrc={railIcon}
            onToggle={toggle(rails, [roads, water], SetSelectRails)}
            showTip={showTip}
            hideTip={hideTip}
          />
          <FilterChip
            active={water}
            text={waterLabel}
            tipKind="filterWater"
            iconSrc={waterIcon}
            onToggle={toggle(water, [roads, rails], SetSelectWater)}
            showTip={showTip}
            hideTip={hideTip}
          />
        </div>
      </div>
    </div>
  );
};
