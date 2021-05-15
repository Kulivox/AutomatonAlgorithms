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

        public Automaton Transform(Automaton input)
        {
            return MinimizeAutomaton(input);
        }


        public Automaton MinimizeAutomaton(Automaton inputAut)
        {
            var previousClassification = CreateInitialClassification(inputAut);
            var newClassification = PerformRoundOfClassification(inputAut, previousClassification);

            while (previousClassification.GetCountOfClasses() != newClassification.GetCountOfClasses())
            {
                previousClassification = newClassification;
                newClassification = PerformRoundOfClassification(inputAut, previousClassification);
            }


            return CreateNewAutomatonFromClassification(inputAut, newClassification);
        }

        private Automaton CreateNewAutomatonFromClassification(Automaton inputAut, NodeClassification newClassification)
        {
            var newStates = new HashSet<INode>();
            var classToState = new Dictionary<int, INode>();

            foreach (var cls in newClassification.GetClasses())
            {
                var newState = new BasicNode {Id = cls.ToString()};
                newStates.Add(newState);
                classToState.Add(cls, newState);
            }

            var newGraph = GraphGenerator.GenerateGraph(Configuration.GraphType, newStates);

            var currentClass = 0;
            foreach (var statesOfClass in newClassification.ClassesToStatesIterator())
            {
                var transition = newClassification.GetTransition(statesOfClass.First());

                foreach (var labelAndClass in
                    inputAut.Alphabet.Zip(transition, (label, i) => new {Label = label, Class = i}))
                    newGraph.CreateTransition(
                        classToState[currentClass], classToState[labelAndClass.Class], labelAndClass.Label);

                currentClass += 1;
            }

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