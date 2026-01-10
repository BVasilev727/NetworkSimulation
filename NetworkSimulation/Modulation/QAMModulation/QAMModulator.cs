using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.Modulation.QAMModulation
{
    public class QAMModulator : Modulator
    {
        private readonly int _modulationOrder;
        private readonly int _bitsPerSymbol;
        private readonly double _frequencyCarrier;

        public QAMModulator(int modulationOrder = 16, uint frequencyCarrier = 5000) : base(44100)
        {
            _frequencyCarrier = frequencyCarrier;
            _modulationOrder = modulationOrder;
            _bitsPerSymbol = (int)Math.Log2(modulationOrder);
        }

        public override byte[] Demodulate(double[] data)
        {
            int samplesPerSymbol = 100;
            int numSymbols = data.Length / samplesPerSymbol;
            int sqrtM = (int)Math.Sqrt(_modulationOrder);

            List<int> recoveredBits = new List<int>();

            for (int s = 0; s < numSymbols; s++)
            {
                double I_sum = 0;
                double Q_sum = 0;
                int start = s * samplesPerSymbol;

                for (int n = 0; n < samplesPerSymbol; n++)
                {
                    double t = (start + n) * deltaT;
                    I_sum += data[start + n] * Math.Cos(2 * Math.PI * _frequencyCarrier * t);
                    Q_sum += data[start + n] * -Math.Sin(2 * Math.PI * _frequencyCarrier * t);
                }

                double I_received = I_sum * 2 / samplesPerSymbol;
                double Q_received = Q_sum * 2 / samplesPerSymbol;

                int col = (int)Math.Round((I_received + (sqrtM - 1)) / 2.0);
                int row = (int)Math.Round((Q_received + (sqrtM - 1)) / 2.0);

                col = Math.Clamp(col, 0, sqrtM - 1);
                row = Math.Clamp(row, 0, sqrtM - 1);

                int symbolValue = row * sqrtM + col;

                for (int i = _bitsPerSymbol - 1; i >= 0; i--)
                {
                    recoveredBits.Add(symbolValue >> i & 0x01);
                }
            }

            return SignalConverter.FromBitStream(recoveredBits.ToArray());
        }

        public override double[] Modulate(byte[] data)
        {
            int totalBits = data.Length * 8;
            int numSymbols = (int)Math.Ceiling((double)totalBits / _bitsPerSymbol);
            int samplesPerSymbol = 100;
            double[] modulated = new double[numSymbols * samplesPerSymbol];

            int sqrtM = (int)Math.Sqrt(_modulationOrder);

            for (int s = 0; s < numSymbols; s++)
            {
                int symbolVal = GetSymbolFromBytes(data, s);
                double I = 2 * (symbolVal % sqrtM) - (sqrtM - 1);
                double Q = 2 * (symbolVal / sqrtM) - (sqrtM - 1);

                for (int n = 0; n < samplesPerSymbol; n++)
                {
                    int idx = s * samplesPerSymbol + n;
                    double t = idx * deltaT;
                    modulated[idx] = I * Math.Cos(2 * Math.PI * _frequencyCarrier * t) - Q * Math.Sin(2 * Math.PI * _frequencyCarrier * t);
                }
            }
            return modulated;
        }

        private int GetSymbolFromBytes(byte[] data, int symbolIndex)
        {
            int bitOffset = symbolIndex * _bitsPerSymbol;
            int symbolValue = 0;

            for (int i = 0; i < _bitsPerSymbol; i++)
            {
                int totalBitIndex = bitOffset + i;
                int byteIndex = totalBitIndex / 8;
                int bitInByte = 7 - totalBitIndex % 8;

                if (byteIndex < data.Length)
                {
                    int bit = data[byteIndex] >> bitInByte & 0x01;
                    symbolValue = symbolValue << 1 | bit;
                }
            }
            return symbolValue;
        }
    }
}
