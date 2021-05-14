using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.Parsers
{
    public class AutomatonLoader
    {
        private readonly IConfiguration _configuration;
        
        public AutomatonLoader()
        {
            _configuration = new BaseConfiguration();
        } 

        public AutomatonLoader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private const string RegexOfInput = @"#states\r?\n((\w+\r?\n)+)" +
                                            @"#initial\r?\n(\w+\r?\n)" +
                                            @"#accepting\r?\n((\w+\r?\n)+)" +
                                            @"#alphabet\r?\n(([A-Za-z0-9$]+\r?\n)+)" +
                                            @"#transitions\r?\n(([A-Za-z0-9:>,$]+\r?\n)+([A-Za-z0-9:>,$]+)?)";

        private const string RegexOfTransition = @"(\w+):(.*)>(.*)";

        
        // todo improve data structures, not ideal for searching
        private IEnumerable<INode> GetStates(Group group)
        {
            return group.Value.Split("\n").Where(item => !string.IsNullOrEmpty(item)).Select(it => new BasicNode {Id = it});
        }
        
        private INode GetInitialState(Group group, IEnumerable<INode> allStates)
        {
            INode node = new BasicNode() {Id = group.Value.Replace("\n", "")};
            if (!allStates.Contains(node))
                throw new FormatException("Unknown initial state");

            return node;
        }
        
        
        private List<ILabel> GetAlphabet(Group group)
        {
            var result = new List<ILabel>() {_configuration.EpsilonTransitionLabel};
            result.AddRange(group.Value.Split("\n")
                .Where(item => !string.IsNullOrEmpty(item))
                .Select(item => new BasicLabel{Name = item}));
            return result;
        }
        
        private IEnumerable<INode> GetAcceptingStates(Group group, IEnumerable<INode> allStates)
        {
            IEnumerable<INode> nodes =  group.Value.Split("\n")
                .Where(item => !string.IsNullOrEmpty(item))
                .Select(it => new BasicNode {Id = it});
            
            var acceptingStates = nodes as INode[] ?? nodes.ToArray();
            
            if (acceptingStates.Except(allStates).Any())
                throw new FormatException("Unknown accepting state(s)");

            return acceptingStates;
        }

        private IEnumerable<INode> DestinationStates(string statesAsString)
        {
            statesAsString = Regex.Replace(statesAsString, @"\s+", "");
            return statesAsString.Split(",").Select(item => new BasicNode(){Id = item});
        }

        private bool ValidateTransition(INode from, INode to, ILabel label, IEnumerable<INode> nodes,
            List<ILabel> alphabet)
        {
            var nodeArray = nodes as INode[] ?? nodes.ToArray();
            
            if (!nodeArray.Contains(from))
                return false;

            if (!nodeArray.Contains(to))
                return false;

            if (!alphabet.Contains(label))
                return false;

            return true;
        }
        
        
        
        private void ParseAndAddTransitions(Group match, IEnumerable<INode> states, List<ILabel> alphabet,
            IGraph<INode, ILabel> graph)
        {
            var re = new Regex(RegexOfTransition);
            
            foreach (var stringTransition in match.Value.Split("\n").Where(item => !string.IsNullOrEmpty(item)))
            {
                var transMatch = re.Match(stringTransition);
                
                var from = new BasicNode() {Id = transMatch.Groups[1].Value};
                var label = new BasicLabel() {Name = transMatch.Groups[2].Value};
                var destinationStates = DestinationStates(transMatch.Groups[3].Value);

                


                foreach (var destination in destinationStates)
                {
                    if (!ValidateTransition(from, destination, label, states, alphabet))
                        throw new FormatException($"Bad format of transition: {stringTransition}");
                    
                    graph.CreateTransition(from, destination, label);
                }

            }
            
        }

        public async Task<Automaton> TryLoadAutomaton(string path)
        {
            if (new FileInfo(path).Length > _configuration.MaxFileSizeBytes)
                throw new FileLoadException($"File size too large for file {path}");
            
            var text = await File.ReadAllTextAsync(path);
            text = Regex.Replace(text, @"\r\n?|\n", "\n");

            var re = new Regex(RegexOfInput);
            var match = re.Match(text);

            if (!match.Success)
                throw new FormatException($"File format wrong for file {path}");

            var allStates = GetStates(match.Groups[1]);
            var initialState = GetInitialState(match.Groups[3], allStates);
            var acceptingStates = GetAcceptingStates(match.Groups[4], allStates);
            var alphabet = GetAlphabet(match.Groups[6]);

            var graph = GraphGenerator.GenerateGraph(_configuration.GraphType, allStates);
            ParseAndAddTransitions(match.Groups[8], allStates, alphabet, graph);

            return new Automaton(initialState, acceptingStates.ToHashSet(), graph, alphabet.ToHashSet(),
                Path.GetFileNameWithoutExtension(path));
        }
    }
}