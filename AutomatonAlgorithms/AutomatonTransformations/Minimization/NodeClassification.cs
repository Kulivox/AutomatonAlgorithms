using System.Collections.Generic;
using System.Linq;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;

namespace AutomatonAlgorithms.AutomatonTransformations.Minimization
{
    public class NodeClassification
    {
        private readonly HashSet<INode>[] _classesToStates;

        private readonly Dictionary<INode, int> _statesToClasses;

        private readonly Dictionary<INode, List<int>> _transitions;

        public NodeClassification(int maxLen)
        {
            _transitions = new Dictionary<INode, List<int>>();
            _classesToStates = new HashSet<INode>[maxLen];
            _statesToClasses = new Dictionary<INode, int>();
        }

        public int GetClassOfState(INode state)
        {
            return _statesToClasses[state];
        }

        public HashSet<INode> GetStatesOfClass(int classId)
        {
            return _classesToStates[classId];
        }

        public void AddStateAndItsClass(int classId, INode state)
        {
            _classesToStates[classId] ??= new HashSet<INode>();

            _classesToStates[classId].Add(state);
            _statesToClasses.Add(state, classId);
        }

        public void AddTransition(INode state, List<int> neighbourClasses)
        {
            _transitions.Add(state, neighbourClasses);
        }

        public List<int> GetTransition(INode state)
        {
            return _transitions[state];
        }

        public int GetCountOfClasses()
        {
            return _classesToStates.TakeWhile(cls => cls != null).Count();
        }

        public IEnumerable<int> GetClasses()
        {
            var i = 0;
            while (_classesToStates[i] != null)
            {
                yield return i;
                i += 1;
            }
        }

        public IEnumerable<HashSet<INode>> ClassesToStatesIterator()
        {
            return _classesToStates.Where(cls => cls != null);
        }
    }
}