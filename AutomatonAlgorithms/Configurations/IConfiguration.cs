using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.Configurations
{
    public interface IConfiguration
    {
        public GraphTypes GraphType { get; set; }
        
        public long MaxFileSizeBytes { get; set; }
        
        public ILabel EpsilonTransitionLabel { get; set; }
        
        public string OutputFolderPath { get; set; }
    }
}