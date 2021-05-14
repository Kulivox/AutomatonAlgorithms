using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.Configurations;

namespace AutomatonAlgorithms.AutomatonTransformations.Canonization
{
    public interface ICanonizer : IAutomatonTransformation
    {
        public IConfiguration Configuration { get; }
        
        public Automaton CanonizeAutomaton(Automaton inputAutomaton);
    }
}