using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.DataStructures.Automatons
{
    public class Automaton
    {
        public Automaton(INode initialState, HashSet<INode> acceptingStates, IGraph<INode, ILabel> statesAndTransitions,
            IEnumerable<ILabel> alphabet, string name)
        {
            InitialState = initialState;
            AcceptingStates = acceptingStates;
            StatesAndTransitions = statesAndTransitions;
            Name = name;

            Alphabet = new List<ILabel>(alphabet.OrderBy(a => a));
            
        }

        public string Name { get; set; }
        public INode InitialState { get; }

        public HashSet<INode> AcceptingStates { get; }

        public IGraph<INode, ILabel> StatesAndTransitions { get; }

        public List<ILabel> Alphabet { get; }

        private bool CountAndCheckTransitions(INode node)
        {
            var result = 0;
            var lettersUsedSoFar = new HashSet<ILabel>();
            foreach (var neigh in StatesAndTransitions.GetNeighbours(node))
            foreach (var label in StatesAndTransitions.GetTransitionLabels(node, neigh))
            {
                // if we found another transition under the same label, automaton is non deterministic
                if (lettersUsedSoFar.Contains(label))
                    return false;

                lettersUsedSoFar.Add(label);
                result += 1;
            }

            // if we find more (which is covered by the previous return but i will leave it as it is anyway)
            // or less transitions than the letters in alphabet, automaton is non deterministic
            return result == Alphabet.Count;
        }

        public AutomatonType GetAutomatonType(ILabel epsilonTransition)
        {
            if (Alphabet.Contains(epsilonTransition))
                return AutomatonType.EpsilonNfa;

            foreach (var node in StatesAndTransitions.Nodes)
                if (!CountAndCheckTransitions(node))
                    return AutomatonType.Nfa;

            return AutomatonType.Dfa;
        }

        public override string ToString()
        {
            var strB = new StringBuilder(StatesAndTransitions.Nodes.Count * 20);

            strB.Append("#states\n");
            foreach (var state in StatesAndTransitions.Nodes) strB.Append($"{state.Id}\n");

            strB.Append("#initial\n");
            strB.Append($"{InitialState.Id}\n");

            strB.Append("#accepting\n");
            foreach (var state in AcceptingStates) strB.Append($"{state.Id}\n");

            strB.Append("#alphabet\n");
            foreach (var letter in Alphabet) strB.Append($"{letter.Name}\n");

            strB.Append("#transitions\n");
            foreach (var state in StatesAndTransitions.Nodes)
            foreach (var neigh in StatesAndTransitions.GetNeighbours(state))
            {
                var labels = StatesAndTransitions.GetTransitionLabels(state, neigh);
                foreach (var label in labels) strB.Append($"{state.Id}:{label.Name}>{neigh.Id}\n");
            }

            return strB.ToString();
        }
    }
}