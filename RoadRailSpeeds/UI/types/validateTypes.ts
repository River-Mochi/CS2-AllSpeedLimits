// File: UI/types/validateTypes.ts
// Purpose: Compile-time check that the UI default export matches CS2 mod registration.

import { ModRegistrar } from "cs2/modding";

async function validateExportTypes() {
    // only default export is processed by the UI, any named exports will be ignored.
    let isIndexFileValid: { 'default': ModRegistrar } = await import('../src/index');
    return isIndexFileValid;
}
