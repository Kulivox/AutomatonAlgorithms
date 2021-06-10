using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutomatonAlgorithms.CommandPipeline;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.Parsers;

namespace AutomatonAlgorithms
{
    public static class ScriptExecution
    {
        public static void Start(string inputPath, string configPath, int maxThreads)
        {
            var configuration = new BaseConfiguration(configPath);
            var loader = new AutomatonLoader(configuration);

            var taskArray = new Task[maxThreads];
            var freeTaskArrayIndices = new HashSet<int>(Enumerable.Range(0, maxThreads));


            foreach (var filePath in Directory.EnumerateFiles(inputPath, "*.pln", SearchOption.AllDirectories))
            {
                if (freeTaskArrayIndices.Count == 0)
                {
                    var newFreeTaskIndex = Task.WaitAny(taskArray.Where(t => t != null).ToArray());
                    freeTaskArrayIndices.Add(newFreeTaskIndex);
                }

                var index = freeTaskArrayIndices.First();

                taskArray[index] = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Started executing {Path.GetFileName(filePath)}");
                    var executor = new PipelineExecutor(configuration, loader);
                    executor.LoadAndExecute(filePath);
                });

                freeTaskArrayIndices.Remove(index);
            }

            Task.WaitAll(taskArray.Where(t => t != null).ToArray());
        }
    }
}