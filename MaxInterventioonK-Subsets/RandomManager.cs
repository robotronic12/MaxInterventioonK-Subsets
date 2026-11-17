using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxInterventioonK_Subsets
{
    public class RandomManager
    {
        private static Random _random;
        private static int _seed;
        private static RandomType _randomType;

        public RandomManager()
        {
            _seed = 1234;
            _random = new Random(_seed);
            _randomType = RandomType.Default;
        }

        public void SetSeed(int seed)
        {
            _random = new Random(seed);
            _seed = seed;
        }

        public void SetRandomType(RandomType randomType)
        {
            _randomType = randomType;
        }

        public int GetSeed()
        {
            return _seed;
        }

        public int Next(int min, int max)
        {
            switch (_randomType)
            {
                case RandomType.Geometric:
                    return Math.Min(_random.Next(min, max), _random.Next(min, max));

                case RandomType.Default:
                    return _random.Next(min, max);

                default:
                    throw new NotImplementedException();
            }
            
        }

        public int Next(int max)
        {
            switch (_randomType)
            {
                case RandomType.Geometric:
                    return Math.Min(_random.Next(max), _random.Next(max));

                case RandomType.Default:
                    return _random.Next(max);

                default:
                    throw new NotImplementedException();
            }
        }

        public float Value()
        {
            switch (_randomType)
            {
                case RandomType.Geometric:
                    return Math.Min((float)_random.NextDouble(), (float)_random.NextDouble());

                case RandomType.Default:
                    return (float)_random.NextDouble();

                default:
                    throw new NotImplementedException();
            }
        }

        public double ValueDouble()
        {
            switch (_randomType)
            {
                case RandomType.Geometric:
                    return Math.Min(_random.NextDouble(), _random.NextDouble());

                case RandomType.Default:
                    return _random.NextDouble();

                default:
                    throw new NotImplementedException();
            }
        }

        public bool Bool()
        {
            return _random.Next(0, 2) == 1;
        }
    }

    public enum RandomType
    {
        Default,
        Geometric
    }
}
