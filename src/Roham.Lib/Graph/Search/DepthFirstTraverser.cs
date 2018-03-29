using System.Collections.Generic;
using System.Linq;

namespace Roham.Lib.Graph.Search
{
    public class DepthFirstTraverser<TVertex, TEdge> : IGraphTraverser<TVertex, TEdge>
    {
        public IEnumerable<TVertex> Traverse(IGraph<TVertex, TEdge> graph, TVertex startVertex)
        {
            var visited = new HashSet<TVertex>();
            var dfsStack = new Stack<TVertex>();

            dfsStack.Push(startVertex);
            while (dfsStack.Count != 0)
            {
                var current = dfsStack.Pop();
                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    yield return current;
                }

                var neighbours = graph.GetEdges(current)
                    .Select(edge => edge.Target.Value)
                    .Where(n => !visited.Contains(n));

                foreach (var neighbour in neighbours.Reverse())
                {
                    dfsStack.Push(neighbour);
                }
            }
        }
    }
}
