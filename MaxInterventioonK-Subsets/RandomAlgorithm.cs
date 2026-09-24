using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxInterventioonK_Subsets
{
    internal class RandomAlgorithm : AAlgorithm
    {
        private float _n;

        public RandomAlgorithm(Instance instance, int n)
        {
            AlgorithmName = "Random";
            _instance = instance;
            _n = n;

            _bestSolution = new Solution(instance);
        }

        public override Solution Run()
        {
            for (int i = 0; i < _n; i++)
            {
                Solution currentSolution = Construct(_instance);

                if (currentSolution.GetMaxInterrsection() > _bestSolution.GetMaxInterrsection())
                {
                    _bestSolution = currentSolution;
                }
            }

            return _bestSolution;
        }

        private Solution Construct(Instance instance)
        {
            // random se crea en una clase estastica para poder aplicar semilla para todos los algoritmos
            Solution s = new Solution(instance);

            Random random = new Random();
            HashSet<int> candidateList = new HashSet<int>();
            for (int i = 0; i < instance.GetElementCount(); i++)
            {
                candidateList.Add(i);
            }

            while (s.GetElementCount() < instance.GetK() && candidateList.Count > 0)
            {
                int index = random.Next(candidateList.Count);
                int element = candidateList.ElementAt(index);
                if(s.GetMaxInterrsectionWithElement(element) > 0 || s.GetElementCount() == 0)
                {
                    s.AddElement(element);
                }
                candidateList.Remove(element);
            }

            return s;
        }
    }
}
