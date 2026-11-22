# Adjust All Speed Limits

All Speed Limits is a Cities: Skylines II mod for changing road, bike-path, rail, and waterway speed limits.

## Features

- Select one segment or drag-select many segments.
- Adjust roads, bike paths, trams, trains, subways, and waterways.
- Use the slider, number box, or preset signs.
- Filter roads, paths, rails, and waterways separately.
- Use the dashed-eye toggle to reveal and select supported invisible networks, including internal building networks. This starts off each time the tool opens.
- Apply speeds to selected segments.
- Reset selected segments to their game defaults.
- Apply a speed to all small, medium, large, or highway roads.
- Apply a speed to all train or subway tracks.
- Clear custom speeds by type: roads, paths, rails, water, or all.
- Choose AUTO, KM/H, or MPH display.
- Show floating speed numbers while the tool is active.
- Show optional city vehicle stats while the panel is open.

## Notes

- Invisible networks use the same Apply and Reset controls as visible networks. Their overlays appear only while this tool is active and the dashed eye is enabled; other mods' marker visibility is left alone.
- Extra Networks and Areas and Show Invisible Networks are not dependencies. A separate placement tool may be needed to draw networks that vanilla menus do not expose.
- Path limits affect vehicle/bike lanes. Pedestrian-only lanes without an editable speed limit are not selectable. Human walking/running speeds and bikes using the game's separate pedestrian-lane cap are unchanged.
- Internal-network edits change lane speeds only. This mod does not replace roads, change building layouts, or patch the vanilla upgrade tool. If an upgrade replaces an entire internal segment, select the new segment and reapply its limit.
- Explicit path-segment limits take precedence over global path tuning such as Bikes + Paths while ASL is active in the playset. Reset removes that override; a later global tuning change can affect that path again.
- New installs use the simpler display scale by default, closer to vanilla road markings.
- Optional doubled-speed display is visual only; it does not change saved speeds.
- Painted road markings are art and may not exactly match the game's prefab speed data.
- Save the city after speed changes or resets if you want to keep them.

## Save Data

Speed changes are saved in the city. The mod also keeps a small per-city JSON backup so resets can restore known defaults.

Local backup folder:

`ModsData/AllSpeedLimits`

Existing backups in `ModsData/SpeedLimits` are migrated automatically.

For a clean uninstall, use the reset/clear buttons, save the city, then remove the mod.

## License

Current versions are licensed under GPL-3.0-or-later with the Cities: Skylines II Linking Exception. See [LICENSE](LICENSE) and [LICENSE-EXCEPTION](LICENSE-EXCEPTION).

Some UI binding helpers include or derive from MIT-licensed code. Their copyright and license notices are preserved in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

## Performance

- Vehicle stats update only while the panel is open.
- Whole-city actions run in batches.
- Floating speed numbers and selection overlays render only while the tool is active.

Source: https://github.com/River-Mochi/CS2-AllSpeedLimits
