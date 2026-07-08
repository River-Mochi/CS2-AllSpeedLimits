// <copyright file="CustomSpeed.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Components/CustomSpeed.cs
// Purpose: ECS component storing a segment's custom speed limit for saves and overlays.

namespace RoadRailSpeeds.Components
{
    using System;
    using Colossal.Serialization.Entities;
    using Unity.Entities;

    public struct CustomSpeed : IComponentData, IQueryTypeParameter, IEquatable<CustomSpeed>, ISerializable
    {
        private const float kMphPerKmh = 0.621371f;
        private const float kSpeedToleranceKmh = 0.01f;

        // Canonical value: km/h, since the UI, JSON backup, and lane apply logic all use km/h.
        public float m_Speed;

        // Cached mph for display/overlay consumers that need imperial text without recalculating.
        // Derived from m_Speed, never independent state, so equality/hashing below ignore it.
        public float m_SpeedMPH;

        public CustomSpeed(float speedKmh)
        {
            m_Speed = NormalizeSpeed(speedKmh);
            m_SpeedMPH = ToMph(m_Speed);
        }

        public readonly bool Equals(CustomSpeed other)
        {
            return Math.Abs(m_Speed - other.m_Speed) <= kSpeedToleranceKmh;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is CustomSpeed other && Equals(other);
        }

        // Quantized to the same tolerance Equals() uses, so two values Equals() treats as equal
        // (e.g. 100.000 and 100.005, within kSpeedToleranceKmh) hash identically. Hashing the raw
        // float directly would violate the .NET contract that Equals == true implies equal hash
        // codes, since near-identical floats can have completely different bit patterns.
        public override readonly int GetHashCode()
        {
            return Math.Round(m_Speed / kSpeedToleranceKmh).GetHashCode();
        }

        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(m_Speed);
            writer.Write(m_SpeedMPH);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out m_Speed);
            reader.Read(out m_SpeedMPH);

            // Keep old/corrupt save data sane if a bad value ever gets deserialized.
            m_Speed = NormalizeSpeed(m_Speed);
            m_SpeedMPH = m_SpeedMPH > 0f ? m_SpeedMPH : ToMph(m_Speed);
        }

        private static float NormalizeSpeed(float speedKmh)
        {
            if (float.IsNaN(speedKmh) || float.IsInfinity(speedKmh))
            {
                return 0f;
            }

            return Math.Max(0f, speedKmh);
        }

        private static float ToMph(float speedKmh)
        {
            return speedKmh * kMphPerKmh;
        }
    }
}
