using System.Collections;
using System.Collections.Generic;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions;

namespace AutomatonAlgorithms.DataStructures.Graphs.Matrix.MatrixTypes
{
    public interface ITransitionMatrix<TIndex>
    {
        public ITransition this[TIndex x, TIndex y] { get; set; }

        public IEnumerable<ITransition> GetRow(TIndex x);
        
        public IEnumerable<ITransition> GetCol(TIndex x);
    }
}