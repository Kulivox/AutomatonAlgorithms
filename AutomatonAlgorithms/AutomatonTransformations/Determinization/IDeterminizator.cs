using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.Configurations;

namespace AutomatonAlgorithms.AutomatonTransformations.Determinization
{
    public interface IDeterminizator : IAutomatonTransformation
    {
        public IConfiguration Configuration { get; }
        
        public Automaton MakeAutomatonDeterministic(Automaton inputAutomaton);
    }
}