namespace ParallelDFS
{
    public static class Settings
    {
        // Налаштування графа
        public const int VERTEX_NUM = 500000;
        public const bool GENERATE_GRAPH = false;
        public const string GRAPH_FILE_PATH = "graph_500000.txt";
        public const bool GRAPH_WRITE_TO_FILE = false;
        public const string GRAPH_WRITE_PATH = "graph_500000.txt";

        // Початкова вершина
        public const int START_VERTEX_NUM = 0;

        // Кінцева вершина
        public const int END_VERTEX_NUM = VERTEX_NUM - 1;
        public const bool WITH_END_VERTEX = false;

        // Кількість використаних потоків
        public const int PARALELLISM_DEGREE = 4;
        
        // Параметри розбивання стеку при розподілу роботи
        public const int SPLIT_ATTEMPTS = 5;
        public const int CUTOFFDEPTH = 100;

        // Кількість запусків програми
        public const int ITERATIONS_NUM = 20;

        // Кількість ітерацій, яку потік чекає при нестачі роботи
        public const int TIMEOUT = 5;
    }
}
