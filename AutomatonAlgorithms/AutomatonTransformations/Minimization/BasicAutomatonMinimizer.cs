using System.Collections.Generic;
using System.Linq;
using AutomatonAlgorithms.Configurations;
using AutomatonAlgorithms.DataStructures.Automatons;
using AutomatonAlgorithms.DataStructures.Comparers;
using AutomatonAlgorithms.DataStructures.Graphs;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;

namespace AutomatonAlgorithms.AutomatonTransformations.Minimization
{
    public class BasicAutomatonMinimizer : IAutomatonMinimizer
    {
        public BasicAutomatonMinimizer(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public  AutomatonType IntendedType => AutomatonType.Dfa;

        public Automaton Transform(Automaton input)
        {
            return MinimizeAutomaton(input);
        }



        public Automaton MinimizeAutomaton(Automaton inputAut)
        {
            // first, we create initial classification of the states
            // this classification contains two classes, first class containing non - accepting states and the second accepting states
            var previousClassification = CreateInitialClassification(inputAut);

            // then we try to classify it by applying classification rules of this algorithm 
            // https://en.wikipedia.org/wiki/DFA_minimization

            var newClassification = PerformRoundOfClassification(inputAut, previousClassification);

            // we repeat this classification until the classification rounds don't change anything
            while (previousClassification.GetCountOfClasses() != newClassification.GetCountOfClasses())
            {
                previousClassification = newClassification;
                newClassification = PerformRoundOfClassification(inputAut, previousClassification);
            }

            // than we create new automaton from the final classification
            // each class is a new node in the minimized automaton
            return CreateNewAutomatonFromClassification(inputAut, newClassification);
        }

        private Automaton CreateNewAutomatonFromClassification(Automaton inputAut, NodeClassification newClassification)
        {
            var newStates = new HashSet<INode>();
            var classToState = new Dictionary<int, INode>();
            // first, we create new nodes from the created classes
            foreach (var cls in newClassification.GetClasses())
            {
                var newState = new BasicNode {Id = cls.ToString()};
                newStates.Add(newState);
                classToState.Add(cls, newState);
            }

            // then we crate graph containing these nodes
            var newGraph = GraphGenerator.GenerateGraph(Configuration.GraphType, newStates);

            var currentClass = 0;
            // then we go trough each class of the found classes, this time iterating over the sets of former states
            foreach (var statesOfClass in newClassification.ClassesToStatesIterator())
            {
                // we retrieve the transition list (list of neighbours, ordered by letters) of the first item in the class
                // we can be sure that the class will not be empty (if previous automaton is DFA), or it will throw an exception
                // which will be caught as it is expected in non NFA automatons
                var transition = newClassification.GetTransition(statesOfClass.First());
                // we can now iterate over zipped ordered alphabet with transitions (because both alphabet and transitions
                // are ordered on the alphabet
                foreach (var labelAndClass in
                        inputAut.Alphabet.Zip(transition, (label, i) => new {Label = label, Class = i}))
                    // we add new transitions for each letter - transition pair
                    newGraph.AddTransition(
                        classToState[currentClass], classToState[labelAndClass.Class], labelAndClass.Label);

                currentClass += 1;
            }

            // this magic retrieves class containing initial state and then returns its Node form
            var initialState = classToState[newClassification.GetClassOfState(
                newClassification.ClassesToStatesIterator()
                    .First(sts => sts.Contains(inputAut.InitialState))
                    .First())];

            var acceptingStates = newClassification
                .GetClasses()
                .Where(cls => newClassification.GetStatesOfClass(cls).Intersect(inputAut.AcceptingStates).Any())
                .Select(cls => classToState[cls])
                .ToHashSet();

            var newAutomaton = new Automaton(initialState, acceptingStates, newGraph, inputAut.Alphabet, inputAut.Name);
            return newAutomaton;
        }

        private static NodeClassification PerformRoundOfClassification(Automaton inputAut,
            NodeClassification oldClassification)
        {
            var currentClassId = -1;
            var newClassification = new NodeClassification(inputAut.StatesAndTransitions.Nodes.Count);
            foreach (var nodesInClass in oldClassification.ClassesToStatesIterator())
            {
                var newlyFoundClasses = new Dictionary<List<int>, int>(new NodeListComparer<int>());

                foreach (var node in nodesInClass)
                {
                    var neighClasses = new List<int>();
                    foreach (var letter in inputAut.Alphabet)
                    {
                        var letterNeighbour =
                            inputAut.StatesAndTransitions.GetNeighbours(node).FirstOrDefault(n =>
                                inputAut.StatesAndTransitions.GetTransitionLabels(node, n).Contains(letter));

                        neighClasses.Add(oldClassification.GetClassOfState(letterNeighbour));
                    }

                    if (!newlyFoundClasses.ContainsKey(neighClasses))
                    {
                        currentClassId += 1;
                        newlyFoundClasses.Add(neighClasses, currentClassId);
                    }

                    newClassification.AddStateAndItsClass(newlyFoundClasses[neighClasses], node);

                    newClassification.AddTransition(node, neighClasses);
                }
            }

            return newClassification;
        }

        private static NodeClassification CreateInitialClassification(Automaton inputAut)
        {
            var initialClassification = new NodeClassification(2);

            var nonAcceptingStates =
                inputAut.StatesAndTransitions.Nodes.Except(inputAut.AcceptingStates);

            foreach (var item in nonAcceptingStates) initialClassification.AddStateAndItsClass(0, item);

            foreach (var item in inputAut.AcceptingStates) initialClassification.AddStateAndItsClass(1, item);

            return initialClassification;
        }
    }
}