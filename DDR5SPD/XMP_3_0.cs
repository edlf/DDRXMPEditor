using Stylet;
using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace DDR4XMPEditor.DDR5SPD
{
    public class XMP_3_0 : PropertyChangedBase
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        private unsafe struct RawXMP {
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

        public const int Size = 0x40;
        private RawXMP rawXMP;
        ushort profileNo;

        public bool IsUserProfile()
        {
            return profileNo == 4 || profileNo == 5;
        }
        public CommandRatesEnum? CommandRate
        {
            get
            {
                ushort index = (ushort)(rawXMP.commandRate & 0xF);
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
                    rawXMP.commandRate = (byte)((rawXMP.commandRate & 0xF0) | (index & 0xF));
                }
            }
        }

        public bool IntelDynamicMemoryBoost {
            get {
                return (rawXMP.memory_boost_realtime_training & (1 << 0)) != 0;
            }
            set {
                rawXMP.memory_boost_realtime_training = Utilities.SetByte(rawXMP.memory_boost_realtime_training, 0, value);
            }
        }

        public bool RealTimeMemoryFrequencyOC
        {
            get
            {
                return (rawXMP.memory_boost_realtime_training & (1 << 1)) != 0;
            }
            set
            {
                rawXMP.memory_boost_realtime_training = Utilities.SetByte(rawXMP.memory_boost_realtime_training, 1, value);
            }
        }
        public ushort VDD
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawXMP.vdd);
            }
            set
            {
                rawXMP.vdd = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }

        public ushort VDDQ
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawXMP.vddq);
            }
            set
            {
                rawXMP.vddq = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }

        public ushort VPP
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawXMP.vpp);
            }
            set
            {
                rawXMP.vpp = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }
        public ushort VMEMCTRL
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawXMP.vmemctrl);
            }
            set
            {
                rawXMP.vmemctrl = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }

        public unsafe ushort MinCycleTime
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.minCycleTime[0], rawXMP.minCycleTime[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.minCycleTime[0], ref rawXMP.minCycleTime[1], value);
            }
        }
        public unsafe ushort tAA
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.tAA[0], rawXMP.tAA[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tAA[0], ref rawXMP.tAA[1], value);
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
                return Utilities.ConvertBytes(rawXMP.tRCD[0], rawXMP.tRCD[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tRCD[0], ref rawXMP.tRCD[1], value);
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
                return Utilities.ConvertBytes(rawXMP.tRP[0], rawXMP.tRP[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tRP[0], ref rawXMP.tRP[1], value);
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
                return Utilities.ConvertBytes(rawXMP.tRAS[0], rawXMP.tRAS[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tRAS[0], ref rawXMP.tRAS[1], value);
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
                return Utilities.ConvertBytes(rawXMP.tRC[0], rawXMP.tRC[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tRC[0], ref rawXMP.tRC[1], value);
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
                return Utilities.ConvertBytes(rawXMP.tWR[0], rawXMP.tWR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tWR[0], ref rawXMP.tWR[1], value);
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
                return Utilities.ConvertBytes(rawXMP.tRFC1[0], rawXMP.tRFC1[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tRFC1[0], ref rawXMP.tRFC1[1], value);
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
                return Utilities.ConvertBytes(rawXMP.tRFC2[0], rawXMP.tRFC2[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tRFC2[0], ref rawXMP.tRFC2[1], value);
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
                return Utilities.ConvertBytes(rawXMP.tRFC[0], rawXMP.tRFC[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tRFC[0], ref rawXMP.tRFC[1], value);
            }
        }
        public unsafe ushort tRFCTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5((uint)(tRFC * 1000), MinCycleTime);
            }
        }

        public unsafe byte[] GetClSupported()
        {
            return new byte[] { rawXMP.clSupported[0], rawXMP.clSupported[1], rawXMP.clSupported[2], rawXMP.clSupported[3], rawXMP.clSupported[4] };
        }

        public unsafe void SetClSupported(int index, byte value)
        {
            rawXMP.clSupported[index] = value;
        }

        public unsafe ushort tRRD_L
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.tRRD_L[0], rawXMP.tRRD_L[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tRRD_L[0], ref rawXMP.tRRD_L[1], value);
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
            get => rawXMP.tRRD_L_lowerLimit;
            set
            {
                rawXMP.tRRD_L_lowerLimit = (byte)value;
            }
        }

        public unsafe ushort tCCD_L
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.tCCD_L[0], rawXMP.tCCD_L[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tCCD_L[0], ref rawXMP.tCCD_L[1], value);
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
            get => rawXMP.tCCD_L_lowerLimit;
            set
            {
                rawXMP.tCCD_L_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tCCD_L_WR
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.tCCD_L_WR[0], rawXMP.tCCD_L_WR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tCCD_L_WR[0], ref rawXMP.tCCD_L_WR[1], value);
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
            get => rawXMP.tCCD_L_WR_lowerLimit;
            set
            {
                rawXMP.tCCD_L_WR_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tCCD_L_WR2
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.tCCD_L_WR2[0], rawXMP.tCCD_L_WR2[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tCCD_L_WR2[0], ref rawXMP.tCCD_L_WR2[1], value);
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
            get => rawXMP.tCCD_L_WR2_lowerLimit;
            set
            {
                rawXMP.tCCD_L_WR2_lowerLimit = (byte)value;
            }
        }

        public unsafe ushort tFAW
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.tFAW[0], rawXMP.tFAW[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tFAW[0], ref rawXMP.tFAW[1], value);
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
            get => rawXMP.tFAW_lowerLimit;
            set
            {
                rawXMP.tFAW_lowerLimit = (byte)value;
            }
        }

        public unsafe ushort tCCD_L_WTR
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.tCCD_L_WTR[0], rawXMP.tCCD_L_WTR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tCCD_L_WTR[0], ref rawXMP.tCCD_L_WTR[1], value);
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
            get => rawXMP.tCCD_L_WTR_lowerLimit;
            set
            {
                rawXMP.tCCD_L_WTR_lowerLimit = (byte)value;
            }
        }
        public unsafe ushort tCCD_S_WTR
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.tCCD_S_WTR[0], rawXMP.tCCD_S_WTR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tCCD_S_WTR[0], ref rawXMP.tCCD_S_WTR[1], value);
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
            get => rawXMP.tCCD_S_WTR_lowerLimit;
            set
            {
                rawXMP.tCCD_S_WTR_lowerLimit = (byte)value;
            }
        }

        public unsafe ushort tRTP
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.tRTP[0], rawXMP.tRTP[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.tRTP[0], ref rawXMP.tRTP[1], value);
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
            get => rawXMP.tRTP_lowerLimit;
            set
            {
                rawXMP.tRTP_lowerLimit = (byte)value;
            }
        }

        public unsafe ushort CRC
        {
            get
            {
                return Utilities.ConvertBytes(rawXMP.checksum[0], rawXMP.checksum[1]);
            }
            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawXMP.checksum[0], ref rawXMP.checksum[1], value);
            }
        }

        public void UpdateCrc()
        {
            var rawXmpBytes = GetBytes();
            CRC = Utilities.Crc16(rawXmpBytes.Take(0x3D + 1).ToArray());
        }

        public byte[] GetBytes()
        {
            IntPtr ptr = IntPtr.Zero;
            byte[] bytes = null;
            try
            {
                var size = Marshal.SizeOf<RawXMP>();
                bytes = new byte[size];
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(rawXMP, ptr, true);
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
                rawXMP = Marshal.PtrToStructure<RawXMP>(handle_xmp.AddrOfPinnedObject()),
                profileNo = profileNumber
            };
            handle_xmp.Free();
            return xmp;
        }
    }
}
