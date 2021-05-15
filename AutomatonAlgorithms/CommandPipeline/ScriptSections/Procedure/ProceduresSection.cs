using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutomatonAlgorithms.AutomatonProcedures;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Procedure
{
    public class ProceduresSection : Section
    {
        private const string ProcedureRegex = @"^(\w+)->(\w+)$";

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
            Dictionary<string, string> textVarDict)
        {
            var inputLines = Regex.Split(input, "\r\n|\r|\n");
            var rx = new Regex(ProcedureRegex);

            var procedures = new List<(string from, string procedure)>();
            var uniqueProcedureNames = new HashSet<string>();


            foreach (var line in inputLines.Where(l => !string.IsNullOrEmpty(l)))
            {
                var lineWithoutWhitespace = Regex.Replace(line, @"\s", "");
                var match = rx.Match(lineWithoutWhitespace);
                if (!match.Success)
                    throw new PipelineTransformationException($"Incorrect syntax of procedure: {line}");

                var from = match.Groups[1].ToString();
                var procedure = match.Groups[2].ToString();

                procedures.Add((from, procedure));
                uniqueProcedureNames.Add(procedure);
            }

            var procDict = GetOperationDictionary<IAutomatonProcedure>(uniqueProcedureNames);
            foreach (var (from, procedure) in procedures) procDict[procedure].Process(autVarDict[@from]);
        }
    }
}