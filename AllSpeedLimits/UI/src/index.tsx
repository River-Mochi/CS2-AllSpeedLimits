// File: UI/src/index.tsx
// Purpose: Registers the road/rail/waterway speed UI extensions with the CS2 module registry.

import { ModRegistrar, ModuleRegistry } from "cs2/modding";
import { VanillaComponentResolver } from "./utils/vanilla/VanillaComponentResolver";
import { SpeedToolHint } from "./entry/SpeedToolHint";
import { SpeedMarkerTooltipOverlay } from "./entry/SpeedMarkerTooltipOverlay";
import { SpeedToolToolbarButton } from "./entry/SpeedToolToolbarButton";
import { SpeedToolWindow } from "./panel/SpeedToolWindow";
import "./images/icon-speedlimit30-classic1.svg";
import "./images/icon-speedlimit30.svg";

const register: ModRegistrar = (moduleRegistry: ModuleRegistry) => {
    VanillaComponentResolver.setRegistry(moduleRegistry);

    // Append our hint message to the Game component (shows before any selection)
    moduleRegistry.append("Game", SpeedToolHint);

    // Append our custom window to the Game component (shows after selection)
    moduleRegistry.append("Game", SpeedToolWindow);

    // Always-mounted world marker tooltip layer. It must not depend on the selected-segment panel.
    moduleRegistry.append("Game", SpeedMarkerTooltipOverlay);

    // Add our toolbar button to the top-left game UI area
    moduleRegistry.append("GameTopLeft", SpeedToolToolbarButton);
};

export default register;
