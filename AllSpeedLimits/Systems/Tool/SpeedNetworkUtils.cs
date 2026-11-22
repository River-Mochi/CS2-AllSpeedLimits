// <copyright file="SpeedNetworkUtils.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

namespace RoadRailSpeeds.Systems
{
    using Game.Common;
    using Game.Net;
    using Game.Prefabs;
    using Game.Tools;
    using Unity.Entities;
    using SubLane = Game.Net.SubLane;

    internal enum SpeedNetworkKind
    {
        None,
        Road,
        Path,
        Rail,
        Water
    }

    internal static class SpeedNetworkUtils
    {
        public static bool IsLiveEdge(EntityManager manager, Entity edge)
        {
            return edge != Entity.Null && manager.Exists(edge) &&
                manager.HasComponent<Edge>(edge) &&
                !manager.HasComponent<Deleted>(edge) &&
                !manager.HasComponent<Temp>(edge) &&
                !manager.HasComponent<Overridden>(edge);
        }

        public static bool IsInvisible(EntityManager manager, Entity edge)
        {
            return manager.HasComponent<Marker>(edge) ||
                (manager.HasComponent<PrefabRef>(edge) &&
                manager.HasComponent<MarkerNetData>(manager.GetComponentData<PrefabRef>(edge).m_Prefab));
        }

        public static bool IsPathwayPrefab(EntityManager manager, Entity edge)
        {
            return manager.HasComponent<PrefabRef>(edge) &&
                manager.HasComponent<PathwayData>(manager.GetComponentData<PrefabRef>(edge).m_Prefab);
        }

        public static SpeedNetworkKind GetKind(EntityManager manager, Entity edge)
        {
            if (!IsLiveEdge(manager, edge))
            {
                return SpeedNetworkKind.None;
            }

            if (manager.HasComponent<Waterway>(edge))
            {
                return SpeedNetworkKind.Water;
            }

            if (manager.HasComponent<Road>(edge) || manager.HasComponent<TramTrack>(edge))
            {
                return SpeedNetworkKind.Road;
            }

            if (manager.HasComponent<TrainTrack>(edge) || manager.HasComponent<SubwayTrack>(edge))
            {
                return SpeedNetworkKind.Rail;
            }

            if (!IsPathwayPrefab(manager, edge))
            {
                return SpeedNetworkKind.None;
            }

            // Invisible vehicle roads can be PathwayPrefabs. Classify by their traffic lanes,
            // not by an asset name or the presence of the Road component alone.
            if (manager.HasBuffer<SubLane>(edge))
            {
                DynamicBuffer<SubLane> lanes = manager.GetBuffer<SubLane>(edge, true);
                for (int i = 0; i < lanes.Length; i++)
                {
                    Entity lane = lanes[i].m_SubLane;
                    if (!manager.HasComponent<Game.Net.CarLane>(lane) || !manager.HasComponent<PrefabRef>(lane))
                    {
                        continue;
                    }

                    Entity prefab = manager.GetComponentData<PrefabRef>(lane).m_Prefab;
                    if (manager.HasComponent<CarLaneData>(prefab) &&
                        (manager.GetComponentData<CarLaneData>(prefab).m_RoadTypes & RoadTypes.Car) != 0)
                    {
                        return SpeedNetworkKind.Road;
                    }
                }
            }

            return SpeedNetworkKind.Path;
        }

        public static bool HasSpeedLanes(EntityManager manager, Entity edge)
        {
            if (!manager.HasBuffer<SubLane>(edge))
            {
                return false;
            }

            DynamicBuffer<SubLane> lanes = manager.GetBuffer<SubLane>(edge, true);
            for (int i = 0; i < lanes.Length; i++)
            {
                Entity lane = lanes[i].m_SubLane;
                if (!manager.HasComponent<Deleted>(lane) &&
                    (manager.HasComponent<Game.Net.CarLane>(lane) || manager.HasComponent<Game.Net.TrackLane>(lane)))
                {
                    return true;
                }
            }

            return false;
        }

        // ASL stores nominal km/h. The optional doubled display is applied only by the UI.
        public static float GetPrefabSpeedKmh(PrefabBase prefab)
        {
            return prefab switch
            {
                RoadPrefab road => road.m_SpeedLimit / 2f,
                PathwayPrefab path => path.m_SpeedLimit / 2f,
                TrackPrefab track => track.m_SpeedLimit / 2f,
                WaterwayPrefab water => water.m_SpeedLimit / 2f,
                _ => -1f
            };
        }
    }
}
