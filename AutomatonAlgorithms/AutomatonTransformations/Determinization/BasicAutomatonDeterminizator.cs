using System.Collections.Generic;
using System.Linq;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.DataStructures.Comparers;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.AutomatonTransformations.Determinization
{
    public class BasicAutomatonDeterminizator : IDeterminizator
    {
        public AutomatonType IntendedType { get; } = AutomatonType.Nfa;
        public BasicAutomatonDeterminizator(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        

        public Automaton Transform(Automaton input)
        {
            return MakeAutomatonDeterministic(input);
        }


        public Automaton MakeAutomatonDeterministic(Automaton inputAutomaton)
        {
            // new sink node is created. This node is used as a replacement for non existing transitions,
            // under which NFA can get stuck
            
            var sink = new HashSet<INode> {new BasicNode {Id = "sink"}};
            var newStates = new HashSet<HashSet<INode>>(new NodeHashSetComparer()) {sink};
            var statesAndTransitions = new Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>>();

            // sink is prepared, which means that new transitions under every
            // letter leading from an to the sink are prepared
            PrepareSink(inputAutomaton, statesAndTransitions, sink);

            // main determinization algorithm is in this method
            FindNewStatesAndTransitions(inputAutomaton, newStates, statesAndTransitions, sink);

            var newRenamedStates = new Dictionary<HashSet<INode>, INode>(new NodeHashSetComparer());
            var newAcceptingStates = new HashSet<INode>();

            RenameNewStates(inputAutomaton, newStates, newRenamedStates, newAcceptingStates);

            var newGraph = CreateNewGraph(newRenamedStates, statesAndTransitions);

            return new Automaton(newRenamedStates[new HashSet<INode> {inputAutomaton.InitialState}],
                newAcceptingStates, newGraph, inputAutomaton.Alphabet, inputAutomaton.Name);
        }

        private void PrepareSink(Automaton inputAutomaton,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions, HashSet<INode> sink)
        {
            statesAndTransitions.Add(sink, new Dictionary<ILabel, HashSet<INode>>());
            foreach (var letter in inputAutomaton.Alphabet) statesAndTransitions[sink].Add(letter, sink);
        }

        private IGraph<INode, ILabel> CreateNewGraph(Dictionary<HashSet<INode>, INode> newRenamedStates,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions)
        {
            var newGraph = GraphGenerator.GenerateGraph(Configuration.GraphType, newRenamedStates.Values);

            foreach (var (state, transitions) in statesAndTransitions)
            foreach (var (label, neighbour) in transitions)
            {
                var renamedS = newRenamedStates[state];
                var renamedNeigh = newRenamedStates[neighbour];
                newGraph.AddTransition(renamedS, renamedNeigh, label);
            }

            return newGraph;
        }

        // names each new state set (creates Nodes for every unique state set), also prepares new Accepting states
        private static void RenameNewStates(Automaton inputAutomaton, HashSet<HashSet<INode>> newStates,
            Dictionary<HashSet<INode>, INode> newRenamedStates,
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

        // this method finds new states of determinized automaton, and transitions between them
        private static void FindNewStatesAndTransitions(Automaton inputAutomaton, HashSet<HashSet<INode>> newStates,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions, HashSet<INode> sink)
        {
            // my determinization approach is based on dfs of new state sets,
            // which are created by exploring neighbours of these new state sets
            // it is initialised by state set containing single element, which is the initial state of the NFA
            var unexploredStates = new Stack<HashSet<INode>>();
            unexploredStates.Push(new HashSet<INode> {inputAutomaton.InitialState});

            while (unexploredStates.Count > 0)
            {
                
                var currentState = unexploredStates.Pop();
                newStates.Add(currentState);

                // prepares new transition dict, containing data structures for transitions to other sets of states
                PrepareDictForNewState(inputAutomaton, statesAndTransitions, currentState);
                
                // fills out this new transition dict
                FindNewNeighbourStates(inputAutomaton, currentState, statesAndTransitions);

                foreach (var (key, value) in statesAndTransitions[currentState])
                {   
                    // if there are no transitions under the label (key), new transition to sink is created
                    if (value.Count == 0)
                        statesAndTransitions[currentState][key].Add(sink.First());
                    
                    // if new set of states is created, we push it to the stack so it can be explored further
                    if (!newStates.Contains(value)) unexploredStates.Push(value);
                }
            }
        }

        
        private static void FindNewNeighbourStates(Automaton inputAutomaton, HashSet<INode> currentState,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions)
        {
            // this foreach goes through every state in current set of states
            foreach (var subState in currentState)
            {
                // then goes through every neighbour of current sub state
                foreach (var neighbour in inputAutomaton.StatesAndTransitions.GetNeighbours(subState))
                {
                    // and creates transitions and adds these nodes to neighbour sets of states
                    CreateNeighbourStatesAndHowToGetToThem(inputAutomaton, currentState, statesAndTransitions, subState,
                        neighbour);
                }
                    
            }
                
            
        }

        private static void CreateNeighbourStatesAndHowToGetToThem(Automaton inputAutomaton,
            HashSet<INode> currentState,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions, INode subState,
            INode neighbour)
        {
            // retrieves all labels of this transition
            var transitionLetters =
                inputAutomaton.StatesAndTransitions.GetTransitionLabels(subState, neighbour);

            // adds this state to sets which are reachable by transLetter
            foreach (var transLetter in transitionLetters)
                statesAndTransitions[currentState][transLetter].Add(neighbour);
        }

        private static void PrepareDictForNewState(Automaton inputAutomaton,
            Dictionary<HashSet<INode>, Dictionary<ILabel, HashSet<INode>>> statesAndTransitions,
            HashSet<INode> currentState)
        {
            statesAndTransitions.Add(currentState, new Dictionary<ILabel, HashSet<INode>>());

            foreach (var letter in inputAutomaton.Alphabet)
                statesAndTransitions[currentState].Add(letter, new HashSet<INode>());
        }
    }
}