// <copyright file="SegmentSelectionOverlayRenderSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Rendering/SegmentSelectionOverlayRenderSystem.cs
// Purpose: Draws waterway-only selection overlays; road/rail use the game's Highlighted outline.

namespace RoadRailSpeeds.Systems
{
    using System.Collections.Generic;       // IReadOnlyCollection, IReadOnlyList
    using Colossal.Mathematics;             // Bezier4x3
    using Game;                             // GameSystemBase
    using Game.Net;                         // Curve, Edge, Waterway
    using Game.Rendering;                   // OverlayRenderSystem, RenderingSystem
    using Unity.Collections;                // Allocator, NativeArray, NativeList
    using Unity.Entities;                   // Entity
    using Unity.Jobs;                       // IJob, JobHandle
    using UnityEngine;                      // Color
    using UnityEngine.Scripting;            // Preserve

    /// <summary>
    /// Adds visible RRS-owned overlays for waterways.
    /// IMPORTANT: this system is waterway-only.
    /// Roads and rails stay on the game Highlighted path because it draws the
    /// outer edge outline without the overlapping curve caps produced by DrawCurve.
    /// </summary>
    [Preserve]
    public partial class SegmentSelectionOverlayRenderSystem : GameSystemBase
    {
        private struct SelectionOverlayCurve
        {
            public Bezier4x3 Curve;
        }

        private struct SelectionOverlayRenderJob : IJob
        {
            public NativeArray<SelectionOverlayCurve> Curves;
            public OverlayRenderSystem.Buffer OverlayBuffer;

            public void Execute()
            {
                for (int index = 0; index < Curves.Length; index++)
                {
                    SelectionOverlayCurve curve = Curves[index];
                    float wideBodyWidth = s_WaterVisualWidth;
                    float centerBodyWidth = wideBodyWidth * 0.42f;
                    float wideOutlineWidth = 1.4f;
                    float dashWidth = 1.2f;
                    float dashLength = 14.0f;
                    float gapLength = 9.0f;
                    OverlayBuffer.DrawCurve(s_WaterNodeEdge, s_WaterSideFill, wideOutlineWidth, (OverlayRenderSystem.StyleFlags)0, curve.Curve, wideBodyWidth);
                    OverlayBuffer.DrawCurve(s_WaterCenterFill, curve.Curve, centerBodyWidth);
                    OverlayBuffer.DrawDashedCurve(s_WhiteDash, curve.Curve, dashWidth, dashLength, gapLength);
                }
            }
        }

        // Waterway selection knobs. Road/rail outlines are not drawn here on purpose.
        // Do not add road/rail DrawCurve rendering here; already tested + rejected for segment joins.
        // This intentionally restores high-visibility test look: magenta sides, cyan center,
        // white lane dash, and yellow node caps/outer edge so waterway segment joins are obvious.
        private static readonly Color s_WaterNodeEdge = new Color(1.00f, 0.90f, 0.12f, 0.78f);
        private static readonly Color s_WaterSideFill = new Color(0.68f, 0.10f, 0.78f, 0.58f);
        private static readonly Color s_WaterCenterFill = new Color(0.42f, 1.00f, 0.88f, 0.72f);
        private static readonly Color s_WhiteDash = new Color(1.00f, 1.00f, 1.00f, 0.86f);
        // Fixed visual width on purpose: prefab waterway widths are huge and make the overlay cover
        // the full seaway surface. This keeps the test-look lane narrow and readable.
        private const float s_WaterVisualWidth = 13.0f;

        private OverlayRenderSystem m_OverlayRenderSystem = null!;
        private RenderingSystem m_RenderingSystem = null!;
        private SegmentSpeedToolSystem m_SegmentSpeedToolSystem = null!;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();

            m_OverlayRenderSystem = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
            m_RenderingSystem = World.GetOrCreateSystemManaged<RenderingSystem>();
            m_SegmentSpeedToolSystem = World.GetOrCreateSystemManaged<SegmentSpeedToolSystem>();
        }

        [Preserve]
        protected override void OnUpdate()
        {
            if (!m_SegmentSpeedToolSystem.IsActive || m_RenderingSystem.hideOverlay)
            {
                return;
            }

            IReadOnlyCollection<Entity> previewEdges = m_SegmentSpeedToolSystem.PreviewEdges;
            IReadOnlyList<Entity> selectedEdges = m_SegmentSpeedToolSystem.SelectedEdges;
            Entity hoverEntity = m_SegmentSpeedToolSystem.HoverEntity;

            if (!m_SegmentSpeedToolSystem.IsDraggingSelection &&
                selectedEdges.Count == 0 &&
                hoverEntity == Entity.Null)
            {
                return;
            }

            NativeList<SelectionOverlayCurve> curves = new NativeList<SelectionOverlayCurve>(Allocator.TempJob);

            try
            {
                if (m_SegmentSpeedToolSystem.IsDraggingSelection)
                {
                    foreach (Entity edge in previewEdges)
                    {
                        AddEdge(curves, edge);
                    }
                }
                else if (selectedEdges.Count > 0)
                {
                    foreach (Entity edge in selectedEdges)
                    {
                        AddEdge(curves, edge);
                    }
                }
                else
                {
                    AddEdge(curves, hoverEntity);
                }

                if (curves.Length == 0)
                {
                    curves.Dispose();
                    return;
                }

                JobHandle dependencies;
                OverlayRenderSystem.Buffer buffer = m_OverlayRenderSystem.GetBuffer(out dependencies);
                JobHandle jobHandle = new SelectionOverlayRenderJob
                {
                    Curves = curves.AsArray(),
                    OverlayBuffer = buffer
                }.Schedule(JobHandle.CombineDependencies(Dependency, dependencies));

                JobHandle disposeHandle = curves.Dispose(jobHandle);
                m_OverlayRenderSystem.AddBufferWriter(jobHandle);
                Dependency = disposeHandle;
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

        private void AddEdge(NativeList<SelectionOverlayCurve> curves, Entity edge)
        {
            // Only Waterway edges are allowed through. Road/rail selection is handled by Highlighted.
            if (edge == Entity.Null ||
                !EntityManager.Exists(edge) ||
                !EntityManager.HasComponent<Edge>(edge) ||
                !EntityManager.HasComponent<Curve>(edge) ||
                !EntityManager.HasComponent<Waterway>(edge))
            {
                return;
            }

            Curve curve = EntityManager.GetComponentData<Curve>(edge);
            curves.Add(new SelectionOverlayCurve
            {
                Curve = curve.m_Bezier
            });
        }
    }
}
