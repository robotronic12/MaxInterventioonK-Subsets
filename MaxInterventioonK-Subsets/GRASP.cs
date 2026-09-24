using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MaxInterventionK_Subsets
{
    internal class GRASP : AAlgorithm
    {
        private float _alpha;
        private int _nIterations;
        private int _nImprovements;
        private GraspType _graspType;

        public GRASP(Instance instance, float alpha, int nIterations, int nImprovements, GraspType GraspType) 
        {
            AlgorithmName = GraspType.ToString();
            _alpha = alpha;
            _nIterations = nIterations;
            _nImprovements = nImprovements;
            _graspType = GraspType;
            _instance = instance;

            if (_alpha > 1)
                throw new ArgumentOutOfRangeException(nameof(_alpha), "Alpha must be lower than 1");

            if (_alpha < 0)
                _alpha = (float)new Random().NextDouble();

            _bestSolution = new Solution(instance);
        }
        public GRASP(Instance instance, float alpha, int nIterations, int nImprovements) : this(instance, alpha, nIterations, nImprovements, GraspType.GRASP)
        {
        }
        public GRASP(Instance instance, float alpha) : this(instance, alpha, 100, 1, GraspType.GRASP)
        {
        }

        public override Solution Run()
        {
            for (int i = 0; i < _nIterations; i++)
            {
                Solution currentSolution = Construct();
                currentSolution = Improve(currentSolution);

                if (currentSolution.GetMaxInterrsection() > _bestSolution.GetMaxInterrsection())
                {
                    _bestSolution = currentSolution;
                }
            }
            return _bestSolution;
        }

        private Solution Construct()
        {
            Solution s = new Solution(_instance);

            HashSet<int> candidateList = new HashSet<int>();
            for (int i = 0; i < _instance.GetElementCount(); i++)
            {
                candidateList.Add(i);
            }

            int element = new Random().Next(_instance.GetElementCount());
            candidateList.Remove(element);
            s.AddElement(element);

            while (s.GetElementCount() < _instance.GetK())
            {
                float threshold = CalculateThreshold(s, candidateList);

                HashSet<int> restrictedCandidateList = new HashSet<int>();
                PriorityList<int> priorityList = new PriorityList<int>();
                int totalGreedyValue = 0;

                foreach (int i in candidateList)
                {
                    int gValue = GetGreedyValue(s, i);
                    if (gValue >= threshold)
                    {
                        restrictedCandidateList.Add(i);

                        totalGreedyValue += gValue;
                        priorityList.Add(i, gValue);
                    }
                }
                if (restrictedCandidateList.Count == 0)
                    break;

                if (_graspType == GraspType.GRASP)
                {
                    BaseGrasp(s, candidateList, restrictedCandidateList);
                }
                else if (_graspType == GraspType.BiasedGRASP)
                {
                    BiasedGrasp(s, candidateList, totalGreedyValue, priorityList);
                }
                else
                {
                    throw new NotImplementedException();
                }
                
            }

            return s;
        }

        private Solution Improve(Solution currentSolution)
        {
            Solution bestSolution = currentSolution;
            Solution improvedSolution = currentSolution;

            bool improved = false;
            do
            {
                // First Improvement / Best Improvement

            } while (improved);

            return bestSolution;
        }

        private Solution BaseGrasp(Solution s, HashSet<int> candidateList, HashSet<int> restrictedCandidateList)
        {
            int index = new Random().Next(restrictedCandidateList.Count);
            int element = restrictedCandidateList.ElementAt(index);

            restrictedCandidateList.Remove(element);
            candidateList.Remove(element);
            s.AddElement(element);

            return s;
        }

        private Solution BiasedGrasp(Solution s, HashSet<int> candidateList, int totalGreedyValue, PriorityList<int> priorityList)
        {
            int element = -1;
            float r = new Random().Next(0, totalGreedyValue) * _alpha;
            int acumulatedGreedyValue = 0;
            while (element < 0)
            {
                var candidate = priorityList.Remove();
                acumulatedGreedyValue += candidate.Priority;
                if (acumulatedGreedyValue >= r)
                {
                    element = candidate.ElementId;
                }
            }

            candidateList.Remove(element);
            s.AddElement(element);
            return s;
        }

        private float CalculateThreshold(Solution s, HashSet<int> candidateList)
        {
            int gMin = int.MaxValue;
            int gMax = int.MinValue;

            foreach (int i in candidateList)
            {
                int gValue = GetGreedyValue(s, i);
                gMin = Math.Min(gMin, gValue);
                gMax = Math.Max(gMax, gValue);
            }

            if (gMin == int.MaxValue)
            {
                throw new InvalidOperationException("No candidates available to calculate threshold.");
            }

            return gMax - _alpha * (gMax - gMin);
        }

        private int GetGreedyValue(Solution solution, int elementId)
        {
            return solution.GetMaxInterrsectionWithElement(elementId);
        }
    }

    public enum GraspType
    {
        GRASP,
        BiasedGRASP
    }
}
