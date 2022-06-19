using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace ParallelDFS.Graph1
{
    // Клас для роботи зі списком вершин. Репрезентує граф.
    public class Graph
    {
        // Всі вершини графа
        public List<Vertex> vertices { get; set; }
        public Graph()
        {
            vertices = new List<Vertex>();
        }

        /// <summary>
        /// Випадкова генерація графа на певну кількість вершин
        /// </summary>
        /// <param name="vertexNum"> Кількість вершин в графі </param>
        /// <param name="bidirectional"> Чи є граф неорієнтованим </param>
        public Graph GenerateGraph(int vertexNum, bool bidirectional = false)
        {
            Graph graph = new Graph();

            for (int i = 0; i < vertexNum; i++)
            {
                Vertex vertex = new Vertex(i);
                graph.vertices.Add(vertex);
            }

            Random rng = new Random();
            for (int i = 0; i < vertexNum; i++)
            {
                for (int j = 0; j < rng.Next(vertexNum); j++)
                {
                    int connected = rng.Next(vertexNum);
                    if (connected == i || graph.vertices[i].Edges.Contains(graph.vertices[connected]))
                    {
                        continue;
                    }
                    graph.vertices[i].Edges.Add(graph.vertices[connected]);
                    if (bidirectional)
                    {
                        graph.vertices[connected].Edges.Add(graph.vertices[i]);
                    }
                }
                Console.SetCursorPosition(0, Math.Max(Console.CursorTop - 1, 0));
                ClearCurrentConsoleLine();
                Console.WriteLine($"{i + 1}/{vertexNum} vertices");
            }

            for (int i = 0; i < vertexNum; i++)
            {
                graph.vertices[i].Edges.Sort();
            }
            Console.WriteLine("Graph generated");
            return graph;
        }

        /// <summary>
        /// Вивід графу в консоль
        /// </summary>
        public void ToString()
        {
            foreach (var v in this.vertices)
            {
                Console.Write(v.Id + " : ");
                foreach (var con in v.Edges)
                {
                    Console.Write(con.Id + ", ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Вивід графу в файл
        /// </summary>
        /// <param name="path"> Шлях до файлу </param>
        public void ToFile(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(vertices.Count);
                foreach (Vertex v in vertices)
                {
                    writer.Write(v.Id + ":");
                    foreach (Vertex to in v.Edges)
                    {
                        writer.Write(to.Id + " ");
                    }
                    writer.WriteLine();
                }
            }
            Console.WriteLine("Written to the file");
        }

        /// <summary>
        /// Читає граф з файлу
        /// </summary>
        public Graph FromFile(string path)
        {
            Graph graph = new Graph();
            using (StreamReader reader = new StreamReader(path))
            {
                int count;
                if (!int.TryParse(reader.ReadLine(), out count))
                {
                    throw new InvalidDataException("Invalid number of vertices");
                }

                if (count != Settings.VERTEX_NUM)
                {
                    throw new InvalidDataException("Number of vertices does not match");
                }

                for (int i = 0; i < count; i++)
                {
                    Vertex vertex = new Vertex(i);
                    graph.vertices.Add(vertex);
                }

                for (int i = 0; i < count; i++)
                {
                    string[] line = reader.ReadLine().Split(":");
                    int id;
                    if (!int.TryParse(line[0], out id))
                    {
                        throw new InvalidDataException("Invalid vertex id");
                    }
                    
                    string[] edges = line[1].Split(" ");
                    foreach (string ed in edges)
                    {
                        int edge;
                        if (int.TryParse(ed, out edge))
                        {
                            graph.vertices[id].Edges.Add(graph.vertices[edge]);
                        }
                    }
                    graph.vertices[id].Edges.Reverse();
                    Console.SetCursorPosition(0, Math.Max(Console.CursorTop - 1, 0));
                    ClearCurrentConsoleLine();
                    Console.WriteLine($"{i + 1}/{count} vertices");

                }
            }
            Console.WriteLine("Graph extracted from file");
            return graph;
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }

    /// <summary>
    /// Клас вершини. Має поле ідентифікації та список суміжних вершин.
    /// </summary>
    public class Vertex : IComparable<Vertex>
    {
        // Індекс вершини
        public int Id { get; set; }

        // Список суміжних вершин
        public List<Vertex> Edges { get; set; }

        public Vertex(int id)
        {
            this.Id = id;
            this.Edges = new List<Vertex>();
        }
        // Порівняння вершин для сортування
        public int CompareTo([AllowNull] Vertex other)
        {
            if (other is null)
            {
                return -1;
            }
            if (other.Id < this.Id)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        // Перевірка на рівність вершин
        public override bool Equals(object obj)
        {
            var item = obj as Vertex;

            if (item == null)
            {
                return false;
            }

            return this.Id == item.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Edges);
        }
    }
}