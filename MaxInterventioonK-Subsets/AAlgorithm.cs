using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxInterventionK_Subsets
{
    internal abstract class AAlgorithm : IAlgorithm
    {
        protected Instance _instance;
        protected Solution _bestSolution;

        public string AlgorithmName { get; protected set; }

        public abstract Solution Run();
        public string PrintSolution()
        {
            string str = string.Empty;

            str += "------------------------------\n";
            str += $"Algorithm: {AlgorithmName}\n";
            str += $"Elements in solution: {_bestSolution.GetElementCount()}\n";

            List<int> elements = new List<int>(_bestSolution.GetElements());
            elements.Sort();

            foreach (int element in elements)
            {
                str += $"\tElement: {element + 1}\n";
            }

            str += $"Max Intersections found: {_bestSolution.GetMaxInterrsection()}\n";

            foreach (int feature in _bestSolution.GetCommonFeatures())
            {
                str += $"\tFeature: {feature + 1}\n";
            }

            Console.Write(str);

            return str;
        }

        public string PrintSummarySolution()
        {
            string str = string.Empty;

            str += "------------------------------\n";
            str += $"Algorithm: {AlgorithmName}\n";
            str += $"Elements in solution: {_bestSolution.GetElementCount()}\n";
            str += $"Max Intersections found: {_bestSolution.GetMaxInterrsection()}\n";

            Console.Write(str);

            return str;
        }
    }
}
