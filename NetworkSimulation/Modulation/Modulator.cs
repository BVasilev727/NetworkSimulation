namespace NetworkSimulation.Modulation
{
    public abstract class Modulator
    {
        protected readonly uint SamplingRate;
        protected readonly double deltaT;

        protected Modulator(uint samplingRate = 44100)
        {
            SamplingRate = samplingRate;
            deltaT = 1.0 / samplingRate;
        }

        public abstract double[] Modulate(byte[] data);

        public abstract byte[] Demodulate(double[] data);
    }
}
