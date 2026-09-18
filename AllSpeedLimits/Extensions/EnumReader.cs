// <copyright file="EnumReader.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Extensions/EnumReader.cs
// Purpose: Lets a numeric choice sent by the panel become the matching C# option.
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
