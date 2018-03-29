using System.Linq;
using NUnit.Framework;

namespace Roham.Lib.Graph
{
    public class GraphTests
    {
        [TestFixture]
        [Category("UnitTests.Graph.UndirectedGraph")]
        internal class GivenAnUndirectedGraph : UnitTestFixture
        {   
            IGraph<TVertex, TEdge> CreateSubject<TVertex, TEdge>()
            {
                return new Graph<TVertex, TEdge>(false);
            }

            [Test]
            public void WhenAddEdgeIsCalled_VerticesAndEdgeAreAddedToGraph()
            {
                var subject = CreateSubject<int, int>();

                subject.AddEdge(5, 10, 2);

                Assert.AreEqual(2, subject.VerticesCount);
                Assert.AreEqual(1, subject.EdgesCount);
                Assert.AreEqual(2, subject.GetEdges(5).Single().Value);
                Assert.AreEqual(2, subject.GetEdges(10).Single().Value);
            }

            [Test]
            public void WhenAddEdgesIsCalled_VerticesAndEdgesAreAddedToGraph()
            {
                var subject = CreateSubject<int, int>();

                subject.AddEdges(new[] {
                    new VertexLink<int, int> { Source = 5, Target = 10, EdgeValue = 1},
                    new VertexLink<int, int> { Source = 5, Target = 20, EdgeValue = 2},
                    new VertexLink<int, int> { Source = 10, Target = 30, EdgeValue = 3},
                    new VertexLink<int, int> { Source = 20, Target = 30, EdgeValue = 4}
                });

                Assert.AreEqual(4, subject.VerticesCount);
                Assert.AreEqual(4, subject.EdgesCount);

                Assert.AreEqual(2, subject.GetEdges(5).Count());
                Assert.IsNotNull(subject.GetEdges(5).Single(e => e.Value == 1));
                Assert.IsNotNull(subject.GetEdges(5).Single(e => e.Value == 2));

                Assert.AreEqual(2, subject.GetEdges(10).Count());
                Assert.IsNotNull(subject.GetEdges(10).Single(e => e.Value == 1));
                Assert.IsNotNull(subject.GetEdges(10).Single(e => e.Value == 3));
                
                Assert.AreEqual(2, subject.GetEdges(20).Count());
                Assert.IsNotNull(subject.GetEdges(20).Single(e => e.Value == 2));
                Assert.IsNotNull(subject.GetEdges(20).Single(e => e.Value == 4));

                Assert.AreEqual(2, subject.GetEdges(30).Count());
                Assert.IsNotNull(subject.GetEdges(30).Single(e => e.Value == 3));
                Assert.IsNotNull(subject.GetEdges(30).Single(e => e.Value == 4));
            }

            [Test]
            public void WhenDuplicateEdgeIsAdded_ExceptionIsThrown()
            {   
                var subject = CreateSubject<string, int>();
                subject.AddEdge("A", "B", 10);

                Assert.Throws<GraphDuplicateEdgeException<string>>(() => subject.AddEdge("A", "B", 20));
            }

            [Test]
            public void WhenEdgesWithDuplicateAreAdded_ExceptionIsThrown()
            {
                var subject = CreateSubject<string, int>();
                var links = new[] {
                    new VertexLink<string, int> { Source = "A", Target ="B" },
                    new VertexLink<string, int> { Source = "B", Target="C" },
                    new VertexLink<string, int> { Source = "B", Target="A" },
                };

                Assert.Throws<GraphDuplicateEdgeException<string>>(() => subject.AddEdges(links));
            }
        }

        [TestFixture]
        [Category("UnitTests.Graph.DirectedGraph")]
        internal class GivenADirectedGraph : UnitTestFixture
        {
            IGraph<TVertex, TEdge> CreateSubject<TVertex, TEdge>()
            {
                return new Graph<TVertex, TEdge>(true);
            }

            [Test]
            public void WhenAddEdgeIsCalled_VerticesAndEdgeAreAddedToGraph()
            {   
                var subject = CreateSubject<int, int>();

                subject.AddEdge(5, 10, 2);

                Assert.AreEqual(2, subject.VerticesCount);
                Assert.AreEqual(1, subject.EdgesCount);
                Assert.AreEqual(2, subject.GetEdges(5).Single().Value);
                Assert.IsNull(subject.GetEdges(10).FirstOrDefault());
            }

            [Test]
            public void WhenAddEdgesIsCalled_VerticesAndEdgesAreAddedToGraph()
            {   
                var subject = CreateSubject<int, int>();

                subject.AddEdges(new[] {
                    new VertexLink<int, int> { Source = 5, Target = 10, EdgeValue = 1},
                    new VertexLink<int, int> { Source = 5, Target = 20, EdgeValue = 2},
                    new VertexLink<int, int> { Source = 10, Target = 5, EdgeValue = 3},
                    new VertexLink<int, int> { Source = 10, Target = 30, EdgeValue = 4},
                    new VertexLink<int, int> { Source = 20, Target = 30, EdgeValue = 5}
                });

                Assert.AreEqual(4, subject.VerticesCount);
                Assert.AreEqual(5, subject.EdgesCount);
                
                Assert.AreEqual(2, subject.GetEdges(5).Count());
                Assert.IsNotNull(subject.GetEdges(5).Single(e => e.Value == 1));
                Assert.IsNotNull(subject.GetEdges(5).Single(e => e.Value == 2));

                Assert.AreEqual(2, subject.GetEdges(10).Count());
                Assert.IsNotNull(subject.GetEdges(10).Single(e => e.Value == 3));
                Assert.IsNotNull(subject.GetEdges(10).Single(e => e.Value == 4));
                
                Assert.AreEqual(1, subject.GetEdges(20).Count());
                Assert.IsNotNull(subject.GetEdges(20).Single(e => e.Value == 5));

                Assert.AreEqual(0, subject.GetEdges(30).Count());
                Assert.IsNull(subject.GetEdges(30).FirstOrDefault());
            }

            [Test]
            public void WhenDuplicateEdgeIsAdded_ExceptionIsThrown()
            {
                var subject = CreateSubject<string, int>();
                subject.AddEdge("A", "B", 10);
                subject.AddEdge("B", "A", 20);

                Assert.Throws<GraphDuplicateEdgeException<string>>(() => subject.AddEdge("A", "B", 30));
            }

            [Test]
            public void WhenEdgesWithDuplicateAreAdded_ExceptionIsThrown()
            {
                var subject = CreateSubject<string, int>();                
                var links = new [] {
                    new VertexLink<string, int> { Source = "A", Target ="B" },
                    new VertexLink<string, int> { Source = "B", Target="A" },
                    new VertexLink<string, int> { Source = "B", Target="C" },
                    new VertexLink<string, int> { Source = "A", Target ="B" },
                };

                Assert.Throws<GraphDuplicateEdgeException<string>>(() => subject.AddEdges(links));
            }
        }
    }
}
