using NetworkSimulation.Enums;
using NetworkSimulation.Providers;

namespace NetworkSimulation.NetworkBase
{
    public class NetworkStandard
    {
        private readonly GenType _genType;
        private readonly double _minRange;
        private readonly double _maxBandwidth;
        private readonly int _latency;
        private readonly int _maxConnections;
        private int _minConnections;


        /// <summary>
        /// <seealso cref="GenType"/>
        /// </summary>
        public GenType Generation => _genType;

        /// <summary>
        /// Maximum effective bandwidth of the standard (in Mbps)
        /// </summary>
        public double MaxBandwidth => _maxBandwidth;

        /// <summary>
        /// Latency of the standard (in ms)
        /// </summary>
        public int Latency => _latency;

        /// <summary>
        /// Range of the standard
        /// </summary>
        public double Range {  get; private set; }

        /// <summary>
        /// Number of connections
        /// </summary>
        public int Connections {  get; private set; }

        /// <summary>
        /// Ctor for network standard
        /// <seealso cref="GenType"/>
        /// </summary>
        /// <param name="generation"></param>
        /// <param name="maxRange"></param>
        /// <param name="maxBandwidth"></param>
        /// <param name="latency"></param>
        /// <param name="maxConnections"></param>
        public NetworkStandard(GenType generation, double maxRange, double minRange, double maxBandwidth, int latency, int maxConnections, int minConnections)
        {
            _genType = generation;
            _minRange = maxRange;
            _minRange = minRange;
            _maxBandwidth = maxBandwidth;
            _latency = latency;
            _maxConnections = maxConnections;
            _minConnections = minConnections;

            Range = RandomProvider.NextDouble() * (maxRange - minRange) + minRange;

            Connections = RandomProvider.Next(minConnections, maxConnections + 1);
        }
    }
}
