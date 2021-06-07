using System.Collections.Generic;
using System.Linq;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.AutomatonTransformations.Canonization
{
    public class BasicCanonizer : ICanonizer
    {
        public AutomatonType IntendedType { get; } = AutomatonType.Dfa;

        private const int SequenceStart = 1;

        public BasicCanonizer(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public Automaton Transform(Automaton input)
        {
            return CanonizeAutomaton(input);
        }


        public Automaton CanonizeAutomaton(Automaton inputAutomaton)
        {
            var oldToNewStateAndTransition =
                RenameStatesAndSaveTransitions(inputAutomaton);


            return CreateNewAutomaton(inputAutomaton, oldToNewStateAndTransition);
        }

        private Automaton CreateNewAutomaton(Automaton inputAutomaton,
            Dictionary<INode, (INode node, List<INode> letterOrderedTransitions)> oldToNewStateAndTransition)
        {
            
            // new graph is created from canonized nodes
            var newGraph = GraphGenerator.GenerateGraph(Configuration.GraphType,
                oldToNewStateAndTransition.Values.Select(val => val.node));

            // transitions to neighbours are added
            foreach (var (node, neighbours) in oldToNewStateAndTransition.Values)
                foreach (var (neighbour, label) in neighbours.Zip(inputAutomaton.Alphabet))
                    newGraph.AddTransition(node, neighbour, label);

            // new initial state is just renamed old initial state
            var newInitialState = oldToNewStateAndTransition[inputAutomaton.InitialState].node;
            var newAcceptingStates =
                inputAutomaton.AcceptingStates.Select(state => oldToNewStateAndTransition[state].node).ToHashSet();
            
            var newAutomaton = new Automaton(newInitialState, newAcceptingStates, newGraph, inputAutomaton.Alphabet,
                inputAutomaton.Name);
            return newAutomaton;
        }

        private static Dictionary<INode, (INode node, List<INode> letterOrderedTransitions)>
            RenameStatesAndSaveTransitions(Automaton inputAutomaton)
        {
            // this data structure will store all explored nodes along with their translations and list of their translated
            // neighbours. Oder of these neighbours is based on order of the letters in automaton alphabet 
            // we also have to store the initial node inside, to make it possible to add renamed transitions during first iteration
            var oldToNewStateAndTransition = new Dictionary<INode, (INode node, List<INode> letterOrderedTransitions)>
            {
                {inputAutomaton.InitialState, (new BasicNode {Id = $"{SequenceStart}"}, new List<INode>())}
            };

            // data structures for DFS are created
            var statesToExplore = new Queue<INode>();
            statesToExplore.Enqueue(inputAutomaton.InitialState);

            var newNodeCounter = SequenceStart + 1;

            while (statesToExplore.Count > 0)
            {
                var currentState = statesToExplore.Dequeue();

                foreach (var letter in inputAutomaton.Alphabet)
                {
                    // neighbour for current letter is retrieved (there should exist precisely one such neighbour,
                    // canonization is used to compare minimized DFA
                    var letterNeigh = GetNeighbourForCurrentLetter(inputAutomaton, currentState, letter);

                    // we check whether we have already translated this letter neighbour
                    // if we have not, we translate it and add it as a neighbour of current node
                    if (!oldToNewStateAndTransition.ContainsKey(letterNeigh))
                    {
                        var newRenamedState = new BasicNode {Id = newNodeCounter.ToString()};
                        statesToExplore.Enqueue(letterNeigh);
                        oldToNewStateAndTransition.Add(letterNeigh, (newRenamedState, new List<INode>()));
                        oldToNewStateAndTransition[currentState].letterOrderedTransitions.Add(newRenamedState);

                        newNodeCounter += 1;
                    }
                    else // we just update neighbours of current node
                    {
                        oldToNewStateAndTransition[currentState]
                            .letterOrderedTransitions.Add(oldToNewStateAndTransition[letterNeigh].node);
                    }
                }
            }

            return oldToNewStateAndTransition;
        }


        private static INode GetNeighbourForCurrentLetter(Automaton inputAutomaton, INode currentState, ILabel letter)
        {
            var letterNeigh =
                inputAutomaton.StatesAndTransitions
                    .GetNeighbours(currentState)
                    .First(neigh =>
                        inputAutomaton.StatesAndTransitions.GetTransitionLabels(currentState, neigh)
                            .Contains(letter));

            return letterNeigh;
        }
    }
}