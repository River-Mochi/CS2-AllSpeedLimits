// <copyright file="SegmentSpeedToolSystem.Apply.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolSystem.Apply.cs
// Purpose: Apply and reset segment speed values.

namespace RoadRailSpeeds.Systems
{
    using Game.Common;
    using Game.Net;
    using Game.Tools;
    using RoadRailSpeeds.Components;
    using RoadRailSpeeds.Data;
    using Unity.Entities;
    using CarLane = Game.Net.CarLane;
    using SubLane = Game.Net.SubLane;

    public partial class SegmentSpeedToolSystem
    {
        public void ApplySpeedToSelection(float speedKmh)
        {
            if (m_SelectedEdges.Count == 0)
            {
                return;
            }

            // Game lane speed uses 2x m/s; km/h converts by / 1.8.
            float speedGameUnits = speedKmh / 1.8f;
            bool storageChanged = false;

            foreach (Entity edge in m_SelectedEdges)
            {
                Entity targetEdge = edge;

                if (EntityManager.HasComponent<Temp>(edge))
                {
                    var temp = EntityManager.GetComponentData<Temp>(edge);
                    targetEdge = temp.m_Original;
                }

                SpeedLimitEntry? existingEntry = PersistentSpeedLimitStorage.GetSpeedLimit(targetEdge.Index);
                float originalSpeed = 0f;

                if (existingEntry == null)
                {
                    if (EntityManager.HasBuffer<SubLane>(edge))
                    {
                        DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);
                        CarLaneFlags ignore = CarLaneFlags.Unsafe | CarLaneFlags.SideConnection;

                        float totalSpeed = 0f;
                        int count = 0;

                        foreach (SubLane subLane in subLanes)
                        {
                            if (EntityManager.HasComponent<CarLane>(subLane.m_SubLane))
                            {
                                CarLane carLane = EntityManager.GetComponentData<CarLane>(subLane.m_SubLane);

                                if ((carLane.m_Flags & ignore) != 0)
                                {
                                    continue;
                                }

                                totalSpeed += carLane.m_SpeedLimit * 1.8f;
                                count++;
                            }
                            else if (EntityManager.HasComponent<Game.Net.TrackLane>(subLane.m_SubLane))
                            {
                                var trackLane = EntityManager.GetComponentData<Game.Net.TrackLane>(subLane.m_SubLane);

                                totalSpeed += trackLane.m_SpeedLimit * 1.8f;
                                count++;
                            }
                        }

                        if (count > 0)
                        {
                            originalSpeed = totalSpeed / count;
                        }
                    }
                }
                else
                {
                    originalSpeed = existingEntry.DefaultSpeedKmh;
                }

                // Store original and custom speed for reset.
                PersistentSpeedLimitStorage.StoreSpeedLimit(
                    targetEdge.Index,
                    originalSpeed,
                    speedKmh);
                storageChanged = true;

                if (!EntityManager.HasComponent<CustomSpeed>(targetEdge))
                {
                    EntityManager.AddComponent<CustomSpeed>(targetEdge);
                }

                EntityManager.SetComponentData(targetEdge, new CustomSpeed(speedKmh));

                SetCarLaneSpeedsImmediate(edge, speedGameUnits);
            }

            // Serializing the whole map once per selected edge can freeze the game for seconds
            // after a city has many saved limits. Persist the completed selection as one write.
            if (storageChanged)
            {
                PersistentSpeedLimitStorage.Save();
            }
        }

        private void SetCarLaneSpeedsImmediate(Entity edge, float speedGameUnits)
        {
            if (!EntityManager.HasBuffer<SubLane>(edge))
            {
                return;
            }

            DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);

            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity laneEntity = subLanes[i].m_SubLane;

                if (EntityManager.HasComponent<CarLane>(laneEntity))
                {
                    CarLane carLane = EntityManager.GetComponentData<CarLane>(laneEntity);

                    // Preserve lane flags.
                    CarLaneFlags originalFlags = carLane.m_Flags;

                    carLane.m_SpeedLimit = speedGameUnits;
                    carLane.m_Flags = originalFlags;

                    EntityManager.SetComponentData(laneEntity, carLane);
                }
                else if (EntityManager.HasComponent<Game.Net.TrackLane>(laneEntity))
                {
                    var trackLane = EntityManager.GetComponentData<Game.Net.TrackLane>(laneEntity);

                    trackLane.m_SpeedLimit = speedGameUnits;

                    EntityManager.SetComponentData(laneEntity, trackLane);
                }
            }
        }

        public void ResetSpeedToOriginal()
        {
            if (m_SelectedEdges.Count == 0)
            {
                return;
            }

            bool storageChanged = false;

            foreach (Entity edge in m_SelectedEdges)
            {
                Entity targetEdge = edge;

                if (EntityManager.HasComponent<Temp>(edge))
                {
                    var temp = EntityManager.GetComponentData<Temp>(edge);
                    targetEdge = temp.m_Original;
                }

                float? originalSpeed = PersistentSpeedLimitStorage.GetDefaultSpeedLimit(targetEdge.Index);

                if (!originalSpeed.HasValue)
                {
                    continue;
                }

                float speedGameUnits = originalSpeed.Value / 1.8f;

                if (EntityManager.HasBuffer<SubLane>(edge))
                {
                    DynamicBuffer<SubLane> subLanes = EntityManager.GetBuffer<SubLane>(edge);

                    foreach (SubLane subLane in subLanes)
                    {
                        if (EntityManager.HasComponent<CarLane>(subLane.m_SubLane))
                        {
                            CarLane carLane = EntityManager.GetComponentData<CarLane>(subLane.m_SubLane);

                            carLane.m_SpeedLimit = speedGameUnits;

                            EntityManager.SetComponentData(subLane.m_SubLane, carLane);
                        }
                        else if (EntityManager.HasComponent<Game.Net.TrackLane>(subLane.m_SubLane))
                        {
                            var trackLane = EntityManager.GetComponentData<Game.Net.TrackLane>(subLane.m_SubLane);

                            trackLane.m_SpeedLimit = speedGameUnits;

                            EntityManager.SetComponentData(subLane.m_SubLane, trackLane);
                        }
                    }
                }

                if (EntityManager.HasComponent<CustomSpeed>(targetEdge))
                {
                    EntityManager.RemoveComponent<CustomSpeed>(targetEdge);
                }

                PersistentSpeedLimitStorage.RemoveSpeedLimit(targetEdge.Index);
                storageChanged = true;
            }

            if (storageChanged)
            {
                PersistentSpeedLimitStorage.Save();
            }
        }
    }
}
