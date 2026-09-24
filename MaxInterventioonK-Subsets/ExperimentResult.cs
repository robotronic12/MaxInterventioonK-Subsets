using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxInterventioonK_Subsets
{
    internal class ExperimentResult
    {
        public string AlgorithmName { get; set; }
        public string ImprovementType { get; set; }
        public float Alpha { get; set; }
        public int Iterations { get; set; }
        public int Improvements { get; set; }
        public int MaxIntersection { get; set; }
        public int SolutionElements { get; set; }
        public double TimeMs { get; set; }

        public ExperimentResult(
            string algorithmName,
            string improvementType,
            float alpha,
            int iterations,
            int improvements,
            int maxIntersection,
            int solutionElements,
            double timeMs)
        {
            AlgorithmName = algorithmName;
            ImprovementType = improvementType;
            Alpha = alpha;
            Iterations = iterations;
            Improvements = improvements;
            MaxIntersection = maxIntersection;
            SolutionElements = solutionElements;
            TimeMs = timeMs;
        }

        /// <summary>
        /// Saves the result of the experiments in a csv format
        /// </summary>
        /// <param name="filePath">The file path to save the results</param>
        /// <param name="over2rite">If true, it will overwrite an existing file if its on the same path</param>
        public void SaveResultCSV(string filePath, bool overwrite)
        {
            StreamWriter writer = new StreamWriter(filePath);
            bool fileExists = File.Exists(filePath);

            StreamWriter streamWriter = new StreamWriter(filePath, append: true, encoding: Encoding.UTF8);
            if (overwrite || !fileExists)
            {
                writer.WriteLine(
                    "AlgorithmName;" +
                    "ImprovementType;" +
                    "Alpha;" +
                    "Iterations;" +
                    "Improvements;" +
                    "MaxIntersection;" +
                    "SolutionElements;" +
                    "TimeMs");
            }

            writer.WriteLine(
                $"{AlgorithmName};" +
                $"{ImprovementType};" +
                $"{Alpha.ToString(CultureInfo.InvariantCulture)};" +
                $"{Iterations};" +
                $"{Improvements};" +
                $"{MaxIntersection};" +
                $"{SolutionElements};" +
                $"{TimeMs.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}
