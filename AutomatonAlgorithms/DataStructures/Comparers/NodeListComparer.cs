using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomatonAlgorithms.DataStructures.Comparers
{
    public class NodeListComparer<T> : IEqualityComparer<List<T>>
    {
        public bool Equals(List<T> x, List<T> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Count == y.Count && x.SequenceEqual(y);
        }

        public int GetHashCode(List<T> obj)
        {
            return HashCode.Combine(obj.Capacity, obj.Count);
        }
    }
}