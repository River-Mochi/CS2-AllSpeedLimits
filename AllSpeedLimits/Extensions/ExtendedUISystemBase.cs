// <copyright file="ExtendedUISystemBase.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Extensions/ExtendedUISystemBase.cs
// Purpose: Keeps panel reads, button clicks, and setting changes connected to C# in one consistent way.
// Derived from MIT-licensed CS2 helper code; see THIRD_PARTY_NOTICES.md.

namespace RoadRailSpeeds.Extensions
{
    using System;
    using Colossal.UI.Binding;
    using Game.UI;

    public abstract partial class ExtendedUISystemBase : UISystemBase
    {
        private const string kBindingPrefix = "BINDING:";
        private const string kTriggerPrefix = "TRIGGER:";

        public ValueBindingHelper<T> CreateBinding<T>(string key, T initialValue)
        {
            ValueBindingHelper<T> helper = new ValueBindingHelper<T>(
                new ValueBinding<T>(
                    Mod.ModId,
                    $"{kBindingPrefix}{key}",
                    initialValue,
                    new GenericUIWriter<T>()));

            AddBinding(helper.Binding);

            return helper;
        }

        public ValueBindingHelper<T> CreateBinding<T>(
            string key,
            T initialValue,
            Action<T>? updateCallback = null)
        {
            ValueBindingHelper<T> helper = new ValueBindingHelper<T>(
                new ValueBinding<T>(
                    Mod.ModId,
                    $"{kBindingPrefix}{key}",
                    initialValue,
                    new GenericUIWriter<T>()),
                updateCallback);

            TriggerBinding<T> trigger = new TriggerBinding<T>(
                Mod.ModId,
                $"{kTriggerPrefix}{key}",
                helper.UpdateCallback,
                new GenericUIReader<T>());

            AddBinding(helper.Binding);
            AddBinding(trigger);

            return helper;
        }

        public ValueBindingHelper<T> CreateBinding<T>(
            string key,
            string setterKey,
            T initialValue,
            Action<T>? updateCallback = null)
        {
            ValueBindingHelper<T> helper = new ValueBindingHelper<T>(
                new ValueBinding<T>(
                    Mod.ModId,
                    $"{kBindingPrefix}{key}",
                    initialValue,
                    new GenericUIWriter<T>()),
                updateCallback);

            // Use a separate write name when the panel should update a value without sharing its read name.
            TriggerBinding<T> trigger = new(
                Mod.ModId,
                $"{kTriggerPrefix}{setterKey}",
                helper.UpdateCallback,
                new GenericUIReader<T>());

            AddBinding(helper.Binding);
            AddBinding(trigger);

            return helper;
        }

        public GetterValueBinding<T> CreateBinding<T>(string key, Func<T> getterFunc)
        {
            GetterValueBinding<T> binding = new GetterValueBinding<T>(
                Mod.ModId,
                key,
                getterFunc,
                new GenericUIWriter<T>());

            AddBinding(binding);

            return binding;
        }

        public TriggerBinding CreateTrigger(string key, Action action)
        {
            TriggerBinding binding = new TriggerBinding(
                Mod.ModId,
                $"{kTriggerPrefix}{key}",
                action);

            AddBinding(binding);

            return binding;
        }

        public TriggerBinding<T1> CreateTrigger<T1>(string key, Action<T1> action)
        {
            TriggerBinding<T1> binding = new TriggerBinding<T1>(
                Mod.ModId,
                $"{kTriggerPrefix}{key}",
                action,
                new GenericUIReader<T1>());

            AddBinding(binding);

            return binding;
        }

        public TriggerBinding<T1, T2> CreateTrigger<T1, T2>(string key, Action<T1, T2> action)
        {
            TriggerBinding<T1, T2> binding = new TriggerBinding<T1, T2>(
                Mod.ModId,
                $"{kTriggerPrefix}{key}",
                action,
                new GenericUIReader<T1>(),
                new GenericUIReader<T2>());

            AddBinding(binding);

            return binding;
        }

        public TriggerBinding<T1, T2, T3> CreateTrigger<T1, T2, T3>(
            string key,
            Action<T1, T2, T3> action)
        {
            TriggerBinding<T1, T2, T3> binding = new TriggerBinding<T1, T2, T3>(
                Mod.ModId,
                $"{kTriggerPrefix}{key}",
                action,
                new GenericUIReader<T1>(),
                new GenericUIReader<T2>(),
                new GenericUIReader<T3>());

            AddBinding(binding);

            return binding;
        }

        public TriggerBinding<T1, T2, T3, T4> CreateTrigger<T1, T2, T3, T4>(
            string key,
            Action<T1, T2, T3, T4> action)
        {
            TriggerBinding<T1, T2, T3, T4> binding = new TriggerBinding<T1, T2, T3, T4>(
                Mod.ModId,
                $"{kTriggerPrefix}{key}",
                action,
                new GenericUIReader<T1>(),
                new GenericUIReader<T2>(),
                new GenericUIReader<T3>(),
                new GenericUIReader<T4>());

            AddBinding(binding);

            return binding;
        }
    }
}
