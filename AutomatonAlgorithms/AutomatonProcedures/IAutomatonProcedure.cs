using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.AutomatonProcedures
{
    public interface  IAutomatonProcedure
    {
        
        public IConfiguration Configuration { get; }

        public void Process(Automaton a);
    }
}