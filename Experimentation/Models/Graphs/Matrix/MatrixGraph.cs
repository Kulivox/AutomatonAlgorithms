using System;
using System.Collections.Generic;
using System.Linq;
using Experimentation.Models.Graphs.Matrix.MatrixTypes;

namespace Experimentation.Models.Graphs.Matrix
{
    public class MatrixGraph<TNode, TLabel> : IGraph<TNode, TLabel>
        where TNode : IEquatable<TNode>
        where TLabel : IEquatable<TLabel>
    {
        private readonly ITransitionMatrix<TNode, TLabel> _transitionTransitionMatrix;

        public MatrixGraph(ICollection<TNode> nodes)
        {
            _transitionTransitionMatrix = new GenericTransitionMatrix<TNode, TLabel>(nodes);
            Nodes = new List<TNode>(nodes);
        }

        public List<TNode> Nodes { get; }

        public IEnumerable<TNode> GetNeighbours(TNode node)
        {
            return Nodes.Where(n => _transitionTransitionMatrix[node, n].Exists);
        }

        public TLabel GetTransitionLabel(TNode left, TNode right)
        {
            return _transitionTransitionMatrix[left, right].Label;
        }

        public bool TryGetTransitionLabel(TNode left, TNode right, out TLabel label)
        {
            try
            {
                label = _transitionTransitionMatrix[left, right].Label;
                return true;
            }
            catch (MatrixIndexException)
            {
                label = default;
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                label = default;
                return false;
            }
        }

        public void CreateTransition(TNode left, TNode right, TLabel label)
        {
            var tr = _transitionTransitionMatrix[left, right];
            tr.Exists = true;
            tr.Label = label;
        }

        public void SetTransitionLabel(TNode left, TNode right, TLabel label)
        {
            _transitionTransitionMatrix[left, right].Label = label;
        }

        public void RemoveTransition(TNode left, TNode right)
        {
            var tr = _transitionTransitionMatrix[left, right];
            tr.Exists = false;
            tr.Label = default;
        }
    }
}