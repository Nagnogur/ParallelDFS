using NUnit.Framework;
using ParallelDFS;
using ParallelDFS.Graph1;
using ParallelDFS.ParallelSearch;
using ParallelDFS.Sequential;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ParallelDfsTests
{
    public class ParallelTests
    {
        [Test]
        public void IsPathPossible()
        {
            int n = 10000;
            Graph graph = new Graph().GenerateGraph(n);
            Vertex start = graph.Vertices[0];
            ParallelDfs parallel = new ParallelDfs();

            parallel.DepthFirstSearch(start);
            var Parents = HelperMethods.DictionaryToArray(parallel.Parents, n);

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
            ParallelDfs parallel = new ParallelDfs();

            parallel.DepthFirstSearch(start);
            var ParentsDictionary = parallel.Parents;

            Assert.IsFalse(ParentsDictionary.ContainsKey(unreachable));
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

            ParallelDfs parallel = new ParallelDfs();

            parallel.DepthFirstSearch(start, end);
            var Parents = HelperMethods.DictionaryToArray(parallel.Parents, n + 1);

            var path = HelperMethods.GetPath(Parents, start, end);
            Assert.True(HelperMethods.CheckPath(path, start));
            Assert.AreEqual(start, Parents[end.Id]);
        }

        [Test]
        public void VisitedSame()
        {
            int n = 10000;
            Graph graph = new Graph().GenerateGraph(n);
            Vertex start = graph.Vertices[0];

            ParallelDfs parallel = new ParallelDfs();
            SequentialDfs sequential = new SequentialDfs();

            parallel.DepthFirstSearch(start);
            var parallelVisited = parallel.Parents.Keys;

            sequential.DepthFirstSearch(n, start);
            HashSet<Vertex> sequentialVisited = sequential.Visited;

            Assert.True(HelperMethods.IsVisitedSame(parallelVisited, sequentialVisited));
        }
    }
}