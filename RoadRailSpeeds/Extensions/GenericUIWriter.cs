// <copyright file="GenericUIWriter.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Extensions/GenericUIWriter.cs
// Purpose: Converts C# values into JSON values that React/COHTML bindings can read.
// Derived from MIT-licensed CS2 UI binding helper code; see THIRD_PARTY_NOTICES.md.

namespace RoadRailSpeeds.Extensions
{
    using System; // Type, Enum, Convert, and Array handling.
    using System.Collections; // Non-generic IEnumerable used by UI binding values.
    using System.Collections.Generic; // Temporary list for enumerable JSON output.
    using System.Reflection; // Public fields/properties for object fallback writing.
    using Colossal.UI.Binding; // IWriter<T>, IJsonWriter, and IJsonWritable for COHTML bindings.
    using Unity.Entities; // Entity JSON support.
    using UnityEngine; // Color JSON support.

    public sealed class GenericUIWriter<T> : IWriter<T>
    {
        public void Write(IJsonWriter writer, T value)
        {
            WriteGeneric(writer, value);
        }

        private static void WriteGeneric(IJsonWriter writer, object? obj)
        {
            if (obj == null)
            {
                writer.WriteNull();
                return;
            }

            if (obj is IJsonWritable jsonWritable)
            {
                jsonWritable.Write(writer);
                return;
            }

            if (obj is int intValue)
            {
                writer.Write(intValue);
                return;
            }

            if (obj is bool boolValue)
            {
                writer.Write(boolValue);
                return;
            }

            if (obj is uint uintValue)
            {
                writer.Write(uintValue);
                return;
            }

            if (obj is float floatValue)
            {
                writer.Write(floatValue);
                return;
            }

            if (obj is double doubleValue)
            {
                writer.Write(doubleValue);
                return;
            }

            if (obj is string stringValue)
            {
                writer.Write(stringValue);
                return;
            }

            if (obj is Enum enumValue)
            {
                // Enums are sent to the UI as their numeric value.
                writer.Write(Convert.ToInt32(enumValue));
                return;
            }

            if (obj is Entity entity)
            {
                writer.Write(entity);
                return;
            }

            if (obj is Color color)
            {
                writer.Write(color);
                return;
            }

            if (obj is Array array)
            {
                WriteArray(writer, array);
                return;
            }

            if (obj is IEnumerable enumerable)
            {
                WriteEnumerable(writer, enumerable);
                return;
            }

            WriteObject(writer, obj.GetType(), obj);
        }

        private static void WriteObject(IJsonWriter writer, Type type, object obj)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            writer.TypeBegin(type.FullName ?? type.Name);

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                writer.PropertyName(propertyInfo.Name);
                WriteGeneric(writer, propertyInfo.GetValue(obj));
            }

            foreach (FieldInfo fieldInfo in fields)
            {
                writer.PropertyName(fieldInfo.Name);
                WriteGeneric(writer, fieldInfo.GetValue(obj));
            }

            writer.TypeEnd();
        }

        private static void WriteArray(IJsonWriter writer, Array array)
        {
            writer.ArrayBegin(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                WriteGeneric(writer, array.GetValue(i));
            }

            writer.ArrayEnd();
        }

        private static void WriteEnumerable(IJsonWriter writer, IEnumerable enumerable)
        {
            List<object?> items = new List<object?>();

            foreach (object? item in enumerable)
            {
                items.Add(item);
            }

            writer.ArrayBegin(items.Count);

            foreach (object? item in items)
            {
                WriteGeneric(writer, item);
            }

            writer.ArrayEnd();
        }
    }
}
