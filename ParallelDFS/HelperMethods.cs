using ParallelDFS.Graph1;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ParallelDFS
{
    public static class HelperMethods
    {
        /// <summary>
        /// Перевірка знаходження всіх елементів першої колекції в другій 
        /// при однаковій кількості елементів
        /// </summary>
        public static bool IsVisitedSame(ICollection<Vertex> first, ICollection<Vertex> second)
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

        public static bool IsPossiblePath(Graph graph, Vertex[] parents)
        {
            for (int i = 0; i < parents.Length; i++)
            {
                if (parents[i] == null)
                {
                    continue;
                }

                if (!graph.Vertices[parents[i].Id].Edges.Contains(graph.Vertices[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckPath(List<Vertex> path, Vertex start)
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
        public static List<Vertex> GetPath(Vertex[] parents, Vertex start, Vertex end)
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

        public static Vertex[] DictionaryToArray(ConcurrentDictionary<Vertex, Vertex> dict, int n)
        {
            var KeyPair = dict.ToArray();
            Vertex[] array = new Vertex[n];
            foreach (var par in KeyPair)
            {
                array[par.Key.Id] = par.Value;
            }
            return array;
        }
    }
}
