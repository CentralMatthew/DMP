using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace laba111
{
    class Program
    {
        static void Main(string[] args)
        {
            // Запускається функція пошуку максимального остового дерева
            KryskMax();
            // Запускається функція пошуку мінімального остового дерева
            KryskMin();
        }

        //Функція пошуку максимального остового дерева
        static void KryskMax()
        {
            int a = 0, b = 0, u = 0, v = 0, n, i, j, ne = 1;
            // створюємо масив з 10 елементів
            int[] visited = new int[10];
            //опис масиву для збереження елементів заданого масиву
            int[,] cost = new int[10, 10];
            int max, maxcost = 0;

            int[] path = new int[100];
            int path_index = 0;

            Console.Write("Enter number of vertex ");
            // задаємо кількість вершин графа
            n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter the adjacency matrix");
            // зчитуємо з файлу матрицю суміжності
            using (StreamReader fstream = new StreamReader("D:\\note.txt"))
            {
                for (i = 1; i <= n; i++)
                    for (j = 1; j <= n; j++)
                    {
                        cost[i, j] = Convert.ToInt32(fstream.ReadLine());
                        if (j == n)
                        {
                            Console.Write("  ");
                            Console.WriteLine(cost[i, j]);
                        }
                        else
                        {
                            Console.Write("  ");
                            Console.Write(cost[i, j]);
                        }
                    }
                visited[1] = 1;
                Console.WriteLine();
            }

            while (ne < n)
            {  //Виконуємо цикл:
                for (i = 1, max = 0; i <= n; i++)
                    for (j = 1; j <= n; j++)
                        if (cost[i, j] > max)
                            if (visited[i] != 0)
                            {   // присвоюємо максимальне значення
                                max = cost[i, j];
                                a = u = i;
                                b = v = j;
                            }

                if (visited[u] == 0 || visited[v] == 0)
                {
                    path[path_index] = b;
                    path_index++;
                    Console.WriteLine("Iteration = " + ne++ + " Vertex1 = " + a + " Vertex2 = " + b + " Cost = " + max);
                    maxcost += max;
                    visited[b] = 1;

                }
                cost[a, b] = cost[b, a] = 0;
            }

            Console.Write("\n1 --> ");
            for (i = 0; i < n - 1; i++)
            {
                Console.Write(path[i]);
                if (i < n - 2)
                    Console.Write(" --> ");
            }

            Console.WriteLine("\nMaximal cost = " + maxcost);

            Console.ReadKey();
        }

        static void KryskMin()
        {
            int a = 0, b = 0, u = 0, v = 0, n, i, j, ne = 1;
            int[] visited = new int[10];
            int[,] cost = new int[10, 10];
            int min, mincost = 0;

            int[] path = new int[100];
            int path_index = 0;

            Console.Write("Enter number of vertex ");
            n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter the adjacency matrix");
            using (StreamReader fstream = new StreamReader("D:\\note.txt"))
            {
                for (i = 1; i <= n; i++)
                    for (j = 1; j <= n; j++)
                    {
                        cost[i, j] = Convert.ToInt32(fstream.ReadLine());
                        if (j == 5)
                        {
                            Console.Write("  ");
                            Console.WriteLine(cost[i, j]);
                        }
                        else
                        {
                            Console.Write("  ");
                            Console.Write(cost[i, j]);
                        }
                        if (cost[i, j] == 0)
                            cost[i, j] = 999;

                    }
                visited[1] = 1;
                Console.WriteLine();
            }


            while (ne < n)
            {
                for (i = 1, min = 999; i <= n; i++)
                    for (j = 1; j <= n; j++)
                        if (cost[i, j] < min)
                            if (visited[i] != 0)
                            {
                                min = cost[i, j];
                                a = u = i;
                                b = v = j;
                            }
                if (visited[u] == 0 || visited[v] == 0)
                {
                    path[path_index] = b;
                    path_index++;
                    Console.WriteLine("Iteration = " + ne++ + " Vertex1 = " + a + " Vertex2 = " + b + " Cost = " + min);
                    mincost += min;
                    visited[b] = 1;

                }
                cost[a, b] = cost[b, a] = 999;
            }

            Console.Write("\n1 --> ");
            for (i = 0; i < n - 1; i++)
            {
                Console.Write(path[i]);
                if (i < n - 2)
                    Console.Write(" --> ");
            }

            Console.WriteLine("\nMinimal cost = " + mincost);

            Console.ReadKey();
        }
    }
}

