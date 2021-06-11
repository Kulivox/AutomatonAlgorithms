using System;
using System.IO;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;
using NLog;

namespace AutomatonAlgorithms.Configurations
{
    public class BaseConfiguration : IConfiguration

    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BaseConfiguration()
        {
            EpsilonTransitionLabel = new BasicLabel {Name = "$"};
        }

        public BaseConfiguration(string path)
        {
            foreach (var item in File.ReadLines(path))
            {
                var splitLine = item.Split("=");
                switch (splitLine[0])
                {
                    case "GraphType":
                        GraphType = (GraphTypes) Enum.Parse(typeof(GraphTypes), splitLine[1]);
                        break;
                    case "MaxFileSizeBytes":
                        MaxFileSizeBytes = int.Parse(splitLine[1]);
                        break;
                    case "EpsilonTransitionLabel":
                        EpsilonTransitionLabel = new BasicLabel {Name = splitLine[1]};
                        break;
                    case "OutputFolderPath":
                        OutputFolderPath = splitLine[1];
                        break;
                    default:
                        Logger.Warn($"Malformed line in {Path.GetFileName(path)}: {item}");
                        break;
                }
            }
        }

        public GraphTypes GraphType { get; set; } = GraphTypes.TransitionMatrixGraph;

        public long MaxFileSizeBytes { get; set; } = 1 << 24;

        public ILabel EpsilonTransitionLabel { get; set; }

        public string OutputFolderPath { get; set; } = ".";
    }
}