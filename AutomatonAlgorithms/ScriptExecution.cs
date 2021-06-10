using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutomatonAlgorithms.CommandPipeline;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.Parsers;
using NLog;

namespace AutomatonAlgorithms
{
    public static class ScriptExecution
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
                    try
                    {
                        var newFreeTaskIndex = Task.WaitAny(taskArray.Where(t => t != null).ToArray());
                        freeTaskArrayIndices.Add(newFreeTaskIndex);
                    }
                    catch (AggregateException e)
                    {
                        Logger.Fatal(e);
                        return;
                    }
                    
                }

                var index = freeTaskArrayIndices.First();

                taskArray[index] = Task.Factory.StartNew(() =>
                {
                    Logger.Info($"Started executing {Path.GetFileName(filePath)}");
                    var executor = new PipelineExecutor(configuration, loader);
                    executor.LoadAndExecute(filePath);
                });

                freeTaskArrayIndices.Remove(index);
            }

            try
            {
                Task.WaitAll(taskArray.Where(t => t != null).ToArray());
            }
            catch (AggregateException e)
            {
                Logger.Fatal(e);
            }
            
        }
    }
}