// <copyright file="SelectionFilterControls.tsx" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: UI/src/panel/components/SelectionFilterControls.tsx
// Purpose: Compact toggles limiting which net types the tool can select (roads / rails / water).

import { Button, Tooltip } from "cs2/ui";
import { useState } from "react";
import type { PanelTooltipKind } from "../types";
import { VanillaComponentResolver } from "../../utils/vanilla/VanillaComponentResolver";
import { useSafeBinding } from "../../shared/useSafeBinding";
import {
  SELECT_PATHS,
  SHOW_INVISIBLE,
  SetSelectPaths,
  SetShowInvisible,
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

import pathIcon from "../../images/Bicycle_max.svg";
import invisibleIcon from "../../images/asl-invisible-dashed-loop-no-transform.svg";

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
  nativeTooltip?: string;
  showActiveDot?: boolean;
  wideIcon?: boolean;
};

const FilterChip = ({ active, text, tipKind, iconSrc, onToggle, showTip, hideTip, nativeTooltip, showActiveDot = true, wideIcon = false }: FilterChipProps) => {
  const [hovered, setHovered] = useState(false);
  // Use the game's resolved button here. A direct import registers controller focus twice and
  // floods the log with duplicate-focus errors.
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

  const chip = (
    <Button
      as="button"
      focusKey={focusDisabled}
      theme={{ button: "" }}
      aria-pressed={active}
      aria-label={text}
      onMouseEnter={() => { setHovered(true); showTip(tipKind); }}
      onMouseLeave={() => { setHovered(false); hideTip(); }}
      onSelect={onToggle}
      style={{
        position: "relative",
        width: USE_ICONS ? (wideIcon ? "44rem" : "30rem") : "auto",
        minWidth: USE_ICONS ? (wideIcon ? "44rem" : "30rem") : "46rem",
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
            width: wideIcon ? "40rem" : "26rem",
            height: "26rem",
            pointerEvents: "none",
            opacity: wideIcon && !active && !hovered ? 0.5 : 1,
            filter: iconFilter,
            transform: hovered ? "scale(1.05)" : "none"
          }}
        />
      ) : (
        <span style={{ fontSize: "10.5rem", fontWeight: 700, color: "#fff", whiteSpace: "nowrap" }}>
          {text}
        </span>
      )}

      {active && showActiveDot && (
        <span style={{
          position: "absolute",
          left: "2rem",
          bottom: "2rem",
          width: "5.5rem",
          height: "5.5rem",
          borderRadius: "50%",
          backgroundColor: kSelectedAccent,
          boxShadow: "0 0 2rem rgba(110, 200, 235, 0.85)",
          pointerEvents: "none"
        }} />
      )}
    </Button>
  );
  // Use a native CS2 tooltip only in the initial hint panel.
  return nativeTooltip
    ? <Tooltip tooltip={nativeTooltip} direction="up">{chip}</Tooltip>
    : chip;
};

type SelectionFilterControlsProps = {
  label: string;
  pathsLabel: string;
  invisibleLabel: string;
  roadsLabel: string;
  railsLabel: string;
  waterLabel: string;
  showTip: (kind: PanelTooltipKind) => void;
  hideTip: () => void;
  nativeTooltips?: {
    roads: string;
    rails: string;
    water: string;
    paths: string;
    invisible: string;
  };
};

export const SelectionFilterControls = (props: SelectionFilterControlsProps) => {
  const {
    label,
    pathsLabel,
    invisibleLabel,
    roadsLabel,
    railsLabel,
    waterLabel,
    showTip,
    hideTip,
    nativeTooltips
  } = props;

  const paths = useSafeBinding(SELECT_PATHS, true);
  const invisible = useSafeBinding(SHOW_INVISIBLE, false);
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
            fontSize: "11rem",
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
            nativeTooltip={nativeTooltips?.roads}
            onToggle={toggle(roads, [paths, rails, water], SetSelectRoads)}
            showTip={showTip}
            hideTip={hideTip}
          />

          <FilterChip
            active={rails}
            text={railsLabel}
            tipKind="filterRails"
            iconSrc={railIcon}
            nativeTooltip={nativeTooltips?.rails}
            onToggle={toggle(rails, [roads, paths, water], SetSelectRails)}
            showTip={showTip}
            hideTip={hideTip}
          />
          <FilterChip
            active={water}
            text={waterLabel}
            tipKind="filterWater"
            iconSrc={waterIcon}
            nativeTooltip={nativeTooltips?.water}
            onToggle={toggle(water, [roads, paths, rails], SetSelectWater)}
            showTip={showTip}
            hideTip={hideTip}
          />
          <FilterChip
            active={paths}
            text={pathsLabel}
            tipKind="filterPaths"
            iconSrc={pathIcon}
            nativeTooltip={nativeTooltips?.paths}
            onToggle={toggle(paths, [roads, rails, water], SetSelectPaths)}
            showTip={showTip}
            hideTip={hideTip}
          />
          <div style={{
            marginLeft: "9rem",
            borderLeftWidth: "1rem",
            borderLeftStyle: "solid",
            borderLeftColor: "rgba(255,255,255,0.22)",
            paddingLeft: "9rem"
          }}>
            <FilterChip
              active={invisible}
              text={invisibleLabel}
              tipKind="showInvisible"
              iconSrc={invisibleIcon}
              showActiveDot={false}
              wideIcon={true}
              nativeTooltip={nativeTooltips?.invisible}
              onToggle={() => SetShowInvisible(!invisible)}
              showTip={showTip}
              hideTip={hideTip}
            />
          </div>
        </div>
      </div>
    </div>
  );
};
