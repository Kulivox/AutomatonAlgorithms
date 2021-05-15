using System;

namespace AutomatonAlgorithms.DataStructures.Graphs.Nodes
{
    public interface INode : IEquatable<INode>
    {
        public string Id { get; set; }
    }
}