using NetworkSimulation.NetworkBase;
namespace NetworkSimulation.Messenger
{
    public class TransferEventArgs : EventArgs
    {
        /// <summary>
        /// The signal(phone message)
        /// </summary>
        public double[] Signal { get; }

        /// <summary>
        /// The path of the signal(phone message)
        /// </summary>
        public List<NetworkStation> Path;

        public TransferEventArgs(double[] signal, List<NetworkStation> path)
        {
            Signal = signal;
            Path = path;
        }
    }
}
