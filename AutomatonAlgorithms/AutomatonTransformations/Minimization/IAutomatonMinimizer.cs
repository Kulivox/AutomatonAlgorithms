using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.Configurations;

namespace AutomatonAlgorithms.AutomatonTransformations.Minimization
{
    public interface IAutomatonMinimizer : IAutomatonTransformation
    {
        public IConfiguration Configuration { get; }
        
        public Automaton MinimizeAutomaton(Automaton inputAut);
    }
}