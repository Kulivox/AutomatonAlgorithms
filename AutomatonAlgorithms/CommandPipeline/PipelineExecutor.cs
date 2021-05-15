using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutomatonAlgorithms.CommandPipeline.ScriptSections;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Exceptions.PureScriptExceptions;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.Parsers;

namespace AutomatonAlgorithms.CommandPipeline
{
    public class PipelineExecutor
    {
        private const string PipelineFileRegex = @"\s*(\w+)\s*{([^{}]+)}";

        
        private IConfiguration Configuration { get; }
        private AutomatonLoader AutLoader { get; }

        public PipelineExecutor(IConfiguration configuration, AutomatonLoader autLoader)
        {
            Configuration = configuration;
            AutLoader = autLoader;
        }

        
        private List<(Section section, string sectString)> LoadFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ScriptFormatException("Specified input file does not exist");

            var rx = new Regex(PipelineFileRegex, RegexOptions.Compiled);

            var matches = rx.Matches(File.ReadAllText(filePath));
            if (matches.Count == 0)
                throw new ScriptFormatException("Input file doesn't have specified format");

            var sectionList = new List<(Section section, string sectString)>();
            var sectionFactory = new SectionFactory(AutLoader, Configuration);

            foreach (Match match in matches)
                sectionList.Add((sectionFactory.BuildSection(match.Groups[1].ToString()), match.Groups[2].ToString()));

            return sectionList;
        }

        public void LoadAndExecute(string path)
        {
            var automatonVariables = new Dictionary<string, Automaton>();
            var textVariables = new Dictionary<string, string>();

            List<(Section section, string sectString)> sectionsAndStrings;
            try
            {
                sectionsAndStrings = LoadFile(path);
            }
            catch (Exception e ) when (e is ScriptException)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
            foreach (var (section, sectString) in sectionsAndStrings.OrderBy(it => it.section.Priority))
            {
                try
                {
                    section.ExecuteSection(sectString, automatonVariables, textVariables);
                }
                catch (Exception e) when(e is ScriptException)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                

            }
        }
    }
}