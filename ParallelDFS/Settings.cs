namespace ParallelDFS
{
    public static class Settings
    {
        const string folder = "1000/";
        const string path = "graph_1000_1.txt";
        // Налаштування графа
        public const int VERTEX_NUM = 1000;
        public const bool BIDIRECTIONAL = false;
        public const bool GENERATE_GRAPH = false;
        public const string GRAPH_FILE_PATH = folder + path;
        public const bool GRAPH_WRITE_TO_FILE = false;
        public const string GRAPH_WRITE_PATH = folder + path;

        // Початкова вершина
        public const int START_VERTEX_NUM = 0;

        // Кінцева вершина
        public const int END_VERTEX_NUM = VERTEX_NUM - 1;
        public const bool WITH_END_VERTEX = true;

        // Кількість використаних потоків
        public const int PARALELLISM_DEGREE = 4;
        
        // Параметри розбивання стеку при розподілу роботи
        public const int SPLIT_ATTEMPTS = 11;
        public const int CUTOFFDEPTH = 500;

        // Кількість запусків програми перед 
        public const int PREP_ITERATIONS_NUM = 5;

        // Кількість запусків програми
        public const int ITERATIONS_NUM = 10;

        // Кількість ітерацій, яку потік чекає при нестачі роботи
        public const int TIMEOUT = 25;
    }
}
