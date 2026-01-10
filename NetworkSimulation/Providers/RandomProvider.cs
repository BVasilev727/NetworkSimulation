using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation.Providers
{
    public static class RandomProvider
    {
        private static readonly Random rng = new Random();
        
        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// Thread-safe for use across the simulation.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number.</param>
        /// <returns>A random integer.</returns>
        public static int Next(int maxValue)
        {
            lock(rng)
            {
                return rng.Next(maxValue);
            }
        }

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// Thread-safe for use across the simulation.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound.</param>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random integer between minValue and maxValue-1.</returns>
        public static int Next(int minValue, int maxValue)
        {
            lock (rng)
            {
                return rng.Next(minValue, maxValue);
            }
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
        /// Thread-safe for use across the simulation.
        /// </summary>
        public static double NextDouble()
        {
            lock (rng)
            {
                return rng.NextDouble();
            }
        }
    }
}
