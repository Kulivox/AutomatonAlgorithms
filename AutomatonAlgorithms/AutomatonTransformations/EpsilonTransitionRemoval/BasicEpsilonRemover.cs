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

        public Automaton Transform(Automaton input)
        {
            return RemoveEpsilonTransitions(input);
        }

        public Automaton RemoveEpsilonTransitions(Automaton inputAutomaton)
        {
            var newAcceptingStates = new HashSet<INode>(inputAutomaton.AcceptingStates);
            var newGraph =
                GraphGenerator.GenerateGraph(Configuration.GraphType, inputAutomaton.StatesAndTransitions.Nodes);

            var newAlphabet = inputAutomaton.Alphabet
                .Where(item => !item.Equals(Configuration.EpsilonTransitionLabel))
                .ToHashSet();

            foreach (var state in inputAutomaton.StatesAndTransitions.Nodes)
                CreateNewTransitionsAndInitialStates(inputAutomaton, newGraph, state, newAcceptingStates, newAlphabet);

            return new Automaton(inputAutomaton.InitialState, newAcceptingStates, newGraph, newAlphabet,
                inputAutomaton.Name);
        }


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

        public void NextThree(Automaton automaton, INode currentNode, HashSet<INode> nextTwo,
            IGraph<INode, ILabel> newGraph, ILabel letter)
        {
            foreach (var state in nextTwo)
            foreach (var reachable in StatesReachableByEpsilonTransitions(automaton.StatesAndTransitions, state))
                newGraph.CreateTransition(currentNode, reachable, letter);
        }

        private void CreateNewTransitionsAndInitialStates(Automaton automaton, IGraph<INode, ILabel> newGraph,
            INode currentState, HashSet<INode> newAcceptingStates, HashSet<ILabel> newAlphabet)
        {
            var nextOne =
                StatesReachableByEpsilonTransitions(automaton.StatesAndTransitions, currentState);

            if (nextOne.Any(state => automaton.AcceptingStates.Contains(state))) newAcceptingStates.Add(currentState);

            foreach (var letter in newAlphabet)
            {
                var nextTwo = GetNextTwo(nextOne, letter, automaton);
                NextThree(automaton, currentState, nextTwo, newGraph, letter);
            }
        }
    }
}