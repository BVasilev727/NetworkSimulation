using NetworkSimulation.Enums;
using NetworkSimulation.Messenger;
using NetworkSimulation.Modulation;
using System.Text;

namespace NetworkSimulation.NetworkBase
{
    public class Phone
    {
        private NetworkStation _connectedStation;

        public GenType GenType { get; private set; }

        public Position Position { get; private set; }

        public NetworkStation? ConnectedStation => _connectedStation;

        private readonly Modulator _modem;

        public Phone(Position position, GenType genType)
        {
            Position = position;
            GenType = genType;
            _modem = ModulatorFactory.GetModulator(genType);
        }

        /// <summary>
        /// Adds the phone to the <see cref="NetworkStation.ConnectedPhones"/> based on distance
        /// </summary>
        /// <param name="stations">All stations of the network</param>
        public void ConnectToNearestStation(List<NetworkStation> stations)
        {
            double minDistance = double.MaxValue;
            NetworkStation nearestStation = null;

            foreach (var station in stations)
            {
                double distance = Position.DistanceTo(station.Position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestStation = station;
                }
            }

            if (nearestStation != null && Position.DistanceTo(nearestStation.Position) <= nearestStation.CellRadius)
            {
                _connectedStation = nearestStation;
                nearestStation.ConnectedPhones.Add(this);
                Console.WriteLine($"Phone connected to station at ({nearestStation.Position.XPosition}, {nearestStation.Position.YPosition}) ");
            }
        }

        /// <summary>
        /// Creates <see cref="Message"/> from the text string
        /// </summary>
        /// <param name="recipient"><see cref="Phone"/> Recipient</param>
        /// <param name="text"></param>
        /// <param name="path"></param>
        /// <returns><see cref="Message"/></returns>
        public Message PrepareMessage(Phone recipient, string text, List<NetworkStation> path)
        {
            byte[] rawData = Encoding.UTF8.GetBytes(text);
            double[] signal = _modem.Modulate(rawData);

            return new Message(this, recipient, signal, path)
            {
                OriginalData = rawData
            };    
        }

        /// <summary>
        /// Calls <see cref="Network.SendMessage(Phone, Phone, string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="network"></param>
        /// <param name="recipient"></param>
        public void SendMessage(string message, Network network, Phone recipient)
        {
            network.SendMessage(this, recipient, message);
        }

        /// <summary>
        /// Called when the recipient <see cref="Phone"/> should receive the signal
        /// </summary>
        /// <param name="message">The signal(phone message)</param>
        public void ReceiveMessage(Message message)
        {
            byte[] recoveredBytes = _modem.Demodulate(message.ModulatedSignal);

            string decodedText = Encoding.UTF8.GetString(recoveredBytes);

            double ber = CalculateBER(message.OriginalData, recoveredBytes);
          
            Console.WriteLine($"This phone received '{decodedText}' | BER: {ber:P2}");

            Messenger.Messenger.AnnounceMessageArrival(message.ModulatedSignal, message.PlannedPath);
        }

        /// <summary>
        /// Calculates the Bit Error Rate
        /// </summary>
        /// <param name="original">The original <see cref="Message"/></param>
        /// <param name="recovered">The recovered <see cref="Message"/></param>
        /// <returns></returns>
        private double CalculateBER(byte[] original, byte[] recovered)
        {
            if (original.Length == 0) return 0;
            int errors = 0;
            int length = Math.Min(original.Length, recovered.Length);

            for (int i = 0; i < length; i++)
            {
                if (original[i] != recovered[i]) errors++;
            }
            return (double)errors / original.Length;
        }
    }
}
