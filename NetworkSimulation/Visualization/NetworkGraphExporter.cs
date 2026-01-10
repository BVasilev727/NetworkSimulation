using NetworkSimulation.NetworkBase;
using System.Text;

namespace NetworkSimulation.Visualization
{
    public class NetworkGraphExporter
    {
        private readonly double _height;
        private readonly double _width;

        public NetworkGraphExporter(double height, double width)
        {
            _height = height;
            _width = width;
        }

        public void ExportToSvg(string fileName, List<NetworkStation> allStations, List<NetworkStation> path)
        {
            double canvasWidth = 800; 
            double canvasHeight = 800;
            double margin = 50;

            double ratioX = canvasWidth / _width;
            double ratioY = canvasHeight / _height;

            Position senderPos = path[0].ConnectedPhones[0].Position;
            Position recipientPos = path.Last().ConnectedPhones[0].Position;
            
            double totalCanvasWidth = 1450;

            double fitRatio = Math.Min((canvasWidth - margin * 2) / _width, (canvasHeight - margin * 2) / _height);

            StringBuilder svg = new StringBuilder();
            svg.AppendLine($"<svg width='{totalCanvasWidth}' height='{canvasHeight}' " +
                   $"xmlns='http://www.w3.org/2000/svg' " +
                   $"style='background:#fcfcfc; border: 1px solid #ddd; margin: 20px; box-shadow: 2px 2px 10px rgba(0,0,0,0.1);'>"); 
            
            foreach (var station in allStations)
            {
                foreach (var neighbor in station.NeighboringStations)
                {
                    svg.AppendLine($"<line x1='{(station.Position.XPosition * fitRatio) + margin}' y1='{(station.Position.YPosition * fitRatio) + margin}' " +
                                   $"x2='{(neighbor.Position.XPosition * fitRatio) + margin}' y2='{(neighbor.Position.YPosition * fitRatio) + margin}' " +
                                   $"stroke='#e0e0e0' stroke-width='1' />");
                }
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                svg.AppendLine($"<line x1='{path[i].Position.XPosition * fitRatio + margin}' y1='{path[i].Position.YPosition * fitRatio + margin}' " +
                   $"x2='{path[i + 1].Position.XPosition * fitRatio + margin}' y2='{path[i + 1].Position.YPosition * fitRatio + margin}' " +
                   $"stroke='#ff4757' stroke-width='3' stroke-linecap='round' />");
            }

            foreach (var station in allStations)
            {
                string color = path.Contains(station) ? "#ff4757" : "#2f3542";
                svg.AppendLine($"<circle cx='{station.Position.XPosition * fitRatio + margin}' cy='{station.Position.YPosition * fitRatio + margin}' r='3' fill='{color}' />");
            }

            if (path.Count > 0)
            {
                svg.AppendLine($"<line x1='{senderPos.XPosition * fitRatio + margin}' y1='{senderPos.YPosition * fitRatio + margin}' " +
               $"x2='{path[0].Position.XPosition * fitRatio + margin}' y2='{path[0].Position.YPosition * fitRatio + margin}' " +
               $"stroke='#3498db' stroke-width='2' stroke-dasharray='4' />");
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                svg.AppendLine($"<line x1='{path[i].Position.XPosition * fitRatio + margin}' y1='{path[i].Position.YPosition * fitRatio + margin}' " +
                               $"x2='{path[i + 1].Position.XPosition * fitRatio + margin}' y2='{path[i + 1].Position.YPosition * fitRatio + margin}' " +
                               $"stroke='#ff4757' stroke-width='3' />");
            }

            if (path.Count > 0)
            {
                svg.AppendLine($"<line x1='{path.Last().Position.XPosition * fitRatio + margin}' y1='{path.Last().Position.YPosition * fitRatio + margin}' " +
                               $"x2='{recipientPos.XPosition * fitRatio + margin}' y2='{recipientPos.YPosition * fitRatio + margin}' " +
                               $"stroke='#2ecc71' stroke-width='2' stroke-dasharray='4' />");
            }

            svg.AppendLine($"<circle cx='{senderPos.XPosition * fitRatio + margin}' cy='{senderPos.YPosition * fitRatio + margin}' r='6' fill='#3498db' />");
            svg.AppendLine($"<circle cx='{recipientPos.XPosition * fitRatio + margin}' cy='{recipientPos.YPosition * fitRatio + margin}' r='6' fill='#2ecc71' />");
            
            DrawLegend(svg, canvasWidth, 50, senderPos, recipientPos);

            svg.AppendLine("</svg>");
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string exportDir = Path.Combine(documentsPath, "NetworkSimulation");

            Directory.CreateDirectory(exportDir);

            string finalPath = Path.Combine(exportDir, fileName);

            try
            {
                File.WriteAllText(finalPath, svg.ToString());
                Console.WriteLine($"Successfully saved to: {finalPath}");
            }
            catch (IOException)
            {
                Console.WriteLine("File is likely open in another program (Excel).");
            }
        }

        /// <summary>
        /// Draws the legend on the right of the graph
        /// </summary>
        /// <param name="svg">The svg up to this point</param>
        /// <param name="startX">Starter x coordinate for the legend</param>
        /// <param name="startY">Starter y coordinate fot the legend</param>
        /// <param name="senderPos"><see cref="Position"/> of the sender(phone)</param>
        /// <param name="recipientPos"><see cref="Position"/> of the recipient(phone)</param>
        private void DrawLegend(StringBuilder svg, double startX, double startY, Position senderPos, Position recipientPos)
        {
            double x = startX;
            double y = startY;
            double rowHeight = 30;

            svg.AppendLine($"<text x='{x}' y='{y}' font-family='Arial' font-size='16' font-weight='bold'>Legend</text>");
            y += 40;

            svg.AppendLine($"<circle cx='{x}' cy='{y}' r='6' fill='#3498db' />");
            svg.AppendLine($"<text x='{x + 20}' y='{y + 5}' font-family='Arial' font-size='14'>Sender (Phone)</text>");

            y += rowHeight;
            svg.AppendLine($"<circle cx='{x}' cy='{y}' r='6' fill='#2ecc71' />");
            svg.AppendLine($"<text x='{x + 20}' y='{y + 5}' font-family='Arial' font-size='14'>Recipient (Phone)</text>");

            y += rowHeight;
            svg.AppendLine($"<circle cx='{x}' cy='{y}' r='4' fill='#2f3542' />");
            svg.AppendLine($"<text x='{x + 20}' y='{y + 5}' font-family='Arial' font-size='14'>Network Station</text>");

            y += rowHeight;
            svg.AppendLine($"<line x1='{x - 10}' y1='{y}' x2='{x + 10}' y2='{y}' stroke='#3498db' stroke-width='2' stroke-dasharray='4' />");
            svg.AppendLine($"<text x='{x + 20}' y='{y + 5}' font-family='Arial' font-size='14'>Wireless Access Link</text>");

            y += rowHeight;
            svg.AppendLine($"<line x1='{x - 10}' y1='{y}' x2='{x + 10}' y2='{y}' stroke='#ff4757' stroke-width='3' />");
            svg.AppendLine($"<text x='{x + 20}' y='{y + 5}' font-family='Arial' font-size='14'>Message Path</text>");

            y += rowHeight;
            svg.AppendLine($"<text x='{x + 20}' y='{y + 5}' font-family='Arial' font-size='14'>Sender Position ({senderPos.XPosition},{senderPos.YPosition})</text>");

            y += rowHeight;
            svg.AppendLine($"<text x='{x + 20}' y='{y + 5}' font-family='Arial' font-size='14'>Sender Position ({recipientPos.XPosition},{recipientPos.YPosition})</text>");

        }
    }
}
