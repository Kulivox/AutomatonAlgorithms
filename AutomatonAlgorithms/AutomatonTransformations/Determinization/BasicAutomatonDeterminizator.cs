using System.Collections.Generic;
using System.Linq;
using AutomatonAlgorithms.Automatons;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Comparers;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.AutomatonTransformations.Determinization
{
    public class BasicAutomatonDeterminizator : IDeterminizator
    {
        public IConfiguration Configuration { get; }
        
        public BasicAutomatonDeterminizator(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Automaton Transform(Automaton input)
        {
            return MakeAutomatonDeterministic(input);
        }

        
        public Automaton MakeAutomatonDeterministic(Automaton inputAutomaton)
        {

            var sink = new HashSet<INode>() {new BasicNode() {Id = "sink"}};
            var newStates = new HashSet<HashSet<INode>>(new NodeHashSetComparer()) {sink};
            var statesAndTransitions = new Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>>();
            
            PrepareSink(inputAutomaton, statesAndTransitions, sink);
            
            FindNewStatesAndTransitions(inputAutomaton, newStates, statesAndTransitions, sink);

            var newRenamedStates = new Dictionary<HashSet<INode>, INode>(new NodeHashSetComparer());
            var newAcceptingStates = new HashSet<INode>();
            
            RenameNewStates(inputAutomaton, newStates, newRenamedStates, newAcceptingStates);

            var newGraph = CreateNewGraph(newRenamedStates, statesAndTransitions);

            return new Automaton(newRenamedStates[new HashSet<INode> {inputAutomaton.InitialState}],
                newAcceptingStates, newGraph, inputAutomaton.Alphabet, inputAutomaton.Name);
        }

        private void PrepareSink(Automaton inputAutomaton, Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions, HashSet<INode> sink)
        {
            statesAndTransitions.Add(sink, new Dictionary<ILabel, HashSet<INode>>());
            foreach (var letter in inputAutomaton.Alphabet)
            {
                statesAndTransitions[sink].Add(letter, sink);
            }
        }

        private IGraph<INode, ILabel> CreateNewGraph(Dictionary<HashSet<INode>, INode> newRenamedStates, Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions)
        {
            var newGraph = GraphGenerator.GenerateGraph(Configuration.GraphType, newRenamedStates.Values);

            foreach (var (state, transitions) in statesAndTransitions)
            {
                foreach (var (label, neighbour) in transitions)
                {
                    var renamedS = newRenamedStates[state];
                    var renamedNeigh = newRenamedStates[neighbour];
                    newGraph.CreateTransition(renamedS, renamedNeigh, label);
                }
            }

            return newGraph;
        }

        private static void RenameNewStates(Automaton inputAutomaton, HashSet<HashSet<INode>> newStates, Dictionary<HashSet<INode>, INode> newRenamedStates,
            HashSet<INode> newAcceptingStates)
        {
            var i = 0;

            foreach (var state in newStates)
            {
                newRenamedStates.Add(state, new BasicNode {Id = i.ToString()});

                if (inputAutomaton.AcceptingStates.Intersect(state).Any())
                    newAcceptingStates.Add(new BasicNode {Id = i.ToString()});

                i += 1;
            }
        }

        private static void FindNewStatesAndTransitions(Automaton inputAutomaton, HashSet<HashSet<INode>> newStates,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions, HashSet<INode> sink)
        {
            var unexploredStates = new Stack<HashSet<INode>>();
            unexploredStates.Push(new HashSet<INode> {inputAutomaton.InitialState});

            while (unexploredStates.Count > 0)
            {
                var currentState = unexploredStates.Pop();
                newStates.Add(currentState);
                
                PrepareDictForNewState(inputAutomaton, statesAndTransitions, currentState);
                

                FindNewNeighbourStates(inputAutomaton, currentState, statesAndTransitions);

                foreach (var (key, value) in statesAndTransitions[currentState])
                {
                    if (value.Count == 0)
                        statesAndTransitions[currentState][key].Add(sink.First());

                    if (!newStates.Contains(value))
                    {
                        unexploredStates.Push(value);
                    }
                    
                    
                }
            }
        }

        private static void FindNewNeighbourStates(Automaton inputAutomaton, HashSet<INode> currentState,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions)
        {
            

            foreach (var subState in currentState)
            {
                foreach (var neighbour in inputAutomaton.StatesAndTransitions.GetNeighbours(subState))
                {
                    CreateNeighbourStatesAndHowToGetToThem(inputAutomaton, currentState, statesAndTransitions, subState, neighbour);
                }
            }
        }

        private static void CreateNeighbourStatesAndHowToGetToThem(Automaton inputAutomaton, HashSet<INode> currentState,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions, INode subState, INode neighbour)
        {
            var transitionLetters =
                inputAutomaton.StatesAndTransitions.GetTransitionLabels(subState, neighbour);

            foreach (var transLetter in transitionLetters)
            {
                statesAndTransitions[currentState][transLetter].Add(neighbour);
            }
        }

        private static void PrepareDictForNewState(Automaton inputAutomaton,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions,
            HashSet<INode> currentState)
        {
            statesAndTransitions.Add(currentState, new Dictionary<ILabel, HashSet<INode>>());

            foreach (var letter in inputAutomaton.Alphabet)
            {
                statesAndTransitions[currentState].Add(letter, new HashSet<INode>());
            }
        }
    }
}