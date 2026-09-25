using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MaxInterventioonK_Subsets;

namespace MaxInterventionK_Subsets
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: MaxInterventioonK-Subsets.exe <folder-path>");
                return;
            }

            string folderPath = args[0];
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("The specified folder does not exist: " + folderPath);
                return;
            }

            string[] instanceFiles = Directory.GetFiles(folderPath, "*.txt", SearchOption.TopDirectoryOnly);
            Array.Sort(instanceFiles);

            if (instanceFiles.Length == 0)
            {
                Console.WriteLine("The folder does not contain any .txt files.");
                return;
            }

            // foreach (string instanceFile in instanceFiles)
            // {
            //     ExecuteInstance(instanceFile);
            // }

            // Paralelizamos cada archivo
            Parallel.ForEach(instanceFiles, instanceFile =>
            {
                ExecuteInstance(instanceFile);
            });

            Console.WriteLine("All instances have finished.");
        }

        private static void ExecuteInstance(string filePath)
        {
            Queue<string> dataCollected = new Queue<string>();

            string info =
                "==============================================\n"
                + $"Executing: {Path.GetFileName(filePath)}\n"
                + "==============================================\n";

            Console.WriteLine(info);
            dataCollected.Enqueue(info);

            Queue<IAlgorithm> algorithms = new Queue<IAlgorithm>();

            try
            {
                Instance instance = new Instance(filePath);

                algorithms.Enqueue(new GRASP(instance, 0.5f, 100, 3, GraspType.GRASP, ImprovementType.FirstImprovement));
                algorithms.Enqueue(new GRASP(instance, 0.5f, 100, 3, GraspType.GRASP, ImprovementType.BestImprovement));
                algorithms.Enqueue(new GRASP(instance, 0.5f, 100, 3, GraspType.BiasedGRASP, ImprovementType.FirstImprovement));
                algorithms.Enqueue(new GRASP(instance, 0.5f, 100, 3, GraspType.BiasedGRASP, ImprovementType.BestImprovement));
                algorithms.Enqueue(new Greedy(instance));
                algorithms.Enqueue(new RandomAlgorithm(instance, 1000));

                // Paralelizado
                Parallel.ForEach(algorithms, algorithm =>
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    algorithm.Run();
                    stopwatch.Stop();

                    string result = algorithm.PrintSummarySolution();
                    string time = $"Execution Time: {stopwatch.ElapsedMilliseconds} ms\n";

                    algorithm.SetTimeElapsed(stopwatch.ElapsedMilliseconds);

                    Console.Write(time);
                    dataCollected.Enqueue(result + time);
                });

                // foreach (IAlgorithm algorithm in algorithms)
                // {
                //     Stopwatch stopwatch = Stopwatch.StartNew();
                //     algorithm.Run();
                //     stopwatch.Stop();
                // 
                //     string result = algorithm.PrintSummarySolution();
                //     string time = $"Execution Time: {stopwatch.ElapsedMilliseconds} ms\n";
                // 
                //     algorithm.SetTimeElapsed(stopwatch.ElapsedMilliseconds);
                // 
                //     Console.Write(time);
                //     dataCollected.Enqueue(result + time);
                // }
            }
            catch (Exception exception)
            {
                string error = $"Error processing instance: {exception.Message}\n";
                Console.WriteLine(error);
                dataCollected.Enqueue(error);
            }

            SaveResults(filePath, dataCollected, algorithms);
        }

        private static void SaveResults(string instanceFilePath, Queue<string> dataCollected, Queue<IAlgorithm> algorithms)
        {
            // Text saves
            string instanceFolder = Path.GetDirectoryName(instanceFilePath);
            string resultsFolder = Path.Combine(instanceFolder, "results");
            Directory.CreateDirectory(resultsFolder);

            string resultFileName = Path.GetFileNameWithoutExtension(instanceFilePath) + "_results.txt";
            string resultFilePath = Path.Combine(resultsFolder, resultFileName);
            string resultText = string.Concat(dataCollected);

            File.WriteAllText(resultFilePath, resultText);
            //Console.WriteLine($"\nResults saved to: {resultFilePath}\n");

            // Table saves
            while (algorithms.Count > 0)
            {
                IAlgorithm algorithm = algorithms.Dequeue();

                algorithm.SaveExperimentResults(instanceFilePath);
            }
        }
    }
}
