using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.Configurations;

namespace AutomatonAlgorithms.AutomatonTransformations.EpsilonTransitionRemoval
{
    public interface IEpsilonRemover : IAutomatonTransformation
    {
        public IConfiguration Configuration { get; }
        public Automaton RemoveEpsilonTransitions(Automaton inputAutomaton);
    }
}