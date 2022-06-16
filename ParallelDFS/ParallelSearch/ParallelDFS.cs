using ParallelDFS.Graph1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace ParallelDFS.ParallelSearch
{
    class DFSParallel
    {
        Vertex[] visited;
        bool found = false;
        async void DFSUtil(Vertex v, Vertex parent, Vertex end = null)
        {
            // Mark the current
            // node as visited and print it
            if (v.Equals(end))
            {
                visited[v.Id] = parent;
                found = true;
                return;
            }

            if (visited[v.Id] != null || found)
            {
                return;
            }

            visited[v.Id] = parent;

            // Recur for all the
            // vertices adjacent to this
            // vertex
            Parallel.ForEach(v.Edges, i =>
           {
               if (visited[i.Id] == null && !found)
                   DFSUtil(i, v, end);
           });
        }

        public Vertex[] DFSPar(int n, Vertex start, Vertex end = null)
        {
            // Mark all the vertices as not visited(set as
            // false by default in java)
            visited = new Vertex[n];

            // Call the recursive helper
            // function to print DFS
            // traversal starting from
            // all vertices one by one
            DFSUtil(start, start, end);
            return visited;
        }
        
    }
}
