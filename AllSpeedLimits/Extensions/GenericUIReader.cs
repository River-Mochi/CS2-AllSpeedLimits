// <copyright file="GenericUIReader.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Extensions/GenericUIReader.cs
// Purpose: Turns values sent by the panel into the C# values used by tool actions and settings.
// Derived from MIT-licensed CS2 helper code; see THIRD_PARTY_NOTICES.md.


namespace RoadRailSpeeds.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Colossal.UI.Binding;
    using Unity.Entities;
    using UnityEngine;

    public class GenericUIReader<T> : IReader<T>
    {
        /// <inheritdoc/>
        public void Read(IJsonReader reader, out T value)
        {
            value = (T)ReadGeneric(reader, typeof(T));
        }

        private static object ReadGeneric(IJsonReader reader, Type type)
        {
            if (type.IsAssignableFrom(typeof(IJsonReadable)))
            {
                IJsonReadable value = (IJsonReadable)Activator.CreateInstance(type);

                value.Read(reader);

                return value;
            }

            if (type == typeof(int))
            {
                reader.Read(out int val);

                return val;
            }

            if (type == typeof(bool))
            {
                reader.Read(out bool val);

                return val;
            }

            if (type == typeof(uint))
            {
                reader.Read(out uint val);

                return val;
            }

            if (type == typeof(float))
            {
                reader.Read(out float val);

                return val;
            }

            if (type == typeof(double))
            {
                reader.Read(out double val);

                return val;
            }

            if (type == typeof(string))
            {
                reader.Read(out string val);

                return val;
            }

            if (type == typeof(Enum))
            {
                reader.Read(out int val);

                return val;
            }

            if (type == typeof(Entity))
            {
                reader.Read(out Entity val);

                return val;
            }

            if (type == typeof(Color))
            {
                reader.Read(out Color val);

                return val;
            }

            if (type.IsArray)
            {
                int length = (int)reader.ReadArrayBegin();
                Array array = (Array)Activator.CreateInstance(type, length);

                for (int i = 0; i < length; i++)
                {
                    reader.ReadArrayElement((ulong)i);

                    array.SetValue(ReadGeneric(reader, type.GetElementType()), i);
                }

                reader.ReadArrayEnd();

                return array;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                ulong length = reader.ReadArrayBegin();
                Type genericListType = typeof(List<>).MakeGenericType(type.GetElementType());
                IList genericList = (IList)Activator.CreateInstance(genericListType);

                for (ulong i = 0ul; i < length; i++)
                {
                    reader.ReadArrayElement(i);

                    genericList.Add(ReadGeneric(reader, type.GetElementType()));
                }

                reader.ReadArrayEnd();

                return genericList;
            }

            return ReadObject(reader, type);
        }

        private static object ReadObject(IJsonReader reader, Type type)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            object obj = Activator.CreateInstance(type);

            reader.ReadMapBegin();

            foreach (PropertyInfo? propertyInfo in properties)
            {
                if (reader.ReadProperty(propertyInfo.Name))
                {
                    propertyInfo.SetValue(obj, ReadGeneric(reader, propertyInfo.PropertyType));
                }
            }

            foreach (FieldInfo fieldInfo in fields)
            {
                if (reader.ReadProperty(fieldInfo.Name))
                {
                    fieldInfo.SetValue(obj, ReadGeneric(reader, fieldInfo.FieldType));
                }
            }

            reader.ReadMapEnd();

            return obj;
        }
    }
}
