using Experimentation.Models.Graphs.Transitions;

namespace Experimentation.Models.Graphs.Matrix.MatrixTypes
{
    public class IntIndexedTransitionMatrix<TValue> : ITransitionMatrix<int, TValue>
    {
        private readonly Transition<TValue>[,] _matrix;

        public IntIndexedTransitionMatrix(int size)
        {
            _matrix = new Transition<TValue>[size, size];

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    _matrix[i, j] = new Transition<TValue>();
                }
            }
            
            
        }
        
        public Transition<TValue> this[int x, int y]
        {
            get => _matrix[x, y];
            set => _matrix[x, y] = value;
        }
        
    }
}