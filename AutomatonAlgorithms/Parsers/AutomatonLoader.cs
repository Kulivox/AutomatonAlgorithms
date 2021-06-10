using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;
using AutomatonAlgorithms.Parsers.Exceptions;

namespace AutomatonAlgorithms.Parsers
{
    public class AutomatonLoader
    {
        private const string RegexOfInput = @"#states\r?\n((\w+\r?\n)+)" +
                                            @"#initial\r?\n(\w+\r?\n)" +
                                            @"#accepting\r?\n((\w+\r?\n)+)" +
                                            @"#alphabet\r?\n(([A-Za-z0-9$]+\r?\n)+)" +
                                            @"#transitions\r?\n(([A-Za-z0-9:>,$]+\r?\n)+([A-Za-z0-9:>,$]+)?)";

        private const string RegexOfTransition = @"(\w+):(.*)>(.*)";
        private readonly IConfiguration _configuration;

        public AutomatonLoader(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        private IEnumerable<INode> GetStates(Group group)
        {
            return group.Value.Split("\n").Where(item => !string.IsNullOrEmpty(item))
                .Select(it => new BasicNode {Id = it}).ToHashSet();
        }

        private INode GetInitialState(Group group, HashSet<INode> allStates)
        {
            INode node = new BasicNode {Id = group.Value.Replace("\n", "")};
            if (!allStates.Contains(node))
                throw new AutomatonFileFormatException("Unknown initial state");

            return node;
        }


        private HashSet<ILabel> GetAlphabet(Group group)
        {
            // epsilon transition labels are not added here, because not every automaton has them, but because
            // the format specifies that they don't have to be in the alphabet explicitly, they are added later
            var result = new HashSet<ILabel>();
            foreach (var item in group.Value.Split("\n")
                .Where(item => !string.IsNullOrEmpty(item))
                .Select(item => new BasicLabel {Name = item}))
                result.Add(item);
            return result;
        }

        private IEnumerable<INode> GetAcceptingStates(Group group, IEnumerable<INode> allStates)
        {
            IEnumerable<INode> nodes = group.Value.Split("\n")
                .Where(item => !string.IsNullOrEmpty(item))
                .Select(it => new BasicNode {Id = it});

            var acceptingStates = nodes as INode[] ?? nodes.ToArray();

            if (acceptingStates.Except(allStates).Any())
                throw new AutomatonFileFormatException("Unknown accepting state(s)");

            return acceptingStates;
        }

        private IEnumerable<INode> DestinationStates(string statesAsString)
        {
            statesAsString = Regex.Replace(statesAsString, @"\s+", "");
            return statesAsString.Split(",").Select(item => new BasicNode {Id = item});
        }

        private bool ValidateTransition(INode from, INode to, ILabel label, HashSet<INode> nodes,
            HashSet<ILabel> alphabet)
        {
            if (!nodes.Contains(from))
                return false;

            if (!nodes.Contains(to))
                return false;


            // if the alphabet contains the label, everything is ok
            if (alphabet.Contains(label))
                return true;
            // if it doesn't, we firs check whether it isn't an epsilon transition, and if it is, we add it to the alphabet
            // and return true
            if (label.Equals(_configuration.EpsilonTransitionLabel))
            {
                alphabet.Add(_configuration.EpsilonTransitionLabel);
                return true;
            }

            // else we don't know this transition and return false
            return false;
        }


        private void ParseAndAddTransitions(Group match, HashSet<INode> states, HashSet<ILabel> alphabet,
            IGraph<INode, ILabel> graph)
        {
            var re = new Regex(RegexOfTransition);

            foreach (var stringTransition in match.Value.Split("\n").Where(item => !string.IsNullOrEmpty(item)))
            {
                var transMatch = re.Match(stringTransition);

                var from = new BasicNode {Id = transMatch.Groups[1].Value};
                var label = new BasicLabel {Name = transMatch.Groups[2].Value};
                var destinationStates = DestinationStates(transMatch.Groups[3].Value);


                foreach (var destination in destinationStates)
                {
                    if (!ValidateTransition(from, destination, label, states, alphabet))
                        throw new AutomatonFileFormatException($"Bad format of transition: {stringTransition}");

                    graph.AddTransition(from, destination, label);
                }
            }
        }

        public Automaton TryLoadAutomaton(string path, string name)
        {
            if (!File.Exists(path))
                throw new AutomatonFileFormatException($"File doesn't exist: {path}");

            if (new FileInfo(path).Length > _configuration.MaxFileSizeBytes)
                throw new AutomatonFileFormatException($"File size too large: {path}");


            var text = File.ReadAllText(path);
            text = Regex.Replace(text, @"\r\n?|\n", "\n");

            var re = new Regex(RegexOfInput);
            var match = re.Match(text);

            if (!match.Success)
                throw new AutomatonFileFormatException($"Wrong file format of file: {path}");

            var allStates = GetStates(match.Groups[1]).ToHashSet();
            var initialState = GetInitialState(match.Groups[3], allStates);
            var acceptingStates = GetAcceptingStates(match.Groups[4], allStates);
            var alphabet = GetAlphabet(match.Groups[6]);

            var graph = GraphGenerator.GenerateGraph(_configuration.GraphType, allStates);
            ParseAndAddTransitions(match.Groups[8], allStates, alphabet, graph);

            return new Automaton(initialState, acceptingStates.ToHashSet(), graph, alphabet.ToHashSet(), name);
        }

        public Automaton TryLoadAutomaton(string path)
        {
            return TryLoadAutomaton(path, Path.GetFileNameWithoutExtension(path));
        }
    }
}