// <copyright file="CustomSpeed.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Components/CustomSpeed.cs
// Purpose: Stores a player-set speed on a network segment so it survives saves and appears on markers.

namespace RoadRailSpeeds.Components
{
    using System;
    using Colossal.Serialization.Entities;
    using Unity.Entities;

    public struct CustomSpeed : IComponentData, IQueryTypeParameter, IEquatable<CustomSpeed>, ISerializable
    {
        private const float kMphPerKmh = 0.621371f;
        private const float kSpeedToleranceKmh = 0.01f;

        // Store one main speed in km/h because the UI, backups, and network updates all use it.
        public float m_Speed;

        // Keep the matching mph value ready for imperial labels. It comes from m_Speed and is not
        // a separate player setting, so comparisons use only the main km/h value.
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

        // Keep tiny calc. differences from making basically the same speed look different
        // when CustomSpeed values are compared or grouped. Rounds only the comparison value;
        // does not change the speed saved in the city.
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

            // Prevent an older or damaged save from loading an invalid speed.
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
