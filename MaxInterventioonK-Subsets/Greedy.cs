using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MaxInterventionK_Subsets
{
    internal class Greedy : AAlgorithm
    {
        public Greedy(Instance instance)
        {
            AlgorithmName = "Greedy";
            _instance = instance;

            _bestSolution = new Solution(instance);
        }

        public override Solution Run()
        {
            _bestSolution = Construct(_instance);
            return _bestSolution;
        }

        private Solution Construct(Instance instance)
        {
            Solution s = new Solution(instance);
            HashSet<int> candidateElements = new HashSet<int>(Enumerable.Range(0, instance.GetElementCount()));

            int bestElement = -1;
            int bestGreedyValue = -1;

            foreach (int e in candidateElements)
            {
                int edgeCount = instance.GetFeaturesByElement(e).Count();
                if (edgeCount > bestGreedyValue)
                {
                    bestElement = e;
                    bestGreedyValue = edgeCount;
                }
            }

            candidateElements.Remove(bestElement);
            s.AddElement(bestElement);

            while (s.GetElementCount() < instance.GetK())
            {
                bestElement = -1;
                bestGreedyValue = -1;

                foreach (int e in candidateElements)
                {
                    int greedyValue = s.GetMaxInterrsectionWithElement(e);
                    if (greedyValue > bestGreedyValue)
                    {
                        bestElement = e;
                        bestGreedyValue = greedyValue;
                    }
                }

                s.AddElement(bestElement);
                candidateElements.Remove(bestElement);
            }

            return s;
        }
    }
}
