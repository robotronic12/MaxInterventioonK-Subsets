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
        private RandomManager _random;
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

            _random = new RandomManager();

            if (_alpha < 0)
                _alpha = _random.Value();

            if (GraspType == GraspType.BiasedGRASP)
                _random.SetRandomType(RandomType.Geometric);            

            _bestSolution = new Solution(instance);
        }

        public GRASP(Instance instance, float alpha, int nIterations, int nImprovements) : 
            this(instance, alpha, nIterations, nImprovements, GraspType.GRASP, ImprovementType.None)
        {
        }

        public GRASP(Instance instance, float alpha) : 
            this(instance, alpha, 100, 1, GraspType.GRASP, ImprovementType.None)
        {
        }

        public GRASP(Instance instance, float alpha, int nIterations, GraspType GraspType) :
            this(instance, alpha, nIterations, -1, GraspType, ImprovementType.None)
        {
        }

        public override Solution Run()
        {
            for (int i = 0; i < _nIterations; i++)
            {
                Solution currentSolution = Construct();

                if(_improvementType != ImprovementType.None)
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

            AddRandomElementToSolution(s, candidateList);
            CalculateNextElement(s, candidateList);

            return s;
        }

        private Solution Improve(Solution solution)
        {
            Solution bestSol = solution.Clone();
            bool improved = false;

            // A separate random manager is used so that the geometric distribution does not affect the improvement process
            var impRandom = new RandomManager();

            for (int impAtempts = 0; impAtempts < _nImprovements; impAtempts++)
            {
                switch(_improvementType)
                {
                    case ImprovementType.FirstImprovement:
                        bestSol = FirstImprove(bestSol);
                        break;

                    case ImprovementType.BestImprovement:
                        bestSol = BestImprove(bestSol);
                        break;

                    default:
                        throw new InvalidOperationException($"Error: Invalid improvement type {_improvementType}");
                }
            }

            return bestSol;
        }

        // NO ESTÁ TERMINADO, HAY  Q HACERLO !!!!!!
        private Solution BestImprove(Solution bestSol)
        {
            // bool improved = false;
            // RandomManager impRandom = new RandomManager();

            return bestSol;
        }

        private Solution FirstImprove(Solution bestSol)
        {
            bool improved = false;
            RandomManager impRandom = new RandomManager();

            do
            {
                improved = false;

                Solution improvedSol = bestSol.Clone();
                List<int> candidateElements = new List<int>();
                for (int i = 0; i < _instance.GetElementCount(); i++)
                {
                    if (!improvedSol.ContainsElement(i))
                        candidateElements.Add(i);
                }

                // Remove a random element
                int elementToRemove = bestSol.GetElements().ElementAt(impRandom.Next(bestSol.GetElementCount()));

                // We remove the element from the solution and add it to the explored elements
                // If the element cannot be removed, we throw an exception
                if (!improvedSol.RemoveElement(elementToRemove))
                    throw new InvalidOperationException($"Error: Could not remove element {elementToRemove} from solution.");

                while (candidateElements.Count > 0)
                {
                    // We add a random element from the candidate list to the solution
                    int nextElement = candidateElements.ElementAt(impRandom.Next(candidateElements.Count));
                    candidateElements.Remove(nextElement);

                    if (improvedSol.AddElement(nextElement))
                    {

                        // If the new solution is better than the current best solution, we update the best solution and set improved to true
                        if (improvedSol.GetMaxInterrsection() > bestSol.GetMaxInterrsection())
                        {
                            bestSol = improvedSol.Clone();
                            improved = true;
                            break;
                        }
                    }
                }

            } while (improved);

            return bestSol;
        }

        private int CalculateNextElement(Solution s, HashSet<int> candidateList)
        {
            int element = -1;
            while (s.GetElementCount() < _instance.GetK() && candidateList.Count > 0)
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

                if(totalGreedyValue == 0)
                {
                    // No candidates have a greedy value greater than the threshold
                    // We add a random element from the candidate list to the solution
                    // Then, we continue the loop and return the element added to the solution

                    element = AddRandomElementToSolution(s, candidateList);

                    if (element == -1)
                        break;

                    continue;
                }

                // if (totalGreedyValue == 0)
                //     break;

                element = CalculateNextRandomElement(s, candidateList, priorityList);
            }

            return element;
        }

        private int CalculateNextRandomElement(Solution s, HashSet<int> candidateList, PriorityList<int> priorityList)
        {
            if(priorityList.Count == 0)
            {
                throw new InvalidOperationException($"Error: No candidates available to select an element.");
            }

            int index = _random.Next(priorityList.Count);
            int element = priorityList.RemoveElementAt(index).ElementId;

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
                string info = $"\tCurrent file: {_instance.GetFileName()}\n" +
                    $"\tCurrent solution: {s.GetElements().Count()} elements\n" +
                    $"\tCandidate list: {candidateList.Count} elements\n" +
                    $"\tAlgorithm: {_algorithmName}\n" +
                    $"\tImprovement Type: {_improvementType}";
                
                throw new InvalidOperationException($"{info}\n" +
                    $"\n\tError: No candidates available to calculate threshold.");
            }

            return gMax - _alpha * (gMax - gMin);
        }

        private int GetGreedyValue(Solution solution, int elementId)
        {
            return solution.GetMaxInterrsectionWithElement(elementId);
        }

        private int AddRandomElementToSolution(Solution s, HashSet<int> candidateList)
        {
            if (candidateList.Count == 0)
                return -1;

            int index = _random.Next(candidateList.Count);
            int element = candidateList.ElementAt(index);

            candidateList.Remove(element);
            s.AddElement(element);

            return element;
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
        BestImprovement,
        None
    }
}
