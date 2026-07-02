// File: UI/src/utils/vanilla/focus.ts
// Purpose: Registry entries for vanilla CS2 focus helpers.

export const VANILLA_FOCUS_MODULES = {
    FOCUS_DISABLED: ["game-ui/common/focus/focus-key.ts", "FOCUS_DISABLED"],
    FOCUS_AUTO: ["game-ui/common/focus/focus-key.ts", "FOCUS_AUTO"],
    useUniqueFocusKey: ["game-ui/common/focus/focus-key.ts", "useUniqueFocusKey"],
} as const;

export type VanillaFocusModuleName = keyof typeof VANILLA_FOCUS_MODULES;
