using NUnit.Framework;
using ParallelDFS;
using System.Collections.Generic;
using ParallelDFS.Sequential;
using ParallelDFS.Graph1;

namespace ParallelDfsTests
{
    public class SequentialTests
    {
        [Test]
        public void IsPathPossible()
        {
            int n = 10000;
            Graph graph = new Graph().GenerateGraph(n);
            Vertex start = graph.Vertices[0];
            SequentialDfs sequential = new SequentialDfs();

            Vertex[] Parents = sequential.DepthFirstSearch(n, start);

            Assert.IsTrue(HelperMethods.IsPossiblePath(graph, Parents));
        }

        [Test]
        public void UnreachableVertexIsNotVisited()
        {
            int n = 10000;
            Graph graph = new Graph().GenerateGraph(n);
            Vertex unreachable = new Vertex(n);
            graph.Vertices.Add(unreachable);

            Vertex start = graph.Vertices[0];
            SequentialDfs sequential = new SequentialDfs();

            Vertex[] Parents = sequential.DepthFirstSearch(n + 1, start);

            Assert.IsFalse(sequential.Visited.Contains(unreachable));
            Assert.IsNull(Parents[n]);
        }

        [Test]
        public void ThereIsPathToEndVertex()
        {
            int n = 10000;
            Graph graph = new Graph().GenerateGraph(n);
            Vertex end = new Vertex(n);
            graph.Vertices.Add(end);
            Vertex start = graph.Vertices[0];
            start.Edges.Add(end);

            SequentialDfs sequential = new SequentialDfs();

            Vertex[] Parents = sequential.DepthFirstSearch(n + 1, start, end);

            var path = HelperMethods.GetPath(Parents, start, end);
            Assert.True(HelperMethods.CheckPath(path, start));
            Assert.IsNotNull(Parents[n]);
        }
    }

}