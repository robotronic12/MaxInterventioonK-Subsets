using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

            foreach (string instanceFile in instanceFiles)
            {
                ExecuteInstance(instanceFile);
            }

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

            try
            {
                Instance instance = new Instance(filePath);

                Queue<IAlgorithm> algorithms = new Queue<IAlgorithm>();

                algorithms.Enqueue(new GRASP(instance, 0.3f, 100, 3, GraspType.GRASP, ImprovementType.FirstImprovement));
                algorithms.Enqueue(new GRASP(instance, 0.3f, 100, 3, GraspType.GRASP, ImprovementType.BestImprovement));
                algorithms.Enqueue(new GRASP(instance, 0.3f, 100, 3, GraspType.BiasedGRASP, ImprovementType.FirstImprovement));
                algorithms.Enqueue(new GRASP(instance, 0.3f, 100, 3, GraspType.BiasedGRASP, ImprovementType.BestImprovement));
                algorithms.Enqueue(new Greedy(instance));
                algorithms.Enqueue(new RandomAlgorithm(instance, 1000));

                while (algorithms.Count > 0)
                {
                    IAlgorithm algorithm = algorithms.Dequeue();

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    algorithm.Run();
                    stopwatch.Stop();

                    string result = algorithm.PrintSummarySolution();
                    string time = $"Execution Time: {stopwatch.ElapsedMilliseconds} ms\n";

                    // algorithm.SaveExperimentResults(stopwatch.ElapsedMilliseconds, "", true);

                    Console.Write(time);
                    dataCollected.Enqueue(result + time);
                }
            }
            catch (Exception exception)
            {
                string error = $"Error processing instance: {exception.Message}\n";
                Console.WriteLine(error);
                dataCollected.Enqueue(error);
            }

            SaveResults(filePath, dataCollected);
        }

        private static void SaveResults(string instanceFilePath, Queue<string> dataCollected)
        {
            string instanceFolder = Path.GetDirectoryName(instanceFilePath);
            string resultsFolder = Path.Combine(instanceFolder, "results");
            Directory.CreateDirectory(resultsFolder);

            string resultFileName = Path.GetFileNameWithoutExtension(instanceFilePath) + "_results.txt";
            string resultFilePath = Path.Combine(resultsFolder, resultFileName);
            string resultText = string.Concat(dataCollected);

            File.WriteAllText(resultFilePath, resultText);
            Console.WriteLine($"\nResults saved to: {resultFilePath}\n");
        }
    }
}
