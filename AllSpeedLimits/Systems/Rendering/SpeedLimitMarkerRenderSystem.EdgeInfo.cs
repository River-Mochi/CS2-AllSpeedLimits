// <copyright file="SpeedLimitMarkerRenderSystem.EdgeInfo.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Rendering/SpeedLimitMarkerRenderSystem.EdgeInfo.cs
// Purpose: Marker identity, network-kind detection, and vanilla speed lookup.

namespace RoadRailSpeeds.Systems
{
    using System;
    using System.Collections.Generic;
    using Colossal.Mathematics;
    using CS2Shared.RiverMochi;
    using Game;
    using Game.City;
    using Game.Input;
    using Game.Net;
    using Game.Prefabs;
    using Game.Rendering;
    using Game.UI;
    using RoadRailSpeeds.Components;
    using TMPro;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    public partial class SpeedLimitMarkerRenderSystem
    {
        private bool IsWaterwayEdge(Entity edge)
        {
            return EntityManager.HasComponent<Waterway>(edge);
        }

        private bool TryGetMarkerRenderIdentity(Entity edge, out MarkerRenderIdentity identity)
        {
            identity = default;

            if (!EntityManager.HasComponent<CustomSpeed>(edge))
            {
                return false;
            }

            CustomSpeed customSpeed = EntityManager.GetComponentData<CustomSpeed>(edge);
            int speedKmh = Mathf.RoundToInt(customSpeed.m_Speed);
            bool isWaterwayType = IsWaterwayEdge(edge);
            bool isDefaultSpeed = IsDefaultSpeed(edge, customSpeed.m_Speed);
            MarkerVisualKind visualKind = GetMarkerVisualKind(edge, isDefaultSpeed);
            MarkerNetworkKind networkKind = GetMarkerNetworkKind(edge);

            identity = new MarkerRenderIdentity(
                customSpeed.m_Speed,
                isWaterwayType,
                new MarkerGroupKey(speedKmh, visualKind, networkKind));

            return true;
        }

        private MarkerNetworkKind GetMarkerNetworkKind(Entity edge)
        {
            if (IsWaterwayEdge(edge))
            {
                return MarkerNetworkKind.Water;
            }

            return IsTrainOrSubwayEdge(edge)
                ? MarkerNetworkKind.Rail
                : MarkerNetworkKind.Road;
        }

        private MarkerVisualKind GetMarkerVisualKind(Entity edge, bool isDefaultSpeed)
        {
            if (isDefaultSpeed)
            {
                return MarkerVisualKind.Default;
            }

            return IsTrainOrSubwayEdge(edge)
                ? MarkerVisualKind.Rail
                : MarkerVisualKind.Custom;
        }

        private bool IsTrainOrSubwayEdge(Entity edge)
        {
            if (EntityManager.HasComponent<TramTrack>(edge))
            {
                return false;
            }

            return EntityManager.HasComponent<TrainTrack>(edge) ||
                EntityManager.HasComponent<SubwayTrack>(edge);
        }


        private bool IsDefaultSpeed(Entity edge, float speedKmh)
        {
            float vanillaSpeed = GetVanillaSpeed(edge);
            return vanillaSpeed > 0f && Mathf.Abs(vanillaSpeed - speedKmh) <= 0.5f;
        }

        private float GetVanillaSpeed(Entity edge)
        {
            if (!EntityManager.HasComponent<PrefabRef>(edge))
            {
                return -1f;
            }

            PrefabRef prefabRef = EntityManager.GetComponentData<PrefabRef>(edge);
            if (!m_PrefabSystem.TryGetPrefab(prefabRef, out PrefabBase prefabBase) || prefabBase == null)
            {
                return -1f;
            }

            return prefabBase switch
            {
                RoadPrefab roadPrefab => roadPrefab.m_SpeedLimit / 2f,
                TrackPrefab trackPrefab => trackPrefab.m_SpeedLimit / 2f,
                WaterwayPrefab waterwayPrefab => waterwayPrefab.m_SpeedLimit / 2f,
                _ => -1f
            };
        }
    }
}
