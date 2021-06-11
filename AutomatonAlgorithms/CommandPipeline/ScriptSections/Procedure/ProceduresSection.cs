using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutomatonAlgorithms.AutomatonProcedures;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.PureScriptExceptions;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Procedure
{
    public class ProceduresSection : Section
    {
        private const string ProcedureRegex = @"^(\S+)->(\w+)$";

        public ProceduresSection(IConfiguration configuration) : base(configuration)
        {
        }

        public override int Priority { get; } = 10;


        public override void ExecuteSection(string sectionString, Dictionary<string, Automaton> automatonVariables,
            Dictionary<string, string> stringVariables)
        {
            ExecuteProcedures(sectionString, automatonVariables, stringVariables);
        }

        private void ExecuteProcedures(string input, Dictionary<string, Automaton> autVarDict,
            Dictionary<string, string> stringVariables)
        {
            var inputLines = Regex.Split(input, "\r\n|\r|\n");
            var rx = new Regex(ProcedureRegex);

            var procedures = new List<(string from, string procedure)>();
            var uniqueProcedureNames = new HashSet<string>();


            foreach (var line in inputLines.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                // we want to remove the parentheses too, to simplify parsing of multiple parameter input
                var lineWithoutWhitespaceOrParentheses = Regex.Replace(line, @"\s|\(|\)", "");
                var match = rx.Match(lineWithoutWhitespaceOrParentheses);
                if (!match.Success)
                    throw new PipelineTransformationException($"Incorrect syntax of procedure: {line}");

                var from = match.Groups[1].ToString();
                var procedure = match.Groups[2].ToString();

                procedures.Add((from, procedure));
                uniqueProcedureNames.Add(procedure);
            }

            var procDict = GetOperationDictionary<IAutomatonProcedure>(uniqueProcedureNames);
            foreach (var (from, procedureString) in procedures)
            {
                if (!procDict.TryGetValue(procedureString, out var procedure))
                    throw new UnknownActionException($"ERROR: {procedureString} is not a name of known procedure");

                var (automata, strings) = ParseOutVariables(from, autVarDict, stringVariables);

                procedure.Process(automata, strings);
            }
        }

        // I kind of dug my own grave here :(
        // Ideal thing to do would be to re-do the whole parser, but I don't think I have enough time, and most importantly,
        // skill (I think I would have to learn how to write real parsers and tokenizers..)
        private (List<Automaton>, List<string>) ParseOutVariables(string from,
            Dictionary<string, Automaton> automatonVariables,
            Dictionary<string, string> stringVariables)
        {
            var inputs = from.Split(",");
            var automataList = new List<Automaton>();
            var stringsList = new List<string>();

            foreach (var input in inputs)
            {
                if (automatonVariables.ContainsKey(input))
                {
                    automataList.Add(automatonVariables[input]);
                    continue;
                }

                if (stringVariables.ContainsKey(input))
                {
                    stringsList.Add(stringVariables[input]);
                    continue;
                }

                throw new ProcedureException($"Unknown variable found during procedure processing: {input}");
            }

            return (automataList, stringsList);
        }
    }
}