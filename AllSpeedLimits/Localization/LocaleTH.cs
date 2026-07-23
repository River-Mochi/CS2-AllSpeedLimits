// <copyright file="LocaleTH.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the MIT License. You may not use this file except in compliance with this License.
// See LICENSE file in the project root for full license information.
// This notice and the MIT License notice must be kept with
// all copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleTH.cs
// Purpose: Thai locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;
    using Colossal.PSI.Common;
    using Game.Areas;
    using Game.Citizens;
    using Game.City;
    using Game.Objects;
    using Game.UI;

    public sealed class LocaleTH : IDictionarySource
    {
        private readonly SpeedLimitsSetting m_Setting;

        public LocaleTH(SpeedLimitsSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            // Options menu title keeps English first for stable sorting.
            string title = $"{Mod.ModName} (ขีดจำกัดความเร็วทั้งหมด)";

            return new Dictionary<string, string>
            {
                // Mod title and tabs
                { m_Setting.GetSettingsLocaleID(), title },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kMainTab), "การทำงาน" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "เกี่ยวกับ" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "ตัวเลือกการแสดงผล" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "คืนค่าเริ่มต้นของเกม" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "วิธีใช้" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "ดีบัก / ล็อก" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "หน่วยความเร็ว" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "เลือกหน่วยในแผงและป้ายลอย\n" +
                    "<AUTO> ใช้ตามชนิดแผนที่: EU = KM/H, NA = MPH\n" +
                    "<KM/H> และ <MPH> บังคับการแสดงผลนั้น" },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "ซิงก์สไลเดอร์กับช่วงที่เลือก" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "<แนะนำให้เปิด>\n" +
                    "เปิด: คลิกช่วงแล้วสไลเดอร์จะไปที่ความเร็วปัจจุบันของช่วงแรกที่เลือก\n" +
                    "ปิด: คลิกช่วงอื่นแล้วคงค่าเป้าหมายล่าสุดไว้\n" +
                    "ถ้าเลือกหลายช่วง ช่วงแรกยังเป็นตำแหน่งเริ่มของสไลเดอร์"
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "ช่วงขั้นของสไลเดอร์" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "ตั้งค่าขนาดขั้นในแผงเมือง\n" +
                    "<ค่าเริ่มต้น = 10>" },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "ขนาดข้อความช่วยเหลือ" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "ม็อดนี้ขยายข้อความในกล่องคำแนะนำเมื่อชี้เมาส์ไปที่รายการของม็อดได้\n" +
                    "<ค่าเริ่มต้น 110%>" },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "แสดงค่าความเร็วสองเท่าของเกม" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<ปิด> แสดงสเกลที่ง่ายกว่า มักใกล้กับตัวเลขบนถนน\n" +
                    "<เปิด> แผงและข้อความลอยจะแสดงสเกลภายในของเกมที่สูงกว่า\n" +
                    "มีประโยชน์เมื่อม็อดคำแนะนำอื่นแสดงค่าภายในของเกมแบบสองเท่า และต้องการให้ตรงกัน\n" +
                    "**นี่เปลี่ยนเฉพาะการแสดงผลเท่านั้น** ความเร็วที่บันทึกไว้ <ไม่ได้เปลี่ยนจริง>\n" +
                    "ถ้าสับสน ให้ปิดไว้ รถจะเคลื่อนที่เหมือนเดิมไม่ว่าจะเปิดหรือปิด\n" +
                    "หมายเหตุ: ตัวเลขและป้ายบนถนนเป็นภาพตกแต่ง จึงอาจไม่ตรงกับข้อมูลความเร็วจริงของเกม ป้าย 35 mph อาจมีค่าจริงเป็น 31 mph เกมคำนวณถนนเป็นหน่วยเมตริกก่อน แล้วจึงแปลงหน่วย"
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "คืนค่าความเร็วเริ่มต้นของเกม" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "ตัวเลือกสำหรับล้างค่าก่อนลบม็อด\n" +
                    "ใช้สิ่งนี้<เฉพาะเมื่อ>ไม่ต้องการเก็บความเร็วที่ม็อดนี้ตั้งไว้\n" +
                    "ไม่จำเป็นสำหรับการลบม็อด ความเร็วถนนที่กำหนดเองยังอยู่ในเมืองได้แม้ไม่มีม็อดนี้\n" +
                    "<============>\n" +
                    "\n" +
                    "สิ่งนี้จะคืนค่าความเร็วที่ม็อดตั้งไว้กลับเป็นค่าเริ่มต้นของเกมที่รู้จัก\n" +
                    "เมื่อเสร็จแล้ว ให้ทำ **เซฟใหม่** ก่อนลบม็อด\n" +
                    "ถ้าลบม็อดโดยไม่ใช้สิ่งนี้ ความเร็วที่กำหนดเองจะอยู่จนกว่าคุณจะเปลี่ยนถนน ฯลฯ"
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "สิ่งนี้จะคืนค่าขีดจำกัดความเร็วที่กำหนดเองทั้งหมดที่รองรับกลับเป็นค่าเริ่มต้นของเกมที่รู้จัก\n" +
                    "ไม่สามารถย้อนกลับอัตโนมัติได้\n" +
                    "เมื่อเสร็จแล้ว ให้บันทึกเมืองเป็นเซฟใหม่ก่อนลบม็อด"
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "แสดงคำแนะนำ" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "แสดงวิธีใช้สั้น ๆ ด้านล่าง" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<แผงเมือง>\n" +
                    "1. คลิกหรือลากเลือกช่วง\n" +
                    "2. ตั้ง <ความเร็วใหม่> แล้วกด <ใช้>\n" +
                    "3. <รีเซ็ต> คืนค่าช่วงที่เลือก\n" +
                    "4. ปุ่มพรีเซ็ตจะใช้ทันที\n" +
                    "\n" +
                    "<ทั้งเมือง>\n" +
                    "เลือกกลุ่มถนน แล้วใช้ <ความเร็วใหม่> กับกลุ่มนั้น\n" +
                    "ใช้ <ถนน>, <ราง>, <น้ำ> หรือ <ทั้งหมด> เพื่อล้างความเร็วที่กำหนดเอง\n" +
                    "<บันทึกเมือง> หลังเปลี่ยนทั้งเมือง"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.UsageText)), string.Empty },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.NameText)), "ม็อด" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.NameText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.VersionText)), "เวอร์ชัน" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.VersionText)), string.Empty },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenParadox)), "เปิดหน้า Paradox Mods ของผู้สร้าง" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)), "รายงานดีบักลงล็อก" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DebugReportToLog)),
                    "<ไม่จำเป็นสำหรับการเล่นปกติ>\n" +
                    "เขียนรายงานครั้งเดียวไปที่ Logs/AllSpeedLimits.log"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLog)), "เปิดล็อก" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLog)),
                    "เปิด <Logs/AllSpeedLimits.log> ถ้าไม่มีไฟล์ จะเปิดโฟลเดอร์ Logs แทน" },
            };
        }

        public void Unload()
        {
        }
    }
}
