using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using AutomatonAlgorithms.CommandPipeline.ScriptSections;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Init;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Procedure;
using AutomatonAlgorithms.CommandPipeline.ScriptSections.Transformation;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.Parsers;

namespace AutomatonAlgorithms.CommandPipeline
{
    public class PipelineExecutor
    {
        private const string PipelineFileRegex = @"(\w+){([^{}]+)}";

        private readonly List<Section> _sectionDictionary;

        public PipelineExecutor(IConfiguration configuration, AutomatonLoader autLoader)
        {
            Configuration = configuration;
            AutLoader = autLoader;

            // maybe improve this by using automatic dependency injection
            _sectionDictionary = new List<Section>
            {
                new InitSection(AutLoader, Configuration),
                new TransformationsSection(Configuration),
                new ProceduresSection(Configuration)
            };
        }

        private IConfiguration Configuration { get; }
        private AutomatonLoader AutLoader { get; }


        private List<(Section section, string sectString)> LoadFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileLoadException("Specified input file does not exist");

            var rx = new Regex(PipelineFileRegex, RegexOptions.Compiled);

            var matches = rx.Matches(File.ReadAllText(filePath));
            if (matches.Count == 0)
                throw new FormatException("Input file doesn't have specified format");

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

            var sectionStrings = LoadFile(path);
        }
    }
}