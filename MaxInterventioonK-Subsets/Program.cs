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
            string targetPath;
            if (args.Length == 0)
            {
                string solutionDir = Path.GetFullPath(
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..")
                );

                string resultsDir = Path.Combine(solutionDir, "instances", "type1");

                Console.WriteLine($"Taken default folder: {resultsDir}");
                targetPath = resultsDir;
            }
            else
            {
                targetPath = args[0];
            }

            string[] instanceFiles;

            if (File.Exists(targetPath))
            {
                // Caso 1: La ruta es un único archivo
                if (!targetPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("The specified file is not a .txt file.");
                    return;
                }
                instanceFiles = new string[] { targetPath };
            }
            else if (Directory.Exists(targetPath))
            {
                // Caso 2: La ruta es una carpeta (busca en ella y en todas sus subcarpetas)
                instanceFiles = Directory.GetFiles(targetPath, "*.txt", SearchOption.AllDirectories);
                Array.Sort(instanceFiles);

                if (instanceFiles.Length == 0)
                {
                    Console.WriteLine("The folder and its subfolders do not contain any .txt files.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("The specified file or folder does not exist: " + targetPath);
                return;
            }

            Parallel.ForEach(instanceFiles, instanceFile =>
            {
                ExecuteInstance(instanceFile);
            });

            Console.WriteLine("All instances have finished.");
        }


        private static void ExecuteInstance(string filePath)
        {
            Queue<string> crashReports = new Queue<string>();
            List<IAlgorithm> algorithms = new List<IAlgorithm>();
            Instance instance = new Instance(filePath);

            // TAREAS

            /*
             * Cambiar random a distribucion geometrica
             * cambiar biased GRASP
             * 
             * Experimento 1 - Solo construcción:
             *  - GRASP 0.25 vs GRASP 0.5 vs GRASP 0.75 vs random
             * Experimento 2 - Solo construcción también:
             *   - Biased GRASP 0.25 vs 0.5 vs 0.75 vs random
             * Experimento 3 - Construcción + LS = Experimento 1 + LS FI vs Experimento 1 + LS BI
             *   - GRASP 0.25 vs GRASP 0.5 vs GRASP 0.75 vs random
             * Experimento 4 - Construcción + LS = Experimento 2 + LS FI vs Experimento 2 + LS BI
             *  - Biased GRASP 0.25 vs 0.5 vs 0.75 vs random
             */

            // Experiment 0
            algorithms.Add(new Greedy(instance)); 
            algorithms.Add(new RandomAlgorithm(instance, 1000));


            // Experiment 1
            algorithms.Add(new GRASP(instance, 0.25f, 100, GraspType.GRASP));
            algorithms.Add(new GRASP(instance, 0.5f, 100, GraspType.GRASP));
            algorithms.Add(new GRASP(instance, 0.75f, 100, GraspType.GRASP));
            algorithms.Add(new GRASP(instance, -1f, 100, GraspType.GRASP));

            // Experiment 2
            algorithms.Add(new GRASP(instance, 0.25f, 100, GraspType.BiasedGRASP));
            algorithms.Add(new GRASP(instance, 0.5f, 100, GraspType.BiasedGRASP));
            algorithms.Add(new GRASP(instance, 0.75f, 100, GraspType.BiasedGRASP));
            algorithms.Add(new GRASP(instance, -1f, 100, GraspType.BiasedGRASP));

            // Experiment 3
            algorithms.Add(new GRASP(instance, 0.25f, 100, 3, GraspType.GRASP, ImprovementType.FirstImprovement));
            algorithms.Add(new GRASP(instance, 0.5f, 100, 3, GraspType.GRASP, ImprovementType.FirstImprovement));
            algorithms.Add(new GRASP(instance, 0.75f, 100, 3, GraspType.GRASP, ImprovementType.FirstImprovement));
            algorithms.Add(new GRASP(instance, -1f, 100, 3, GraspType.GRASP, ImprovementType.FirstImprovement));

            algorithms.Add(new GRASP(instance, 0.25f, 100, 3, GraspType.GRASP, ImprovementType.BestImprovement));
            algorithms.Add(new GRASP(instance, 0.5f, 100, 3, GraspType.GRASP, ImprovementType.BestImprovement));
            algorithms.Add(new GRASP(instance, 0.75f, 100, 3, GraspType.GRASP, ImprovementType.BestImprovement));
            algorithms.Add(new GRASP(instance, -1f, 100, 3, GraspType.GRASP, ImprovementType.BestImprovement));

            // Experiment 4
            algorithms.Add(new GRASP(instance, 0.25f, 100, 3, GraspType.BiasedGRASP, ImprovementType.FirstImprovement));
            algorithms.Add(new GRASP(instance, 0.5f, 100, 3, GraspType.BiasedGRASP, ImprovementType.FirstImprovement));
            algorithms.Add(new GRASP(instance, 0.75f, 100, 3, GraspType.BiasedGRASP, ImprovementType.FirstImprovement));
            algorithms.Add(new GRASP(instance, -1f, 100, 3, GraspType.BiasedGRASP, ImprovementType.FirstImprovement));

            algorithms.Add(new GRASP(instance, 0.25f, 100, 3, GraspType.BiasedGRASP, ImprovementType.BestImprovement));
            algorithms.Add(new GRASP(instance, 0.5f, 100, 3, GraspType.BiasedGRASP, ImprovementType.BestImprovement));
            algorithms.Add(new GRASP(instance, 0.75f, 100, 3, GraspType.BiasedGRASP, ImprovementType.BestImprovement));
            algorithms.Add(new GRASP(instance, -1f, 100, 3, GraspType.BiasedGRASP, ImprovementType.BestImprovement));

            foreach (IAlgorithm algorithm in algorithms)
            {
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    algorithm.Run();
                    stopwatch.Stop();

                    string result = algorithm.PrintSummarySolution();
                    string time = $"Execution Time: {stopwatch.ElapsedMilliseconds} ms\n";

                    algorithm.SetTimeElapsed(stopwatch.ElapsedMilliseconds);
                }
                catch (Exception exception)
                {
                    algorithms.Remove(algorithm);

                    string error = $"Error processing instance: {Path.GetFileName(filePath)}, Exception Message:\n {exception.Message}\n";
                    Console.WriteLine(error);
                    crashReports.Enqueue(error);
                }
            }

            Console.WriteLine($"Finished processing instance succesfully: {Path.GetFileName(filePath)}");
            SaveResults(filePath, crashReports, algorithms);
        }

        private static void SaveResults(string path, Queue<string> crashReports, List<IAlgorithm> algorithms)
        {
            // Table saves
            foreach (IAlgorithm algorithm in algorithms)
            {
                algorithm.SaveExperimentResults(path);
            }

            // Error logs saves
            if (crashReports.Count > 0)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string logFilePath = Path.Combine(path, $"ErrorLog_{timestamp}.txt");

                using (StreamWriter writer = new StreamWriter(logFilePath))
                {
                    while (crashReports.Count > 0)
                    {
                        string error = crashReports.Dequeue();
                        writer.WriteLine(error);
                    }
                }
            }
        }
    }
}
