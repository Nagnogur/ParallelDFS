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
        // Показує чи знайдено кінцеву вершину. 
        // Так як використовується кількома потоками позначено ключовим словом Volatile
        volatile bool resultFound = false;

        // Кількість потоків для використання.
        static int threadNumber = Settings.PARALELLISM_DEGREE;
        
        // Список стеків. Для кожного потоку створюється свій стек
        // Для синхронізації при можливому розбиванню використано ConcurrentStack як альтернатива lock
        List<ConcurrentStack<Vertex>> Stacks = new List<ConcurrentStack<Vertex>>(threadNumber);

        // Список Вершин та їх батьків. Об'єднує в собі функції списків Visited і Parents та 
        // використовує потоко-безпечну версію словника.
        public ConcurrentDictionary<Vertex, Vertex> Parents { get; set; } =
            new ConcurrentDictionary<Vertex, Vertex>();

        /// <summary>
        /// Функція виклику паралельного пошуку в глибину. Обробляє початкову вершину, 
        /// створює потоки та очікує їх завершення
        /// </summary>
        /// <param name="start"> Початкова вершина пошуку </param>
        /// <param name="end"> Вершина, до якої треба знайти шлях. 
        /// Якщо не передана, алгоритм пройде кожну досяжну з початкової вершину </param>
        public void DepthFirstSearch(Vertex start, Vertex end = null)
        {
            // Перевірка чи не співпадають початкова та кінцева вершини
            if (start.Equals(end))
            {
                // TODO
                // start == end
                return;
            }

            // Створення токену для можливості дострокового завершення потоків
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            // Ініціалізація та початкове наповнення стеків.
            // Якщо обробляти початкову вершину разом з іншими, можна додати її до першого стеку, 
            // а інші ініціалізувати та залишити пустими.
            Stacks = SplitVertexList(start.Edges, threadNumber);

            // Додаємо початкову вершину як батьківську до її суміжних вершин.
            foreach (var edge in start.Edges)
            {
                Parents.TryAdd(edge, start);
            }

            // Масив потоків
            Task[] tasks = new Task[threadNumber];

            // Створення потоків.
            for (int i = 0; i < threadNumber; i++)
            {
                // Щоб не виникало помилок при передачі номеру потоку, передаємо копію.
                int j = i;
                tasks[j] = Task.Factory.StartNew(() => Dfs(j, Stacks[j], end), token);
            }

            // Залежно від потрібної задачі очікуємо на завершення всіх потоків або 
            // знаходження кінцевої вершини
            if (end != null)
            {
                WhenFound(tasks);
                // Закінчуємо роботу потоків якщо кінцеву вершину було знайдено
                tokenSource.Cancel();
            }
            else
            {
                Task.WaitAll(tasks);
            }
        }

        /// <summary>
        /// Ділить список вершин на n приблизно рівних частин та записує їх в стеки
        /// </summary>
        /// <returns> Повертає список стеків з вершинами </returns>
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

        /// <summary>
        /// Пошук в глибину, який виконує один потік
        /// </summary>
        /// <param name="stackId"> Номер виділеного стеку </param>
        /// <param name="st"> Стек потоку </param>
        /// <param name="end"> Кінцева вершина, якщо треба знайти шлях </param>
        void Dfs(int stackId, ConcurrentStack<Vertex> st, Vertex end = null)
        {
            // Лічильник до завершення потоку в разі неактивності
            int timeout = 0;

            // Поки є непереглянуті вершини та час очікування не вийшов
            while (!st.IsEmpty || timeout < Settings.TIMEOUT)
            {
                timeout++;
                
                // Якщо стек пустий, потік намагається отримати вершини для обробки 
                // зі стеків інших потоків. При невдалій спробі пропускаємо крок.
                if (st.IsEmpty)
                {
                    if (!SplitStack(stackId))
                    {
                        continue;
                    }
                }
                

                Vertex current;

                // Записуємо вершину стеку в поточну якщо успішно її отримали 
                if (st.TryPop(out current))
                {
                    // Обнулення лічильника
                    timeout = 0;

                    // Перевіряємо чи не знайшли кінцеву вершину
                    if (current.Equals(end))
                    {
                        resultFound = true;
                        return;
                    }

                    var neighbours = current.Edges;

                    // Проходимось по суміжних вершинах. 
                    // Використано їх копії заради уникнення помилок при паралельній роботі.
                    foreach (var neighbour in neighbours.ToList())
                    {
                        // Невідвіданих вершини додаємо в стек та записуємо 
                        // поточну вершину їх батьківською.
                        if (!Parents.ContainsKey(neighbour))
                        {
                            st.Push(neighbour);
                            Parents.TryAdd(neighbour, current);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Намагається отримати роботу з стеків інших потоків
        /// </summary>
        /// <param name="stackId"> Номер поточного стеку </param>
        /// <returns> Повертає True якщо вдалось отримати вершини, False в іншому випадку </returns>
        bool SplitStack(int stackId)
        {
            // Номер потоку в якого намагається взяти роботу
            int target = stackId;

            // Робить певну кількість спроб, обмежену Settings.NUMRETRY
            for (int j = 0; j < Settings.NUMRETRY; j++)
            {
                target = (target + 1) % threadNumber;
                // Пропускаємо поточний стек
                if (target == stackId)
                {
                    continue;
                }

                // Якщо цільовий стек містить достатню кількість елементів
                if (Stacks[target].Count >= Settings.CUTOFFDEPTH)
                {
                    // Намагаємось забрати половину.
                    Vertex[] vertices = new Vertex[Stacks[target].Count / 2 + 1];
                    int poped = Stacks[target].TryPopRange(vertices, 0, Stacks[target].Count / 2);
                    if (poped == 0)
                    {
                        // Не вийшло забрати вершини, повертаємо false
                        return false;
                    }
                    // Додаємо отримані вершини в поточний стек та повертаємо True.
                    Stacks[stackId].PushRange(vertices.Take(poped).Reverse().ToArray());
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Функція для перевірки знаходження цільової вершини
        /// </summary>
        /// <param name="tasks"> Список потоків </param>
        async void WhenFound(Task[] tasks)
        {
            // Кількість потоків, що завершили роботу
            int completed = 0;
            // Поки всі потоки не завершились
            while (completed < tasks.Length)
            {
                // Отримуємо потік та перевіряємо його статус.
                Task task = await Task.WhenAny(tasks);
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    // Закінчуємо очікування якщо кінцеву вершину було знайдено
                    if (resultFound)
                    {
                        return;
                    }

                    completed++;
                }
            }
        }
    }
}
