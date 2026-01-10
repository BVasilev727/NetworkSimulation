using NetworkSimulation.Enums;
using NetworkSimulation.NetworkBase;
using NetworkSimulation.Visualization;
using System.Net;

namespace NetworkSimulation
{
    public class Network
    {
        private readonly double _areaHeight;
        private readonly double _areaWidth;
        private readonly double _squareArea;

        private int _subscribers;
        private NetworkStandard _networkStandard;

        List<NetworkStation> Stations;

        public double NoiseLevel { get; private set; } = 0.03;

        public ulong WorkingFrequency { get; private set; } = 5000;

        public Network(double areaHeight, double areaWidth, int subscribers, NetworkStandard networkStandard)
        {
            _areaHeight = areaHeight;
            _areaWidth = areaWidth;
            _subscribers = subscribers;
            _networkStandard = networkStandard;
        }

        public Network(double squareArea, int subscribers, NetworkStandard networkStandard)
        {
            _squareArea = squareArea;
            _subscribers = subscribers;
            _networkStandard = networkStandard;
        }

        public void SetNoise(double noise)
        {        
            NoiseLevel = Math.Max(default, Math.Min(1.0, noise));        
        }

        public void SetWorkingFrequency(ulong frequency)
        {
            if(frequency > 0)
            {
                WorkingFrequency = frequency;
            }
        }

        /// <summary>
        /// Creates the network.
        /// Organizes the stations on a grid-like manner and sets <see cref="NetworkStation.NeighboringStations"/>
        /// </summary>
        public void InitializeStaticNetwork()
        {
            Stations = new List<NetworkStation>();
            double stride = _networkStandard.Range * 1.5;

            int cols = (int)Math.Ceiling(_areaWidth / stride) + 1;
            int rows = (int)Math.Ceiling(_areaHeight / stride) + 1;

            NetworkStation[,] grid = new NetworkStation[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Position pos = new Position(j * stride, i * stride);

                    if (pos.XPosition <= _areaWidth && pos.YPosition <= _areaHeight)
                    {
                        var station = new NetworkStation(_networkStandard.Generation, pos);
                        grid[i, j] = station;
                        Stations.Add(station);
                    }
                }
            }

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    NetworkStation current = grid[r, c];
                    if (current == null) continue;

                    for (int dr = -1; dr <= 1; dr++)
                    {
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            if (dr == 0 && dc == 0) continue;

                            int nr = r + dr;
                            int nc = c + dc;

                            if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                            {
                                NetworkStation neighbor = grid[nr, nc];
                                if (neighbor != null)
                                {
                                    current.NeighboringStations.Add(neighbor);
                                }
                            }
                        }
                    }
                }
            }

            InitializeVisualization();
        }

        /// <summary>
        /// Determines the shortest path to the recipient station
        /// </summary>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        public List<NetworkStation> FindShortestPath(NetworkStation start, NetworkStation finish)
        {
            var path = new List<NetworkStation>();
            if (start == null || finish == null) return path;

            var priorityQueue = new PriorityQueue<NetworkStation, double>();
            var cameFrom = new Dictionary<NetworkStation, NetworkStation>();

            priorityQueue.Enqueue(start, 0);
            cameFrom[start] = null;

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();

                if (current == finish) break;

                foreach (var neighbor in current.NeighboringStations)
                {
                    if (!cameFrom.ContainsKey(neighbor))
                    {
                        cameFrom[neighbor] = current;

                        double distanceToTarget = neighbor.Position.DistanceTo(finish.Position);
                        priorityQueue.Enqueue(neighbor, distanceToTarget);
                    }
                }
            }

            if (!cameFrom.ContainsKey(finish)) return path;

            NetworkStation step = finish;
            while (step != null)
            {
                path.Add(step);
                step = cameFrom[step];
            }
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Assigns/connects a single <see cref="Phone"/> to a station
        /// </summary>
        /// <param name="phone"></param>
        public void AssignSubscriberToTower(Phone phone)
        {
            phone.ConnectToNearestStation(Stations);
        }

        /// <summary>
        /// Assigns/connects multiple <see cref="Phone"/>s to a station
        /// </summary>
        /// <param name="phones"></param>
        public void AssignSubscribersToTowers(List<Phone> phones)
        {
            foreach (Phone phone in phones)
            {
                AssignSubscriberToTower(phone);
            }
        }

        /// <summary>
        /// Sends a message thru the network
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="recipient"></param>
        /// <param name="text"></param>
        public void SendMessage(Phone sender, Phone recipient, string text)
        {
            if (sender.ConnectedStation == null)
            {
                Console.WriteLine("Message Failed: Sender not connected to any station");
                return;
            }

            if (recipient.ConnectedStation == null)
            {
                Console.WriteLine("Message Failed: Recipient not connected to any station");
                return;
            }

            var path = FindShortestPath(sender.ConnectedStation, recipient.ConnectedStation);   
            Message message = sender.PrepareMessage(recipient, text, path);
            sender.ConnectedStation.ReceiveAndForward(message, NoiseLevel);

            if (sender.ConnectedStation == recipient.ConnectedStation)
            {
                Console.WriteLine("Phones connected to the same station. Delivering immediately");
                return;
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="VisualizationHandler"/>
        /// </summary>
        private void InitializeVisualization()
        {
            _ = new VisualizationHandler(_areaHeight, _areaHeight, Stations);
        }
    }
}
