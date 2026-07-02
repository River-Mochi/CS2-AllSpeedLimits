// <copyright file="ValueBindingHelper.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Extensions/ValueBindingHelper.cs
// Purpose: Wraps a ValueBinding so C# can update React/COHTML values with simple property syntax.
// Derived from MIT-licensed CS2 helper code; see THIRD_PARTY_NOTICES.md.

namespace RoadRailSpeeds.Extensions
{
    using System;
    using Colossal.UI.Binding;

    public sealed class ValueBindingHelper<T>
    {
        private readonly Action<T>? m_UpdateCallback;

        public ValueBindingHelper(ValueBinding<T> binding, Action<T>? updateCallback = null)
        {
            Binding = binding ?? throw new ArgumentNullException(nameof(binding));
            m_UpdateCallback = updateCallback;
        }

        public ValueBinding<T> Binding { get; }

        public T Value
        {
            get => Binding.value;
            set => Binding.Update(value);
        }

        // Called by TriggerBinding when React sends a new value back to C#.
        public void UpdateCallback(T value)
        {
            Binding.Update(value);
            m_UpdateCallback?.Invoke(value);
        }

        public static implicit operator T(ValueBindingHelper<T> helper)
        {
            return helper.Binding.value;
        }
    }
}
