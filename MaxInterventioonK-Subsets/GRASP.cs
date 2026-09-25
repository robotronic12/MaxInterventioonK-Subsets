using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MaxInterventioonK_Subsets;

namespace MaxInterventionK_Subsets
{
    internal class GRASP : AAlgorithm
    {
        private float _alpha;
        private int _nIterations;
        private int _nImprovements;
        private GraspType _graspType;
        private ImprovementType _improvementType;

        public GRASP(Instance instance, float alpha, int nIterations, int nImprovements, GraspType GraspType, ImprovementType improvementType) 
        {
            if (alpha > 1)
                throw new ArgumentOutOfRangeException(nameof(_alpha), "Alpha must be lower than 1");

            _alpha = alpha;
            _nIterations = nIterations;
            _nImprovements = nImprovements;
            _graspType = GraspType;
            _improvementType = improvementType;

            _algorithmName = _graspType.ToString();
            _extraInfo = $"Improvement Type: {_improvementType.ToString()}\n";

            _instance = instance;
            

            if (_alpha < 0)
                _alpha = RandomManager.Value();

            _bestSolution = new Solution(instance);
        }

        public GRASP(Instance instance, float alpha, int nIterations, int nImprovements) : 
            this(instance, alpha, nIterations, nImprovements, GraspType.GRASP, ImprovementType.FirstImprovement)
        {
        }

        public GRASP(Instance instance, float alpha) : 
            this(instance, alpha, 100, 1, GraspType.GRASP, ImprovementType.FirstImprovement)
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

        public override void SaveExperimentResults(string path)
        {
            ExperimentResult result = new ExperimentResult(
                _algorithmName, 
                _improvementType.ToString(), 
                _alpha, 
                _nIterations, 
                _nImprovements, 
                _bestSolution.GetMaxInterrsection(), 
                _bestSolution.GetElementCount(), 
                _elapsedMilliseconds);

            result.SaveResultCSV(path);
        }

        private Solution Construct()
        {
            Solution s = new Solution(_instance);

            HashSet<int> candidateList = new HashSet<int>();
            for (int i = 0; i < _instance.GetElementCount(); i++)
            {
                candidateList.Add(i);
            }

            int element = RandomManager.Next(_instance.GetElementCount());
            candidateList.Remove(element);
            s.AddElement(element);

            CalculateNextElement(s, candidateList);

            return s;
        }

        private Solution Improve(Solution solution)
        {
            Solution bestSol = solution.Clone();

            bool improved = false;
            do
            {
                improved = false;

                Solution sol = bestSol.Clone();
                HashSet<int> blackListedElements = new HashSet<int>();
                HashSet<int> candidateElements = new HashSet<int>();
                for (int i = 0; i < _instance.GetElementCount(); i++)
                {
                    if (!sol.GetElements().Contains(i))
                    {
                        candidateElements.Add(i);
                    }
                }

                for (int i = 0; i < _nImprovements; i++)
                {
                    // We remove a random element from the solution
                    int randomElementId;
                    int randomElement;
                    do
                    {
                        randomElementId = RandomManager.Next(sol.GetElementCount());
                        randomElement = sol.GetElements().ElementAt(randomElementId);
                    } while (!blackListedElements.Add(randomElement));

                    sol.RemoveElement(randomElement);

                    Solution improvedSol;
                    // We add a random element from the candidate list
                    switch (_improvementType)
                    {
                        case ImprovementType.FirstImprovement:
                            improvedSol = sol.Clone();
                            CalculateNextElement(improvedSol, candidateElements);

                            if (improvedSol.GetMaxInterrsection() > sol.GetMaxInterrsection())
                            {
                                improved = true;
                                sol = improvedSol;
                            }
                            
                            break;

                        case ImprovementType.BestImprovement:
                            int actualElement;
                            int actualSolInttersecttion;
                            int bestElement = -1;
                            int bestSolIntersection = -1;

                            do
                            {
                                improvedSol = sol.Clone();
                                actualElement = CalculateNextElement(improvedSol, candidateElements);
                                actualSolInttersecttion = improvedSol.GetMaxInterrsection();

                                if (actualSolInttersecttion > sol.GetMaxInterrsection())
                                {
                                    improved = true;
                                    bestSolIntersection = actualSolInttersecttion;
                                    bestElement = actualElement;
                                }
                            } while (bestElement > 0 && candidateElements.Count > 0);

                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            } while (improved);

            return bestSol;
            }

        private int CalculateNextElement(Solution s, HashSet<int> candidateList)
        {
            int element = -1;
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

                
                switch (_graspType)
                {
                    case GraspType.GRASP:
                        element = BaseGrasp(s, candidateList, restrictedCandidateList);
                        break;

                    case GraspType.BiasedGRASP:
                        element = BiasedGrasp(s, candidateList, totalGreedyValue, priorityList);
                        break;

                    default:
                        throw new NotImplementedException();
                }  
            }

            return element;
        }

        private int BaseGrasp(Solution s, HashSet<int> candidateList, HashSet<int> restrictedCandidateList)
        {
            int index = RandomManager.Next(restrictedCandidateList.Count);
            int element = restrictedCandidateList.ElementAt(index);

            restrictedCandidateList.Remove(element);
            candidateList.Remove(element);
            s.AddElement(element);

            return element;
        }

        private int BiasedGrasp(Solution s, HashSet<int> candidateList, int totalGreedyValue, PriorityList<int> priorityList)
        {
            int element = -1;
            float r = RandomManager.Next(totalGreedyValue) * _alpha;
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
            return element;
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

    public enum ImprovementType
    {
        FirstImprovement,
        BestImprovement
    }
}
