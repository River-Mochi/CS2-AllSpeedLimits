// <copyright file="useSafeBinding.ts" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: UI/src/shared/useSafeBinding.ts
// Purpose: Reads CS2 UI bindings without letting startup timing issues break the panel.

import { useValue } from "cs2/api";

export const useSafeBinding = <T,>(binding: any, fallbackValue: T): T => {
    try {
        const value = useValue<T>(binding);
        return value ?? fallbackValue;
    } catch (e) {
        return fallbackValue;
    }
};
