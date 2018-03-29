using System.Collections.Generic;

namespace Roham.Lib.Graph.Search
{
    public interface IGraphTraverser<TVertex, TEdge>
    {
        IEnumerable<TVertex> Traverse(IGraph<TVertex, TEdge> graph, TVertex startVertex);
    }
}
