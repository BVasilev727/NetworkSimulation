using NetworkSimulation.NetworkBase;

namespace NetworkSimulation.Messenger
{
    public static class Messenger
    {
        /// <summary>
        /// Static event for received message
        /// </summary>
        public static event EventHandler<TransferEventArgs> OnMessageReceived;

        /// <summary>
        /// Triggers the <see cref="OnMessageReceived"/>
        /// </summary>
        /// <param name="signal">The signal(phone message) as a double[]</param>
        /// <param name="path">The path of the phone message</param>
        public static void AnnounceMessageArrival(double[] signal, List<NetworkStation> path)
        {
            OnMessageReceived?.Invoke(null, new TransferEventArgs(signal, path));
        }
    }
}
