// <copyright file="ReflectionExtensions..cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Extensions/ReflectionExtensions..cs
// Purpose: Small reflection helpers used by the UI binding helper classes.

namespace RoadRailSpeeds.Extensions
{
    using System.Reflection;

    using CS2Shared.RiverMochi;

    public static class ReflectionExtensions
    {
        // Used when callers need a broad search across public/private instance/static members.
        public static readonly BindingFlags AllFlags =
            BindingFlags.DeclaredOnly |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.GetField |
            BindingFlags.GetProperty;

        public static object? GetMemberValue(this object obj, string memberName)
        {
            MemberInfo? memberInfo = GetMemberInfo(obj, memberName);
            if (memberInfo == null)
            {
                LogMissingMember(obj, memberName);
                return null;
            }

            return memberInfo switch
            {
                PropertyInfo propertyInfo => propertyInfo.GetValue(obj, null),
                FieldInfo fieldInfo => fieldInfo.GetValue(obj),
                _ => null
            };
        }

        public static object? SetMemberValue(this object obj, string memberName, object? newValue)
        {
            MemberInfo? memberInfo = GetMemberInfo(obj, memberName);
            if (memberInfo == null)
            {
                LogMissingMember(obj, memberName);
                return null;
            }

            object? oldValue = obj.GetMemberValue(memberName);

            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(obj, newValue, null);
                    break;

                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(obj, newValue);
                    break;
            }

            return oldValue;
        }

        private static MemberInfo? GetMemberInfo(object obj, string memberName)
        {
            // Property first because UI helper members are commonly exposed as properties.
            PropertyInfo? propertyInfo = obj.GetType().GetProperty(
                memberName,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.FlattenHierarchy);

            if (propertyInfo != null)
            {
                return propertyInfo;
            }

            // Fallback to fields for older helper classes and private state.
            return obj.GetType().GetField(
                memberName,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.FlattenHierarchy);
        }

        private static void LogMissingMember(object obj, string memberName)
        {
            string typeName = obj.GetType().FullName ?? obj.GetType().Name;
            LogUtils.WarnOnce(
                $"ReflectionExtensions.MissingMember.{typeName}.{memberName}",
                () => $"{Mod.ModTag} ReflectionExtensions could not find member '{memberName}' on type {typeName}.");
        }

        [System.Diagnostics.DebuggerHidden]
        private static T As<T>(this object obj)
        {
            return (T)obj;
        }
    }
}
