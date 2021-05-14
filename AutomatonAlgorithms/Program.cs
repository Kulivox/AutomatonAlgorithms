using System;
using System.IO;
using AutomatonAlgorithms.AutomatonProcedures;
using AutomatonAlgorithms.AutomatonProcedures.SaveAutomaton;
using AutomatonAlgorithms.AutomatonTransformations;
using AutomatonAlgorithms.AutomatonTransformations.Canonization;
using AutomatonAlgorithms.AutomatonTransformations.Determinization;
using AutomatonAlgorithms.AutomatonTransformations.EpsilonTransitionRemoval;
using AutomatonAlgorithms.AutomatonTransformations.Minimization;
using AutomatonAlgorithms.CommandPipeline;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.Parsers;

namespace AutomatonAlgorithms
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            
            var loader = new AutomatonLoader();

            var config =
                new BaseConfiguration(
                    @"C:\Users\Michal\RiderProjects\PV178\AutomatonAlgorithms\AutomatonAlgorithms\config.cfg");


            var pipeline = new PipelineExecutor(config, loader);
            pipeline.LoadAndExecute(@"C:\Users\Michal\RiderProjects\PV178\AutomatonAlgorithms\AutomatonAlgorithms\pipeline.pln");

        }
    }
}