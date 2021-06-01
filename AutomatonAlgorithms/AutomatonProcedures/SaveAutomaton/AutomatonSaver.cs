using System;
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
            try
            {
                File.WriteAllText(outputPath, a.ToString());
            }
            catch (Exception e)
            {
                throw new ProcedureException("Error while generating output file, check output folder", e);
            }
        }
    }
}