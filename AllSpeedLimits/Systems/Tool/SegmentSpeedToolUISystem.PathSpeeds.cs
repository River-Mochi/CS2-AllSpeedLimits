// <copyright file="SegmentSpeedToolUISystem.PathSpeeds.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under GPL-3.0-or-later with the CS2 Linking Exception.
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

            int nextPercent = math.clamp(percent, 100, 500);
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
            m_GlobalPathSpeedSystem.ScheduleApply(nextPercent);
            LogUtils.Info(() => $"{Mod.ModTag} Global pathway speed requested: {nextPercent}%.");
            RequestUpdate();
        }
    }
}
