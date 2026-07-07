// File: UI/types/react.d.ts
// Purpose: COHTML-specific React attribute extensions.

import * as React from 'react';

declare module 'react' {
    interface HTMLAttributes<T> extends React.AriaAttributes, React.DOMAttributes<T> {
      // extends React's HTMLAttributes
      cohinline?: string;
    }
}
