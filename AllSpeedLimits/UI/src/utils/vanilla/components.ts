// <copyright file="components.ts" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: UI/src/utils/vanilla/components.ts
// Purpose: Registry entries for vanilla CS2 React components.

export const VANILLA_COMPONENT_MODULES = {
    Panel: ["game-ui/common/panel/panel.tsx", "Panel"],
    Section: ["game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx", "Section"],
    ToolButton: ["game-ui/game/components/tool-options/tool-button/tool-button.tsx", "ToolButton"],
    StepToolButton: ["game-ui/game/components/tool-options/tool-button/tool-button.tsx", "StepToolButton"],
    Checkbox: ["game-ui/common/input/toggle/checkbox/checkbox.tsx", "Checkbox"],
    DescriptionTooltip: ["game-ui/common/tooltip/description-tooltip/description-tooltip.tsx", "DescriptionTooltip"],
    ColorField: ["game-ui/common/input/color-picker/color-field/color-field.tsx", "ColorField"],
    ColorPicker: ["game-ui/common/input/color-picker/color-picker/color-picker.tsx", "ColorPicker"],
    ColorPickerSliderMode: ["game-ui/common/input/color-picker/color-picker/color-picker.tsx", "ColorPickerSliderMode"],
    Slider: ["game-ui/common/input/slider/slider.tsx", "Slider"],
    InfoSection: ["game-ui/game/components/selected-info-panel/shared-components/info-section/info-section.tsx", "InfoSection"],
    InfoRow: ["game-ui/game/components/selected-info-panel/shared-components/info-row/info-row.tsx", "InfoRow"],
    InfoLink: ["game-ui/game/components/selected-info-panel/shared-components/info-link/info-link.tsx", "InfoLink"],
    PageSelector: ["game-ui/menu/components/whats-new-panel/page-selector/page-selector.tsx", "PageSelector"],
    PageSwitcher: ["game-ui/common/animations/paging/page-switcher.tsx", "PageSwitcher"],
    Page: ["game-ui/common/animations/paging/page-switcher.tsx", "Page"],
    Scrollable: ["game-ui/common/scrolling/scrollable.tsx", "Scrollable"],
    TextInput: ["game-ui/common/input/text/text-input.tsx", "TextInput"],
    DropdownItem: ["game-ui/common/input/dropdown/items/dropdown-item.tsx", "DropdownItem"],
} as const;

export type VanillaComponentModuleName = keyof typeof VANILLA_COMPONENT_MODULES;
