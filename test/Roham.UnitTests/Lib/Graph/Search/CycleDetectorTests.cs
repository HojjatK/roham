using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Roham.Lib.Graph.Search
{
    [TestFixture]    
    [Category("UnitTests.Graph.CycleDetector")]
    public class CycleDetectorTest : UnitTestFixture
    {
        [Test]
        public void UndirectedTreeGraph_WithNoCycles()
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

            var cycleTraverser = new CycleDetector<string, string>();
            var cycles = cycleTraverser.FindAllCycles(graph, "A").ToList();
            
            Assert.AreEqual(0, cycles.Count, "Cycles count does not match");
        }

        [Test]
        public void DirectedTreeGraph_WithNoCycles()
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

            var cycleTraverser = new CycleDetector<string, string>();
            var cycles = cycleTraverser.FindAllCycles(graph, "A").ToList();
            
            Assert.AreEqual(0, cycles.Count, "Cycles count does not match");
        }

        [Test]
        public void DirectedGraph_WithBackEdge_WithCycles()
        {
            var graph = new Graph<string, string>();
            graph.AddEdges(new List<VertexLink<string, string>>
            {
                new VertexLink<string,string>("A", "B"),
                new VertexLink<string,string>("A", "C"),
                new VertexLink<string,string>("B", "D"),
                new VertexLink<string,string>("D", "A"),
            });

            var cycleTraverser = new CycleDetector<string, string>();
            var cycles = cycleTraverser.FindAllCycles(graph, "A").ToList();
            Assert.AreEqual(1, cycles.Count, "Cycles count does not match");

            var vertexA = graph.Vertices.Single(v => v.Equals("A"));
            var vertexB = graph.Vertices.Single(v => v.Equals("B"));
            var vertexD = graph.Vertices.Single(v => v.Equals("D"));
            var expectedPath = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vertexA, vertexB, vertexD }));

            Console.WriteLine("ExpectedPath: {0}", expectedPath);
            Console.WriteLine("FoundPath: {0}", cycles[0]);

            Assert.IsTrue(expectedPath.Equals(cycles[0]), "Path A->B->D not found");
        }

        [Test]
        public void DirectedGraph_WithCrossEdges_WithNoCycles()
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
            });

            var cycleTraverser = new CycleDetector<string, string>();
            var cycles = cycleTraverser.FindAllCycles(graph, "A").ToList();
            
            Assert.AreEqual(0, cycles.Count, "Cycles count does not match");
        }

        [Test]
        public void DirectedGraph_WithSelfEdges_WithCycles()
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

            var cycleTraverser = new CycleDetector<string, string>();
            var cycles = cycleTraverser.FindAllCycles(graph, "A").ToList();
            
            Assert.AreEqual(4, cycles.Count, "Cycles count does not match");

            var vertexA = graph.Vertices.Single(v => v.Equals("A"));
            var vertexB = graph.Vertices.Single(v => v.Equals("B"));
            var vertexC = graph.Vertices.Single(v => v.Equals("C"));
            var vertexD = graph.Vertices.Single(v => v.Equals("D"));
            var expectedPathA = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vertexA }));
            var expectedPathB = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vertexB }));
            var expectedPathC = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vertexC }));
            var expectedPathD = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vertexD }));

            Console.WriteLine("ExpectedPathA: {0}", expectedPathA);
            Console.WriteLine("ExpectedPathB: {0}", expectedPathB);
            Console.WriteLine("ExpectedPathC: {0}", expectedPathC);
            Console.WriteLine("ExpectedPathD: {0}", expectedPathD);
            for (int i = 0; i < cycles.Count; i++)
            {
                Console.WriteLine("FoundPath{0}: {1}", i, cycles[i]);
            }

            Assert.IsTrue(expectedPathA.Equals(cycles[0]) || expectedPathA.Equals(cycles[1]) ||
                expectedPathA.Equals(cycles[2]) || expectedPathA.Equals(cycles[3]), "Path A not found");

            Assert.IsTrue(expectedPathB.Equals(cycles[0]) || expectedPathB.Equals(cycles[1]) ||
                expectedPathB.Equals(cycles[2]) || expectedPathB.Equals(cycles[3]), "Path B not found");

            Assert.IsTrue(expectedPathC.Equals(cycles[0]) || expectedPathC.Equals(cycles[1]) ||
                expectedPathC.Equals(cycles[2]) || expectedPathC.Equals(cycles[3]), "Path C not found");

            Assert.IsTrue(expectedPathD.Equals(cycles[0]) || expectedPathD.Equals(cycles[1]) ||
                expectedPathD.Equals(cycles[2]) || expectedPathD.Equals(cycles[3]), "Path D not found");
        }

        [Test]
        public void DirectedGraph_With_BackCrossSelfEdges_WithCycles_1()
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
            var cycleTraverser = new CycleDetector<string, string>();
            var cycles = cycleTraverser.FindAllCycles(graph, "A").ToList();

            for (int i = 0; i < cycles.Count; i++)
            {
                Console.WriteLine("FoundPath{0}: {1}", i, cycles[i]);
            }
            Assert.AreEqual(4, cycles.Count, "Cycles count does not match");

            var vertexA = graph.Vertices.Single(v => v.Equals("A"));
            var vertexB = graph.Vertices.Single(v => v.Equals("B"));
            var vertexC = graph.Vertices.Single(v => v.Equals("C"));
            var vertexD = graph.Vertices.Single(v => v.Equals("D"));
            var vertexE = graph.Vertices.Single(v => v.Equals("E"));
            var expectedPath1 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vertexA, vertexB, vertexD }));
            var expectedPath2 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vertexA, vertexC, vertexD }));
            var expectedPath3 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vertexA, vertexC, vertexB, vertexD }));
            var expectedPath4 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vertexE }));

            Console.WriteLine("ExpectedPath1: {0}", expectedPath1);
            Console.WriteLine("ExpectedPath2: {0}", expectedPath2);
            Console.WriteLine("ExpectedPath3: {0}", expectedPath3);
            Console.WriteLine("ExpectedPath4: {0}", expectedPath4);

            Assert.IsTrue(expectedPath1.Equals(cycles[0]) || expectedPath1.Equals(cycles[1]) ||
                expectedPath1.Equals(cycles[2]) || expectedPath1.Equals(cycles[3]), "Path A->B->D not found");

            Assert.IsTrue(expectedPath2.Equals(cycles[0]) || expectedPath2.Equals(cycles[1]) ||
                expectedPath2.Equals(cycles[2]) || expectedPath2.Equals(cycles[3]), "Path A->C->D not found");

            Assert.IsTrue(expectedPath3.Equals(cycles[0]) || expectedPath3.Equals(cycles[1]) ||
                expectedPath3.Equals(cycles[2]) || expectedPath3.Equals(cycles[3]), "Path A->C->B->D not found");

            Assert.IsTrue(expectedPath4.Equals(cycles[0]) || expectedPath4.Equals(cycles[1]) ||
                expectedPath4.Equals(cycles[2]) || expectedPath4.Equals(cycles[3]), "Path E not found");
        }

        [Test]
        public void DirectedGraph_With_BackCrossSelfEdges_WithCycles_2()
        {
            var graph = new Graph<string, string>();
            graph.AddEdges(new List<VertexLink<string, string>>
            {
                new VertexLink<string,string>("A", "A"),
                new VertexLink<string,string>("A", "B"),
                new VertexLink<string,string>("B", "A"),
                new VertexLink<string,string>("B", "C"),
                new VertexLink<string,string>("B", "D"),
                new VertexLink<string,string>("C", "C"),
                new VertexLink<string,string>("C", "A"),
                new VertexLink<string,string>("C", "G"),
                new VertexLink<string,string>("D", "E"),
                new VertexLink<string,string>("E", "F"),
                new VertexLink<string,string>("E", "B"),
                new VertexLink<string,string>("E", "A"),
                new VertexLink<string,string>("F", "D"),
            });
            var vA = graph.Vertices.Single(v => v.Equals("A"));
            var vB = graph.Vertices.Single(v => v.Equals("B"));
            var vC = graph.Vertices.Single(v => v.Equals("C"));
            var vD = graph.Vertices.Single(v => v.Equals("D"));
            var vE = graph.Vertices.Single(v => v.Equals("E"));
            var vF = graph.Vertices.Single(v => v.Equals("F"));

            var cycleTraverser = new CycleDetector<string, string>();
            var cycles = cycleTraverser.FindAllCycles(graph, vA).ToList();

            for (int i = 0; i < cycles.Count; i++)
            {
                Console.WriteLine("FoundPath{0}: {1}", i, cycles[i]);
            }
            Assert.AreEqual(7, cycles.Count, "Cycles count does not match");

            var expectedPath1 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vA }));
            var expectedPath2 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vA, vB }));
            var expectedPath3 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vA, vB, vC }));
            var expectedPath4 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vC }));
            var expectedPath5 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vA, vB, vD, vE }));
            var expectedPath6 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vB, vD, vE }));
            var expectedPath7 = new GraphPath<string, string>(CreateCycleEdges(graph, new List<string> { vD, vE, vF }));

            Console.WriteLine("ExpectedPath1: {0}", expectedPath1);
            Console.WriteLine("ExpectedPath2: {0}", expectedPath2);
            Console.WriteLine("ExpectedPath3: {0}", expectedPath3);
            Console.WriteLine("ExpectedPath4: {0}", expectedPath4);
            Console.WriteLine("ExpectedPath5: {0}", expectedPath5);
            Console.WriteLine("ExpectedPath6: {0}", expectedPath6);
            Console.WriteLine("ExpectedPath7: {0}", expectedPath7);

            Assert.IsTrue(expectedPath1.Equals(cycles[0]) || expectedPath1.Equals(cycles[1]) ||
                expectedPath1.Equals(cycles[2]) || expectedPath1.Equals(cycles[3]) ||
                expectedPath1.Equals(cycles[4]) || expectedPath1.Equals(cycles[5]) || expectedPath1.Equals(cycles[6]),
                "Path A not found");

            Assert.IsTrue(expectedPath2.Equals(cycles[0]) || expectedPath2.Equals(cycles[1]) ||
                expectedPath2.Equals(cycles[2]) || expectedPath2.Equals(cycles[3]) ||
                expectedPath2.Equals(cycles[4]) || expectedPath2.Equals(cycles[5]) || expectedPath2.Equals(cycles[6]),
                "Path A->B not found");

            Assert.IsTrue(expectedPath3.Equals(cycles[0]) || expectedPath3.Equals(cycles[1]) ||
                expectedPath3.Equals(cycles[2]) || expectedPath3.Equals(cycles[3]) ||
                expectedPath3.Equals(cycles[4]) || expectedPath3.Equals(cycles[5]) || expectedPath3.Equals(cycles[6]),
                "Path A->B->C not found");

            Assert.IsTrue(expectedPath4.Equals(cycles[0]) || expectedPath4.Equals(cycles[1]) ||
                expectedPath4.Equals(cycles[2]) || expectedPath4.Equals(cycles[3]) ||
                expectedPath4.Equals(cycles[4]) || expectedPath4.Equals(cycles[5]) || expectedPath4.Equals(cycles[6]),
                "Path C not found");

            Assert.IsTrue(expectedPath5.Equals(cycles[0]) || expectedPath5.Equals(cycles[1]) ||
                expectedPath5.Equals(cycles[2]) || expectedPath5.Equals(cycles[3]) ||
                expectedPath5.Equals(cycles[4]) || expectedPath5.Equals(cycles[5]) || expectedPath5.Equals(cycles[6]),
                "Path A->B->D->E not found");

            Assert.IsTrue(expectedPath6.Equals(cycles[0]) || expectedPath6.Equals(cycles[1]) ||
                expectedPath6.Equals(cycles[2]) || expectedPath6.Equals(cycles[3]) ||
                expectedPath6.Equals(cycles[4]) || expectedPath6.Equals(cycles[5]) || expectedPath6.Equals(cycles[6]),
                "Path B->D->E not found");

            Assert.IsTrue(expectedPath7.Equals(cycles[0]) || expectedPath7.Equals(cycles[1]) ||
                expectedPath7.Equals(cycles[2]) || expectedPath7.Equals(cycles[3]) ||
                expectedPath7.Equals(cycles[4]) || expectedPath7.Equals(cycles[5]) || expectedPath7.Equals(cycles[6]),
                "Path E->E->F not found");
        }

        private List<Edge<string, string>> CreateCycleEdges(IGraph<string, string> graph, List<string> vertices)
        {
            var edges = new List<Edge<string, string>>();
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                edges.Add(new DirectedEdge<string, string>(
                    new Vertex<string, string>(graph) { Value = vertices[i] },
                    new Vertex<string, string>(graph) { Value = vertices[i + 1] },
                    ""));
            }
            edges.Add(new DirectedEdge<string, string>(
                new Vertex<string, string>(graph) { Value = vertices[vertices.Count - 1] },
                new Vertex<string, string>(graph) { Value = vertices[0] },
                ""));
            return edges;
        }
    }
}
