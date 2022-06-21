using System;
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
            Vertex start = graph.Vertices[Settings.START_VERTEX_NUM];
            Vertex end;
            if (Settings.WITH_END_VERTEX)
            {
                end = graph.Vertices[Settings.END_VERTEX_NUM];
            }
            else
            {
                end = null;
            }
            RunBothSearches(graph, start, end);

            //PreparationRuns(graph, graph.Vertices[Settings.START_VERTEX_NUM], end, 5);
            //RunBoth(graph, graph.Vertices[Settings.START_VERTEX_NUM], end, Settings.ITERATIONS_NUM);
        }

        static void RunBothSearches(Graph graph, Vertex start, Vertex end)
        {
            ParallelDfs parallel = new ParallelDfs();
            SequentialDfs sequential = new SequentialDfs();

            // Запуск паралельного пошуку в глибину
            parallel.DepthFirstSearch(start, end);
            var parallelParents = HelperMethods.DictionaryToArray(parallel.Parents, Settings.VERTEX_NUM);

            // Запуск послідовного пошуку в глибину
            Vertex[] sequentialParents = sequential.DepthFirstSearch(Settings.VERTEX_NUM, start, end);

            var parallelPath = HelperMethods.GetPath(parallelParents, start, end);
            var sequentialPath = HelperMethods.GetPath(sequentialParents, start, end);

            Console.WriteLine("Parallel path:\n");
            PrintList(parallelPath, " - ", 20);
            Console.WriteLine("\nSequential path:\n");
            PrintList(sequentialPath, " - ", 20);
        }

        /// <summary>
        /// Отримує граф відповідно до налаштувань
        /// </summary>
        static Graph GetGraph(bool bidirectional)
        {
            Graph graph;
            if (Settings.GENERATE_GRAPH)
            {
                graph = new Graph().GenerateGraph(Settings.VERTEX_NUM, bidirectional, true);
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

            double minSeq = 100000000;
            double maxSeq = 0;

            double minPar = 100000000;
            double maxPar = 0;

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

                minPar = Math.Min(minPar, parallelTime);
                maxPar = Math.Max(maxPar, parallelTime);

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

                minSeq = Math.Min(minSeq, sequentialTime);
                maxSeq = Math.Max(maxSeq, sequentialTime);

                Console.WriteLine($"Speedup: " +
                    $"{Math.Round(speedup, 2)} µs");

                Console.WriteLine();
            }

            Console.WriteLine();
            double avgParallelTime = totalParallelTime * 1000 / Settings.ITERATIONS_NUM;
            double avgSequentialTime = totalSequentialTime * 1000 / Settings.ITERATIONS_NUM;
            Console.WriteLine($"Avarage Parallel Execution Time: {Math.Round(avgParallelTime)} µs");
            Console.WriteLine($"Avarage Sequential Execution Time: {Math.Round(avgSequentialTime)} µs");
            Console.WriteLine($"Speedup: {Math.Round(avgSpeedup / Settings.ITERATIONS_NUM, 2)}");

            Console.WriteLine();
            Console.WriteLine($"Min time Sequential: {Math.Round(minSeq)}");
            Console.WriteLine($"Max time Sequential: {Math.Round(maxSeq)}");
            
            Console.WriteLine();
            Console.WriteLine($"Min time Parallel: {Math.Round(minPar)}");
            Console.WriteLine($"Max time Parallel: {Math.Round(maxPar)}");
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

        static void PrintList(List<Vertex> list, string separator, int itemLimit) 
        {
            int k = 0;
            for (int i = 0; i < list.Count - 1; i++, k++)
            {
                if (k == itemLimit)
                {
                    Console.WriteLine();
                    k = 0;
                }
                Console.Write(list[i].Id + separator);
            }
            Console.WriteLine(list[list.Count - 1].Id);
        }
    }
}
