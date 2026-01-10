using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.Modulation.QAMModulation
{
    public static class SignalConverter
    {
        public static int[] ToBitStream(byte[] data)
        {
            int[] bits = new int[data.Length * 8];
            for (int i = 0; i < data.Length; i++)
            {
                for (int bit = 0; bit < 8; bit++)
                {
                    bits[i * 8 + bit] = (data[i] >> (7 - bit)) & 1;
                }
            }
            return bits;
        }

        public static byte[] FromBitStream(int[] bits)
        {
            int byteCount = bits.Length / 8;
            byte[] data = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
            {
                byte currentByte = 0;
                for (int bit = 0; bit < 8; bit++)
                {
                    if (bits[i * 8 + bit] == 1)
                    {
                        currentByte |= (byte)(1 << (7 - bit));
                    }
                }
                data[i] = currentByte;
            }
            return data;
        }
    }
}
