using System;

namespace Roham.Lib.Graph
{
    public class GraphException : Exception
    {
        public GraphException(string message) : base(message) { }
    }

    public class GraphDuplicateEdgeException<TVertex> : GraphException
    {
        public GraphDuplicateEdgeException(TVertex source, TVertex target, string message)
            : base($"{message} -> duplicate edge from {source} to {target}") { }
    }
}