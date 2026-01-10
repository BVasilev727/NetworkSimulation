using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.Modulation
{
    public class PSKModulator : Modulator
    {
        private readonly double _frequencyCarrier;
        private readonly int _samplesPerSymbol;

        public PSKModulator(uint frequencyCarrier = 5000, int samplesPerSymbol = 100)
            : base(44100)
        {
            _frequencyCarrier = frequencyCarrier;
            _samplesPerSymbol = samplesPerSymbol;
        }

        /// <inheritdoc/>
        public override byte[] Demodulate(double[] data)
        {
            int symbols = data.Length / _samplesPerSymbol;
            byte[] recovered = new byte[symbols / 4];

            for (int s = 0; s < symbols; s++)
            {
                double I_sum = 0;
                double Q_sum = 0;
                int start = s * _samplesPerSymbol;

                for (int n = 0; n < _samplesPerSymbol; n++)
                {
                    double t = (start + n) * deltaT;
                    I_sum += data[start + n] * Math.Cos(2 * Math.PI * _frequencyCarrier * t);
                    Q_sum += data[start + n] * -Math.Sin(2 * Math.PI * _frequencyCarrier * t);
                }

                double angle = Math.Atan2(Q_sum, I_sum);
                if (angle < 0) angle += 2 * Math.PI;

                int dibit;
                if (angle >= 0 && angle < Math.PI / 2) dibit = 0;            
                else if (angle >= Math.PI / 2 && angle < Math.PI) dibit = 1;     
                else if (angle >= Math.PI && angle < 3 * Math.PI / 2) dibit = 2; 
                else dibit = 3;                                                

                int byteIndex = s / 4;
                int bitShift = 2 * (s % 4);

                recovered[byteIndex] |= (byte)(dibit << bitShift);
            }
            return recovered;
        }

        /// <inheritdoc/>
        public override double[] Modulate(byte[] data)
        {
            int numSymbols = data.Length * 4;
            double[] modulated = new double[numSymbols * _samplesPerSymbol];

            for (int i = 0; i < data.Length; i++)
            {
                for (int bitPair = 0; bitPair < 4; bitPair++)
                {
                    int dibit = (data[i] >> (2 * bitPair)) & 0x03;

                    double phase = (dibit * (Math.PI / 2)) + (Math.PI / 4);

                    for (int n = 0; n < _samplesPerSymbol; n++)
                    {
                        int index = (i * 4 + bitPair) * _samplesPerSymbol + n;
                        double t = index * deltaT;
                        modulated[index] = Math.Cos(2 * Math.PI * _frequencyCarrier * t + phase);
                    }
                }
            }
            return modulated;
        }
    }
}
