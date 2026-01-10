using NetworkSimulation.Enums;
using NetworkSimulation.NetworkBase;

namespace NetworkSimulation.Factories
{
    public static class NetworkConfiguration
    {
        private static readonly Dictionary<GenType, NetworkStandard> _specs = new Dictionary<GenType, NetworkStandard>()
        {
            {
                GenType.G1,
                new NetworkStandard(
                    generation: GenType.G1,
                    maxRange: 20.0,
                    minRange: 2.0,
                    maxBandwidth: 0.01,
                    latency: 1000,
                    maxConnections: 50,
                    minConnections: 30)
            },
            {
                GenType.G2,
                new NetworkStandard(
                    generation: GenType.G2,
                    maxRange: 15.0,
                    minRange: 1.0,
                    maxBandwidth: 0.1,
                    latency: 500,
                    maxConnections: 200,
                    minConnections: 100)
            },
            {
                GenType.G3,
                new NetworkStandard(
                    generation: GenType.G3,
                    maxRange: 8.0,
                     minRange: 1.0,
                     maxBandwidth: 5,
                    latency: 150,
                    maxConnections: 1000,
                    minConnections: 400)
            },
            {
                GenType.G4,
                new NetworkStandard(
                    generation: GenType.G4,
                    maxRange: 5.0,
                    minRange : 1.0, maxBandwidth: 100,
                    latency: 150,
                    maxConnections: 5000,
                    minConnections: 1500)
            },
            {
                GenType.G5,
                new NetworkStandard(
                    generation: GenType.G5,
                    maxRange: 0.6,
                    minRange : 0.2,
                    maxBandwidth: 1000000,
                    latency: 1,
                    maxConnections: 1000000,
                    minConnections: 50000)
            }
        };

        public static NetworkStandard GetSpecs(GenType generation) => _specs[generation];

    }
}
