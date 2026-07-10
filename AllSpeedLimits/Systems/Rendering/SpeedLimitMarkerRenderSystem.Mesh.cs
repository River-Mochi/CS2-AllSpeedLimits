// <copyright file="SpeedLimitMarkerRenderSystem.Mesh.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Rendering/SpeedLimitMarkerRenderSystem.Mesh.cs
// Purpose: TextMeshPro marker mesh creation, cache keys, map theme, and cache cleanup.

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
        private TextMeshInfo CreateTextMesh(int speedKmh, MarkerVisualKind visualKind)
        {
            // The game shares ONE TextMeshPro instance (OverlayRenderSystem.GetTextMesh) across
            // every world-UI overlay label, including vanilla road and district name labels.
            // Anything we leave mutated on it leaks: the game later regenerates those labels
            // through our state and caches them with it, so a custom-speed cyan face color turns
            // every road name cyan and it stays cyan until the game is reloaded. We snapshot the
            // shared instance up front and restore it in the finally below so we leave no trace.
            TextMeshPro textMesh = m_OverlayRenderSystem.GetTextMesh();

            Vector2 prevSizeDelta = textMesh.rectTransform.sizeDelta;
            float prevFontSize = textMesh.fontSize;
            TextAlignmentOptions prevAlignment = textMesh.alignment;
            Color prevColor = textMesh.color;
            float prevCharacterSpacing = textMesh.characterSpacing;
            FontStyles prevFontStyle = textMesh.fontStyle;
            string prevText = textMesh.text;

            try
            {
                bool isEUMap = IsMapEuropean();
                bool showMetric = m_Settings?.ShouldShowMetric(isEUMap) ?? isEUMap;
                bool doubleDisplay = m_Settings?.DoubleSpeedDisplay ?? false;
                int multiplier = doubleDisplay ? 2 : 1;
                Color textColor = GetMarkerTextColor(visualKind);

                textMesh.rectTransform.sizeDelta = new Vector2(176f, 92f);
                // Base font size for floating speed number before zoom scaling is applied above.
                textMesh.fontSize = 31f;
                textMesh.alignment = TextAlignmentOptions.Center;
                textMesh.color = textColor;
                textMesh.characterSpacing = 0f;
                textMesh.fontStyle = FontStyles.Bold;

                string speedText;
                if (showMetric)
                {
                    speedText = (speedKmh * multiplier).ToString();
                }
                else
                {
                    int speedMph = Mathf.RoundToInt(speedKmh * 0.621371f);
                    speedText = (speedMph * multiplier).ToString();
                }

                textMesh.text = speedText;
                textMesh.ForceMeshUpdate(ignoreActiveState: true, forceTextReparsing: true);

                TMP_TextInfo textInfo = textMesh.textInfo;
                if (textInfo.meshInfo.Length == 0)
                {
                    return default;
                }

                TMP_MeshInfo tmpMeshInfo = textInfo.meshInfo[0];
                if (tmpMeshInfo.vertexCount == 0)
                {
                    return default;
                }

                string unitSuffix = showMetric ? "kmh" : "mph";
                string doubleSuffix = doubleDisplay ? "_2x" : string.Empty;
                string styleSuffix = GetMarkerVisualKindSuffix(visualKind);

                int vertexCount = tmpMeshInfo.vertexCount;
                int triangleCount = (vertexCount >> 2) * 6;
                Vector3[] vertices = new Vector3[vertexCount];
                Vector2[] uvs0 = new Vector2[vertexCount];
                Vector2[] uvs2 = new Vector2[vertexCount];
                Color32[] colors = new Color32[vertexCount];
                int[] triangles = new int[triangleCount];

                Array.Copy(tmpMeshInfo.vertices, 0, vertices, 0, vertexCount);
                Array.Copy(tmpMeshInfo.uvs0, 0, uvs0, 0, vertexCount);
                Array.Copy(tmpMeshInfo.uvs2, 0, uvs2, 0, vertexCount);
                Array.Copy(tmpMeshInfo.colors32, 0, colors, 0, vertexCount);
                Array.Copy(tmpMeshInfo.triangles, 0, triangles, 0, triangleCount);

                Mesh mesh = new Mesh
                {
                    name = $"SpeedLimit_{styleSuffix}_{speedKmh}_{unitSuffix}{doubleSuffix}"
                };
                if (vertexCount > 65535)
                {
                    mesh.indexFormat = IndexFormat.UInt32;
                }

                mesh.vertices = vertices;
                mesh.uv = uvs0;
                mesh.uv2 = uvs2;
                mesh.colors32 = colors;
                mesh.triangles = triangles;

                mesh.RecalculateBounds();

                Material material;
                if (m_PrefabSystem.TryGetSingletonPrefab(
                    m_OverlaySettingsQuery,
                    out OverlayConfigurationPrefab overlayConfiguration) &&
                    overlayConfiguration.m_TextMaterial != null)
                {
                    material = new Material(overlayConfiguration.m_TextMaterial);
                }
                else
                {
                    material = new Material(tmpMeshInfo.material);
                }

                material.name = $"SpeedLimitMaterial_{styleSuffix}_{speedKmh}_{unitSuffix}{doubleSuffix}";

                m_OverlayRenderSystem.CopyFontAtlasParameters(tmpMeshInfo.material, material);
                material.SetColor(m_FaceColorID, textColor);

                if (material.HasProperty("_OutlineWidth"))
                {
                    material.SetFloat("_OutlineWidth", 0f);
                }

                return new TextMeshInfo
                {
                    Mesh = mesh,
                    Material = material
                };
            }
            catch (Exception ex)
            {
                LogUtils.Error(
                    () => $"Failed to create speed text mesh for {speedKmh} km/h: {ex.GetType().Name}: {ex.Message}",
                    ex);

                return default;
            }
            finally
            {
                // Restore the shared TextMeshPro to the exact state we found it. This is what keeps
                // vanilla road/district name labels white and normal-weight; without it our marker
                // color and bold style bleed into every cached world-UI label.
                textMesh.rectTransform.sizeDelta = prevSizeDelta;
                textMesh.fontSize = prevFontSize;
                textMesh.alignment = prevAlignment;
                textMesh.color = prevColor;
                textMesh.characterSpacing = prevCharacterSpacing;
                textMesh.fontStyle = prevFontStyle;
                textMesh.SetText(prevText);
            }
        }


        private static Color GetMarkerTextColor(MarkerVisualKind visualKind)
        {
            return visualKind switch
            {
                MarkerVisualKind.Default => s_DefaultMarkerTextColor,
                MarkerVisualKind.Rail => s_RailMarkerTextColor,
                _ => s_CustomMarkerTextColor
            };
        }

        private static string GetMarkerVisualKindSuffix(MarkerVisualKind visualKind)
        {
            return visualKind switch
            {
                MarkerVisualKind.Default => "default",
                MarkerVisualKind.Rail => "rail",
                _ => "custom"
            };
        }

        private static int GetTextMeshCacheKey(int speedKmh, MarkerVisualKind visualKind)
        {
            return (speedKmh * 4) + (int)visualKind;
        }

        private string GetCurrentMapTheme()
        {
            try
            {
                if (m_CityConfigurationSystem.defaultTheme != Entity.Null)
                {
                    ThemePrefab theme =
                        m_PrefabSystem.GetPrefab<ThemePrefab>(m_CityConfigurationSystem.defaultTheme);

                    return theme?.name ?? "Unknown";
                }
            }
            catch (Exception ex)
            {
                LogUtils.Warn(
                    () => $"Failed to get map theme: {ex.GetType().Name}: {ex.Message}",
                    ex);
            }

            return "Unknown";
        }

        private bool IsMapEuropean()
        {
            string theme = GetCurrentMapTheme();

            // CS2 theme names are usually "North American" or "European".
            return !theme.Equals("North American", StringComparison.Ordinal);
        }

        private void ClearTextMeshCache()
        {
            foreach (TextMeshInfo meshInfo in m_TextMeshCache.Values)
            {
                if (meshInfo.Mesh != null)
                {
                    UnityEngine.Object.Destroy(meshInfo.Mesh);
                }

                if (meshInfo.Material != null)
                {
                    UnityEngine.Object.Destroy(meshInfo.Material);
                }
            }

            m_TextMeshCache.Clear();
        }
    }
}
