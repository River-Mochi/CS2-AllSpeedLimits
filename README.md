# Adjust All Speed Limits

Change speed limits on roads, railways and invisible roads in Cities: Skylines II.

[Download on Paradox Mods](https://mods.paradoxplaza.com/mods/149990/Windows)

## Key features

- **Click a road to start.** Drag to select several segments, then set a speed using preset signs, a slider or precise steps.
- Works on roads, tram/train/subway tracks, waterways and bike-capable paths.
- **Invisible roads:** use the dashed-lines button to reveal and edit supported hidden networks, including building-internal roads. Assign an optional hotkey in Options → Actions → Keybinds; it works even with ASL closed.
- Filter network types, switch between km/h and mph, and show floating speed numbers.
- Apply speed limits citywide to road groups, trains or subway tracks. Reset individual segments or whole network types.

## Bonus features

- **Paths tab:** separate citywide speed limits for bicycle/mixed paths and pedestrian-only paths. Individual bike-path edits take priority.
- **Options → Bicycles:** optional bicycle and electric scooter maximum speed, acceleration and braking. OFF by default to avoid conflicts with other vehicle-speed mods.
- Personal vehicle statistics, parked counts and a hidden-car diagnostic report in Options.

## Compatibility and saves

- Remove the old Road Speed Adjuster (RSA) before using ASL. Existing custom limits can still be edited or reset.
- ASL replaces Bikes + Paths (B+P). Remove B+P; no reset is required. B+P 1.1.7 is compatible during the transition.
- No dependency on ToggleMarkers, Show Invisible Networks or Extra Networks and Areas. Marker visibility is shared with the game's own setting.
- Vehicles still obey their model's maximum speed and network limits. ASL does not change walking speed or building layouts.
- Custom network limits remain after uninstalling. To restore vanilla limits, use the Reset buttons and save your city. Turn off optional vehicle tuning before removing ASL.

## Technical notes

- Bike and pedestrian citywide scales: 30–300%. Bicycle/scooter vehicle scale: 30–1000% (100% = game default). Saved vehicle percentages are retained when tuning is disabled.
- Mixed paths follow the Bike scale; pedestrian-only paths are not individually editable. Resetting an individual bike path returns it to the active Bike scale. The Paths tab resets each scale to 100%.
- The citywide Paths sliders exclude hidden vehicle-only roads; edit supported ones individually. Legacy B+P changes to these roads may remain. If you prefer to restore all B+P values first, use Reset to Game Defaults with B+P alone and save before switching.
- Speed changes are saved with the city. Backups in `ModsData/AllSpeedLimits`; old `ModsData/SpeedLimits` backups are migrated.
- Whole-city actions are batched; path changes run on request or city load, even while paused, then go idle. In-city stats and speed overlays run only while the tool is open.

## Feedback and license

[Discord](https://discord.gg/gwXgvtyhjc) · Issues and PRs welcome.

GPL-3.0-or-later with the Cities: Skylines II Linking Exception. See [LICENSE](LICENSE), [LICENSE-EXCEPTION](LICENSE-EXCEPTION), and [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
