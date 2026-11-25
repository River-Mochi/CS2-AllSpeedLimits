// <copyright file="SegmentSpeedToolUISystem.PathSpeeds.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

namespace RoadRailSpeeds.Systems
{
    using CS2Shared.RiverMochi;
    using Unity.Mathematics;

    public partial class SegmentSpeedToolUISystem
    {
        private void HandleOpenAllPaths()
        {
            if (m_SegmentSpeedTool.IsActive)
            {
                m_OpenAllPathsBinding.Value = true;
                RequestUpdate();
            }
        }

        private void HandleCloseAllPaths()
        {
            m_OpenAllPathsBinding.Value = false;
            RequestUpdate();
        }

        private void HandleApplyGlobalPathPercent(int percent)
        {
            m_Settings ??= Mod.Settings;
            if (m_Settings == null || m_GlobalPathSpeedSystem.IsApplying)
            {
                return;
            }

            int nextPercent = math.clamp(percent, SpeedLimitsSetting.kMinPathSpeedPercent, SpeedLimitsSetting.kMaxPathSpeedPercent);
            // Save the chosen value before queuing work, so a city reload reapplies it.
            if (m_Settings.GlobalPathSpeedPercent != nextPercent)
            {
                m_Settings.GlobalPathSpeedPercent = nextPercent;
                m_Settings.ApplyAndSave();
            }

            m_GlobalPathPercentBinding.Value = nextPercent;
            m_GlobalPathBusyBinding.Value = true;
            m_GlobalPathProcessedBinding.Value = 0;
            m_GlobalPathTotalBinding.Value = 0;
            m_GlobalPathSpeedSystem.ScheduleApply(
                nextPercent, m_Settings.GlobalPedPathSpeedPercent);
            LogUtils.Info(() => $"{Mod.ModTag} Global bike path speed requested: {nextPercent}%.");
            RequestUpdate();
        }
        private void HandleApplyGlobalPedPathPercent(int percent)
        {
            m_Settings ??= Mod.Settings;
            if (m_Settings == null || m_GlobalPathSpeedSystem.IsApplying)
            {
                return;
            }

            int nextPercent = math.clamp(percent, SpeedLimitsSetting.kMinPathSpeedPercent, SpeedLimitsSetting.kMaxPathSpeedPercent);
            if (m_Settings.GlobalPedPathSpeedPercent != nextPercent)
            {
                m_Settings.GlobalPedPathSpeedPercent = nextPercent;
                m_Settings.ApplyAndSave();
            }

            m_GlobalPedPathPercentBinding.Value = nextPercent;
            m_GlobalPathBusyBinding.Value = true;
            m_GlobalPathProcessedBinding.Value = 0;
            m_GlobalPathTotalBinding.Value = 0;
            m_GlobalPathSpeedSystem.ScheduleApply(
                m_Settings.GlobalPathSpeedPercent, nextPercent);
            LogUtils.Info(() =>
                $"{Mod.ModTag} Global pedestrian-only path speed requested: {nextPercent}%.");
            RequestUpdate();
        }
    }
}
