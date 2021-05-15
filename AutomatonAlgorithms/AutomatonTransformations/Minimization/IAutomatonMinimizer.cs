using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.AutomatonTransformations.Minimization
{
    public interface IAutomatonMinimizer : IAutomatonTransformation
    {
        public IConfiguration Configuration { get; }

        public Automaton MinimizeAutomaton(Automaton inputAut);
    }
}