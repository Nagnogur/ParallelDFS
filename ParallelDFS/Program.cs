﻿using System;
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
            Graph graph;
            if (Settings.GENERATE_GRAPH)
            {
                graph = new Graph().GenerateGraph(Settings.VERTEX_NUM);
            }
            else
            {
                graph = new Graph().FromFile(Settings.GRAPH_FILE_PATH);
            }

            if (Settings.GRAPH_WRITE_TO_FILE)
            {
                graph.ToFile(Settings.GRAPH_WRITE_PATH);
            }
            
            /*graph.ToString();
            Console.WriteLine();*/

            // starting vertex
            Vertex start = graph.vertices[Settings.START_VERTEX_NUM];
            Vertex end;
            if (Settings.WITH_END_VERTEX)
            {
                end = graph.vertices[Settings.END_VERTEX_NUM];
            }
            else
            {
                end = null;
            }

            long totalParallelTime = 0;
            long totalSequentialTime = 0;

            for (int i = 0; i < Settings.ITERATIONS_NUM; i++)
            {
                // initialize classes
                Try search = new Try();
                DFS sequential = new DFS();

                // parallel run
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                search.Start(start, end);
                watch.Stop();
                Console.WriteLine($"Parallel Execution Time №{i}: {watch.ElapsedMilliseconds} ms");
                totalParallelTime += watch.ElapsedMilliseconds;

                // sequential run
                watch.Restart();
                Vertex[] sequentialParents = sequential.DepthFirstTraversal(Settings.VERTEX_NUM, start, end);
                watch.Stop();
                Console.WriteLine($"Sequential Execution Time №{i}: {watch.ElapsedMilliseconds} ms");
                totalSequentialTime += watch.ElapsedMilliseconds;

                if (!Settings.WITH_END_VERTEX)
                {
                    var parallelVisited = search.Visited.Keys;
                    HashSet<Vertex> sequentialVisited = sequential.visited;

                    if (!IsVisitedSame(parallelVisited, sequentialVisited))
                    {
                        Console.WriteLine("Visited vertices are not the same");
                        return;
                    }
                }

                var parallelParentsKeyPair = search.Parents.ToArray();
                Vertex[] parallelParents = new Vertex[Settings.VERTEX_NUM];
                foreach (var par in parallelParentsKeyPair)
                {
                    parallelParents[par.Key.Id] = par.Value;
                }

                // Check if parents possible (there are paths from each parent)
                if (!IsPossiblePath(graph, parallelParents))
                {
                    Console.WriteLine("Path is not possible par");
                }
                if (!IsPossiblePath(graph, sequentialParents))
                {
                    Console.WriteLine("Path is not possible seq");
                }

                if (Settings.WITH_END_VERTEX)
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

                Console.WriteLine();
            }

            Console.WriteLine();
            long avgParallelTime = (totalParallelTime) / Settings.ITERATIONS_NUM;
            long avgSequentialTime = (totalSequentialTime) / Settings.ITERATIONS_NUM;
            Console.WriteLine($"Parallel Avarage Execution Time: {avgParallelTime} ms");
            Console.WriteLine($"Sequential Avarage Execution Time: {avgSequentialTime} ms");
        }

        static bool IsVisitedSame(ICollection<Vertex> first, ICollection<Vertex> second)
        {
            // check if visited match (no goal vertex)
            foreach (var visited in first)
            {
                if (first.Count != second.Count)
                {
                    return false;
                }
                if (!second.Contains(visited))
                {
                    return false;
                }
            }
            return true;
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
                    path.Reverse();
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
