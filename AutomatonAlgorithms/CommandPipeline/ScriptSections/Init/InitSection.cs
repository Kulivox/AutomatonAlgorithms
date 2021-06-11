using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.PureScriptExceptions;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.Parsers;
using AutomatonAlgorithms.Parsers.Exceptions;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Init
{
    public class InitSection : Section
    {
        private const string InitLineRegex = @"^\s*([A|T])\s+(\w+)\s*=\s*(\S+)\s*$";

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

            foreach (var line in inputLines.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                var match = rx.Match(line);
                if (!match.Success)
                    throw new PipelineInitException($"Syntax error detected at {line}");

                try
                {
                    switch (match.Groups[1].ToString())
                    {
                        case "A":

                            var automaton = _loader.TryLoadAutomaton(match.Groups[3].ToString(),
                                match.Groups[2].ToString());
                            autVarDict.Add(match.Groups[2].ToString(), automaton);
                            break;
                        case "T":
                            textVarDict.Add(match.Groups[2].ToString(), File.ReadAllText(match.Groups[3].ToString()));
                            break;
                        default:
                            throw new PipelineInitException($"Unknown variable type for {line}");
                    }
                }
                catch (Exception e) when (e is AutomatonFileFormatException or FileNotFoundException)
                {
                    throw new PipelineInitException(e.Message);
                }
            }
        }
    }
}