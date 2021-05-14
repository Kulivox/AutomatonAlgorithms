using System;
using System.IO;
using System.Text;
using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.Configurations;
using GraphVizNet;

namespace AutomatonAlgorithms.AutomatonProcedures
{
    public class GenerateVisualisation : IAutomatonProcedure
    {
        public IConfiguration Configuration { get; }

        public GenerateVisualisation(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private string CreateStringForRendering(Automaton automaton)
        {
            var sb = new StringBuilder();

            sb.Append($"digraph {automaton.Name} "+ "{\n");

            sb.Append("node [shape = doublecircle];");
            foreach (var state in automaton.AcceptingStates)
            {
                sb.Append($" {state.Id}");
            }

            sb.Append(";node [shape = circle];\n");

            foreach (var state in automaton.StatesAndTransitions.Nodes)
            {
                foreach (var neighbour in automaton.StatesAndTransitions.GetNeighbours(state))
                {
                    var labels = automaton.StatesAndTransitions.GetTransitionLabels(state, neighbour);
                    foreach (var label in labels)
                    {
                        sb.Append($"\"{state.Id}\" -> \"{neighbour.Id}\" [label=\"{label.Name}\"]\n");
                    }
                }
            }

            sb.Append('}');
            Console.WriteLine();
            return sb.ToString();
        }
        
        public void GenerateImage(Automaton automaton)
        {
            var graphViz = new GraphViz();

            var path = Configuration.OutputFolderPath + Path.DirectorySeparatorChar + automaton.Name + "Image.png";
            var dotString = CreateStringForRendering(automaton);

            graphViz.LayoutAndRenderDotGraph(dotString, path, "png");
        }
        
        public void Process(Automaton a)
        {
            GenerateImage(a);
        }
    }
}