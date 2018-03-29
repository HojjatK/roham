using System.Collections.Generic;
using System.Linq;
using Roham.Lib.Graph.Search;

namespace Roham.Lib.Graph
{
    public interface IGraph<TVertex, TEdge>
    {
        bool IsDirected { get; }

        int VerticesCount { get; }
        int EdgesCount { get; }

        IEnumerable<TVertex> Vertices { get; }
        IEnumerable<Edge<TVertex, TEdge>> GetEdges(TVertex vertex);

        Edge<TVertex, TEdge> AddEdge(TVertex vertex1, TVertex vertex2, TEdge edgeValue = default(TEdge));
        IList<Edge<TVertex, TEdge>> AddEdges(IEnumerable<VertexLink<TVertex, TEdge>> vertexLinks);

        bool DeleteVertex(TVertex vertex);
        bool DeleteEdge(Edge<TVertex, TEdge> edge);

        void Clear();
    }

    public class Graph<TVertex, TEdge> : IGraph<TVertex, TEdge>
    {
        private readonly EdgeValidator _edgeValidator;
        private readonly IList<Vertex<TVertex, TEdge>> _vertices = new List<Vertex<TVertex, TEdge>>();
        private readonly IList<Edge<TVertex, TEdge>> _edges = new List<Edge<TVertex, TEdge>>();

        public Graph(bool isDirected = true)
        {
            IsDirected = isDirected;
            _edgeValidator = isDirected ? (EdgeValidator)new DirectedEdgeValidator() : new UnDirectedEdgeValidator();
        }

        public bool IsDirected { get; }

        public int VerticesCount => _vertices.Count;

        public int EdgesCount => _edges.Count;

        public IEnumerable<TVertex> Vertices => _vertices.Select(v => v.Value);

        public IEnumerable<Edge<TVertex, TEdge>> GetEdges(TVertex vertex)
        {
            var vertexNode = FindVertexNode(vertex);
            if (vertexNode == null)
            {
                yield break;
            }
            if (IsDirected)
            {
                //Get the edges where vertex is referenced
                foreach (Edge<TVertex, TEdge> edge in _edges)
                {
                    var directedEdge = edge as DirectedEdge<TVertex, TEdge>;
                    if (directedEdge.OutboundVertex.Equals(vertexNode))
                    {
                        yield return edge;
                    }
                }
            }
            else
            {
                //Get the edges where vertex is referenced
                foreach (Edge<TVertex, TEdge> edge in _edges)
                {
                    if (edge.Source.Equals(vertexNode) || edge.Target.Equals(vertexNode))
                    {
                        yield return edge;
                    }
                }
            }
        }

        public Edge<TVertex, TEdge> AddEdge(TVertex vertex1, TVertex vertex2, TEdge edgeValue = default(TEdge))
        {
            var vertexNode1 = FindVertexNode(vertex1);
            var vertexNode2 = vertex1.Equals(vertex2) ? vertexNode1 : FindVertexNode(vertex2);
            if (vertexNode1 == null)
            {
                _vertices.Add(vertexNode1 = new Vertex<TVertex, TEdge>(this) { Value = vertex1 });
            }
            if (vertexNode2 == null)
            {
                vertexNode2 = new Vertex<TVertex, TEdge>(this) { Value = vertex2 };
                if (vertexNode2.Equals(vertexNode1))
                {
                    vertexNode2 = vertexNode1;
                }
                else
                {
                    _vertices.Add(vertexNode2);
                }
            }
            _edgeValidator.OnCreate(vertexNode1, vertexNode2);
            var newEdge = IsDirected ?
                new DirectedEdge<TVertex, TEdge>(vertexNode1, vertexNode2, edgeValue) :
                new Edge<TVertex, TEdge>(vertexNode1, vertexNode2, edgeValue);
            _edges.Add(newEdge);
            return newEdge;
        }

        public IList<Edge<TVertex, TEdge>> AddEdges(IEnumerable<VertexLink<TVertex, TEdge>> vertexLinks)
        {
            var addedEdges = new List<Edge<TVertex, TEdge>>();
            foreach (var pair in vertexLinks)
            {
                addedEdges.Add(AddEdge(pair.Source, pair.Target, pair.EdgeValue));
            }
            return addedEdges;
        }

        public bool DeleteVertex(TVertex vertex)
        {
            var vertexNode = new Vertex<TVertex, TEdge>(this) { Value = vertex };
            var foundVertexNode = _vertices.SingleOrDefault(v => v.Equals(vertex));
            if (foundVertexNode == null)
            {
                return false;
            }

            _edges.RemoveAny(_edges.Where(e => e.Source.Equals(vertexNode) || e.Target.Equals(vertexNode)));
            return _vertices.Remove(foundVertexNode);
        }

        public bool DeleteEdge(Edge<TVertex, TEdge> edge)
        {
            var foundEdge = _edges.SingleOrDefault(e => e.Equals(edge));
            if (foundEdge != null)
            {
                _edges.Remove(foundEdge);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _vertices.Clear();
            _edges.Clear();
        }

        private Vertex<TVertex, TEdge> FindVertexNode(TVertex vertex)
        {
            return _vertices
                .SingleOrDefault(v => v.Equals(new Vertex<TVertex, TEdge>(this) { Value = vertex }));
        }

        #region Nested Classes

        private abstract class EdgeValidator
        {
            public abstract void OnCreate(Vertex<TVertex, TEdge> source, Vertex<TVertex, TEdge> target);
        }

        private class DirectedEdgeValidator : EdgeValidator
        {
            public override void OnCreate(Vertex<TVertex, TEdge> source, Vertex<TVertex, TEdge> target)
            {
                // check if an edge exists between these two vertices.
                foreach (var soureEdge in source.Edges)
                {
                    if (soureEdge.Target.Equals(target))
                    {
                        // The edge already exists, throw an exception.
                        throw new GraphDuplicateEdgeException<TVertex>(soureEdge.Source.Value, soureEdge.Target.Value,
                            "A directed graph can only have one directed edge going from source vertex to target vertex.");
                    }
                }
            }
        }

        private class UnDirectedEdgeValidator : EdgeValidator
        {
            public override void OnCreate(Vertex<TVertex, TEdge> source, Vertex<TVertex, TEdge> target)
            {
                // check there is only one edge between source and target
                foreach (var soureEdge in source.Edges)
                {
                    if (soureEdge.Target.Equals(target))
                        throw new GraphDuplicateEdgeException<TVertex>(soureEdge.Source.Value, soureEdge.Target.Value,
                            "An undirected graph can have at most one edge between a pair of vertices.");
                }
                foreach (var targetEdge in target.Edges)
                {
                    if (targetEdge.Source.Equals(target))
                        throw new GraphDuplicateEdgeException<TVertex>(targetEdge.Target.Value, targetEdge.Source.Value,
                            "An undirected graph can have at most one edge between a pair of vertices.");
                }
            }
        }

        #endregion
    }

    public static class GraphExtensions
    {
        public static IEnumerable<TVertex> DepthFirstTraverse<TVertex, TEdge>(this IGraph<TVertex, TEdge> graph, TVertex startVertex)
        {
            return new DepthFirstTraverser<TVertex, TEdge>().Traverse(graph, startVertex);
        }

        public static IEnumerable<TVertex> BreadthFirstTraverse<TVertex, TEdge>(this IGraph<TVertex, TEdge> graph, TVertex startVertex)
        {
            return new BreadthFirstTraverser<TVertex, TEdge>().Traverse(graph, startVertex);
        }

        public static IEnumerable<GraphPath<TVertex, TEdge>> GetCycles<TVertex, TEdge>(this IGraph<TVertex, TEdge> graph)
        {
            var startVertex = graph.Vertices.FirstOrDefault();
            if (startVertex == null)
            {
                return Enumerable.Empty<GraphPath<TVertex, TEdge>>();
            }
            return new CycleDetector<TVertex, TEdge>().FindAllCycles(graph, startVertex);
        }
    }
}
