using ParallelDFS.Graph1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;

namespace ParallelDFS.ParallelSearch
{
    public class Try
    {
        volatile bool stopThreads = false;
        static int numberOfCores = 4;

        List<ConcurrentStack<Vertex>> stacks = new List<ConcurrentStack<Vertex>>(numberOfCores);

        public ConcurrentDictionary<Vertex, byte> Visited { get; set; } = new ConcurrentDictionary<Vertex, byte>();
        public ConcurrentDictionary<Vertex, Vertex> Parents { get; set; } = new ConcurrentDictionary<Vertex, Vertex>();

        public void Start(Vertex start, Vertex end = null)
        {
            stacks = SplitVertexList(start.Edges, numberOfCores);
            Visited.TryAdd(start, 0);
            if (start.Equals(end))
            {
                // TODO
                // start == end
                return;
            }
            for (int i = 0; i < numberOfCores; i++)
            {
                int j = i;
                Thread thread = new Thread(() => Search(j, start, end));
                thread.Start();
            }
        }
        public List<ConcurrentStack<Vertex>> SplitVertexList(List<Vertex> vertices, int n)
        {
            var stacks = new List<ConcurrentStack<Vertex>>();
            var size = vertices.Count / n;
            for (int i = 0; i < n - 1; i++)
            {
                ConcurrentStack<Vertex> st = new ConcurrentStack<Vertex>();
                st.PushRange(vertices.Skip(i * size).Take(size).ToArray());
                stacks.Add(st);
            }

            ConcurrentStack<Vertex> st1 = new ConcurrentStack<Vertex>();
            st1.PushRange(vertices.Skip((n - 1) * size).ToArray());
            stacks.Add(st1);
            return stacks;
        }

        public void Search(int stackId, Vertex previous, Vertex end = null)
        {
            while (!stacks[stackId].IsEmpty)
            {
                if (stopThreads)
                {
                    return;
                }

                // if thread stack is empty try to get work 
                if (stacks[stackId].IsEmpty)
                {
                    // TODO
                    return;
                }

                Vertex current;
                // There is elements in stack. Get top one
                if (stacks[stackId].TryPop(out current))
                {
                    // current == end
                    if (current.Equals(end))
                    {
                        stopThreads = true;
                        return;
                    }

                    if (!Visited.TryAdd(current, 0))
                    {
                        // TODO if visited
                        continue;
                    }

                    // maybe need synch, todo
                    //parents.TryAdd(current, previous);

                    var neighbours = current.Edges;
                    neighbours.Reverse();

                    // copy elements and go through them
                    foreach (var neighbour in neighbours.ToList())
                    {
                        if (!Visited.ContainsKey(neighbour))
                        {
                            stacks[stackId].Push(neighbour);
                            Parents.TryAdd(neighbour, current);
                        }
                    }
                }
            }
        }
    }
}
