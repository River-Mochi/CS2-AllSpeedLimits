# Adjust All Speed Limits

Fine-tune speed limits in Cities: Skylines II.

## Features

- Open ASL and click a road, rail, waterway or bike path to open the full panel. Drag to select several segments. Filters, invisible networks and the Paths tab are available in the full panel.
- Change speeds on roads, bike-capable paths, tram/train/subway tracks, and waterways.
- Options → Bicycles: bicycle and electric scooter maximum speed (30–1000%, default 100%). Acceleration and braking adjust automatically.
- Bicycles tab: personal vehicle counts, parked vehicles and hidden cars at outside connections; status refreshes on demand in Options. Detailed hidden-car report available in the ASL log.
- Adjust individual segments with a slider, number box, or preset signs.
- Switch to **Paths** for separate global limits on bike/mixed paths and pedestrian-only paths (100% = vanilla).
- Set each global path category from **30% to 300%**, with independent Apply and Reset.
- Global changes update eligible existing and new paths; individual ASL bike speeds take priority.
- Reveal the game's **blue/green dashed invisible networks** with the dashed-lines button.
- Edit supported invisible roads and bike-capable paths; also works on supported building-internal networks.
- Assign an optional **Invisible Networks Hotkey** under Options → Actions → Keybinds. It works even with the ASL panel closed.
- Reset selected speeds, clear custom speeds by type, or apply citywide road/rail speeds.
- Switch between km/h and mph, show floating speeds, and view optional vehicle stats.

## Important

- The dashed-lines button shows vanilla markers globally, including pedestrian-only paths that ASL cannot edit individually.
- **New:** Bicycle and mixed bicycle/pedestrian paths can be changed per segment. Pedestrian-only paths can be changed globally, not individually.
- The Bike slider controls bike-only and mixed paths. The Pedestrian slider controls pedestrian-only paths. Mixed paths always use the Bike slider. For example, 200% changes a 30 km/h bike limit to 60 km/h; pedestrian walking speed is unchanged. ASL changes bicycle/e-scooter maximum speed only through the separate Options setting. It does not change citizen walking speed or other vehicle maximum speeds.
- Hidden vehicle-only roads are **not** affected by either global Paths slider; edit supported ones individually under Speed Limits.
- Example: Bike 200% makes a 30 km/h default bike path show 60 km/h current speed. Individual segment edits take priority; resetting an individual bike path returns it to the active Bike scale, not necessarily 100%.
- **Speed Limits → Paths Reset** clears individual bike/mixed speeds so they follow the global Bike scale. Each **Paths → Reset** returns that category to 100%.
- **Moving from Bikes + Paths?** ASL replaces B+P. Remove Bikes + Paths when using ASL; no reset is required. Optional: to restore vanilla values first, use Reset to Game Defaults with B+P alone, let the changes finish and save. Legacy hidden vehicle pathways may retain modified limits otherwise. B+P 1.1.6+ is compatible during the transition; older versions can conflict.
- No dependency on ToggleMarkers, Show Invisible Networks, or Extra Networks and Areas. Mods that show invisible networks share the game's visibility setting.
- ASL does not modify building layouts. Vehicles still obey their maximum speeds and network limits. Only bicycle/e-scooter maximum speed is tunable in Options.
- Custom network speeds remain after removing ASL. **Reset them and save your city before uninstalling** if you want vanilla values. Reset the bicycle speed in Options to 100% as well.

## Saves and performance

- Speed changes are saved with the city. A small backup is stored in `ModsData/AllSpeedLimits`; old `ModsData/SpeedLimits` backups are migrated.
- Whole-city actions run in batches. Global Paths applies even while paused. Floating numbers, selection overlays, and vehicle stats run only while the ASL tool is open.

## License

GPL-3.0-or-later with the Cities: Skylines II Linking Exception. See [LICENSE](LICENSE), [LICENSE-EXCEPTION](LICENSE-EXCEPTION), and [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
