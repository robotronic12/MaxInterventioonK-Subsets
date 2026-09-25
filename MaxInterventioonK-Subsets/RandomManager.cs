using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxInterventioonK_Subsets
{
    public static class RandomManager
    {
        private static Random _random = new Random();
        private static int _seed = -1;

        public static void SetSeed(int seed)
        {
            _random = new Random(seed);
            _seed = seed;
        }

        public static int GetSeed()
        {
            return _seed;
        }

        public static int Next(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static int Next(int max)
        {
            return _random.Next(max);
        }

        public static float Value()
        {
            return (float)_random.NextDouble();
        }

        public static double ValueDouble()
        {
            return _random.NextDouble();
        }

        public static bool Bool()
        {
            return _random.Next(0, 2) == 1;
        }
    }
}
