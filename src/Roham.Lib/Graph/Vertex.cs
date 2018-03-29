using System;
using System.Collections.Generic;

namespace Roham.Lib.Graph {
    public class Vertex<TVertex, TEdge> : IEquatable<Vertex<TVertex, TEdge>> {
        private readonly IGraph<TVertex, TEdge> _owner;

        public Vertex(IGraph<TVertex, TEdge> owner) {
            _owner = owner;
        }

        public TVertex Value { get; set; }

        public IEnumerable<Edge<TVertex, TEdge>> Edges => _owner.GetEdges(Value);

        public override bool Equals(object obj) {
            return Equals((Vertex<TVertex, TEdge>)obj);
        }

        public bool Equals(Vertex<TVertex, TEdge> other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(other.Value, Value);
        }

        public override int GetHashCode() {
            return Value == null ? 0 : Value.GetHashCode();
        }

        public static bool operator ==(Vertex<TVertex, TEdge> left, Vertex<TVertex, TEdge> right) {
            return Equals(left, right);
        }

        public static bool operator !=(Vertex<TVertex, TEdge> left, Vertex<TVertex, TEdge> right) {
            return !Equals(left, right);
        }

        public override string ToString() {
            if (Value != null)
                return Value.ToString();

            return base.ToString();
        }
    }

    public class VertexLink<TVertex, TEdge> {
        public VertexLink() { }
        public VertexLink(TVertex source, TVertex target, TEdge edgeData = default(TEdge)) {
            Source = source;
            Target = target;
        }

        public TVertex Source { get; set; }
        public TVertex Target { get; set; }
        public TEdge EdgeValue { get; set; }
    }
}
