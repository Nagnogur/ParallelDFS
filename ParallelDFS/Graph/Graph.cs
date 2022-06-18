using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace ParallelDFS.Graph1
{
    public class Graph
    {
        public List<Vertex> vertices { get; set; }
        public Graph()
        {
            vertices = new List<Vertex>();
        }

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
            }

            for (int i = 0; i < vertexNum; i++)
            {
                graph.vertices[i].Edges.Sort();
            }

            return graph;
        }

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
        }

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

                }
            }
            return graph;
        }
    }

    public class Vertex : IComparable<Vertex>
    {
        // Name of the vertex.
        public int Id { get; set; }

        // list of accessible vertexes
        public List<Vertex> Edges { get; set; }

        public Vertex(int id)
        {
            this.Id = id;
            this.Edges = new List<Vertex>();
        }
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
        public override bool Equals(object obj)
        {
            var item = obj as Vertex;

            if (item == null)
            {
                return false;
            }

            return this.Id == item.Id;
        }
    }
}