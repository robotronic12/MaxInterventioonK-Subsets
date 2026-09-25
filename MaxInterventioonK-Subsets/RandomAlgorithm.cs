using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaxInterventioonK_Subsets;

namespace MaxInterventionK_Subsets
{
    internal class RandomAlgorithm : AAlgorithm
    {
        private int _nIterations;

        public RandomAlgorithm(Instance instance, int n)
        {
            _algorithmName = "Random";
            _instance = instance;
            _nIterations = n;

            _bestSolution = new Solution(instance);
        }

        public override Solution Run()
        {
            for (int i = 0; i < _nIterations; i++)
            {
                Solution currentSolution = Construct(_instance);

                if (currentSolution.GetMaxInterrsection() > _bestSolution.GetMaxInterrsection())
                {
                    _bestSolution = currentSolution;
                }
            }

            return _bestSolution;
        }

        public override void SaveExperimentResults(string path)
        {
            ExperimentResult result = new ExperimentResult(
                _algorithmName,
                "",
                -1,
                _nIterations,
                0,
                _bestSolution.GetMaxInterrsection(),
                _bestSolution.GetElementCount(),
                _elapsedMilliseconds);

            result.SaveResultCSV(path);
        }

        private Solution Construct(Instance instance)
        {
            // random se crea en una clase estastica para poder aplicar semilla para todos los algoritmos
            Solution s = new Solution(instance);
            HashSet<int> candidateList = new HashSet<int>();
            for (int i = 0; i < instance.GetElementCount(); i++)
            {
                candidateList.Add(i);
            }

            while (s.GetElementCount() < instance.GetK() && candidateList.Count > 0)
            {
                int index = RandomManager.Next(candidateList.Count);
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
