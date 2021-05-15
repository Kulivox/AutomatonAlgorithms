using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.AutomatonTransformations
{
    public interface IAutomatonTransformation
    {
        public Automaton Transform(Automaton input);
    }
}