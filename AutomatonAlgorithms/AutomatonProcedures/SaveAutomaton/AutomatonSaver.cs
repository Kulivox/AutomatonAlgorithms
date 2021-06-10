using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using NLog;

namespace AutomatonAlgorithms.AutomatonProcedures.SaveAutomaton
{
    public class AutomatonSaver : IAutomatonProcedure
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AutomatonSaver(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void Process(List<Automaton> automata, List<string> strings)
        {
            if (automata.Count != 1 || strings.Count != 0)
                throw new ProcedureException("AutomatonSaver: bad amount of inputs, this procedure requires single input of automaton type");

            SaveAutomaton(automata[0]);
        }


        public void SaveAutomaton(Automaton a)
        {
            // this kind of synchronization might lead to starvation, better synchronization should be implemented
            // it's not that important though, this problem might arise only in situation which is not really intended
            // and that is when user tries to save his output from multiple scripts to the same file
            var outputPath = Configuration.OutputFolderPath + Path.DirectorySeparatorChar + a.Name + "result.txt";
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    File.WriteAllText(outputPath, a.ToString());
                    Logger.Info($"Automaton saved to: {outputPath}");
                    return;
                }
                catch (IOException)
                {
                    Thread.Sleep(50);
                }
            }
            
            try
            {
                File.WriteAllText(outputPath, a.ToString());
                Logger.Info($"Automaton saved to: {outputPath}");
            }
            catch (IOException e)
            {
                throw new ProcedureException($"Automaton saver was not able to save to: {outputPath}", e);
            }
            
        }
    }
}