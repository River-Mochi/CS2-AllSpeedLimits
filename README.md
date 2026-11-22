# Adjust All Speed Limits

Fine-tune speed limits in Cities: Skylines II.

## Features

- Click a network segment or drag to select several.
- Change speeds on roads, bike-capable paths, tram/train/subway tracks, and waterways.
- Adjust individual segments with a slider, number box, or preset signs.
- Switch to **All Paths** in the same panel to change visible/invisible pathway prefab limits citywide (100% = vanilla).
- New and existing bicycle-capable paths are updated in batches. Individual ASL speed limits take priority.
- Reveal the game's **blue/green dashed invisible networks** with the eye icon.
- Edit supported invisible roads and bike-capable paths; also works on supported building-internal networks.
- Assign an optional **Dashed Eye hotkey** under Options → Actions → Keybinds. It works even with the ASL panel closed.
- Reset selected speeds, clear custom speeds by type, or apply citywide road/rail speeds.
- Switch between km/h and mph, show floating speeds, and view optional vehicle stats.

## Important

- The eye shows vanilla markers globally, including pedestrian-only paths that ASL cannot edit individually.
- Bicycle and mixed bicycle/pedestrian paths with speed-editable lanes can be changed per segment. Pure pedestrian paths are not currently selectable.
- Global **All Paths** adjusts pathway prefab/composition limits; it does not change how fast citizens walk.
- **Bikes & Paths** also modifies global pathway limits. Avoid applying both global pathway sliders at the same time.
- No dependency on ToggleMarkers, Show Invisible Networks, or Extra Networks and Areas. Mods that show invisible networks share the game's visibility setting.
- ASL edits speed limits, not building layouts or vehicle top speeds. Vehicles still obey their maximum speeds.
- Custom speeds remain after removing ASL. **Reset them and save your city before uninstalling** if you want vanilla values.

## Saves and performance

- Speed changes are saved with the city. A small backup is stored in `ModsData/AllSpeedLimits`; old `ModsData/SpeedLimits` backups are migrated.
- Whole-city actions run in batches. Floating numbers, selection overlays, and vehicle stats run only while the ASL tool is open.

## License

GPL-3.0-or-later with the Cities: Skylines II Linking Exception. See [LICENSE](LICENSE), [LICENSE-EXCEPTION](LICENSE-EXCEPTION), and [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
