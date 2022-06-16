using ParallelDFS.Graph1;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParallelDFS.Sequential
{
    class DFS
    {
        public HashSet<Vertex> visited = new HashSet<Vertex>();
        public Vertex[] DepthFirstTraversal(int n, Vertex start, Vertex end = null)
        {
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
