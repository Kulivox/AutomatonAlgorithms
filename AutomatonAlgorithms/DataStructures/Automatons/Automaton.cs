using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.Automatons
{
    public class Automaton
    {
        public string Name { get; }
        public INode InitialState { get; }
        
        public HashSet<INode> AcceptingStates { get; }
        
        public IGraph<INode, ILabel> StatesAndTransitions { get; }
        
        public SortedSet<ILabel> Alphabet { get; }

        public Automaton(INode initialState, HashSet<INode> acceptingStates, IGraph<INode, ILabel> statesAndTransitions,
            IEnumerable<ILabel> alphabet, string name)
        {
            InitialState = initialState;
            AcceptingStates = acceptingStates;
            StatesAndTransitions = statesAndTransitions;
            Name = name;

            Alphabet = new SortedSet<ILabel>();
            foreach (var letter in alphabet)
            {
                Alphabet.Add(letter);
            }

        }

        public override string ToString()
        {
            var strB = new StringBuilder(StatesAndTransitions.Nodes.Count * 20);

            strB.Append("#states\n");
            foreach (var state in StatesAndTransitions.Nodes)
            {
                strB.Append($"{state.Id}\n");
            }
            
            strB.Append("#initial\n");
            strB.Append($"{InitialState.Id}\n");
            
            strB.Append("#accepting\n");
            foreach (var state in AcceptingStates)
            {
                strB.Append($"{state.Id}\n");
            }
            
            strB.Append("#alphabet\n");
            foreach (var letter in Alphabet)
            {
                strB.Append($"{letter.Name}\n");
            }
            
            strB.Append("#transitions\n");
            foreach (var state in StatesAndTransitions.Nodes)
            {
                foreach (var neigh in StatesAndTransitions.GetNeighbours(state))
                {
                    var labels = StatesAndTransitions.GetTransitionLabels(state, neigh);
                    foreach (var label in labels)
                    {
                        strB.Append($"{state.Id}:{label.Name}>{neigh.Id}\n");
                    }
                }
            }
            
            return strB.ToString();
        }
    }
}