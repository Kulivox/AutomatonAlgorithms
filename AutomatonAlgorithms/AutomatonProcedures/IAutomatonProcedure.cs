using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.Configurations;

namespace AutomatonAlgorithms.AutomatonProcedures
{
    public interface IAutomatonProcedure
    {
        public IConfiguration Configuration { get; }

        public void Process(Automaton a);
    }
}