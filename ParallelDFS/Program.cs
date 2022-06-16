using System;
using System.Collections.Generic;
using ParallelDFS.Graph1;
using ParallelDFS.Sequential;
using ParallelDFS.ParallelSearch;
using System.Linq;

namespace ParallelDFS
{
    class Program
    {
        static void Main(string[] args)
        {
            // Graph generation
            Graph graph = new Graph().GenerateGraph(Settings.VERTEX_NUM);
            /*graph.ToString();
            Console.WriteLine();*/

            // starting vertex
            Vertex start = graph.vertices[0];
            Vertex end = null; // graph.vertices[Settings.VERTEX_NUM - 1];
            int n = Settings.VERTEX_NUM;

            // initialize classes
            Try search = new Try();
            DFS sequential = new DFS();

            // parallel run
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            search.Start(start);
            watch.Stop();
            Console.WriteLine($"Parallel Execution Time: {watch.ElapsedMilliseconds} ms");

            // sequential run
            watch.Restart();
            Vertex[] sequentialParents = sequential.DepthFirstTraversal(n, start);
            watch.Stop();
            Console.WriteLine($"Sequential Execution Time: {watch.ElapsedMilliseconds} ms");

            // parallel results
            var parallelVisitedVertexes = search.Visited.Keys;
            var parallelParentsKeyPair = search.Parents.ToArray();

            List<int> parallelVisitedIds = parallelVisitedVertexes.Select((v) => v.Id).OrderBy((i) => i).ToList();
            Vertex[] parallelParents = new Vertex[n];
            foreach (var par in parallelParentsKeyPair)
            {
                parallelParents[par.Key.Id] = par.Value;
            }

            // sequential results
            var sequentialVisitedVertexes = sequential.visited;

            List<int> sequentialVisitedIds = sequentialVisitedVertexes.Select((v) => v.Id).OrderBy((i) => i).ToList();
            List<int> sequentialParentsIds = sequentialParents.Select((v) => v == null ? -1 : v.Id).ToList();

            // check if visited match (no goal vertex)
            for (int i = 0; i < parallelVisitedIds.Count; i++)
            {
                if (i >= sequentialParentsIds.Count || parallelVisitedIds[i] != sequentialVisitedIds[i])
                {
                    Console.WriteLine("Visited vertices do not match!!!");
                }
            }

            // Check if parents possible (there are paths from each parent)
            if (!IsPossiblePath(graph, parallelParents))
            {
                Console.WriteLine("Path is not possible");
            }
            if (!IsPossiblePath(graph, sequentialParents))
            {
                Console.WriteLine("Path is not possible");
            }

            if (end != null)
            {
                // Check if sequential path is right (with end vertex)
                var sequentialPath = GetPath(sequentialParents, start, end);
                if (!CheckPath(sequentialPath, start))
                {
                    Console.WriteLine("Sequential path is wrong");
                }

                var parallelPath = GetPath(parallelParents, start, end);
                if (!CheckPath(parallelPath, start))
                {
                    Console.WriteLine("Parallel path is wrong");
                }
            }




            //Console.ReadLine();
        }

        static bool IsPossiblePath(Graph graph, Vertex[] parents)
        {
            for (int i = 0; i < parents.Length; i++)
            {
                if (parents[i] == null)
                {
                    continue;
                }

                if (!graph.vertices[parents[i].Id].Edges.Contains(graph.vertices[i]))
                {
                    Console.WriteLine(parents[i].Id + " does not have path to " + i);
                    return false;
                }
            }
            return true;
        }

        static bool CheckPath(List<Vertex> path, Vertex start)
        {
            path.Reverse();
            Vertex v = start;
            for (int i = 1; i < path.Count; i++)
            {
                if (!v.Edges.Contains(path[i]))
                {
                    return false;
                }
                v = path[i];
            }
            return true;
        }

        // Get path from parents
        static List<Vertex> GetPath(Vertex[] vertices, Vertex start, Vertex end)
        {
            int counter = 0;
            Vertex v = vertices[end.Id];
            List<Vertex> path = new List<Vertex>();
            path.Add(end);

            while (v != null)
            {
                path.Add(v);
                if (v.Equals(start))
                {                    
                    return path;
                }

                if (counter > vertices.Length)
                {
                    return new List<Vertex>();
                }

                v = vertices[v.Id];
                counter++;
            }
            return new List<Vertex>();
        }

        static void PrintList(List<Vertex> list, string separator) 
        { 
            for (int i = 0; i < list.Count - 1; i++)
            {
                Console.Write(list[i].Id + separator);
            }
            Console.WriteLine(list[list.Count - 1].Id);
        }
    }
}
