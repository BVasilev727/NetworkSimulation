using NetworkSimulation.Modulation.QAMModulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.Modulation
{
    public class FrequencyModulator : Modulator
    {
        private readonly uint _frequencyCarrier;
        private readonly uint _frequencyDeviation;

        public FrequencyModulator(uint frequencyCarrier = 5000, uint frequencyDeviation = 1000) : base(44100)
        {
            _frequencyCarrier = frequencyCarrier;
            _frequencyDeviation = frequencyDeviation;
        }

        /// <inheritdoc/>
        public override byte[] Demodulate(double[] data)
        {
            int samplesPerBit = 100;
            int numBits = data.Length / samplesPerBit;
            int[] recovered = new int[numBits];

            double lowFreqExpected = (_frequencyCarrier * 2.0 * samplesPerBit) / SamplingRate;
            double highFreqExpected = ((_frequencyCarrier + _frequencyDeviation) * 2.0 * samplesPerBit) / SamplingRate;

            double threshold = (lowFreqExpected + highFreqExpected) / 2.0;

            for (int i = 0; i < numBits; i++)
            {
                int zeroCrossings = 0;
                int start = i * samplesPerBit;

                for (int j = start + 1; j < start + samplesPerBit; j++)
                {
                    if ((data[j - 1] >= 0 && data[j] < 0) || (data[j - 1] < 0 && data[j] >= 0))
                    {
                        zeroCrossings++;
                    }
                }

                recovered[i] = (zeroCrossings > threshold) ? 1 : 0;
            }

            return SignalConverter.FromBitStream(recovered);
        }

        /// <inheritdoc/>
        public override double[] Modulate(byte[] data)
        {
            int[] bits = SignalConverter.ToBitStream(data);
            int samplesPerBit = 100;
            double[] modulated = new double[samplesPerBit * bits.Length];

            for (int i = 0; i < bits.Length; i++)
            {
                double currentFrequency = (bits[i] > 0)
                    ? (_frequencyCarrier + _frequencyDeviation)
                    : (_frequencyCarrier);

                for (int j = 0; j < samplesPerBit; j++)
                {
                    int index = (i * samplesPerBit) + j;
                    double t = index * deltaT;
                    modulated[index] = Math.Sin(2 * Math.PI * currentFrequency * t);
                }
            }
            return modulated;
        }
    }
}
