// <copyright file="SegmentSpeedToolUISystem.MarkerTooltip.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Tool/SegmentSpeedToolUISystem.MarkerTooltip.cs
// Purpose: Marker tooltip and selection-click binding helpers for the speed tool panel.

namespace RoadRailSpeeds.Systems
{
    public partial class SegmentSpeedToolUISystem
    {
        private void UpdateMarkerTooltipBindings(bool toolActive)
        {
            if (!toolActive ||
                !m_SpeedLimitMarkerRenderSystem.TryGetMarkerTooltip(out string text, out float x, out float y))
            {
                ClearMarkerTooltipBindings();
                return;
            }

            if (m_MarkerTooltipTextBinding.Value == text &&
                System.Math.Abs(m_MarkerTooltipXBinding.Value - x) < 1f &&
                System.Math.Abs(m_MarkerTooltipYBinding.Value - y) < 1f)
            {
                return;
            }

            m_MarkerTooltipTextBinding.Value = text;
            m_MarkerTooltipXBinding.Value = x;
            m_MarkerTooltipYBinding.Value = y;
            RequestUpdate();
        }

        private void ClearMarkerTooltipBindings()
        {
            if (string.IsNullOrEmpty(m_MarkerTooltipTextBinding.Value) &&
                m_MarkerTooltipXBinding.Value == 0f &&
                m_MarkerTooltipYBinding.Value == 0f)
            {
                return;
            }

            m_MarkerTooltipTextBinding.Value = string.Empty;
            m_MarkerTooltipXBinding.Value = 0f;
            m_MarkerTooltipYBinding.Value = 0f;
            RequestUpdate();
        }

        private void ClearSelectionClickBindings()
        {
            m_SelectionClickXBinding.Value = 0f;
            m_SelectionClickYBinding.Value = 0f;
        }
    }
}
