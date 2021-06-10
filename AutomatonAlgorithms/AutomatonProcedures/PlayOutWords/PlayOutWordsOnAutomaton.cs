using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutomatonAlgorithms.AutomatonTransformations.EpsilonTransitionRemoval;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using Microsoft.Toolkit.HighPerformance;
using NLog;


namespace AutomatonAlgorithms.AutomatonProcedures.PlayOutWords
{
    public class PlayOutWordsOnAutomaton : IAutomatonProcedure
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IConfiguration Configuration { get; }

        public PlayOutWordsOnAutomaton(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void Process(List<Automaton> automata, List<string> strings)
        {
            if (automata.Count != 1 || strings.Count != 1)
                throw new ProcedureException("TryAutomaton: bad amount of inputs," +
                                             " this procedure automaton and text variable containing words to try");
            
            PlayOut(automata[0], strings[0]);
        }
        private void PlayOut (Automaton automaton, string words)
        {

            var sb = new StringBuilder();
            foreach (var word in words.Tokenize('\n'))
            {
                if (word == "")
                    continue;

                var result = false;
                if (!word.Contains('.'))
                {
                    result = PlayOutMagic(word.ToString().Select(c => c.ToString()).ToArray(), automaton);
                }
                else
                {
                    var wordSplit = word.ToString().Split(".");
                    result = PlayOutMagic(wordSplit, automaton);
                }

                sb.Append($"Word ' {word.ToString()} ' -> {result.ToString()}\n");
            }
            
            var path = Configuration.OutputFolderPath + Path.DirectorySeparatorChar + $"{automaton.Name}PlayOut.txt";
            File.WriteAllText(path, sb.ToString());
            Logger.Info($"Result of word play out saved at: {path}");
        }

        private bool PlayOutMagic(string[] word, Automaton automaton)
        {
            var searchStack = new Stack<(INode state, int charIndex)>();
            searchStack.Push((automaton.InitialState, 0));

            // epsilon transitions NEED to be removed, i decided to reuse the full epsilon remover because basically the same algorithm
            // would be needed during the play out
            // epsilon transitions have to be removed (or all possible paths have to be pre-calculated -> this is what the Epsilon remover does under the hood
            // and that's also why a decided to reuse it here), to remove the possibility of infinite loop during search
            if (automaton.GetAutomatonType(Configuration.EpsilonTransitionLabel) == AutomatonType.EpsilonNfa)
            {
                var epsilonRemover = new BasicEpsilonRemover(Configuration);
                automaton = epsilonRemover.RemoveEpsilonTransitions(automaton);
            }
            

            while (searchStack.Count != 0)
            {
                var (currentState, charIndex) = searchStack.Pop();

                if (charIndex == word.Length && automaton.AcceptingStates.Contains(currentState))
                    return true;
                
                var transitions = automaton
                    .StatesAndTransitions
                    .GetTransitionsFromNode(currentState);

                foreach (var transition in transitions)
                {

                    if (transition.Labels.Any(l => l.Equals(Configuration.EpsilonTransitionLabel)))
                    {
                        searchStack.Push((transition.To, charIndex));
                    }
                    
                    if (transition.Labels.Any(l => l.Name == word[charIndex]))
                    {
                        searchStack.Push((transition.To, charIndex + 1));
                    }
                    
                }
            }

            return false;
        }
    }
}