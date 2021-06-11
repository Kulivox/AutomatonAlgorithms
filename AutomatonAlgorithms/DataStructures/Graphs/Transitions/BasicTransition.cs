using System;
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

        public bool Equals(ITransition other)
        {
            if (other is null)
                return false;

            return From.Equals(other?.From) && To.Equals(other?.To) && Labels.SetEquals(other.Labels);
        }

        protected bool Equals(BasicTransition other)
        {
            return From.Equals(other.From) && To.Equals(other.To) && Labels.Equals(other.Labels);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == GetType() && Equals((BasicTransition) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, Labels);
        }
    }
}