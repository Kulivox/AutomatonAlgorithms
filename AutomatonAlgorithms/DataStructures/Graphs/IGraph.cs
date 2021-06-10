using System;
using System.Collections.Generic;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.DataStructures.Graphs
{
    public interface IGraph<TNode, TLabel> where TNode : IEquatable<TNode>
    {
        public HashSet<TNode> Nodes { get; }

        public void AddNode(TNode node);

        public IEnumerable<TNode> GetNeighbours(TNode node);

        public IEnumerable<ITransition> GetTransitionsFromNode(TNode node);

        public HashSet<ILabel> GetTransitionLabels(TNode left, TNode right);

        public bool TryGetTransitionLabels(TNode left, TNode right, out HashSet<ILabel> label);

        public void AddTransition(TNode left, TNode right, TLabel label);

        public void SetTransitionLabel(TNode left, TNode right, TLabel label);

        public void RemoveTransition(TNode left, TNode right);
    }
}