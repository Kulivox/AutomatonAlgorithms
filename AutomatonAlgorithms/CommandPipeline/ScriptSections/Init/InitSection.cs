using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.Parsers;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Init
{
    public class InitSection : Section
    {
        private const string InitLineRegex = @"^([A|T])\s+(\w+)\s*=\s*(\S+)\s*$";

        private readonly AutomatonLoader _loader;

        public InitSection(AutomatonLoader loader, IConfiguration configuration) : base(configuration)
        {
            _loader = loader;
        }

        public override int Priority => 0;


        public override void ExecuteSection(string sectionString, Dictionary<string, Automaton> automatonVariables,
            Dictionary<string, string> stringVariables)
        {
            ExecuteInit(sectionString, automatonVariables, stringVariables);
        }

        private void ExecuteInit(string input, Dictionary<string, Automaton> autVarDict,
            Dictionary<string, string> textVarDict)
        {
            var inputLines = Regex.Split(input, "\r\n|\r|\n");
            var rx = new Regex(InitLineRegex);

            foreach (var line in inputLines.Where(l => !string.IsNullOrEmpty(l)))
            {
                var match = rx.Match(line);
                if (!match.Success)
                    throw new PipelineInitException($"Syntax error detected at {line}");

                //todo implement proper async
                switch (match.Groups[1].ToString())
                {
                    case "A":
                        var automatonTask = _loader.TryLoadAutomaton(match.Groups[3].ToString());
                        automatonTask.Wait();
                        autVarDict.Add(match.Groups[2].ToString(), automatonTask.Result);
                        break;

                    case "T":
                        textVarDict.Add(match.Groups[2].ToString(), File.ReadAllText(match.Groups[3].ToString()));
                        break;
                    default:
                        throw new PipelineInitException($"Unknown variable type for {line}");
                }
            }
        }
    }
}