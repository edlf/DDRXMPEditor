using Stylet;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace DDR5XMPEditor.DDR5SPD
{
    public class XMP_3_0 : PropertyChangedBase
    {
        public static readonly ushort Size = 0x40;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        private unsafe struct RawXMPProfile
        {
            public byte vpp;
            public byte vdd;
            public byte vddq;
            public byte unkown_03;
            public byte vmemctrl;
            public fixed byte minCycleTime[2];
            public fixed byte clSupported[5];
            public byte unkown_0A;
            public fixed byte tAA[2];
            public fixed byte tRCD[2];
            public fixed byte tRP[2];
            public fixed byte tRAS[2];
            public fixed byte tRC[2];
            public fixed byte tWR[2];
            public fixed byte tRFC1[2];
            public fixed byte tRFC2[2];
            public fixed byte tRFC[2];
            public fixed byte tRRD_L[2];
            public byte tRRD_L_lowerLimit;
            public fixed byte tCCD_L_WR[2];
            public byte tCCD_L_WR_lowerLimit;
            public fixed byte tCCD_L_WR2[2];
            public byte tCCD_L_WR2_lowerLimit;
            public fixed byte tCCD_L_WTR[2];
            public byte tCCD_L_WTR_lowerLimit;
            public fixed byte tCCD_S_WTR[2];
            public byte tCCD_S_WTR_lowerLimit;
            public fixed byte tCCD_L[2];
            public byte tCCD_L_lowerLimit;
            public fixed byte tRTP[2];
            public byte tRTP_lowerLimit;
            public fixed byte tFAW[2];
            public byte tFAW_lowerLimit;
            public byte unk_07;
            public byte unk_08;
            public byte unk_09;
            public byte unk_0A;
            public byte memory_boost_realtime_training;
            public byte commandRate;
            public byte unk_0D;

            // Byte 0x3E-0x4F
            public fixed byte checksum[2];
        }
        public enum CommandRatesEnum
        {
            _undefined,
            _1n,
            _2n,
            _3n,
            Count
        }

        private static readonly CommandRatesEnum[] commandRatesMap = new CommandRatesEnum[(int)CommandRatesEnum.Count]
        {
            CommandRatesEnum._undefined,
            CommandRatesEnum._1n,
            CommandRatesEnum._2n,
            CommandRatesEnum._3n
        };

        private RawXMPProfile rawXMPProfile;
        ushort profileNo;

        public bool IsUserProfile()
        {
            return profileNo == 4 || profileNo == 5;
        }
        public CommandRatesEnum? CommandRate
        {
            get
            {
                ushort index = (ushort)(rawXMPProfile.commandRate & 0xF);
                if (index >= (ushort)CommandRatesEnum.Count)
                {
                    return null;
                }
                return commandRatesMap[index];
            }
            set
            {
                if (value.HasValue)
                {
                    int index = Array.FindIndex(commandRatesMap, d => d == value.Value);
                    rawXMPProfile.commandRate = (byte)((rawXMPProfile.commandRate & 0xF0) | (index & 0xF));
                }
            }
        }
        public bool IntelDynamicMemoryBoost
        {
            get
            {
                return (rawXMPProfile.memory_boost_realtime_training & (1 << 0)) != 0;
            }
            set
            {
                rawXMPProfile.memory_boost_realtime_training = Utilities.SetBit(rawXMPProfile.memory_boost_realtime_training, 0, value);
            }
        }
        public bool RealTimeMemoryFrequencyOC
        {
            get
            {
                return (rawXMPProfile.memory_boost_realtime_training & (1 << 1)) != 0;
            }
            set
            {
                rawXMPProfile.memory_boost_realtime_training = Utilities.SetBit(rawXMPProfile.memory_boost_realtime_training, 1, value);
            }
        }
        public ushort VDD
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawXMPProfile.vdd);
            }
            set
            {
                rawXMPProfile.vdd = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }
        public ushort VDDQ
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawXMPProfile.vddq);
            }
            set
            {
                rawXMPProfile.vddq = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }
        public ushort VPP
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawXMPProfile.vpp);
            }
            set
            {
                rawXMPProfile.vpp = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }
        public ushort VMEMCTRL
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawXMPProfile.vmemctrl);
            }
            set
            {
                rawXMPProfile.vmemctrl = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }
        public unsafe ushort MinCycleTime
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.minCycleTime[0], rawXMPProfile.minCycleTime[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.minCycleTime[0], ref rawXMPProfile.minCycleTime[1], value);
            }
        }
        public unsafe bool IsCLSupported(int cl)
        {
            int[] mask = { 1, 2, 4, 8, 16, 32, 64, 128 };

            // All valid CAS latencies are even numbers between 20 and 98
            if (cl < 20 || cl > 98 || (cl % 2 != 0))
            {
                return false;
            }

            const int offset = 20;
            int bit = (cl - offset) / 2;

            if (cl >= 20 && cl <= 34)
            {
                int index = bit;
                return (rawXMPProfile.clSupported[0] & mask[index]) == mask[index];
            }
            else if (cl >= 36 && cl <= 50)
            {
                int index = bit - 8;
                return (rawXMPProfile.clSupported[1] & mask[index]) == mask[index];
            }
            else if (cl >= 52 && cl <= 66)
            {
                int index = bit - 16;
                return (rawXMPProfile.clSupported[2] & mask[index]) == mask[index];
            }
            else if (cl >= 68 && cl <= 82)
            {
                int index = bit - 24;
                return (rawXMPProfile.clSupported[3] & mask[index]) == mask[index];
            }
            else if (cl >= 84 && cl <= 98)
            {
                int index = bit - 32;
                return (rawXMPProfile.clSupported[4] & mask[index]) == mask[index];
            }

            // Should never reach this point
            return false;
        }
        public unsafe void SetCLSupported(int cl, bool supported)
        {
            // All valid CAS latencies are even numbers between 20 and 98
            if (cl < 20 || cl > 98 || (cl % 2 != 0))
            {
                return;
            }

            int[] mask = { 1, 2, 4, 8, 16, 32, 64, 128 };

            const int offset = 20;
            int bit = (cl - offset) / 2;

            if (cl >= 20 && cl <= 34)
            {
                int index = bit;
                if (supported)
                {
                    rawXMPProfile.clSupported[0] |= (byte)mask[index];
                }
                else
                {
                    rawXMPProfile.clSupported[0] &= (byte)~mask[index];
                }
            }
            else if (cl >= 36 && cl <= 50)
            {
                int index = bit - 8;
                if (supported)
                {
                    rawXMPProfile.clSupported[1] |= (byte)mask[index];
                }
                else
                {
                    rawXMPProfile.clSupported[1] &= (byte)~mask[index];
                }
            }
            else if (cl >= 52 && cl <= 66)
            {
                int index = bit - 16;
                if (supported)
                {
                    rawXMPProfile.clSupported[2] |= (byte)mask[index];
                }
                else
                {
                    rawXMPProfile.clSupported[2] &= (byte)~mask[index];
                }
            }
            else if (cl >= 68 && cl <= 82)
            {
                int index = bit - 24;
                if (supported)
                {
                    rawXMPProfile.clSupported[3] |= (byte)mask[index];
                }
                else
                {
                    rawXMPProfile.clSupported[3] &= (byte)~mask[index];
                }
            }
            else if (cl >= 84 && cl <= 98)
            {
                int index = bit - 32;
                if (supported)
                {
                    rawXMPProfile.clSupported[4] |= (byte)mask[index];
                }
                else
                {
                    rawXMPProfile.clSupported[4] &= (byte)~mask[index];
                }
            }
        }
        public unsafe bool CL20
        {
            get
            {
                return IsCLSupported(20);
            }
            set
            {
                SetCLSupported(20, value);
            }
        }
        public unsafe bool CL22
        {
            get
            {
                return IsCLSupported(22);
            }
            set
            {
                SetCLSupported(22, value);
            }
        }

        public unsafe bool CL24
        {
            get
            {
                return IsCLSupported(24);
            }
            set
            {
                SetCLSupported(24, value);
            }
        }

        public unsafe bool CL26
        {
            get
            {
                return IsCLSupported(26);
            }
            set
            {
                SetCLSupported(26, value);
            }
        }

        public unsafe bool CL28
        {
            get
            {
                return IsCLSupported(28);
            }
            set
            {
                SetCLSupported(28, value);
            }
        }

        public unsafe bool CL30
        {
            get
            {
                return IsCLSupported(30);
            }
            set
            {
                SetCLSupported(30, value);
            }
        }
        public unsafe bool CL32
        {
            get
            {
                return IsCLSupported(32);
            }
            set
            {
                SetCLSupported(32, value);
            }
        }

        public unsafe bool CL34
        {
            get
            {
                return IsCLSupported(34);
            }
            set
            {
                SetCLSupported(34, value);
            }
        }

        public unsafe bool CL36
        {
            get
            {
                return IsCLSupported(36);
            }
            set
            {
                SetCLSupported(36, value);
            }
        }

        public unsafe bool CL38
        {
            get
            {
                return IsCLSupported(38);
            }
            set
            {
                SetCLSupported(38, value);
            }
        }

        public unsafe bool CL40
        {
            get
            {
                return IsCLSupported(40);
            }
            set
            {
                SetCLSupported(40, value);
            }
        }
        public unsafe bool CL42
        {
            get
            {
                return IsCLSupported(42);
            }
            set
            {
                SetCLSupported(42, value);
            }
        }

        public unsafe bool CL44
        {
            get
            {
                return IsCLSupported(44);
            }
            set
            {
                SetCLSupported(44, value);
            }
        }

        public unsafe bool CL46
        {
            get
            {
                return IsCLSupported(46);
            }
            set
            {
                SetCLSupported(46, value);
            }
        }

        public unsafe bool CL48
        {
            get
            {
                return IsCLSupported(48);
            }
            set
            {
                SetCLSupported(48, value);
            }
        }
        public unsafe bool CL50
        {
            get
            {
                return IsCLSupported(50);
            }
            set
            {
                SetCLSupported(50, value);
            }
        }
        public unsafe bool CL52
        {
            get
            {
                return IsCLSupported(52);
            }
            set
            {
                SetCLSupported(52, value);
            }
        }

        public unsafe bool CL54
        {
            get
            {
                return IsCLSupported(54);
            }
            set
            {
                SetCLSupported(54, value);
            }
        }

        public unsafe bool CL56
        {
            get
            {
                return IsCLSupported(56);
            }
            set
            {
                SetCLSupported(56, value);
            }
        }

        public unsafe bool CL58
        {
            get
            {
                return IsCLSupported(58);
            }
            set
            {
                SetCLSupported(58, value);
            }
        }
        public unsafe bool CL60
        {
            get
            {
                return IsCLSupported(60);
            }
            set
            {
                SetCLSupported(60, value);
            }
        }
        public unsafe bool CL62
        {
            get
            {
                return IsCLSupported(62);
            }
            set
            {
                SetCLSupported(62, value);
            }
        }

        public unsafe bool CL64
        {
            get
            {
                return IsCLSupported(64);
            }
            set
            {
                SetCLSupported(64, value);
            }
        }

        public unsafe bool CL66
        {
            get
            {
                return IsCLSupported(66);
            }
            set
            {
                SetCLSupported(66, value);
            }
        }

        public unsafe bool CL68
        {
            get
            {
                return IsCLSupported(68);
            }
            set
            {
                SetCLSupported(68, value);
            }
        }
        public unsafe bool CL70
        {
            get
            {
                return IsCLSupported(70);
            }
            set
            {
                SetCLSupported(70, value);
            }
        }
        public unsafe bool CL72
        {
            get
            {
                return IsCLSupported(72);
            }
            set
            {
                SetCLSupported(72, value);
            }
        }

        public unsafe bool CL74
        {
            get
            {
                return IsCLSupported(74);
            }
            set
            {
                SetCLSupported(74, value);
            }
        }

        public unsafe bool CL76
        {
            get
            {
                return IsCLSupported(76);
            }
            set
            {
                SetCLSupported(76, value);
            }
        }

        public unsafe bool CL78
        {
            get
            {
                return IsCLSupported(78);
            }
            set
            {
                SetCLSupported(78, value);
            }
        }
        public unsafe bool CL80
        {
            get
            {
                return IsCLSupported(80);
            }
            set
            {
                SetCLSupported(80, value);
            }
        }
        public unsafe bool CL82
        {
            get
            {
                return IsCLSupported(82);
            }
            set
            {
                SetCLSupported(82, value);
            }
        }

        public unsafe bool CL84
        {
            get
            {
                return IsCLSupported(84);
            }
            set
            {
                SetCLSupported(84, value);
            }
        }

        public unsafe bool CL86
        {
            get
            {
                return IsCLSupported(86);
            }
            set
            {
                SetCLSupported(86, value);
            }
        }

        public unsafe bool CL88
        {
            get
            {
                return IsCLSupported(88);
            }
            set
            {
                SetCLSupported(88, value);
            }
        }
        public unsafe bool CL90
        {
            get
            {
                return IsCLSupported(90);
            }
            set
            {
                SetCLSupported(90, value);
            }
        }
        public unsafe bool CL92
        {
            get
            {
                return IsCLSupported(92);
            }
            set
            {
                SetCLSupported(92, value);
            }
        }

        public unsafe bool CL94
        {
            get
            {
                return IsCLSupported(94);
            }
            set
            {
                SetCLSupported(94, value);
            }
        }

        public unsafe bool CL96
        {
            get
            {
                return IsCLSupported(96);
            }
            set
            {
                SetCLSupported(96, value);
            }
        }

        public unsafe bool CL98
        {
            get
            {
                return IsCLSupported(98);
            }
            set
            {
                SetCLSupported(98, value);
            }
        }
        public unsafe ushort tAA
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tAA[0], rawXMPProfile.tAA[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tAA[0], ref rawXMPProfile.tAA[1], value);
            }
        }
        public unsafe ushort tAATicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tAA, MinCycleTime);
            }
        }
        public unsafe ushort tRCD
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tRCD[0], rawXMPProfile.tRCD[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tRCD[0], ref rawXMPProfile.tRCD[1], value);
            }
        }
        public unsafe ushort tRCDTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRCD, MinCycleTime);
            }
        }
        public unsafe ushort tRP
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tRP[0], rawXMPProfile.tRP[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tRP[0], ref rawXMPProfile.tRP[1], value);
            }
        }
        public unsafe ushort tRPTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRP, MinCycleTime);
            }
        }
        public unsafe ushort tRAS
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tRAS[0], rawXMPProfile.tRAS[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tRAS[0], ref rawXMPProfile.tRAS[1], value);
            }
        }
        public unsafe ushort tRASTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRAS, MinCycleTime);
            }
        }
        public unsafe ushort tRC
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tRC[0], rawXMPProfile.tRC[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tRC[0], ref rawXMPProfile.tRC[1], value);
            }
        }
        public unsafe ushort tRCTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRC, MinCycleTime);
            }
        }
        public unsafe ushort tWR
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tWR[0], rawXMPProfile.tWR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tWR[0], ref rawXMPProfile.tWR[1], value);
            }
        }
        public unsafe ushort tWRTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tWR, MinCycleTime);
            }

        }
        public unsafe ushort tRFC1
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tRFC1[0], rawXMPProfile.tRFC1[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tRFC1[0], ref rawXMPProfile.tRFC1[1], value);
            }
        }
        public unsafe ushort tRFC1Ticks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5((uint)(tRFC1 * 1000), MinCycleTime);
            }
        }
        public unsafe ushort tRFC2
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tRFC2[0], rawXMPProfile.tRFC2[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tRFC2[0], ref rawXMPProfile.tRFC2[1], value);
            }
        }
        public unsafe ushort tRFC2Ticks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5((uint)(tRFC2 * 1000), MinCycleTime);
            }
        }
        public unsafe ushort tRFC
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tRFC[0], rawXMPProfile.tRFC[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tRFC[0], ref rawXMPProfile.tRFC[1], value);
            }
        }
        public unsafe ushort tRFCTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5((uint)(tRFC * 1000), MinCycleTime);
            }
        }
        public unsafe ushort tRRD_L
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tRRD_L[0], rawXMPProfile.tRRD_L[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tRRD_L[0], ref rawXMPProfile.tRRD_L[1], value);
            }
        }
        public unsafe ushort tRRD_LTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRRD_L, MinCycleTime);
            }
        }
        public unsafe ushort tRRD_L_lowerLimit
        {
            get => rawXMPProfile.tRRD_L_lowerLimit;
            set
            {
                rawXMPProfile.tRRD_L_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tCCD_L
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tCCD_L[0], rawXMPProfile.tCCD_L[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tCCD_L[0], ref rawXMPProfile.tCCD_L[1], value);
            }
        }
        public unsafe ushort tCCD_LTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_L, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_L_lowerLimit
        {
            get => rawXMPProfile.tCCD_L_lowerLimit;
            set
            {
                rawXMPProfile.tCCD_L_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tCCD_L_WR
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tCCD_L_WR[0], rawXMPProfile.tCCD_L_WR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tCCD_L_WR[0], ref rawXMPProfile.tCCD_L_WR[1], value);
            }
        }
        public unsafe ushort tCCD_L_WRTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_L_WR, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_L_WR_lowerLimit
        {
            get => rawXMPProfile.tCCD_L_WR_lowerLimit;
            set
            {
                rawXMPProfile.tCCD_L_WR_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tCCD_L_WR2
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tCCD_L_WR2[0], rawXMPProfile.tCCD_L_WR2[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tCCD_L_WR2[0], ref rawXMPProfile.tCCD_L_WR2[1], value);
            }
        }
        public unsafe ushort tCCD_L_WR2Ticks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_L_WR2, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_L_WR2_lowerLimit
        {
            get => rawXMPProfile.tCCD_L_WR2_lowerLimit;
            set
            {
                rawXMPProfile.tCCD_L_WR2_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tFAW
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tFAW[0], rawXMPProfile.tFAW[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tFAW[0], ref rawXMPProfile.tFAW[1], value);
            }
        }
        public unsafe ushort tFAWTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tFAW, MinCycleTime);
            }
        }
        public unsafe ushort tFAW_lowerLimit
        {
            get => rawXMPProfile.tFAW_lowerLimit;
            set
            {
                rawXMPProfile.tFAW_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tCCD_L_WTR
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tCCD_L_WTR[0], rawXMPProfile.tCCD_L_WTR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tCCD_L_WTR[0], ref rawXMPProfile.tCCD_L_WTR[1], value);
            }
        }
        public unsafe ushort tCCD_L_WTRTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_L_WTR, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_L_WTR_lowerLimit
        {
            get => rawXMPProfile.tCCD_L_WTR_lowerLimit;
            set
            {
                rawXMPProfile.tCCD_L_WTR_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tCCD_S_WTR
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tCCD_S_WTR[0], rawXMPProfile.tCCD_S_WTR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tCCD_S_WTR[0], ref rawXMPProfile.tCCD_S_WTR[1], value);
            }
        }
        public unsafe ushort tCCD_S_WTRTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_S_WTR, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_S_WTR_lowerLimit
        {
            get => rawXMPProfile.tCCD_S_WTR_lowerLimit;
            set
            {
                rawXMPProfile.tCCD_S_WTR_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tRTP
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.tRTP[0], rawXMPProfile.tRTP[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.tRTP[0], ref rawXMPProfile.tRTP[1], value);
            }
        }
        public unsafe ushort tRTPTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRTP, MinCycleTime);
            }
        }
        public unsafe ushort tRTP_lowerLimit
        {
            get => rawXMPProfile.tRTP_lowerLimit;
            set
            {
                rawXMPProfile.tRTP_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort CRC
        {
            get
            {
                return Utilities.ConvertBytes(rawXMPProfile.checksum[0], rawXMPProfile.checksum[1]);
            }
            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMPProfile.checksum[0], ref rawXMPProfile.checksum[1], value);
            }
        }
        public void LoadSample()
        {
            MinCycleTime = 312;
            CommandRate = CommandRatesEnum._2n;

            VDD = 110;
            VDDQ = 110;
            VPP = 180;
            VMEMCTRL = 120;

            CL22 = true;
            CL26 = true;
            CL28 = true;
            CL30 = true;
            CL32 = true;
            CL36 = true;
            CL40 = true;
            CL42 = true;
            CL46 = true;
            CL48 = true;
            CL50 = true;
            CL52 = true;
            CL54 = true;
            CL56 = true;

            tAA = 16250;
            tRCD = 16250;
            tRP = 16250;
            tRAS = 32000;
            tRC = 48250;
            tWR = 30000;
            tRFC1 = 295;
            tRFC2 = 160;
            tRFC = 130;

            tRRD_L = 5000;
            tRRD_L_lowerLimit = 8;
            tCCD_L = 5000;
            tCCD_L_lowerLimit = 8;
            tCCD_L_WR = 20000;
            tCCD_L_WR_lowerLimit = 32;
            tCCD_L_WR2 = 10000;
            tCCD_L_WR2_lowerLimit = 16;
            tFAW = 10000;
            tFAW_lowerLimit = 32;
            tCCD_L_WTR = 10000;
            tCCD_L_WTR_lowerLimit = 16;
            tCCD_S_WTR = 2500;
            tCCD_S_WTR_lowerLimit = 4;
            tRTP = 7500;
            tRTP_lowerLimit = 12;
            UpdateCrc();
        }
        public void UpdateCrc()
        {
            CRC = CalculateCRC();
        }
        public unsafe ushort CalculateCRC()
        {
            var rawXmpBytes = GetBytes();
            return Utilities.Crc16(rawXmpBytes.Take(0x3D + 1).ToArray());
        }
        public bool CheckCRCValidity()
        {
            return CRC == CalculateCRC();
        }
        public bool IsEmpty()
        {
            var rawXmpBytes = GetBytes();

            for (int i = 0; i < rawXmpBytes.Length; i++)
            {
                if (rawXmpBytes[i] != 0x00)
                {
                    return false;
                }
            }

            return true;
        }
        public void Wipe()
        {
            var rawXmpBytes = GetBytes();

            for (int i = 0; i < rawXmpBytes.Length; i++)
            {
                rawXmpBytes[i] = 0x00;
            }

            Parse(rawXmpBytes);
        }
        public byte[] GetBytes()
        {
            IntPtr ptr = IntPtr.Zero;
            byte[] bytes = null;
            try
            {
                var size = Marshal.SizeOf<RawXMPProfile>();
                bytes = new byte[size];
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(rawXMPProfile, ptr, true);
                Marshal.Copy(ptr, bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return bytes;
        }
        public void Parse(byte[] bytes)
        {
            if (bytes.Length != Size)
            {
                return;
            }

            var handle_xmp = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            rawXMPProfile = Marshal.PtrToStructure<RawXMPProfile>(handle_xmp.AddrOfPinnedObject());
            handle_xmp.Free();
        }
        /// <summary>
        /// Parses a single XMP 3.0 profile.
        /// </summary>
        /// <param name="profileNumber">The number of the XMP 3.0 profile.</param>
        /// <param name="bytes">The raw bytes of the XMP 3.0 profile.</param>
        /// <returns>An <see cref="XMP_3_0"/> object.</returns>
        public static XMP_3_0 Parse(ushort profileNumber, byte[] bytes)
        {
            if (bytes.Length != Size)
            {
                return null;
            }

            var handle_xmp = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            XMP_3_0 xmp = new XMP_3_0
            {
                rawXMPProfile = Marshal.PtrToStructure<RawXMPProfile>(handle_xmp.AddrOfPinnedObject()),
                profileNo = profileNumber
            };
            handle_xmp.Free();
            return xmp;
        }
    }
}
