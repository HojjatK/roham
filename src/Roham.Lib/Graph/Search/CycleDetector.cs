using System.Collections.Generic;
using System.Linq;

namespace Roham.Lib.Graph.Search
{
    public class CycleDetector<TVertex, TEdge>
    {
        private readonly IList<CyclePath<TVertex, TEdge>> _foundCyles = new List<CyclePath<TVertex, TEdge>>();

        public IEnumerable<GraphPath<TVertex, TEdge>> FindAllCycles(IGraph<TVertex, TEdge> graph, TVertex startVertex)
        {
            if (graph.EdgesCount <= (graph.VerticesCount - 1))
            {
                // Cycle is not possible
                return Enumerable.Empty<GraphPath<TVertex, TEdge>>();
            }
            _foundCyles.Clear();
            var startVertexNode = new Vertex<TVertex, TEdge>(graph) { Value = startVertex };
            var alreadyVisited = new Dictionary<Vertex<TVertex, TEdge>, Edge<TVertex, TEdge>>();
            alreadyVisited.Add(startVertexNode, null);
            return FindAllCycles(alreadyVisited, startVertexNode)
                .Cast<GraphPath<TVertex, TEdge>>();
        }

        private IEnumerable<CyclePath<TVertex, TEdge>> FindAllCycles(
            IDictionary<Vertex<TVertex, TEdge>, Edge<TVertex, TEdge>> alreadyVisited,
            Vertex<TVertex, TEdge> vertex)
        {
            foreach (var edge in vertex.Edges)
            {
                alreadyVisited[vertex] = edge;
                if (alreadyVisited.ContainsKey(edge.Target))
                {
                    var cycle = BuildCycle(alreadyVisited, edge.Target);
                    if (cycle == null)
                    {
                        // Cycle already found
                        continue;
                    }
                    yield return cycle;
                }
                else
                {
                    var newVisited = new Dictionary<Vertex<TVertex, TEdge>, Edge<TVertex, TEdge>>(alreadyVisited);
                    newVisited.Add(edge.Target, null);
                    foreach (var cycle in FindAllCycles(newVisited, edge.Target))
                    {
                        yield return cycle;
                    }
                }
            }
        }

        private CyclePath<TVertex, TEdge> BuildCycle(
            IDictionary<Vertex<TVertex, TEdge>, Edge<TVertex, TEdge>> alreadyVisited,
            Vertex<TVertex, TEdge> vertex)
        {
            var path = new List<Edge<TVertex, TEdge>>();
            bool startAdding = false;
            foreach (var visited in alreadyVisited)
            {
                if (visited.Key.Equals(vertex))
                {
                    startAdding = true;
                }
                if (startAdding)
                {
                    path.Add(visited.Value);
                }
            }

            var newCycle = new CyclePath<TVertex, TEdge>(path);
            foreach (var foundCycle in _foundCyles)
            {
                if (foundCycle.Equals(newCycle))
                {
                    return null;
                }
            }
            _foundCyles.Add(newCycle);
            return newCycle;
        }

        #region Nestes Classes

        private class CyclePath<TV, TE> : GraphPath<TV, TE>
        {
            public CyclePath(IList<Edge<TV, TE>> path)
                : base(path)
            {
            }

            public override int GetHashCode()
            {
                return (_vertices != null ? _vertices.OrderBy(o => o.Value).GetHashCode() : 0);
            }

            protected override bool AreEquals(GraphPath<TV, TE> other)
            {
                return _vertices.OrderBy(a => a.Value).
                    SequenceEqual(other.Vertices.OrderBy(o => o.Value));
            }
        }

        #endregion
    }
}
