using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using GraphVizNet;
using NLog;

namespace AutomatonAlgorithms.AutomatonProcedures
{
    public class GenerateVisualisation : IAutomatonProcedure
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public GenerateVisualisation(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void Process(List<Automaton> automata, List<string> strings)
        {
            if (automata.Count != 1 || strings.Count != 0)
                throw new ProcedureException("AutomatonSaver: bad amount of inputs, this procedure requires single input of automaton type");
            
            GenerateImage(automata[0]);
        }

        
        

        private string CreateStringForRendering(Automaton automaton)
        {
            var sb = new StringBuilder();

            sb.Append($"digraph {automaton.Name} " + "{\n");

            sb.Append("node [shape = doublecircle];");
            foreach (var state in automaton.AcceptingStates) sb.Append($" {state.Id}");

            sb.Append(";\nnode [shape = point ]; qi");
            sb.Append($";\nnode [shape = circle];\nqi -> {automaton.InitialState.Id}\n");


            foreach (var state in automaton.StatesAndTransitions.Nodes)
            foreach (var neighbour in automaton.StatesAndTransitions.GetNeighbours(state))
            {
                var labels = automaton.StatesAndTransitions.GetTransitionLabels(state, neighbour);
                sb.Append($"\"{state.Id}\" -> \"{neighbour.Id}\" " +
                          $"[label=\"{labels.Aggregate("", (s, label) => $"{s}{label.Name}, ", s => s[..^2])}\"]\n");
            }

            sb.Append('}');
            // Console.WriteLine(sb.ToString());
            return sb.ToString();
        }

        private void GenerateImage(Automaton automaton)
        {
            var graphViz = new GraphViz();
            try
            {
                var path = Configuration.OutputFolderPath + Path.DirectorySeparatorChar + automaton.Name + "Image.png";

                Logger.Info($"Output image created: {path}");
                var dotString = CreateStringForRendering(automaton);
                graphViz.LayoutAndRenderDotGraph(dotString, path, "png");
            }
            catch (Exception e)
            {
                throw new ProcedureException(
                    $"Error while, generating image, please, check output folder, Exception: {e}", e);
            }
        }
    }
}