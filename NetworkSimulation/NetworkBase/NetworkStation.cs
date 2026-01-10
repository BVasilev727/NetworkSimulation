using NetworkSimulation.Enums;
using NetworkSimulation.Factories;
using NetworkSimulation.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.NetworkBase
{
    public class NetworkStation
    {
        private GenType _genType;

        /// <summary>
        /// <see cref="Position"/>
        /// </summary>
        public Position Position {  get; private set; }
        public double CellRadius { get; private set; }

        public List<NetworkStation> NeighboringStations { get; private set; } = new List<NetworkStation>();

        public int MaxCapacity { get; private set; }

        public List<Phone> ConnectedPhones { get; } = new List<Phone>();

        public string Id { get; private set; }

        public NetworkStation(GenType genType, Position position)
        {
            _genType = genType;
            Position = position;

            var specs = NetworkConfiguration.GetSpecs(genType);
            CellRadius = specs.Range;
            MaxCapacity = specs.Connections;

            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Sets the <see cref="NeighboringStations"/>
        /// </summary>
        /// <param name="neighbors"></param>
        public void SetNeighbors(List<NetworkStation> neighbors)
        {
            NeighboringStations.Clear();
            NeighboringStations.AddRange(neighbors);
        }

        /// <summary>
        /// Passes the signal(phone message) from station to station.
        /// !!! Recursive method !!!
        /// </summary>
        /// <param name="msg"><see cref="Message"/></param>
        /// <param name="networkNoiseLevel">Noise to be applied to the message</param>
        public void ReceiveAndForward(Message msg, double networkNoiseLevel)
        {
            int entryHopCount = msg.HopCount;
            msg.HopCount++;
            Console.WriteLine($"the signal has hopped {msg.HopCount} times");

            ApplyNoise(msg.ModulatedSignal, networkNoiseLevel);

            if (msg.PlannedPath.Last() == this)
            {
                msg.Recipient.ReceiveMessage(msg);
                return;
            }

            var nextStation = msg.PlannedPath[entryHopCount];

            nextStation.ReceiveAndForward(msg, networkNoiseLevel);
        }

        private void ApplyNoise(double[] signal, double noiseLevel)
        {
            for (int i = 0; i < signal.Length; i++)
            {
                signal[i] += (RandomProvider.NextDouble() - 0.5) * noiseLevel;
            }
        }
    }
}
