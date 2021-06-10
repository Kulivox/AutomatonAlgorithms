using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using NLog;
using NLog.Fluent;

namespace AutomatonAlgorithms.AutomatonProcedures.CompareAutomatons
{
    public class AutomatonComparer : IAutomatonProcedure
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IConfiguration Configuration { get; }

        public AutomatonComparer(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void Process(List<Automaton> automata, List<string> strings)
        {
            Logger.Warn("This procedure is intended for comparison of minimised and canonised automatons. Other inputs might cause the output to be innacurate");
            if (automata.Count != 2 || strings.Count != 0)
                throw new ProcedureException("AutomatonComparer: bad amount of inputs, this procedure requires two automaton variables to compare");
            
            CompareAutomatonsAndSaveResult(automata[0], automata[1]);
        }


        private void SaveResult(string path, bool result, string optionalData = "")
        {
            var resultString = $"{result.ToString().ToUpper()} | {optionalData}\n";
            File.WriteAllText(path, resultString);

        }

        private void CompareTransitions(Automaton left, Automaton right, INode leftNode, INode rightNode, List<string> differingTransitions)
        {
            var pls = left.StatesAndTransitions
                .GetTransitionsFromNode(leftNode).ToList();
            var zippedTransitions = left.StatesAndTransitions
                .GetTransitionsFromNode(leftNode)
                .Zip(right.StatesAndTransitions.GetTransitionsFromNode(rightNode));
            foreach (var (lTrans, rTrans) in zippedTransitions)
            {
                if (!lTrans.Equals(rTrans))
                {
                    differingTransitions
                        .Add($"{lTrans.From.Id} -- {lTrans.Labels} --> {lTrans.To.Id}" +
                             $"  !=  {rTrans.From.Id} -- {rTrans.Labels} --> {rTrans.To.Id}\n");
                }
            }
        }
        
        private void CompareAutomatonsAndSaveResult(Automaton left, Automaton right)
        {
            var outputPath = Configuration.OutputFolderPath + Path.DirectorySeparatorChar +
                             $"{left.Name}-{right.Name}-comparisonResult.txt";

            
            if (left.Alphabet != right.Alphabet)
            {
                SaveResult(outputPath, false, "Automata differ in their alphabet");
                return;
            }

            if (left.AcceptingStates.Count != right.AcceptingStates.Count)
            {
                SaveResult(outputPath, false, "Automata differ in amount of accepting states");
                return;
            }

            if (left.StatesAndTransitions.Nodes.Count != right.StatesAndTransitions.Nodes.Count)
            {
                SaveResult(outputPath, false, "Automata differ in amount of states");
                return;
            }


            var leftStates = left.StatesAndTransitions.Nodes.OrderBy(n => n.Id).ToArray();
            var rightStates = right.StatesAndTransitions.Nodes.OrderBy(n => n.Id).ToArray();

            var differingStates = new List<string>();
            var differingTransitions = new List<string>();
            
            for (var stateIndex = 0; stateIndex < leftStates.Length; stateIndex++)
            {
                if (!leftStates[stateIndex].Equals(rightStates[stateIndex]))
                {
                    differingStates.Add($"{leftStates[stateIndex].Id} and {rightStates[stateIndex].Id}\n");
                }

                CompareTransitions(left, right, leftStates[stateIndex],
                    rightStates[stateIndex], differingTransitions);
            }
            
            
            Logger.Info($"Comparison complete, result is in: {outputPath}");
            if (differingStates.Count == 0 && differingTransitions.Count == 0)
            {
                SaveResult(outputPath, true, "Automatons are equal");
                return;
            }

            var sb = new StringBuilder();
            sb.Append("Different states:\n");
            foreach (var ds in differingStates)
            {
                sb.Append(ds);
            }
            
            sb.Append("Different transitions:\n");
            foreach (var dt in differingTransitions)
            {
                sb.Append(dt);
            }
            
            SaveResult(outputPath, true, "Automata differ in:\n" + sb.ToString());
            
        }
    }
}