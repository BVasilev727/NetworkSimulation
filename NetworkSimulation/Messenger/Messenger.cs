using NetworkSimulation.NetworkBase;

namespace NetworkSimulation.Messenger
{
    public static class Messenger
    {
        public static event EventHandler<TransferEventArgs> OnMessageReceived;

        public static void AnnounceMessageArrival(double[] signal, List<NetworkStation> path)
        {
            OnMessageReceived?.Invoke(null, new TransferEventArgs(signal, path));
        }
    }
}
