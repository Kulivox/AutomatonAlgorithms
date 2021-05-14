using System;

namespace AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels
{
    public interface ILabel : IEquatable<ILabel>, IComparable<ILabel>
    {
        public string Name { get; set; }
    }
}