using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.AutomatonTransformations.Determinization
{
    public interface IDeterminizator : IAutomatonTransformation
    {
        public IConfiguration Configuration { get; }

        public Automaton MakeAutomatonDeterministic(Automaton inputAutomaton);
    }
}