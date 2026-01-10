namespace NetworkSimulation.NetworkBase
{
    public class Position
    {
        private double xPosition;
        private double yPosition;

        public double XPosition => xPosition;
        public double YPosition => yPosition;

        public Position(double x, double y)
        {
            xPosition = x;
            yPosition = y;
        }

        public double DistanceTo(Position position)
        {
            var distanceX = xPosition - position.XPosition;
            var distanceY = yPosition - position.YPosition;
            return Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
        }
    }
}
