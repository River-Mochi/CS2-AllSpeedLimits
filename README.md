# Adjust All Speed Limits

Fine-tune speed limits in Cities: Skylines II.

## Features

- Click a network segment or drag to select several.
- Change speeds on roads, bike-capable paths, tram/train/subway tracks, and waterways.
- Adjust individual segments with a slider, number box, or preset signs.
- Switch to **Paths** for global bike-only and mixed bike/pedestrian path limits (100% = vanilla).
- Set global bike-path limits from **30% to 500%**, with Apply and Reset.
- Global changes update eligible existing and new bike/mixed paths; individual ASL bike speeds take priority.
- Reveal the game's **blue/green dashed invisible networks** with the eye icon.
- Edit supported invisible roads and bike-capable paths; also works on supported building-internal networks.
- Assign an optional **Dashed Eye hotkey** under Options → Actions → Keybinds. It works even with the ASL panel closed.
- Reset selected speeds, clear custom speeds by type, or apply citywide road/rail speeds.
- Switch between km/h and mph, show floating speeds, and view optional vehicle stats.

## Important

- The eye shows vanilla markers globally, including pedestrian-only paths that ASL cannot edit individually.
- **New:** Bicycle and mixed bicycle/pedestrian paths can be changed per segment. Pedestrian-only paths are not speed-editable.
- Global **Paths** changes only bike-only and mixed path speed-limit data, not pedestrian-only paths or pedestrian walking speed.
- Hidden vehicle-only roads are **not** affected by the global Paths slider; edit supported ones individually under Speed Limits.
- **Speed Limits → Paths Reset** clears individual bike/mixed speeds so they follow the global bike scale. **Paths → Reset** returns the global bike scale to 100%.
- **Bikes & Paths** also modifies global pathway limits. Avoid applying both global pathway sliders at the same time.
- No dependency on ToggleMarkers, Show Invisible Networks, or Extra Networks and Areas. Mods that show invisible networks share the game's visibility setting.
- ASL edits speed limits, not building layouts or vehicle top speeds. Vehicles still obey their maximum speeds.
- Custom speeds remain after removing ASL. **Reset them and save your city before uninstalling** if you want vanilla values.

## Saves and performance

- Speed changes are saved with the city. A small backup is stored in `ModsData/AllSpeedLimits`; old `ModsData/SpeedLimits` backups are migrated.
- Whole-city actions run in batches. Global Paths applies even while paused. Floating numbers, selection overlays, and vehicle stats run only while the ASL tool is open.

## License

GPL-3.0-or-later with the Cities: Skylines II Linking Exception. See [LICENSE](LICENSE), [LICENSE-EXCEPTION](LICENSE-EXCEPTION), and [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
