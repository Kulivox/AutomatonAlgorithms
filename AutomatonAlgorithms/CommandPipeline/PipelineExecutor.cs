using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AutomatonAlgorithms.AutomatonProcedures;
using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.AutomatonTransformations;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.Parsers;
using SectionAction = System.Action<
    string,
    System.Collections.Generic.Dictionary<string, AutomatonAlgorithms.Automatons.Automaton>,
    System.Collections.Generic.Dictionary<string, string>>;

namespace AutomatonAlgorithms.CommandPipeline
{
    public class PipelineExecutor
    {
        private const string PipelineFileRegex = @"(\w+){([^{}]+)}";

        private const string InitLineRegex = @"^([A|T])\s+(\w+)\s*=\s*(\S+)\s*$";

        private const string TransformationsLineRegex = @"^(\w+)((->\w+)+)=>(\$?\w+)$";
        
        private const string ProcedureRegex = @"^(\w+)->(\w+)$";

        private readonly string[] _sectionOrder = { "init", "transformations", "procedures" };

        private IConfiguration Configuration { get; }
        private AutomatonLoader AutLoader { get; }

        private readonly Dictionary<string, SectionAction> _sectionDictionary;

        public PipelineExecutor(IConfiguration configuration, AutomatonLoader autLoader)
        {
            Configuration = configuration;
            AutLoader = autLoader;
            
            _sectionDictionary = new Dictionary<string, SectionAction>  {
                {"init", ExecuteInit},
                {"transformations", ExecuteTransformations},
                {"procedures", ExecuteProcedures}
            };
        }


        private void ExecuteInit(string input, Dictionary<string, Automaton> autVarDict,
            Dictionary<string, string> textVarDict)
        {
             var inputLines =  Regex.Split(input, "\r\n|\r|\n");
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
                         var automatonTask =  AutLoader.TryLoadAutomaton(match.Groups[3].ToString());
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
        
        private void ExecuteTransformations(string input, Dictionary<string, Automaton> autVarDict,
            Dictionary<string, string> textVarDict)
        {
            var inputLines =  Regex.Split(input, "\r\n|\r|\n");
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

                foreach (var transformation in transformations)
                {
                    tempAut = transformationDict[transformation].Transform(tempAut);
                }

                if (to[0] == '$')
                {
                    autVarDict.Add(to[1..], tempAut);
                }
                else
                {
                    autVarDict[to] = tempAut;
                }
                
            }
            
        }

        private Dictionary<string, T> GetOperationDictionary<T>(HashSet<string> uniqueTransformationNames)
        {
            var resultType = typeof(T);
            var transformationDict = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => resultType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Where(t => uniqueTransformationNames.Contains(t.Name))
                .ToDictionary(type => type.Name,
                    type => (T) Activator.CreateInstance(type, Configuration));
            return transformationDict;
        }

        private void ExecuteProcedures(string input, Dictionary<string, Automaton> autVarDict,
            Dictionary<string, string> textVarDict)
        {
            var inputLines =  Regex.Split(input, "\r\n|\r|\n");
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
            foreach (var (from, procedure) in procedures)
            {
                procDict[procedure].Process(autVarDict[from]);
            }

        }

        private Dictionary<string, string> LoadFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileLoadException("Specified input file does not exist");
            
            var rx = new Regex(PipelineFileRegex, RegexOptions.Compiled);

            var matches = rx.Matches(File.ReadAllText(filePath));
            if (matches.Count == 0)
                throw new FormatException("Input file doesn't have specified format");

            var sectionsDictionary = new Dictionary<string, string>();
            foreach (Match match in matches)
            {
                if (!sectionsDictionary.TryAdd(match.Groups[1].ToString(), match.Groups[2].ToString()))
                    throw new FormatException("Multiple sections of same type are not allowed");
            }

            if (!sectionsDictionary.ContainsKey("init"))
                throw new FormatException("input file has to contain init section");

            return sectionsDictionary;
        }

        public void LoadAndExecute(string path)
        {
            var automatonVariables = new Dictionary<string, Automaton>();
            var textVariables = new Dictionary<string, string>();

            var sectionStrings = LoadFile(path);
            

            foreach (var section in _sectionOrder)
            {
                var sectionString = sectionStrings[section];
                
                if (_sectionDictionary.ContainsKey(section))
                    _sectionDictionary[section](sectionString, automatonVariables, textVariables);
                
            }
            
        }
    }
}