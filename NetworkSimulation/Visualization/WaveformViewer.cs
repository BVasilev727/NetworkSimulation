using NetworkSimulation.Messenger;
using ScottPlot;

namespace NetworkSimulation.Visualization
{
    public class WaveformViewer
    {
        private readonly double _samplingRate;

        public WaveformViewer(double samplingRate = 44100)
        {
            _samplingRate = samplingRate;

            Messenger.Messenger.OnMessageReceived += ShowPlot;
        }

        private void ShowPlot(object? sender, TransferEventArgs e)
        {
            ScottPlot.Plot plt = new();
            
            double[] signal = e.FinalSignal;
            double[] times = new double[signal.Length];
            for (int i = 0; i < times.Length; i++)
                times[i] = i / _samplingRate;

            plt.Add.Scatter(times, signal);
            plt.Title($"Waveform for Message");
            plt.XLabel("Time (seconds)");
            plt.YLabel("Amplitude");

            string windowPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                "NetworkSimulation", $"plot.png");
            plt.SavePng(windowPath, 800, 400);

            Console.WriteLine($"Plot image generated at: {windowPath}");
        }
    }
}
