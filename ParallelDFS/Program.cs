﻿using System;
using System.Collections.Generic;
using ParallelDFS.Graph1;
using ParallelDFS.Sequential;
using ParallelDFS.ParallelSearch;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ParallelDFS
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph graph;
            try
            {
                 graph = GetGraph(Settings.BIDIRECTIONAL);
            }
            catch (InvalidDataException)
            {
                Console.WriteLine("Graph file is invalid");
                return;
            }
            // starting vertex
            Vertex end;
            if (Settings.WITH_END_VERTEX)
            {
                end = graph.vertices[Settings.END_VERTEX_NUM];
            }
            else
            {
                end = null;
            }
            PreparationRuns(graph, graph.vertices[Settings.START_VERTEX_NUM], end, 5);
            RunBoth(graph, graph.vertices[Settings.START_VERTEX_NUM], end, Settings.ITERATIONS_NUM);
        }

        /// <summary>
        /// Отримує граф відповідно до налаштувань
        /// </summary>
        static Graph GetGraph(bool bidirectional)
        {
            Graph graph;
            if (Settings.GENERATE_GRAPH)
            {
                graph = new Graph().GenerateGraph(Settings.VERTEX_NUM, bidirectional);
            }
            else
            {
                graph = new Graph().FromFile(Settings.GRAPH_FILE_PATH);
            }

            if (Settings.GRAPH_WRITE_TO_FILE)
            {
                graph.ToFile(Settings.GRAPH_WRITE_PATH);
            }
            return graph;
        }

        /// <summary>
        /// Запускає паралельний та послідовний алгоритми певну кількість ітерацій на одному графі.
        /// </summary>
        static void RunBoth(Graph graph, Vertex start, Vertex end, int iterations)
        {
            double totalParallelTime = 0;
            double totalSequentialTime = 0;
            double avgSpeedup = 0;

            for (int i = 0; i < iterations; i++)
            {
                // Ініціалізація класів
                ParallelDfs parallel = new ParallelDfs();
                SequentialDfs sequential = new SequentialDfs();

                // Запуск паралельного пошуку в глибину
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                parallel.DepthFirstSearch(start, end);

                watch.Stop();
                double parallelTime = watch.Elapsed.TotalMilliseconds * 1000;
                Console.WriteLine($"Parallel Execution Time №{i}: " +
                    $"{Math.Round(parallelTime, 2)} µs");
                totalParallelTime += watch.Elapsed.TotalMilliseconds;

                // Запуск послідовного пошуку в глибину
                watch.Restart();

                Vertex[] sequentialParents = sequential.DepthFirstSearch(Settings.VERTEX_NUM, start, end);
                
                watch.Stop();
                double sequentialTime = watch.Elapsed.TotalMilliseconds * 1000;
                Console.WriteLine($"Sequential Execution Time №{i}: " +
                    $"{Math.Round(sequentialTime, 2)} µs");
                totalSequentialTime += watch.Elapsed.TotalMilliseconds;

                double speedup = sequentialTime / parallelTime;
                avgSpeedup += speedup;

                Console.WriteLine($"Speedup: " +
                    $"{Math.Round(speedup, 2)} µs");
                // Перевірка при обході графа чи співпадають відвідані вершини
                if (!Settings.WITH_END_VERTEX)
                {
                    var parallelVisited = parallel.Parents.Keys;
                    HashSet<Vertex> sequentialVisited = sequential.Visited;

                    if (!IsVisitedSame(parallelVisited, sequentialVisited))
                    {
                        Console.WriteLine("Visited vertices are not the same");
                        return;
                    }
                }

                // Підготовка до перевірки шляхів
                var parallelParentsKeyPair = parallel.Parents.ToArray();
                Vertex[] parallelParents = new Vertex[Settings.VERTEX_NUM];
                foreach (var par in parallelParentsKeyPair)
                {
                    parallelParents[par.Key.Id] = par.Value;
                }

                // Перевірка чи можливі шляхи з визначених алгоритмами батьківських вершин
                if (!IsPossiblePath(graph, parallelParents))
                {
                    Console.WriteLine("Path is not possible par");
                }
                if (!IsPossiblePath(graph, sequentialParents))
                {
                    Console.WriteLine("Path is not possible seq");
                }

                // Перевірки на знаходження шляху до кінцевої вершини
                if (Settings.WITH_END_VERTEX)
                {
                    var sequentialPath = GetPath(sequentialParents, start, end);
                    if (!CheckPath(sequentialPath, start))
                    {
                        Console.WriteLine("Sequential dfs did not find path to end vertex " +
                            "or path is invalid");
                    }

                    var parallelPath = GetPath(parallelParents, start, end);
                    if (!CheckPath(parallelPath, start))
                    {
                        Console.WriteLine("Parallel dfs did not find path to end vertex " +
                            "or path is invalid");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            double avgParallelTime = totalParallelTime * 1000 / Settings.ITERATIONS_NUM;
            double avgSequentialTime = totalSequentialTime * 1000 / Settings.ITERATIONS_NUM;
            Console.WriteLine($"Avarage Parallel Execution Time: {Math.Round(avgParallelTime, 2)} µs");
            Console.WriteLine($"Avarage Sequential Execution Time: {Math.Round(avgSequentialTime, 2)} µs");
            Console.WriteLine($"Speedup: {Math.Round(avgSpeedup / Settings.ITERATIONS_NUM, 2)}");
        }

        static void PreparationRuns(Graph graph, Vertex start, Vertex end, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                // Ініціалізація класів
                ParallelDfs parallel = new ParallelDfs();
                SequentialDfs sequential = new SequentialDfs();

                // Запуск паралельного пошуку в глибину
                parallel.DepthFirstSearch(start, end);

                // Запуск послідовного пошуку в глибину
                sequential.DepthFirstSearch(Settings.VERTEX_NUM, start, end);
            }
        }

        /// <summary>
        /// Перевірка знаходження всіх елементів першої колекції в другій 
        /// при однаковій кількості елементів
        /// </summary>
        static bool IsVisitedSame(ICollection<Vertex> first, ICollection<Vertex> second)
        {
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

        // Отримує шлях до кінцевої вершини з початкової при даному наборі батьківських вершин
        static List<Vertex> GetPath(Vertex[] parents, Vertex start, Vertex end)
        {
            int counter = 0;
            Vertex v = parents[end.Id];
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

                if (counter > parents.Length)
                {
                    return new List<Vertex>();
                }

                v = parents[v.Id];
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
