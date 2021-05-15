using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.AutomatonTransformations
{
    public interface IAutomatonTransformation
    {
        public AutomatonType IntendedType { get; }
        public Automaton Transform(Automaton input);
    }
}