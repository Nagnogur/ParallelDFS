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
        static int numberOfCores = Settings.PARALELLISM_DEGREE;

        List<ConcurrentStack<Vertex>> Stacks = new List<ConcurrentStack<Vertex>>(numberOfCores);

        public ConcurrentDictionary<Vertex, byte> Visited { get; set; } = new ConcurrentDictionary<Vertex, byte>();
        public ConcurrentDictionary<Vertex, Vertex> Parents { get; set; } = new ConcurrentDictionary<Vertex, Vertex>();

        public void Start(Vertex start, Vertex end = null)
        {
            Stacks = SplitVertexList(start.Edges, numberOfCores);
            Visited.TryAdd(start, 0);
            foreach (var edge in start.Edges)
            {
                Parents.TryAdd(edge, start);
            }
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
            int timeout = 0;
            while (!Stacks[stackId].IsEmpty || timeout < Settings.TIMEOUT)
            {
                timeout++;
                if (stopThreads)
                {
                    return;
                }

                // if thread stack is empty try to get work 
                if (Stacks[stackId].IsEmpty)
                {
                    // TODO
                    SplitStack(stackId);
                    return;
                }

                Vertex current;
                // There is elements in stack. Get top one
                if (Stacks[stackId].TryPop(out current))
                {
                    timeout = 0;
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
                            Stacks[stackId].Push(neighbour);
                            Parents.TryAdd(neighbour, current);
                        }
                    }
                }
            }
        }

        void SplitStack(int stackId)
        {
            int target = (stackId + 1) % numberOfCores;
            for (int j = 0; j < Settings.NUMRETRY; j++)
            {
                target = (target + 1) % numberOfCores;
                if (target == stackId)
                {
                    continue;
                }

                int count = Stacks[target].Count;
                if (count >= Settings.CUTOFFDEPTH)
                {
                    lock (Stacks[target])
                    {
                        Vertex[] vertices = new Vertex[count / 2 + 1];
                        Stacks[target].TryPopRange(vertices, 0, count / 2);
                        Stacks[stackId].PushRange(vertices);
                    }
                    return;
                }
            }

        }
    }
}
