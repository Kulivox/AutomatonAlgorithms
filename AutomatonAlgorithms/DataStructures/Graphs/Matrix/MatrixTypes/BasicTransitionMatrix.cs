using System.Collections.Generic;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions;

namespace AutomatonAlgorithms.DataStructures.Graphs.Matrix.MatrixTypes
{
    public class BasicTransitionMatrix: ITransitionMatrix<INode>
    {
        private readonly ITransition[,] _matrix;
        private readonly Dictionary<string, int> _genericIndexToIntIndex;

        public BasicTransitionMatrix(IEnumerable<INode> indexes)
        {
            _genericIndexToIntIndex = new Dictionary<string, int>();
            
            var i = 0;
            foreach (var item in indexes)
            {
                if (!_genericIndexToIntIndex.TryAdd(item.Id, i))
                    throw new MatrixIndexException("Matrix indices have to be unique");

                i += 1;
            }

            _matrix = new ITransition[i, i];
            
        }

        public ITransition this[INode x, INode y]
        {
            get => _matrix[_genericIndexToIntIndex[x.Id], _genericIndexToIntIndex[y.Id]];
            set => _matrix[_genericIndexToIntIndex[x.Id], _genericIndexToIntIndex[y.Id]] = value;
        }
    }
}