using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Roham.Lib.Graph.Search
{
    public class DepthFirstTraverserTests 
    {
        [TestFixture]        
        [Category("UnitTests.Graph.DirectedGraphSearch")]
        internal class GivenADirectedGraph : UnitTestFixture
        {
            [Test]
            public void DFS_TreeGraph()
            {
                var graph = new Graph<string, string>();
                graph.AddEdges(new List<VertexLink<string, string>>
                {
                    new VertexLink<string,string>("A", "B"),
                    new VertexLink<string,string>("A", "C"),
                    new VertexLink<string,string>("A", "D"),
                    new VertexLink<string,string>("B", "E"),
                    new VertexLink<string,string>("B", "F"),
                    new VertexLink<string,string>("C", "G"),
                    new VertexLink<string,string>("C", "H"),
                    new VertexLink<string,string>("D", "I"),
                    new VertexLink<string,string>("F", "J"),
                    new VertexLink<string,string>("G", "K"),
                    new VertexLink<string,string>("K", "L"),
                });
                
                var dfsTraverser = new DepthFirstTraverser<string, string>();
                var traversedVertices = dfsTraverser.Traverse(graph, "A").ToList();
                
                Assert.AreEqual(12, traversedVertices.Count, "Traversed vertices does not match");
                Assert.AreEqual("A", traversedVertices[0], "A does not match");
                Assert.AreEqual("B", traversedVertices[1], "B does not match");
                Assert.AreEqual("E", traversedVertices[2], "E does not match");
                Assert.AreEqual("F", traversedVertices[3], "F does not match");
                Assert.AreEqual("J", traversedVertices[4], "J does not match");
                Assert.AreEqual("C", traversedVertices[5], "C does not match");
                Assert.AreEqual("G", traversedVertices[6], "G does not match");
                Assert.AreEqual("K", traversedVertices[7], "K does not match");
                Assert.AreEqual("L", traversedVertices[8], "L does not match");
                Assert.AreEqual("H", traversedVertices[9], "H does not match");
                Assert.AreEqual("D", traversedVertices[10], "D does not match");
                Assert.AreEqual("I", traversedVertices[11], "I does not match");
            }

            [Test]
            public void DFS_Graph_WithBackEdge()
            {
                var graph = new Graph<string, string>();
                graph.AddEdges(new List<VertexLink<string, string>>
                {
                    new VertexLink<string,string>("A", "B"),
                    new VertexLink<string,string>("A", "C"),
                    new VertexLink<string,string>("B", "D"),
                    new VertexLink<string,string>("D", "A"),
                });
                
                var dfsTraverser = new DepthFirstTraverser<string, string>();
                var traversedVertices = dfsTraverser.Traverse(graph, "A").ToList();
                
                Assert.AreEqual(4, traversedVertices.Count, "Traversed vertices does not match");
                Assert.AreEqual("A", traversedVertices[0], "A does not match");
                Assert.AreEqual("B", traversedVertices[1], "B does not match");
                Assert.AreEqual("D", traversedVertices[2], "D does not match");
                Assert.AreEqual("C", traversedVertices[3], "C does not match");
            }

            [Test]
            public void DFS_Graph_WithCrossEdges()
            {
                var graph = new Graph<string, string>();
                graph.AddEdges(new List<VertexLink<string, string>>
                {
                    new VertexLink<string,string>("A", "B"),
                    new VertexLink<string,string>("A", "C"),
                    new VertexLink<string,string>("A", "D"),
                    new VertexLink<string,string>("B", "E"),
                    new VertexLink<string,string>("B", "C"),
                    new VertexLink<string,string>("D", "C"),
                    new VertexLink<string,string>("C", "F"),
                    new VertexLink<string,string>("E", "F"),
            }   );
                
                var dfsTraverser = new DepthFirstTraverser<string, string>();
                var traversedVertices = dfsTraverser.Traverse(graph, "A").ToList();
                
                Assert.AreEqual(6, traversedVertices.Count, "Traversed vertices does not match");
                Assert.AreEqual("A", traversedVertices[0], "A does not match");
                Assert.AreEqual("B", traversedVertices[1], "B does not match");
                Assert.AreEqual("E", traversedVertices[2], "E does not match");
                Assert.AreEqual("F", traversedVertices[3], "F does not match");
                Assert.AreEqual("C", traversedVertices[4], "C does not match");
                Assert.AreEqual("D", traversedVertices[5], "D does not match");
            }

            [Test]
            public void DFS_Graph_WithSelfEdges()
            {
                var graph = new Graph<string, string>();
                graph.AddEdges(new List<VertexLink<string, string>>
                {
                    new VertexLink<string,string>("A", "B"),
                    new VertexLink<string,string>("A", "C"),
                    new VertexLink<string,string>("B", "D"),
                    new VertexLink<string,string>("C", "D"),
                    new VertexLink<string,string>("A", "A"),
                    new VertexLink<string,string>("B", "B"),
                    new VertexLink<string,string>("C", "C"),
                    new VertexLink<string,string>("D", "D"),
                });
                
                var dfsTraverser = new DepthFirstTraverser<string, string>();
                var traversedVertices = dfsTraverser.Traverse(graph, "A").ToList();
                
                Assert.AreEqual(4, traversedVertices.Count, "Traversed vertices does not match");
                Assert.AreEqual("A", traversedVertices[0], "A does not match");
                Assert.AreEqual("B", traversedVertices[1], "B does not match");
                Assert.AreEqual("D", traversedVertices[2], "D does not match");
                Assert.AreEqual("C", traversedVertices[3], "C does not match");
            }

            [Test]
            public void DFS_Graph_WithBackCrossSelfEdges()
            {
                var graph = new Graph<string, string>();
                graph.AddEdges(new List<VertexLink<string, string>>
                {
                    new VertexLink<string,string>("A", "B"),
                    new VertexLink<string,string>("A", "C"),
                    new VertexLink<string,string>("A", "E"),
                    new VertexLink<string,string>("C", "B"),
                    new VertexLink<string,string>("B", "D"),
                    new VertexLink<string,string>("C", "D"),
                    new VertexLink<string,string>("D", "A"),
                    new VertexLink<string,string>("D", "E"),
                    new VertexLink<string,string>("E", "E")
                });
                
                var dfsTraverser = new DepthFirstTraverser<string, string>();
                var traversedVertices = dfsTraverser.Traverse(graph, "A").ToList();
                
                Assert.AreEqual(5, traversedVertices.Count, "Traversed vertices does not match");
                Assert.AreEqual("A", traversedVertices[0], "A does not match");
                Assert.AreEqual("B", traversedVertices[1], "B does not match");
                Assert.AreEqual("D", traversedVertices[2], "D does not match");
                Assert.AreEqual("E", traversedVertices[3], "E does not match");
                Assert.AreEqual("C", traversedVertices[4], "C does not match");
            }
        }

        [TestFixture]
        [Category("UnitTests.Graph.UndirectedGraphSearch")]
        internal class GivenAnUndirectedGraph : UnitTestFixture
        {
            [Test]
            public void DFS_UndirectedTreeGraph()
            {
                var graph = new Graph<string, string>(false);
                graph.AddEdges(new List<VertexLink<string, string>>
                {
                    new VertexLink<string,string>("A", "B"),
                    new VertexLink<string,string>("A", "C"),
                    new VertexLink<string,string>("B", "D"),
                    new VertexLink<string,string>("B", "E"),
                    new VertexLink<string,string>("C", "F"),
                    new VertexLink<string,string>("C", "G"),
                });
                
                var dfsTraverser = new DepthFirstTraverser<string, string>();
                var traversedVertices = dfsTraverser.Traverse(graph, "A").ToList();
                
                Assert.AreEqual(7, traversedVertices.Count, "Traversed vertices does not match");
                Assert.AreEqual("A", traversedVertices[0], "A does not match");
                Assert.AreEqual("B", traversedVertices[1], "B does not match");
                Assert.AreEqual("D", traversedVertices[2], "D does not match");
                Assert.AreEqual("E", traversedVertices[3], "E does not match");
                Assert.AreEqual("C", traversedVertices[4], "C does not match");
                Assert.AreEqual("F", traversedVertices[5], "F does not match");
                Assert.AreEqual("G", traversedVertices[6], "G does not match");
            }
        }
    }
}