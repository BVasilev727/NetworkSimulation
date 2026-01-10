using NetworkSimulation.Modulation.QAMModulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.Modulation
{
    public class AmplitudeModulator : Modulator
    {
        private readonly ulong _frequencyCarrier;
        private readonly double _amplitudeCarrier;
        private readonly double _amplitudeMessage;

        public AmplitudeModulator(ulong frequencyCarrier, double amplitudeCarrier, double amplitudeMessage) : base(44100)
        {
            _frequencyCarrier = frequencyCarrier;
            _amplitudeCarrier = amplitudeCarrier;
            _amplitudeMessage = amplitudeMessage;
        }

        /// <inheritdoc/>
        public override byte[] Demodulate(double[] data)
        {
            int samplesPerBit = 100;
            int numBits = data.Length / samplesPerBit;
            int[] recoveredBits = new int[numBits];

            for (int i = 0; i < numBits; i++)
            {
                double sumPower = 0;
                int start = i * samplesPerBit;

                for (int j = start; j < start + samplesPerBit; j++)
                {
                    sumPower += Math.Abs(data[j]);
                }

                double averageAmplitude = sumPower / samplesPerBit;

                recoveredBits[i] = (averageAmplitude > (_amplitudeCarrier * 0.6)) ? 1 : 0;
            }

            return SignalConverter.FromBitStream(recoveredBits);
        }

        /// <inheritdoc/>
        public override double[] Modulate(byte[] data)
        {
            int[] bits = SignalConverter.ToBitStream(data); 
            int samplesPerBit = 100;
            double[] modulated = new double[bits.Length * samplesPerBit];

            for (int i = 0; i < bits.Length; i++)
            {
                double currentAmplitude = (bits[i] == 1) ? _amplitudeCarrier + _amplitudeMessage : _amplitudeCarrier - _amplitudeMessage;

                for (int j = 0; j < samplesPerBit; j++)
                {
                    int index = (i * samplesPerBit) + j;
                    double t = index * deltaT;
                    modulated[index] = currentAmplitude * Math.Cos(2 * Math.PI * _frequencyCarrier * t);
                }
            }
            return modulated;
        }
    }
}
