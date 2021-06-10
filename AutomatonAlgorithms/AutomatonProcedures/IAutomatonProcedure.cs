using System.Collections.Generic;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.AutomatonProcedures
{
    public interface  IAutomatonProcedure
    {
        
        public IConfiguration Configuration { get; }

        public void Process(List<Automaton> automata, List<string> strings);
        
        
        
    }
}