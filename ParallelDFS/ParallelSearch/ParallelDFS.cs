using ParallelDFS.Graph1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelDFS.ParallelSearch
{
    public class ParallelDfs
    {
        volatile bool stopThreads = false;
        static int numberOfCores = Settings.PARALELLISM_DEGREE;
        

        List<ConcurrentStack<Vertex>> Stacks = new List<ConcurrentStack<Vertex>>(numberOfCores);

        public ConcurrentDictionary<Vertex, Vertex> Parents { get; set; } = new ConcurrentDictionary<Vertex, Vertex>();

        public void DepthFirstSearch(Vertex start, Vertex end = null)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Stacks = SplitVertexList(start.Edges, numberOfCores);

            Task[] tasks = new Task[numberOfCores];

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
                tasks[j] = Task.Factory.StartNew(() => Dfs(j, Stacks[j], end), token);
            }

            if (end != null)
            {
                WhenFound(tasks);
                tokenSource.Cancel();
            }
            else
            {
                Task.WaitAll(tasks);
            }
        }

        List<ConcurrentStack<Vertex>> SplitVertexList(List<Vertex> vertices, int n)
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


        void Dfs(int stackId, ConcurrentStack<Vertex> st, Vertex end = null)
        {
            int timeout = 0;
            while (!st.IsEmpty || timeout < Settings.TIMEOUT)
            {
/*                if (stopThreads)
                {
                    return;
                }*/
                timeout++;
                

                // if thread stack is empty try to get work 
                if (st.IsEmpty)
                {
                    // TODO
                    if (!SplitStack(stackId))
                    {
                        return;
                    }
                }
                

                Vertex current;
                // There is elements in stack. Get top one
                
                if (st.TryPop(out current))
                {
                    timeout = 0;
                    // current == end
                    if (current.Equals(end))
                    {
                        stopThreads = true;
                        return;
                    }

                    var neighbours = current.Edges;

                    // copy elements and go through them
                    foreach (var neighbour in neighbours.ToList())
                    {
                        if (!Parents.ContainsKey(neighbour))
                        {
                            st.Push(neighbour);
                            Parents.TryAdd(neighbour, current);
                        }
                    }
                }
                
            }
            
        }

        bool SplitStack(int stackId)
        {
            int target = (stackId + 1) % numberOfCores;
            for (int j = 0; j < Settings.NUMRETRY; j++)
            {
                target = (target + 1) % numberOfCores;
                if (target == stackId)
                {
                    continue;
                }

                if (Stacks[target].Count >= Settings.CUTOFFDEPTH)
                {
                    Vertex[] vertices = new Vertex[Stacks[target].Count / 2 + 1];
                    int poped = Stacks[target].TryPopRange(vertices, 0, Stacks[target].Count / 2);
                    if (poped == 0)
                    {
                        return false;
                    }
                    Stacks[stackId].PushRange(vertices.Take(poped).Reverse().ToArray());
                    return true;
                }
            }
            return false;

        }

        async void WhenFound(Task[] tasks)
        {
            int completed = 0;
            while (tasks.Length > 0)
            {
                Task task = await Task.WhenAny(tasks);
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    completed++;
                    if (stopThreads || completed >= tasks.Length)
                    {
                        return;
                    }
                    
                }
            }
        }
    }
}
