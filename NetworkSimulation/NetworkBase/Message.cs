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
        public double[] ModulatedSignal { get; set; }
        
        public byte[] OriginalData { get; init; }

        public Phone Sender { get; }
        public Phone Recipient { get; }
        public int HopCount { get; set; } = 0;

        public List<NetworkStation> PlannedPath { get; set; }

        public int MaxHops { get; private set; } = 15;

        public HashSet<string> VisitedStations { get; } = new HashSet<string>();

        public Message(Phone sender, Phone recipient, double[] signal, byte[] originalData, List<NetworkStation> path)
        {
            Sender = sender;
            Recipient = recipient;
            ModulatedSignal = signal;
            OriginalData = originalData;
            if(sender.GenType == GenType.G5)
            {
                MaxHops = 50;
            }
            PlannedPath = path;
        }

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
