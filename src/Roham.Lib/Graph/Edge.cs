namespace Roham.Lib.Graph
{
    public class Edge<TVertex, TEdge>
    {
        public Edge(Vertex<TVertex, TEdge> source, Vertex<TVertex, TEdge> target, TEdge edgeValue)
        {
            Source = source;
            Target = target;
            Value = edgeValue;
        }
        public Vertex<TVertex, TEdge> Source { get; protected set; }
        public Vertex<TVertex, TEdge> Target { get; protected set; }
        public TEdge Value { get; set; }
    }

    public class DirectedEdge<TVertex, TEdge> : Edge<TVertex, TEdge>
    {
        public DirectedEdge(Vertex<TVertex, TEdge> source, Vertex<TVertex, TEdge> target, TEdge edgeValue) :
            base(source, target, edgeValue)
        {
        }

        public Vertex<TVertex, TEdge> OutboundVertex => Source;
    }
}
