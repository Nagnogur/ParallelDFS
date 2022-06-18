using ParallelDFS.Graph1;
using System.Collections.Generic;

namespace ParallelDFS.Sequential
{
    class SequentialDfs
    {
        // Список відвіданих вершин
        public HashSet<Vertex> Visited = new HashSet<Vertex>();

        /// <summary>
        /// Виконує послідовний пошук в ширину
        /// </summary>
        /// <param name="vertexNum"> Число вершин в графі </param>
        /// <param name="start"> Початкова вершина </param>
        /// <param name="end"> Вершина, до якої треба знайти шлях. 
        /// Якщо не передана, алгоритм пройде кожну досяжну з початкової вершину </param>
        /// <returns> Для кожної вершини повертає батьківську вершину </returns>
        public Vertex[] DepthFirstSearch(int vertexNum, Vertex start, Vertex end = null)
        {
            // Ініціалізація стеку та списку батьківських вершин
            Stack<Vertex> stack = new Stack<Vertex>();
            Vertex[] parents = new Vertex[vertexNum];

            // Поміщаємо початкову вершину в стек
            stack.Push(start);

            // Поки є непереглянуті вершини
            while (stack.Count != 0)
            {
                // Дістаємо верхню вершину зі стеку
                Vertex current = stack.Pop();

                // Перевіряємо чи не знайшли потрібну вершину. Якщо знайшли, то алгоритм закінчує роботу
                if (current.Equals(end))
                {
                    return parents;
                }
                
                // Пропускаємо крок якщо поточна вершина вже відвідана. Інакше додаємо її в список відвіданих
                if (!Visited.Add(current))
                    continue;

                List<Vertex> neighbours = current.Edges;

                // Проходимось по суміжних вершинах. 
                // Всі невідвідані додаємо в стек та записуємо поточну вершину їх батьківською
                foreach (Vertex neighbour in neighbours) //
                {
                    if (!Visited.Contains(neighbour))
                    {
                        stack.Push(neighbour);
                        parents[neighbour.Id] = current;
                    }
                }
            }

            // Закінчуємо алгоритм якщо в стеці закінчились вершини
            return parents;
        }
    }
}
