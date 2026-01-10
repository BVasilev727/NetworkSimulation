using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.Visualization
{
    public class WaveformExporter
    {
        private readonly double _samplingRate;

        public WaveformExporter(double samplingRate = 44100)
        {
            _samplingRate = samplingRate;
        }

        /// <summary>
        /// Creates a csv file based on the signal
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="signal"></param>
        public void ExportToCsv(string fileName, double[] signal)
        {
            double deltaT = 1.0 / _samplingRate;

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string exportDir = Path.Combine(documentsPath, "NetworkSimulation");

            Directory.CreateDirectory(exportDir);

            string finalPath = Path.Combine(exportDir, fileName);

            try
            {
                using (var writer = new StreamWriter(finalPath))
                {
                    writer.WriteLine("Time(s),Amplitude");
                    for (int i = 0; i < signal.Length; i++)
                    {
                        writer.WriteLine($"{i * deltaT},{signal[i]}");
                    }
                }
                Console.WriteLine($"Successfully saved to: {finalPath}");
            }
            catch (IOException)
            {
                Console.WriteLine("File is likely open in another program (Excel).");
            }
        }
    }
}
