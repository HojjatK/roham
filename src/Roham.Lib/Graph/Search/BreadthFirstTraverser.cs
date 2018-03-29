using System.Collections.Generic;
using System.Linq;

namespace Roham.Lib.Graph.Search
{
    public class BreadthFirstTraverser<TVertex, TEdge> : IGraphTraverser<TVertex, TEdge>
    {
        public IEnumerable<TVertex> Traverse(IGraph<TVertex, TEdge> graph, TVertex startVertex)
        {
            var visited = new HashSet<TVertex>();
            var queue = new Queue<TVertex>();

            queue.Enqueue(startVertex);
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    yield return current;
                }
                var neighbours = graph
                    .GetEdges(current)
                    .Select(edge => edge.Target.Value)
                    .Where(n => !visited.Contains(n));

                foreach (var neighbour in neighbours)
                {
                    queue.Enqueue(neighbour);
                }
            }
        }
    }
}
