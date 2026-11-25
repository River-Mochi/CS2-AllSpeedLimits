// <copyright file="LocaleTH.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleTH.cs
// Purpose: Thai locale Options UI settings.

namespace RoadRailSpeeds
{
    using System.Collections.Generic;
    using Colossal;

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
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kBicycleTab), "จักรยาน" },
                { m_Setting.GetOptionTabLocaleID(SpeedLimitsSetting.kAboutTab), "เกี่ยวกับ" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kDisplayGroup), "ตัวเลือกการแสดงผล" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kKeybindsGroup), "ปุ่มลัด" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kBicycleGroup), "จักรยานและสกู๊ตเตอร์ไฟฟ้า" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kPersonalStatusGroup),     "สถานะยานพาหนะส่วนบุคคล" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kResetGroup), "คืนค่าเริ่มต้นของเกม" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kUsageGroup), "วิธีใช้" },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutInfoGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutLinksGroup), string.Empty },
                { m_Setting.GetOptionGroupLocaleID(SpeedLimitsSetting.kAboutDebugGroup), "ดีบัก / ล็อก" },

                // Optional invisible-network hotkey (unassigned by default).
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "ปุ่มลัดเครือข่ายที่ซ่อนอยู่" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ToggleInvisibleMarkersBinding)), "คลิก «ยังไม่ตั้งค่า» แล้วเลือกปุ่ม ใช้ได้แม้ปิดหน้าต่าง ASL" },

                // Speed unit preference
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)), "หน่วยความเร็ว" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SpeedUnitPreference)),
                    "เลือกหน่วยในแผงและป้ายลอย\n" +
                    "<AUTO> ใช้ตามชนิดแผนที่:\n" +
                    "- EU = KM/H, NA = MPH\n" +
                    "เลือก <KM/H หรือ MPH> เพื่อบังคับการแสดงผลนั้น"
                },

                // Panel behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)), "ซิงก์สไลเดอร์กับช่วงที่เลือก" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.SyncSliderWithSelection)),
                    "**[ ✓ ] แนะนำให้เปิด**\n" +
                    "เปิด: คลิกช่วงแล้วสไลเดอร์จะไปที่ความเร็วปัจจุบันของช่วงนั้น แทนที่จะเริ่มที่ 5\n" +
                    "- ถ้าเลือกหลายช่วง ช่วงแรกยังเป็นตำแหน่งเริ่มของสไลเดอร์\n" +
                    "ปิด: คลิกช่วงอื่นแล้วยังคงค่าเป้าหมายล่าสุดไว้"
                },

                // Slider increment
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)), "ช่วงขั้นของสไลเดอร์" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelSliderIncrement)),
                    "ตั้งค่าขนาดขั้นของสไลเดอร์ในแผงจำกัดความเร็วของเมือง\n" +
                    "<ค่าเริ่มต้น = 10>"
                },

                // Tooltip font scale
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)), "ขนาดข้อความช่วยเหลือ" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.TooltipFontScale)),
                    "ม็อดนี้ขยายข้อความในกล่องคำแนะนำเมื่อชี้เมาส์ที่ฟีเจอร์จำกัดความเร็วได้\n" +
                    "<ค่าเริ่มต้น 110%>"
                },

                // Double speed display
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "แสดงคำแนะนำ ASL" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.PanelTooltipsEnabled)), "แสดงคำแนะนำในเมือง ปุ่มข้อมูลใช้เปิดอีกครั้งได้" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)), "แสดงค่าความเร็วสองเท่าของเกม" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.DoubleSpeedDisplay)),
                    "<ปิด> แสดงสเกลที่ง่ายกว่า มักใกล้กับตัวเลขบนถนน\n" +
                    "<เปิด> แผงและข้อความลอยจะแสดงสเกลภายในของเกมที่สูงกว่า\n" +
                    "มีประโยชน์เมื่อม็อดคำแนะนำอื่นหรือ Scene Explorer แสดงค่าภายในของเกมแบบสองเท่า และต้องการให้ตรงกัน\n" +
                    "**นี่เปลี่ยนเฉพาะการแสดงผลเท่านั้น** ความเร็วที่บันทึกไว้ <ไม่ได้เปลี่ยนจริง>\n" +
                    "ถ้าสับสน ให้ปิดไว้ รถจะเคลื่อนที่เหมือนเดิมไม่ว่าจะเปิดหรือปิด\n" +
                    "หมายเหตุ: <เครื่องหมายบนถนนเป็นภาพตกแต่ง> จึงอาจไม่ตรงกับข้อมูลความเร็วจริงของเกม ป้าย 35 mph อาจมีค่าจริงเป็น ~31 mph เกมกำหนดความเร็วถนนเป็นหน่วยเมตริกก่อน แล้วจึงแปลงหน่วย"
                },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Auto), "AUTO" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Metric), "KM/H" },
                { m_Setting.GetEnumValueLocaleID(SpeedLimitsSetting.SpeedUnit.Imperial), "MPH" },

                // Clear all custom speeds
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "ความเร็วสูงสุดของยานพาหนะ" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.BicycleSpeedPercent)), "100% = ค่าเริ่มต้นของเกม ระบบปรับการเร่งและเบรกอัตโนมัติ ยังใช้ขีดจำกัดความเร็วของทางอยู่ หากใช้ Bikes + Paths ให้อัปเดตด้วย" },
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "คืนค่าความเร็ว" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ResetBicycleSpeed)), "คืนค่าความเร็วจักรยานและสกู๊ตเตอร์ไฟฟ้าเป็น 100%" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)), "กลุ่มจักรยาน" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary1)),
                    "จักรยานและสกู๊ตเตอร์ไฟฟ้า\n" +
                    "**กำลังใช้งาน** = มีเลนปัจจุบัน (กำลังเคลื่อนที่)\n" +
                    "**จอดทั้งหมด** = รวมสถานะ Parked จากทุกที่ ไม่ใช่เฉพาะลานจอดรถ"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)), "กลุ่มรถยนต์" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary2)),
                    "เฉพาะรถยนต์ส่วนบุคคล (ไม่รวมกลุ่มจักรยาน)\n" +
                    "**กำลังใช้งาน** = มีเลนปัจจุบัน (กำลังเคลื่อนที่)\n" +
                    "**จอดอยู่** = รถที่มีสถานะ **ParkedCar** ทั้งหมด ไม่ใช่เฉพาะในลานจอดรถ\n " +
                    "การสแกนทำงานเฉพาะเมื่อเปิดเมนูตัวเลือก ไม่ทำงานระหว่างเล่นในเมือง จึงไม่กระทบ FPS"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)), "รถที่จอดแบบซ่อนอยู่" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.StatusSummary3)),
                    "**รวมที่ขอบเขต OC** = รถยนต์ในกลุ่มรถที่มี ParkedCar ณ จุดเชื่อมต่อนอกเมือง (OC)\n" +
                    "บางเมืองอาจมีรถจำนวนมากติดค้างในสถานะจอดที่จุดเชื่อมต่อนอกเมือง\n" +
                    "ใช้ <บันทึกรถที่ซ่อนอยู่> เพื่อดูตัวอย่างการแบ่งกลุ่มรถที่ซ่อนอยู่\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)), "บันทึกรถที่ซ่อนอยู่" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.LogBorderHiddenCars)),
                    "เขียนรายงานหนึ่งครั้งไปยัง **Logs/AllSpeedLimits.log**\n" +
                    "มีจำนวนรวม + การแบ่ง Bucket A/B/C และหมายเลข ID ตัวอย่าง\n" +
                    "ใช้ม็อด Scene Explorer เพื่อ Jump To ไปยัง ID เอนทิตี Vehicle ที่แสดงไว้และตรวจสอบ"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)), "เปิดไฟล์บันทึก" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.OpenLogFromStatus)),
                    "เปิด **Logs/AllSpeedLimits.log** หากมีไฟล์นี้\n" +
                    "หากยังไม่พบไฟล์ จะเปิดโฟลเดอร์ Logs แทน"
                },

                // Status fallback keys
                { "ASL_PERSONAL_STATUS_NOT_LOADED",     "ยังไม่ได้โหลดสถานะ" },
                { "ASL_PERSONAL_STATS_NOT_AVAIL",       "ไม่มีเมือง... ¯\\_(ツ)_/¯ ...ไม่มีสถิติ" },
                { "ASL_PERSONAL_STATS_CARS_NOT_AVAIL",  "เดินเกมสักครู่เพื่อเก็บข้อมูล" },

                // Status rows
                { "ASL_PERSONAL_STATS_BIKES_ROW1", "{0} ใช้งาน | {1} จักรยาน | {2} สกู๊ตเตอร์ | {3} / {4} จอด/ทั้งหมด" },
                { "ASL_PERSONAL_STATS_CARS_ROW2",  "{0} ใช้งาน | {1} จอด | {2} ทั้งหมด | {3} รถพ่วง" },

                // Row3 shows TOTAL OC hidden
                { "ASL_PERSONAL_STATS_CARS_ROW3",  "{0} ซ่อนอยู่ที่ขอบเขต OC | อัปเดต {1}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)), "คืนค่าความเร็วเริ่มต้นของเกม" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "ตัวเลือกสำหรับล้างค่าก่อนลบม็อด\n" +
                    "ใช้สิ่งนี้ <เฉพาะเมื่อ> ไม่ต้องการเก็บความเร็วที่ม็อดนี้ตั้งไว้\n" +
                    "ไม่จำเป็นสำหรับการลบม็อด ความเร็วที่กำหนดเองยังอยู่ในเมืองได้แม้ไม่มีม็อดนี้\n" +
                    "<============>\n" +
                    "\n" +
                    "ปุ่มนี้จะคืนค่ามาตรฐานของเกมที่รู้จักในทุกตำแหน่งที่ม็อดใช้ความเร็วที่กำหนดเอง\n" +
                    "เมื่อเสร็จแล้ว ให้ทำ **เซฟใหม่** ก่อนลบม็อด\n" +
                    "ถ้าลบม็อดโดยไม่ใช้สิ่งนี้ ความเร็วที่กำหนดเองจะยังอยู่จนกว่าจะเปลี่ยนถนน ราง หรือทางน้ำ"
                },

                { m_Setting.GetOptionWarningLocaleID(nameof(SpeedLimitsSetting.ClearAllCustomSpeeds)),
                    "สิ่งนี้จะคืนค่าขีดจำกัดความเร็วที่ม็อดกำหนดเองทั้งหมดกลับเป็นค่าเริ่มต้นของเกมที่รู้จัก\n" +
                    "ไม่สามารถย้อนกลับอัตโนมัติได้\n" +
                    "เมื่อเสร็จแล้ว ให้บันทึกเมืองเป็นเซฟใหม่ก่อนลบม็อด"
                },

                // Usage instructions
                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "แสดงคำแนะนำ" },
                { m_Setting.GetOptionDescLocaleID(nameof(SpeedLimitsSetting.ShowUsage)), "แสดงวิธีใช้สั้น ๆ ด้านล่าง" },

                { m_Setting.GetOptionLabelLocaleID(nameof(SpeedLimitsSetting.UsageText)),
                    "<แผงเมือง>\n" +
                    "▪ คลิกหรือลากเพื่อเลือกช่วงทาง ตั้ง<ความเร็วใหม่>แล้วกด<ใช้>\n" +
                    "▪ <ค่าที่ตั้งไว้ล่วงหน้า> (ตัวเลขในวงกลม) มีผลทันทีโดยไม่ต้องกดใช้\n" +
                    "▪ <คืนค่า> ช่วงทางที่เลือก\n" +
                    "▪ <เครือข่ายที่ซ่อนอยู่>: ปุ่มเส้นประแสดง/ซ่อนถนนและทางที่ซ่อนอยู่\n" +
                    "<ทั้งเมือง>\n" +
                    "▪ เลือกกลุ่มถนนแล้วใช้ความเร็วใหม่\n" +
                    "▪ <ปุ่มคืนค่าสีแดง>: ถนน ราง ทางน้ำ ทางเดิน หรือทั้งหมด ล้างความเร็วที่ตั้งเองตามประเภท\n" +
                    "▪ <ทางเดิน>: ตัวเลื่อนจักรยานและคนเดินใช้ทั้งเมือง 100% = ค่าเริ่มต้น 200% = ขีดจำกัดสองเท่า\n" +
                    "▪ <ค่ารายช่วงของทางจักรยาน> มีความสำคัญกว่า ทางคนเดินเท่านั้นปรับได้ทั้งเมือง\n" +
                    "▪ <บันทึกเมือง> เพื่อเก็บการแก้ไขเมื่อโหลดใหม่"
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
