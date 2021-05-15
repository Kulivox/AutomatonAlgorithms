using System.IO;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.AutomatonProcedures.SaveAutomaton
{
    public class AutomatonSaver : IAutomatonProcedure
    {
        public AutomatonSaver(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Process(Automaton a)
        {
            SaveAutomaton(a);
        }

        public void SaveAutomaton(Automaton a)
        {
            var outputPath = Configuration.OutputFolderPath + Path.DirectorySeparatorChar + a.Name + "result.txt";
            File.WriteAllText(outputPath, a.ToString());
        }
    }
}