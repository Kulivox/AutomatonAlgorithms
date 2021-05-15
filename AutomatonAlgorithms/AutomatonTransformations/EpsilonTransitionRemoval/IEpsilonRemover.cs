using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.AutomatonTransformations.EpsilonTransitionRemoval
{
    public interface IEpsilonRemover : IAutomatonTransformation
    {
        public IConfiguration Configuration { get; }
        public Automaton RemoveEpsilonTransitions(Automaton inputAutomaton);
    }
}