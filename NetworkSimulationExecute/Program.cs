using NetworkSimulation;
using NetworkSimulation.Enums;
using NetworkSimulation.Factories;
using NetworkSimulation.NetworkBase;

public class Program
{
    static void Main(string[] args)
    {
        var config = NetworkConfiguration.GetSpecs(generation: GenType.G5);

        var network = new Network(10, 10, 20000, config);
        Random rng = new Random();
        Position posA = new Position(rng.NextDouble() * 10, rng.NextDouble() * 10);
        Position posB = new Position(rng.NextDouble() * 10, rng.NextDouble() * 10);

        Phone phoneA = new Phone(posA, GenType.G5);
        Phone phoneB = new Phone(posB, GenType.G5);

        network.InitializeStaticNetwork();
        network.AssignSubscribersToTowers(new List<Phone> { phoneA, phoneB });  

        phoneA.SendMessage("hello", network, phoneB);
    }
}