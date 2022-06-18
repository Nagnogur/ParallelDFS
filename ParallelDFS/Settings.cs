using System;
using System.Collections.Generic;
using System.Text;

namespace ParallelDFS
{
    public static class Settings
    {
        // Graph settings
        public const int VERTEX_NUM = 10000;
        public const bool GENERATE_GRAPH = false;
        public const string GRAPH_FILE_PATH = "graph_10000.txt";
        public const bool GRAPH_WRITE_TO_FILE = false;
        public const string GRAPH_WRITE_PATH = "graph_15000.txt";

        // Start vertex
        public const int START_VERTEX_NUM = 0;

        // End vertex
        public const int END_VERTEX_NUM = VERTEX_NUM - 1;
        public const bool WITH_END_VERTEX = false;

        // Max number of threads
        public const int PARALELLISM_DEGREE = 2;
        
        // Stack split parameters
        public const int NUMRETRY = 5;
        public const int CUTOFFDEPTH = 100;

        // Number of iterations
        public const int ITERATIONS_NUM = 20;

        // Timeout for stack search
        public const int TIMEOUT = 5;


    }
}
