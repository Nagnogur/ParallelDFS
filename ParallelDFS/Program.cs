using System;
using System.Collections.Generic;
using ParallelDFS.Graph1;
using ParallelDFS.Sequential;
using ParallelDFS.ParallelSearch;

namespace ParallelDFS
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph().GenerateGraph(Settings.VERTEX_NUM);
            /*graph.ToString();
            Console.WriteLine();*/

            Vertex start = graph.vertices[0];
            int n = Settings.VERTEX_NUM;

            Try search = new Try();
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            search.Start(start);
            watch.Stop();
            Console.WriteLine($"Parallel Execution Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            Vertex[] res1 = DFS.DepthFirstTraversal(n, start);
            watch.Stop();
            Console.WriteLine($"Sequential Execution Time: {watch.ElapsedMilliseconds} ms");

            var v = search.Visited;
            var p = search.Parents.ToArray();

            //Console.ReadLine();
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

        static bool CheckVisited(Vertex[] path, Vertex[] pathParallel)
        {
            if (path.Length != pathParallel.Length)
            {
                return false;
            }
            for (int i = 0; i < path.Length; i++)
            {
                if ((path[i] == null && pathParallel[i] != null) || (path[i] != null && pathParallel[i] == null))
                {
                    return false;
                }
            }
            return true;
        }

        static List<Vertex> GetPath(Vertex[] vertices, Vertex start, Vertex end)
        {
            int counter = 0;
            Vertex v = vertices[end.Id];
            List<Vertex> path = new List<Vertex>();
            path.Add(end);

            //Console.Write(end.Id + " ");

            while (v != null)
            {
                path.Add(v);
                if (v.Equals(start))
                {
                    //Console.Write(v.Id + " ");
                    
                    return path;
                }

                if (counter > vertices.Length)
                {
                    return new List<Vertex>();
                }

                //Console.Write(v.Id + " ");
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
