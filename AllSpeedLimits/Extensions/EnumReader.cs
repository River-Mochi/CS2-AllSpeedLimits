// <copyright file="EnumReader.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Extensions/EnumReader.cs
// Purpose:
// Derived from MIT-licensed CS2 helper code; see THIRD_PARTY_NOTICES.md.

namespace Platter.Extensions
{
    using Colossal.UI.Binding;

    public class EnumReader<T> : IReader<T>
    {
        /// <inheritdoc/>
        public void Read(IJsonReader reader, out T value)
        {
            reader.Read(out int value2);
            value = (T)(object)value2;
        }
    }
}
