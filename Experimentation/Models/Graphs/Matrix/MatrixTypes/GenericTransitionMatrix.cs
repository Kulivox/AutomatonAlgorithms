using System.Collections.Generic;
using Experimentation.Models.Graphs.Transitions;

namespace Experimentation.Models.Graphs.Matrix.MatrixTypes
{
    public class GenericTransitionMatrix<TIndexType, TValue> : ITransitionMatrix<TIndexType, TValue>
    {
        private readonly Transition<TValue>[,] _matrix;
        private readonly Dictionary<TIndexType, int> _genericIndexToIntIndex;

        public GenericTransitionMatrix(IEnumerable<TIndexType> indexes)
        {
            _genericIndexToIntIndex = new Dictionary<TIndexType, int>();
            
            var i = 0;
            foreach (var item in indexes)
            {
                if (!_genericIndexToIntIndex.TryAdd(item, i))
                    throw new MatrixIndexException("Matrix indices have to be unique");

                i += 1;
            }

            _matrix = new Transition<TValue>[i, i];
            

            for (var x = 0; x < i; x++)
            {
                for (var y = 0; y < i; y++)
                {
                    _matrix[x, y] = new Transition<TValue>();
                }
            }
        }

        public Transition<TValue> this[TIndexType x, TIndexType y]
        {
            get => _matrix[_genericIndexToIntIndex[x], _genericIndexToIntIndex[y]];
            set => _matrix[_genericIndexToIntIndex[x], _genericIndexToIntIndex[y]] = value;
        }
    }
}