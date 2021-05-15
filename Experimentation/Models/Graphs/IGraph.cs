using System;
using System.Collections.Generic;

namespace Experimentation.Models.Graphs
{
    public interface IGraph<TNode, TLabel> where TNode : IEquatable<TNode>
    {
        public List<TNode> Nodes { get; }

        public IEnumerable<TNode> GetNeighbours(TNode node);

        public TLabel GetTransitionLabel(TNode left, TNode right);

        public bool TryGetTransitionLabel(TNode left, TNode right, out TLabel label);

        public void CreateTransition(TNode left, TNode right, TLabel label);

        public void SetTransitionLabel(TNode left, TNode right, TLabel label);

        public void RemoveTransition(TNode left, TNode right);
    }
}