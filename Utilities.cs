using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDR4XMPEditor
{
    public class Utilities
    {
        public static ushort Crc16(byte[] bytes)
        {
            int crc = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                crc ^= bytes[i] << 8;
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) == 0x8000)
                    {
                        crc = (crc << 1) ^ 0x1021;
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }

            return (ushort)(crc & 0xFFFF);
        }

        public static unsafe ushort ConvertBytes(byte lsb, byte msb)
        {
            return (ushort)((msb << 8) + lsb);
        }

        public static unsafe void Convert16bitUnsignedInteger(ref byte lsb, ref byte msb, ushort value)
        {
            lsb = (byte)(value & 0xff);
            msb = (byte)((value & 0xff00) >> 8);
        }

        public static uint TimeToTicksDDR5(uint time, uint MinCycleTime)
        {
            // 0.30% per the rounding algorithm per JESD400-5B
            uint correctionFactor = 3;

            // Apply correction factor, scaled by 1000
            float temp = time * (1000 - correctionFactor);
            // Initial nCK calculation, scaled by 1000
            float tempNck = temp / MinCycleTime;
            // Add 1, scaled by 1000, to effectively round up
            tempNck += 1000;
            // Round down to next integer
            return (uint)(tempNck / 1000);
        }

        public static void SetCLSupportedDDR5(byte[] clSupported, int cl, bool supported)
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
                    clSupported[0] |= (byte)mask[index];
                }
                else
                {
                    clSupported[0] &= (byte)~mask[index];
                }
            }
            else if (cl >= 36 && cl <= 50)
            {
                int index = bit - 8;
                if (supported)
                {
                    clSupported[1] |= (byte)mask[index];
                }
                else
                {
                    clSupported[1] &= (byte)~mask[index];
                }
            }
            else if (cl >= 52 && cl <= 66)
            {
                int index = bit - 16;
                if (supported)
                {
                    clSupported[2] |= (byte)mask[index];
                }
                else
                {
                    clSupported[2] &= (byte)~mask[index];
                }
            }
            else if (cl >= 68 && cl <= 82)
            {
                int index = bit - 24;
                if (supported)
                {
                    clSupported[3] |= (byte)mask[index];
                }
                else
                {
                    clSupported[3] &= (byte)~mask[index];
                }
            }
            else if (cl >= 84 && cl <= 98)
            {
                int index = bit - 32;
                if (supported)
                {
                    clSupported[4] |= (byte)mask[index];
                }
                else
                {
                    clSupported[4] &= (byte)~mask[index];
                }
            }
        }

        public static bool IsCLSupportedDDR5(byte[] clSupported, int cl)
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
                return (clSupported[0] & mask[index]) == mask[index];
            }
            else if (cl >= 36 && cl <= 50)
            {
                int index = bit - 8;
                return (clSupported[1] & mask[index]) == mask[index];
            }
            else if (cl >= 52 && cl <= 66)
            {
                int index = bit - 16;
                return (clSupported[2] & mask[index]) == mask[index];
            }
            else if (cl >= 68 && cl <= 82)
            {
                int index = bit - 24;
                return (clSupported[3] & mask[index]) == mask[index];
            }
            else if (cl >= 84 && cl <= 98)
            {
                int index = bit - 32;
                return (clSupported[4] & mask[index]) == mask[index];
            }

            // Should never reach this point
            return false;
        }
        public static ushort ConvertByteToVoltageDDR5(byte val)
        {
            ushort ones = (ushort)(val >> 5);
            ushort hundredths = (ushort)(val & 0x1F);

            return (ushort)(ones * 100 + hundredths * 5);
        }

        public static byte ConvertVoltageToByteDDR5(ushort voltage)
        {
            ushort ones = (ushort)(voltage / 100);
            ushort hundredths = (ushort)(voltage % 100);

            return (byte)((ones << 5) + (hundredths / 5));
        }

        public static byte SetByte(byte bits, ushort bitNumber, bool value)
        {
            if (value)
            {
                bits |= (byte)(1 << bitNumber);
            }
            else
            {
                bits &= (byte)(byte.MaxValue ^ (1 << bitNumber));
            }

            return bits;
        }
    }
}
