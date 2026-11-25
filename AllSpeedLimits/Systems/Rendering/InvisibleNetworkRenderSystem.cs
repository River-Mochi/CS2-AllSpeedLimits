// <copyright file="InvisibleNetworkRenderSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Native rendering draws the dashed blue/green markers. Only selected and hovered
// speed-editable invisible edges need an ASL overlay: never scan the entire city.
namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;
    using Colossal.Mathematics;
    using Game;
    using Game.Net;
    using Game.Rendering;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public partial class InvisibleNetworkRenderSystem : GameSystemBase
    {
        private struct OverlayCurve
        {
            public Bezier4x3 Curve;
            public Color OutlineColor;
            public Color FillColor;
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
                    // Project wider, translucent dashes with a thin, brighter border.
                    // Keep vanilla blue/green marker lines visible underneath.
                    Buffer.DrawDashedCurve(
                        curve.OutlineColor,
                        curve.FillColor,
                        0.12f,
                        OverlayRenderSystem.StyleFlags.Projected,
                        curve.Curve,
                        curve.Width,
                        4.8f,
                        3f);
                }
            }
        }

        private SegmentSpeedToolSystem m_Tool = null!;
        private OverlayRenderSystem m_Overlay = null!;
        private RenderingSystem m_Rendering = null!;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            m_Tool = World.GetOrCreateSystemManaged<SegmentSpeedToolSystem>();
            m_Overlay = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
            m_Rendering = World.GetOrCreateSystemManaged<RenderingSystem>();
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (!m_Tool.IsActive || !m_Tool.ShowInvisible || m_Rendering.hideOverlay)
            {
                return;
            }

            IReadOnlyList<Entity> selected = m_Tool.SelectedEdges;
            IReadOnlyCollection<Entity> preview = m_Tool.PreviewEdges;
            if (!m_Tool.IsDraggingSelection && selected.Count == 0 &&
                m_Tool.HoverEntity == Entity.Null)
            {
                return;
            }

            NativeList<OverlayCurve> curves = new NativeList<OverlayCurve>(Allocator.TempJob);
            try
            {
                if (m_Tool.IsDraggingSelection)
                {
                    foreach (Entity edge in preview)
                    {
                        AddEdge(curves, edge);
                    }
                }
                else if (selected.Count > 0)
                {
                    foreach (Entity edge in selected)
                    {
                        AddEdge(curves, edge);
                    }
                }
                else
                {
                    AddEdge(curves, m_Tool.HoverEntity);
                }

                if (curves.Length == 0)
                {
                    curves.Dispose();
                    return;
                }

                OverlayRenderSystem.Buffer buffer = m_Overlay.GetBuffer(out JobHandle dependencies);
                JobHandle draw = new DrawJob
                {
                    Curves = curves.AsArray(),
                    Buffer = buffer
                }.Schedule(JobHandle.CombineDependencies(Dependency, dependencies));

                m_Overlay.AddBufferWriter(draw);
                Dependency = curves.Dispose(draw);
            }
            catch
            {
                if (curves.IsCreated)
                {
                    curves.Dispose();
                }

                throw;
            }
        }

        private void AddEdge(NativeList<OverlayCurve> curves, Entity edge)
        {
            if (!SpeedNetworkUtils.IsLiveEdge(EntityManager, edge) ||
                !EntityManager.HasComponent<Curve>(edge) ||
                !SpeedNetworkUtils.IsInvisible(EntityManager, edge) ||
                !m_Tool.IsEdgeTypeAllowed(edge))
            {
                return;
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

            bool edited = m_Tool.IsRecentlyEdited(edge);
            bool hovered = edge == m_Tool.HoverEntity;
            curves.Add(new OverlayCurve
            {
                Curve = curve,
                // Soft yellow distinguishes selection from vanilla blue/green markers;
                // a brief white flash confirms a successful apply or reset.
                // Retain the same thin outlines and translucent dash fills.
                OutlineColor = edited ? new Color(1f, 1f, 1f, 0.87f) :
                    hovered ? new Color(229f / 255f, 225f / 255f, 147f / 255f, 0.78f) :
                    new Color(221f / 255f, 224f / 255f, 126f / 255f, 0.90f),
                FillColor = edited ? new Color(1f, 1f, 1f, 0.28f) :
                    hovered ? new Color(229f / 255f, 225f / 255f, 147f / 255f, 0.19f) :
                    new Color(221f / 255f, 224f / 255f, 126f / 255f, 0.27f),
                Width = edited ? 1.30f : hovered ? 1.05f : 1.20f
            });
        }
    }
}
