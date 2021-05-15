using System.Collections.Generic;
using System.Linq;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.AutomatonTransformations.EpsilonTransitionRemoval
{
    public class BasicEpsilonRemover : IEpsilonRemover
    {
        public BasicEpsilonRemover(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public AutomatonType IntendedType { get; } = AutomatonType.EpsilonNfa;

        public Automaton Transform(Automaton input)
        {
            return RemoveEpsilonTransitions(input);
        }

        public Automaton RemoveEpsilonTransitions(Automaton inputAutomaton)
        {
            // old accepting states are a subset of new accepting states so we can initialise it like this
            var newAcceptingStates = new HashSet<INode>(inputAutomaton.AcceptingStates);
            
            // nodes wont change, only transitions will, so we can create a new graph
            var newGraph =
                GraphGenerator.GenerateGraph(Configuration.GraphType, inputAutomaton.StatesAndTransitions.Nodes);

            // new alphabet will be the same as the old one, but without the epsilon transitions
            var newAlphabet = inputAutomaton.Alphabet
                .Where(item => !item.Equals(Configuration.EpsilonTransitionLabel))
                .ToHashSet();

            // here lies the bulk of this algorithm, which is creating new transitions and accepting states
            foreach (var state in inputAutomaton.StatesAndTransitions.Nodes)
                CreateNewTransitionsAndAcceptingStates(inputAutomaton, newGraph, state, newAcceptingStates, newAlphabet);

            return new Automaton(inputAutomaton.InitialState, newAcceptingStates, newGraph, newAlphabet,
                inputAutomaton.Name);
        }


        // standard dfs, finds states reachable by epsilon transitions
        private HashSet<INode> StatesReachableByEpsilonTransitions(IGraph<INode, ILabel> graph, INode startState)
        {
            var visitedStates = new HashSet<INode>();
            var stateStack = new Stack<INode>();
            stateStack.Push(startState);

            while (stateStack.Count != 0)
            {
                var currentState = stateStack.Pop();
                visitedStates.Add(currentState);

                foreach (var neighbour in graph.GetNeighbours(currentState))
                {
                    var traversalCondition =
                        graph.GetTransitionLabels(currentState, neighbour)
                            .Contains(Configuration.EpsilonTransitionLabel)
                        && !visitedStates.Contains(neighbour);

                    if (traversalCondition) stateStack.Push(neighbour);
                }
            }

            return visitedStates;
        }

        private HashSet<INode> GetNeighboursReachableBySpecificLetter(IGraph<INode, ILabel> graph, INode state,
            ILabel letter)
        {
            return graph.GetNeighbours(state)
                .Where(neigh => graph.GetTransitionLabels(state, neigh).Contains(letter))
                .ToHashSet();
        }

        private HashSet<INode> GetNextTwo(HashSet<INode> nextOne, ILabel letter, Automaton automaton)
        {
            var result = new HashSet<INode>();
            foreach (var state in nextOne)
                result.UnionWith(GetNeighboursReachableBySpecificLetter(automaton.StatesAndTransitions, state, letter));

            return result;
        }

        private void NextThree(Automaton automaton, INode currentNode, HashSet<INode> nextTwo,
            IGraph<INode, ILabel> newGraph, ILabel letter)
        {
            foreach (var state in nextTwo)
            {
                foreach (var reachable in StatesReachableByEpsilonTransitions(automaton.StatesAndTransitions, state))
                    newGraph.AddTransition(currentNode, reachable, letter);
            }
            
        }

        private void CreateNewTransitionsAndAcceptingStates(Automaton automaton, IGraph<INode, ILabel> newGraph,
            INode currentState, HashSet<INode> newAcceptingStates, HashSet<ILabel> newAlphabet)
        {
            // as a first step, we find all states reachable only by epsilon transitions from current state
            // This also includes current state
            var nextOne =
                StatesReachableByEpsilonTransitions(automaton.StatesAndTransitions, currentState);

            // if any of these states is accepting state, we make current state an accepting state
            if (nextOne.Any(state => automaton.AcceptingStates.Contains(state))) newAcceptingStates.Add(currentState);

            // then we go through every letter in alphabet
            foreach (var letter in newAlphabet)
            {
                // we obtain all states reachable from the nextOne set by current letter
                var nextTwo = GetNextTwo(nextOne, letter, automaton);
                // then we find all states that are reachable by epsilon transition from this nextTwo set, and add them to our newGraph
                NextThree(automaton, currentState, nextTwo, newGraph, letter);
            }
        }
    }
}