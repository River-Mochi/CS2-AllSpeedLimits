// <copyright file="ReflectionExtensions.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Extensions/ReflectionExtensions.cs
// Purpose: Finds UI helper values even when different helper versions expose them differently.

namespace RoadRailSpeeds.Extensions
{
    using System.Reflection;

    using CS2Shared.RiverMochi;

    public static class ReflectionExtensions
    {
        // Search every member shape because helper versions differ in visibility and placement.
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
            // Current helper versions normally expose the value as a property.
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

            // Older helpers may expose the same value as a field, including private state.
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
