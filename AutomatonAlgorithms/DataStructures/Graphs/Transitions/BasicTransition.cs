using System.Collections.Generic;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.DataStructures.Graphs.Transitions
{
    public class BasicTransition : ITransition
    {
        public BasicTransition(INode from, INode to)
        {
            From = from;
            To = to;
        }

        public INode From { get; }

        public INode To { get; }

        public HashSet<ILabel> Labels { get; set; }
    }
}