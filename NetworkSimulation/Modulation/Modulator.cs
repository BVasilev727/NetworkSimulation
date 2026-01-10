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

        /// <summary>
        /// Modulates the signal byte array to time-domain signal(double array)
        /// </summary>
        /// <param name="data">Signal to be sent</param>
        /// <returns>Ready to send signal</returns>
        public abstract double[] Modulate(byte[] data);


        /// <summary>
        /// Demodulates the signal from a double array to a byte array.
        /// The double array is noisy.
        /// </summary>
        /// <param name="data">The received signal</param>
        /// <returns>Demodulated byte array</returns>
        public abstract byte[] Demodulate(double[] data);
    }
}
