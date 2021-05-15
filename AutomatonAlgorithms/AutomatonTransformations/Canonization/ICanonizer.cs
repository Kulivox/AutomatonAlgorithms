using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.AutomatonTransformations.Canonization
{
    public interface ICanonizer : IAutomatonTransformation
    {
        public IConfiguration Configuration { get; }

        public Automaton CanonizeAutomaton(Automaton inputAutomaton);
    }
}