using NetworkSimulation;
using NetworkSimulation.Enums;
using NetworkSimulation.Factories;
using NetworkSimulation.NetworkBase;

public class Program
{
    static void Main(string[] args)
    {
        var config = NetworkConfiguration.GetSpecs(generation: GenType.G5);

        double networkWidth = 10;
        double networkHeight = 10;

        var network = new Network(networkHeight, networkWidth, 20000, config);
        Random rng = new Random();
        Position posA = new Position(rng.NextDouble() * networkHeight, rng.NextDouble() * networkWidth);
        Position posB = new Position(rng.NextDouble() * networkHeight, rng.NextDouble() * networkWidth);

        Phone phoneA = new Phone(posA, GenType.G5);
        Phone phoneB = new Phone(posB, GenType.G5);

        network.InitializeStaticNetwork();
        network.AssignSubscribersToTowers(new List<Phone> { phoneA, phoneB });  

        phoneA.SendMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. ", network, phoneB);
    }
}