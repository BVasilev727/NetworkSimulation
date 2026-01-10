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

        public void InitializeNetwork(Phone phone1, Phone phone2)
        {
            Stations = new List<NetworkStation>();

            int multiplier = phone1.GenType == GenType.G5 ? 4 : 1;

            int stationCount = (int)(Math.Ceiling(phone1.Position.DistanceTo(phone2.Position)) + 1) * multiplier;

            for (int i = 0; i < stationCount; i++)
            {
                double lerpFactor = (double)i / (stationCount - 1);
                Position stationPosition = new Position(
                    phone1.Position.XPosition + (phone2.Position.XPosition - phone1.Position.XPosition) * lerpFactor,
                    phone1.Position.YPosition + (phone2.Position.YPosition - phone1.Position.YPosition) * lerpFactor);

                Stations.Add(new NetworkStation(_networkStandard.Generation, stationPosition));
            }

            double neighborThreshold = _networkStandard.Range * 1.5;

            foreach (var station in Stations)
            {
                var nearbyStations = Stations.Where(x => x!= station && 
                    station.Position.DistanceTo(x.Position) <= neighborThreshold);

                station.SetNeighbors(nearbyStations.ToList());
            }

            AssignSubscribersToTowers(new List<Phone> { phone1, phone2 });
            InitializeVisualization();
        }

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

        public void AssignSubscriberToTower(Phone phone)
        {
            phone.ConnectToNearestStation(Stations);
        }

        public void AssignSubscribersToTowers(List<Phone> phones)
        {
            foreach (Phone phone in phones)
            {
                AssignSubscriberToTower(phone);
            }
        }

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

        private void InitializeVisualization()
        {
            _ = new VisualizationHandler(_areaHeight, _areaHeight, Stations);
        }
    }
}
