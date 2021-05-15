using Experimentation.Models.Graphs.Transitions;

namespace Experimentation.Models.Graphs.Matrix.MatrixTypes
{
    public interface ITransitionMatrix<TIndex, TValue>
    {
        public Transition<TValue> this[TIndex x, TIndex y] { get; set; }
    }
}