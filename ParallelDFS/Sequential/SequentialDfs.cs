using ParallelDFS.Graph1;
using System.Collections.Generic;

namespace ParallelDFS.Sequential
{
    class SequentialDfs
    {
        public HashSet<Vertex> Visited = new HashSet<Vertex>();
        public Vertex[] DepthFirstSearch(int vertexNum, Vertex start, Vertex end = null)
        {
            Stack<Vertex> stack = new Stack<Vertex>();
            Vertex[] parents = new Vertex[vertexNum];

            stack.Push(start);

            while (stack.Count != 0)
            {
                Vertex current = stack.Pop();

                if (current.Equals(end))
                {
                    return parents;
                }

                if (!Visited.Add(current))
                    continue;

                List<Vertex> neighbours = current.Edges;

                foreach (Vertex neighbour in neighbours.ToArray())
                {
                    if (!Visited.Contains(neighbour))
                    {
                        stack.Push(neighbour);
                        parents[neighbour.Id] = current;
                    }
                }
            }

            return parents;
        }
    }
}
