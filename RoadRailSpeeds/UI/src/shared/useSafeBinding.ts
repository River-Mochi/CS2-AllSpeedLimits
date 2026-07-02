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
