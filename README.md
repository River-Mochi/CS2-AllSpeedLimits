# Adjust All Speed Limits

Fine-tune speed limits in Cities: Skylines II.

## Features

- Click a network segment or drag to select several.
- Change speeds on roads, bike-capable paths, tram/train/subway tracks, and waterways.
- Options: bicycle and electric scooter maximum speed (30–1000%, default 100%). Acceleration and braking adjust automatically.
- Adjust individual segments with a slider, number box, or preset signs.
- Switch to **Paths** for separate global limits on bike/mixed paths and pedestrian-only paths (100% = vanilla).
- Set each global path category from **30% to 500%**, with independent Apply and Reset.
- Global changes update eligible existing and new paths; individual ASL bike speeds take priority.
- Reveal the game's **blue/green dashed invisible networks** with the eye icon.
- Edit supported invisible roads and bike-capable paths; also works on supported building-internal networks.
- Assign an optional **Dashed Eye hotkey** under Options → Actions → Keybinds. It works even with the ASL panel closed.
- Reset selected speeds, clear custom speeds by type, or apply citywide road/rail speeds.
- Switch between km/h and mph, show floating speeds, and view optional vehicle stats.

## Important

- The eye shows vanilla markers globally, including pedestrian-only paths that ASL cannot edit individually.
- **New:** Bicycle and mixed bicycle/pedestrian paths can be changed per segment. Pedestrian-only paths can be changed globally, not individually.
- The Bike slider controls bike-only and mixed paths. The Pedestrian slider controls pedestrian-only paths. Mixed paths always use the Bike slider. ASL changes bicycle/e-scooter maximum speed only through the separate Options setting. It does not change citizen walking speed or other vehicle maximum speeds.
- Hidden vehicle-only roads are **not** affected by either global Paths slider; edit supported ones individually under Speed Limits.
- **Speed Limits → Paths Reset** clears individual bike/mixed speeds so they follow the global Bike scale. Each **Paths → Reset** returns that category to 100%.
- **Bikes & Paths 1.1.6+** delegates overlapping tuning to ASL. Update B+P before using both mods. Older B+P versions can still overwrite pathway limits.
- No dependency on ToggleMarkers, Show Invisible Networks, or Extra Networks and Areas. Mods that show invisible networks share the game's visibility setting.
- ASL does not modify building layouts. Vehicles still obey their maximum speeds and network limits. Only bicycle/e-scooter maximum speed is tunable in Options.
- Custom network speeds remain after removing ASL. **Reset them and save your city before uninstalling** if you want vanilla values. Reset the bicycle speed in Options to 100% as well.

## Saves and performance

- Speed changes are saved with the city. A small backup is stored in `ModsData/AllSpeedLimits`; old `ModsData/SpeedLimits` backups are migrated.
- Whole-city actions run in batches. Global Paths applies even while paused. Floating numbers, selection overlays, and vehicle stats run only while the ASL tool is open.

## License

GPL-3.0-or-later with the Cities: Skylines II Linking Exception. See [LICENSE](LICENSE), [LICENSE-EXCEPTION](LICENSE-EXCEPTION), and [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
