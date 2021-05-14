using System;
using System.Collections.Generic;
using AutomatonAlgorithms.DataStructures.Graphs.Matrix;
using AutomatonAlgorithms.DataStructures.Graphs.Nodes;
using AutomatonAlgorithms.DataStructures.Graphs.Transitions.Labels;

namespace AutomatonAlgorithms.DataStructures.Graphs
{
    public static class GraphGenerator 
    {
        public static IGraph<INode, ILabel> GenerateGraph(GraphTypes type)
        {
            return type switch
            {
                GraphTypes.TransitionMatrixGraph => new MatrixGraph(),
                GraphTypes.BasicGraph => throw new NotImplementedException(),
                _ => throw new ArgumentException("Unknown graph type")
            };
        }
        
        public static IGraph<INode, ILabel> GenerateGraph(GraphTypes type, IEnumerable<INode> nodes)
        {
            return type switch
            {
                GraphTypes.TransitionMatrixGraph => new MatrixGraph(nodes),
                GraphTypes.BasicGraph => throw new NotImplementedException(),
                _ => throw new ArgumentException("Unknown graph type")
            };
        }
    }
}