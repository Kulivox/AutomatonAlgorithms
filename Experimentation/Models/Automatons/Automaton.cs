using System;
using System.Collections.Generic;
using Experimentation.Models.Graphs;

namespace Experimentation.Models.Automatons
{
    public class Automaton<TState, TLetterType> where TState : IEquatable<TState>
    {
        public Automaton(TState initialState, List<TState> acceptingStates,
            IGraph<TState, TLetterType> statesAndTransitions,
            List<TLetterType> alphabet)
        {
            InitialState = initialState;
            AcceptingStates = acceptingStates;
            StatesAndTransitions = statesAndTransitions;
            Alphabet = alphabet;
        }

        public TState InitialState { get; }

        public List<TState> AcceptingStates { get; }

        public IGraph<TState, TLetterType> StatesAndTransitions { get; }

        public List<TLetterType> Alphabet { get; }
    }
}