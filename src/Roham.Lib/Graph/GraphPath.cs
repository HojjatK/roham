using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roham.Lib.Graph
{
    public class GraphPath<TVertex, TEdge> : IEquatable<GraphPath<TVertex, TEdge>>
    {
        protected readonly IList<Vertex<TVertex, TEdge>> _vertices = new List<Vertex<TVertex, TEdge>>();
        private readonly IEnumerable<Edge<TVertex, TEdge>> _edges;

        public GraphPath(IEnumerable<Edge<TVertex, TEdge>> edges)
        {
            _edges = edges;
            foreach (var edge in _edges)
            {
                _vertices.Add(edge.Source);
            }
        }

        public IEnumerable<Vertex<TVertex, TEdge>> Vertices => _vertices;

        public IEnumerable<Edge<TVertex, TEdge>> Path => _edges;

        public override string ToString()
        {
            var sb = new StringBuilder();
            int counter = 0;
            foreach (var v in _vertices)
            {
                sb.AppendFormat(counter++ == 0 ? "{0}" : "->{0}", v.Value);
            }
            return sb.ToString();
        }

        public bool Equals(GraphPath<TVertex, TEdge> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AreEquals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((GraphPath<TVertex, TEdge>)obj);
        }

        public override int GetHashCode()
        {
            return (_vertices != null ? _vertices.GetHashCode() : 0);
        }

        public static bool operator ==(GraphPath<TVertex, TEdge> left, GraphPath<TVertex, TEdge> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GraphPath<TVertex, TEdge> left, GraphPath<TVertex, TEdge> right)
        {
            return !Equals(left, right);
        }

        protected virtual bool AreEquals(GraphPath<TVertex, TEdge> other)
        {
            return _vertices.SequenceEqual(other._vertices);
        }
    }
}
