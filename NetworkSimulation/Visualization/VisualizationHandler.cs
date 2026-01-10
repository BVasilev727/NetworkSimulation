using NetworkSimulation.Messenger;
using NetworkSimulation.NetworkBase;

namespace NetworkSimulation.Visualization
{
    public class VisualizationHandler
    {
        private readonly NetworkGraphExporter _graphExporter;
        private readonly WaveformExporter _waveExporter;
        private readonly List<NetworkStation> _allStations;
        private readonly WaveformViewer _waveformViewer;


        public VisualizationHandler(double areaHeight, double areaWidth, List<NetworkStation> allStations)
        {
            _allStations = allStations;
            _graphExporter = new NetworkGraphExporter(areaHeight, areaWidth);
            _waveExporter = new WaveformExporter();
            _waveformViewer = new WaveformViewer(); 
         
            Messenger.Messenger.OnMessageReceived += HandleDrawing;
        }

        private void HandleDrawing(object? sender, TransferEventArgs e)
        {
            _graphExporter.ExportToSvg("path.svg", _allStations, e.Path);

            _waveExporter.ExportToCsv("waveform.csv", e.FinalSignal);


        }
    }
}
