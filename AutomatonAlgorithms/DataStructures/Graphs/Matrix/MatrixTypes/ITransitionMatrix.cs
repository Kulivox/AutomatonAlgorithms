using AutomatonAlgorithms.DataStructures.Graphs.Transitions;

namespace AutomatonAlgorithms.DataStructures.Graphs.Matrix.MatrixTypes
{
    public interface ITransitionMatrix<TIndex>
    {
        public ITransition this[TIndex x, TIndex y] { get; set; }
    }
}