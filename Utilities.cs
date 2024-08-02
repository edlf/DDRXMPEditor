namespace DDR5XMPEditor
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
        public static byte SetBit(byte bits, ushort bitNumber, bool value)
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
        public static bool GetBit(byte bits, ushort bitNumber)
        {
            return (bits & (1 << bitNumber)) != 0;
        }
    }
}
