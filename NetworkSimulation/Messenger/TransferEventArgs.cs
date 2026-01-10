using NetworkSimulation.NetworkBase;
namespace NetworkSimulation.Messenger
{
    public class TransferEventArgs : EventArgs
    {
        public double[] FinalSignal { get; }

        public List<NetworkStation> Path;

        public TransferEventArgs(double[] signal, List<NetworkStation> path)
        {
            FinalSignal = signal;
            Path = path;
        }
    }
}
