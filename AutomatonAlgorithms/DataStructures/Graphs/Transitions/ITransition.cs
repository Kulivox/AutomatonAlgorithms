using System.Collections.Generic;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.DataStructures.Graphs.Transitions
{
    public interface ITransition
    {
        public INode From { get; }
        
        public INode To { get; }

        public HashSet<ILabel> Labels { get; set; }
    }
}