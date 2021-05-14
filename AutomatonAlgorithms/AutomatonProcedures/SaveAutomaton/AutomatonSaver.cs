using System.IO;
using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.Configurations;

namespace AutomatonAlgorithms.AutomatonProcedures.SaveAutomaton
{
    public class AutomatonSaver : IAutomatonProcedure
    {
        public IConfiguration Configuration { get; }
        public void Process(Automaton a)
        {
            SaveAutomaton(a);
        }

        public AutomatonSaver(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void SaveAutomaton(Automaton a)
        {
            var outputPath = Configuration.OutputFolderPath + Path.DirectorySeparatorChar + a.Name + "result.txt";
            File.WriteAllText(outputPath, a.ToString());
        }


    }
}