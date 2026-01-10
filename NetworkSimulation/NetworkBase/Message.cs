using NetworkSimulation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.NetworkBase
{
    public class Message
    {
        /// <summary>
        /// Modulated signal(phone message)
        /// </summary>
        public double[] ModulatedSignal { get; set; }
        
        /// <summary>
        /// Signal(phone message) before the modulation.
        /// Used for calculating BER(Bit Error Rate)
        /// </summary>
        public byte[] OriginalData { get; init; }

        public Phone Sender { get; }
        public Phone Recipient { get; }
        public int HopCount { get; set; } = 0;

        public List<NetworkStation> PlannedPath { get; set; }

        public int MaxHops { get; private set; } = 15;

        public HashSet<string> VisitedStations { get; } = new HashSet<string>();

        public Message(Phone sender, Phone recipient, double[] signal, List<NetworkStation> path)
        {
            Sender = sender;
            Recipient = recipient;
            ModulatedSignal = signal;
            if (sender.GenType == GenType.G5)
            {
                MaxHops = 50;
            }
            PlannedPath = path;
        }

    }
}
