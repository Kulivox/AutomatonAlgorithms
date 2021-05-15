using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutomatonAlgorithms.AutomatonTransformations;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.FailedOperations;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.PureScriptExceptions;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;

namespace AutomatonAlgorithms.CommandPipeline.ScriptSections.Transformation
{
    public class TransformationsSection : Section
    {
        private const string TransformationsLineRegex = @"^(\w+)((->\w+)+)=>(\$?\w+)$";

        public TransformationsSection(IConfiguration configuration) : base(configuration)
        {
        }

        public override int Priority { get; } = 5;


        public override void ExecuteSection(string sectionString, Dictionary<string, Automaton> automatonVariables,
            Dictionary<string, string> stringVariables)
        {
            ExecuteTransformations(sectionString, automatonVariables);
        }
        
        private void ExecuteTransformations(string input, Dictionary<string, Automaton> autVarDict)
        {
            var inputLines = Regex.Split(input, "\r\n|\r|\n");
            var rx = new Regex(TransformationsLineRegex);

            var uniqueTransformationNames = new HashSet<string>();
            var transformationLines = new List<(string from, List<string> transNames, string to)>();

            foreach (var line in inputLines.Where(l => !string.IsNullOrEmpty(l)))
            {
                var lineWithoutWhitespace = Regex.Replace(line, @"\s", "");
                var match = rx.Match(lineWithoutWhitespace);
                if (!match.Success)
                    throw new PipelineTransformationException($"Incorrect syntax of transformation: {line}");

                var from = match.Groups[1].ToString();
                var to = match.Groups[4].ToString();
                var transformations = new List<string>();

                // we skip first item because it is empty (line looks like this: '-> stuff -> ...)
                foreach (var tr in match.Groups[2].ToString().Split("->").Skip(1))
                {
                    transformations.Add(tr);
                    uniqueTransformationNames.Add(tr);
                }

                transformationLines.Add((from, transformations, to));
            }

            var transformationDict =
                GetOperationDictionary<IAutomatonTransformation>(uniqueTransformationNames);
            

            foreach (var (from, transformations, to) in transformationLines)
            {
                var fromAut = autVarDict[from];
                var tempAut = fromAut;

                foreach (var transformationString in transformations)
                {
                    if (!transformationDict.TryGetValue(transformationString, out var transformation))
                        throw new UnknownActionException(
                            $"{transformationString} is not a name of a valid transformation");
                   
                    
                    var autType = tempAut.GetAutomatonType(Configuration.EpsilonTransitionLabel);
                    if (transformation.IntendedType != autType)
                    {
                        Console.WriteLine($"WARNING [{from} -> {to}]: {transformationString} is not intended for automatons of" +
                                          $" {autType.ToString()} type\n" +
                                          "The transformation might fail or not work as intended");
                    }

                    try
                    {
                        tempAut = transformation.Transform(tempAut);
                    }
                    catch (Exception e)
                    {
                        throw new TransformationFailedException($"Transformation of [{from} -> {to}] failed\n" +
                                                                $"Reason: {e.Message}");
                    }
                    
                }
                
                // $ is used to create new variables, if it is used on the right side of the expression
                if (to[0] == '$')
                    autVarDict.Add(to[1..], tempAut);
                else
                    autVarDict[to] = tempAut;
            }
        }
    }
}