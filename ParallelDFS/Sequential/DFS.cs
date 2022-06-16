using ParallelDFS.Graph1;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParallelDFS.Sequential
{
    class DFS
    {
        bool[] visited;

        void DFSUtil(Vertex v)
        {
            // Mark the current
            // node as visited and print it
            visited[v.Id] = true;
            //Console.Write(v.Id + " - ");

            // Recur for all the
            // vertices adjacent to this
            // vertex
            foreach (var i in v.Edges)
            {
                if (!visited[i.Id])
                    DFSUtil(i);
            }
        }

        public bool[] DFSSequential(Graph graph, int startId)
        {
            // Mark all the vertices as not visited(set as
            // false by default in java)
            int verticesNum = graph.vertices.Count;
            visited = new bool[verticesNum];

            // Call the recursive helper
            // function to print DFS
            // traversal starting from
            // all vertices one by one
            DFSUtil(graph.vertices[startId]);
            return visited;
        }

        public static Vertex[] DepthFirstTraversal(int n, Vertex start, Vertex end = null)
        {
            var visited = new HashSet<Vertex>();
            var stack = new Stack<Vertex>();
            Vertex[] parentMap = new Vertex[n];

            stack.Push(start);

            while (stack.Count != 0)
            {
                var current = stack.Pop();

                if (current.Equals(end))
                {
                    return parentMap;
                }

                if (!visited.Add(current))
                    continue;

                var neighbours = current.Edges;
                neighbours.Reverse();

                foreach (var neighbour in neighbours.ToArray())
                {
                    if (!visited.Contains(neighbour))
                    {
                        stack.Push(neighbour);
                        parentMap[neighbour.Id] = current;
                    }
                }
            }

            return parentMap;
        }
    }
}
