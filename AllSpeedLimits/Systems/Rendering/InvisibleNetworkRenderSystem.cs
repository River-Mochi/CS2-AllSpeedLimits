// <copyright file="InvisibleNetworkRenderSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;
    using Colossal.Mathematics;
    using Game;
    using Game.Common;
    using Game.Net;
    using Game.Prefabs;
    using Game.Rendering;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Scripting;
    using SubLane = Game.Net.SubLane;

    // Tool-owned overlays avoid changing RenderingSystem.markersVisible, which other tools
    // and visibility mods also control. Nothing here modifies a network or its building owner.
    public partial class InvisibleNetworkRenderSystem : GameSystemBase
    {
        private struct OverlayCurve
        {
            public Bezier4x3 Curve;
            public Color Color;
            public float Width;
        }

        private struct DrawJob : IJob
        {
            [ReadOnly]
            public NativeArray<OverlayCurve> Curves;
            public OverlayRenderSystem.Buffer Buffer;

            public void Execute()
            {
                for (int i = 0; i < Curves.Length; i++)
                {
                    OverlayCurve curve = Curves[i];
                    Buffer.DrawCurve(curve.Color, curve.Curve, curve.Width);
                }
            }
        }

        private EntityQuery m_Edges;
        private SegmentSpeedToolSystem m_Tool = null!;
        private OverlayRenderSystem m_Overlay = null!;
        private RenderingSystem m_Rendering = null!;
        private CameraUpdateSystem m_Camera = null!;
        private readonly HashSet<Entity> m_Selected = new HashSet<Entity>();
        private readonly Plane[] m_Frustum = new Plane[6];

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            m_Tool = World.GetOrCreateSystemManaged<SegmentSpeedToolSystem>();
            m_Overlay = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
            m_Rendering = World.GetOrCreateSystemManaged<RenderingSystem>();
            m_Camera = World.GetOrCreateSystemManaged<CameraUpdateSystem>();
            m_Edges = SystemAPI.QueryBuilder()
                .WithAll<Edge, Curve, PrefabRef, SubLane>()
                .WithAny<Marker, Owner>()
                .WithNone<Deleted, Temp, Overridden>()
                .Build();
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (!m_Tool.IsActive || !m_Tool.ShowInvisible || m_Rendering.hideOverlay)
            {
                return;
            }

            Camera camera = m_Camera.activeCamera;
            if (camera == null)
            {
                return;
            }

            GeometryUtility.CalculateFrustumPlanes(camera, m_Frustum);
            m_Selected.Clear();
            IEnumerable<Entity> selection = m_Tool.IsDraggingSelection ? m_Tool.PreviewEdges : m_Tool.SelectedEdges;
            foreach (Entity edge in selection)
            {
                m_Selected.Add(edge);
            }

            using NativeArray<Entity> edges = m_Edges.ToEntityArray(Allocator.Temp);
            NativeList<OverlayCurve> curves = new NativeList<OverlayCurve>(Allocator.TempJob);
            for (int i = 0; i < edges.Length; i++)
            {
                Entity edge = edges[i];
                if (!SpeedNetworkUtils.IsInvisible(EntityManager, edge))
                {
                    continue;
                }

                Bezier4x3 curve = EntityManager.GetComponentData<Curve>(edge).m_Bezier;
                if (EntityManager.HasComponent<Elevation>(edge))
                {
                    float2 elevation = math.min(EntityManager.GetComponentData<Elevation>(edge).m_Elevation, 0f);
                    curve.a.y -= elevation.x;
                    curve.b.y -= math.lerp(elevation.x, elevation.y, 1f / 3f);
                    curve.c.y -= math.lerp(elevation.x, elevation.y, 2f / 3f);
                    curve.d.y -= elevation.y;
                }

                float3 min = math.min(math.min(curve.a, curve.b), math.min(curve.c, curve.d));
                float3 max = math.max(math.max(curve.a, curve.b), math.max(curve.c, curve.d));
                Bounds bounds = new Bounds((Vector3)((min + max) * 0.5f), (Vector3)(max - min + 4f));
                if (!GeometryUtility.TestPlanesAABB(m_Frustum, bounds) || !m_Tool.IsEdgeTypeAllowed(edge))
                {
                    continue;
                }

                bool selected = m_Selected.Contains(edge);
                bool hovered = edge == m_Tool.HoverEntity;
                bool path = SpeedNetworkUtils.GetKind(EntityManager, edge) == SpeedNetworkKind.Path;
                curves.Add(new OverlayCurve
                {
                    Curve = curve,
                    Color = selected ? new Color(1f, 0.82f, 0.2f, 0.95f) :
                        hovered ? new Color(1f, 1f, 1f, 0.95f) :
                        path ? new Color(0.4f, 0.95f, 0.65f, 0.7f) : new Color(0.35f, 0.8f, 1f, 0.7f),
                    Width = selected || hovered ? 1.2f : 0.6f
                });
            }

            if (curves.Length == 0)
            {
                curves.Dispose();
                return;
            }

            OverlayRenderSystem.Buffer buffer = m_Overlay.GetBuffer(out JobHandle dependencies);
            JobHandle draw = new DrawJob { Curves = curves.AsArray(), Buffer = buffer }
                .Schedule(JobHandle.CombineDependencies(Dependency, dependencies));
            m_Overlay.AddBufferWriter(draw);
            Dependency = curves.Dispose(draw);
        }
    }
}
