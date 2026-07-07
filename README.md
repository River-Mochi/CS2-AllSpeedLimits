# Adjust All Speed Limits

Adjust Road/Rail Speeds is a Cities: Skylines II mod for changing speed limits on road, rail, and waterway segments.



## Features

- Select one segment or drag-select many segments.
- Adjust roads, tram/subway/train tracks, and waterways.
- Use the slider, number arrows, preset signs, or 50% buttons.
- Apply a new speed to selected segments.
- Reset selected segments to their game defaults.
- Apply a new speed to a whole road group: small, medium, large, or highway.
- Clear custom speeds by type: roads, rails, waterways, or all supported segments.
- Choose AUTO, KM/H, or MPH display.
- Show floating speed-limit signs while the tool is active.
- Show optional city vehicle stats while the panel is open.

## Notes

- New installs show the simpler speed scale by default, which is closer to vanilla road markings.
- The optional doubled-speed display is only a display scale. It helps players compare with tooltip mods or game data that show higher internal speed values.
- Painted road markings are art and may not exactly match the game's prefab speed data.
- Save the city after speed changes or resets if you want to keep them.

## Save Data

Speed changes are written to the loaded city. The mod also keeps a small per-city JSON backup so Reset can restore the original game defaults.

Local backup folder:

`ModsData/Mochi_RoadRailSpeeds`

If you remove the mod without clearing custom speeds, saved speed changes usually remain in the city, but this mod can no longer reset or reapply them.
For a clean uninstall, use **Clear All Custom Speeds**, save the city, then remove the mod.

## Performance

- Vehicle stats update only while the tool panel is open.
- Whole-city actions run in batches to avoid long freezes.
- Floating speed signs and selection overlays render only while the tool is active.

Source: https://github.com/River-Mochi/CS2-AllSpeedLimits
