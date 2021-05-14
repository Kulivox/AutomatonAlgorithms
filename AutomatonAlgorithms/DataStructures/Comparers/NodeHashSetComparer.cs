using System;
using System.Collections.Generic;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;

namespace AutomatonAlgorithms.DataStructures.Comparers
{
    public class NodeHashSetComparer : IEqualityComparer<HashSet<INode>>
    {
        public bool Equals(HashSet<INode> x, HashSet<INode> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Count == y.Count && SetsAreEqual(x, y);
        }

        private bool SetsAreEqual(HashSet<INode> x, HashSet<INode> y)
        {
            foreach (var item in x)
            {
                if (!y.Contains(item))
                    return false;

            }

            return true;
        }

        public int GetHashCode(HashSet<INode> obj)
        {
            return HashCode.Combine(obj.Comparer, obj.Count);
        }
    }
}