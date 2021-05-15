using System;
using System.IO;
using System.Text;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using GraphVizNet;

namespace AutomatonAlgorithms.AutomatonProcedures
{
    public class GenerateVisualisation : IAutomatonProcedure
    {
        public GenerateVisualisation(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Process(Automaton a)
        {
            GenerateImage(a);
        }

        private string CreateStringForRendering(Automaton automaton)
        {
            var sb = new StringBuilder();

            sb.Append($"digraph {automaton.Name} " + "{\n");

            sb.Append("node [shape = doublecircle];");
            foreach (var state in automaton.AcceptingStates) sb.Append($" {state.Id}");

            sb.Append(";node [shape = circle];\n");

            foreach (var state in automaton.StatesAndTransitions.Nodes)
            foreach (var neighbour in automaton.StatesAndTransitions.GetNeighbours(state))
            {
                var labels = automaton.StatesAndTransitions.GetTransitionLabels(state, neighbour);
                foreach (var label in labels)
                    sb.Append($"\"{state.Id}\" -> \"{neighbour.Id}\" [label=\"{label.Name}\"]\n");
            }

            sb.Append('}');
            return sb.ToString();
        }

        private void GenerateImage(Automaton automaton)
        {
            var graphViz = new GraphViz();

            var path = Configuration.OutputFolderPath + Path.DirectorySeparatorChar + automaton.Name + "Image.png";
            var dotString = CreateStringForRendering(automaton);

            graphViz.LayoutAndRenderDotGraph(dotString, path, "png");
        }
    }
}