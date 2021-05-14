using System;
using System.Collections.Generic;
using System.Linq;
using AutomatonAlgorithms.DataStructures.Graphs.Matrix.MatrixTypes;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.DataStructures.Graphs.Matrix
{
    public class MatrixGraph : IGraph<INode, ILabel>
        
    {
        private readonly ITransitionMatrix<INode> _transitionTransitionMatrix;

        public HashSet<INode> Nodes { get;}
        
        public MatrixGraph()
        {
            Nodes = new HashSet<INode>();
        }
        public MatrixGraph(IEnumerable<INode> nodes)
        {
            
            _transitionTransitionMatrix = new BasicTransitionMatrix(nodes);
            Nodes = new HashSet<INode>(nodes);
        }

        public void AddNode(INode node)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<INode> GetNeighbours(INode node)
        {
            return Nodes.Where(n => _transitionTransitionMatrix[node, n] != null);
        }

        public HashSet<ILabel> GetTransitionLabels(INode left, INode right)
        {
           return _transitionTransitionMatrix[left, right].Labels;
        }

      

        public bool TryGetTransitionLabels(INode left, INode right, out HashSet<ILabel> label)
        {
            try
            {
                label = _transitionTransitionMatrix[left, right].Labels;
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

        public void CreateTransition(INode left, INode right, ILabel label)
        {
            var item = _transitionTransitionMatrix[left, right];
            if (item is null)
            {
                _transitionTransitionMatrix[left, right] = new BasicTransition(left, right) {Labels = new HashSet<ILabel>(){label}};
                return;
            }
            
            item.Labels.Add(label);

        }
        

        public void SetTransitionLabel(INode left, INode right, ILabel label)
        {
            _transitionTransitionMatrix[left, right].Labels.Add(label);
        }

        public void RemoveTransition(INode left, INode right)
        {
            _transitionTransitionMatrix[left, right] = null;

        }
    }
}