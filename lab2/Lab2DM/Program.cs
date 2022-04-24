using System;
using System.Collections.Generic;
using System.IO;

namespace Lab2DM
{
    class Program
    {
        static int vertexCount;
        static Dictionary<Int32, Edge> edges;
        static List<Int32> oddVertices;

        static List<UnitePath>[] oddUnitePaths;
        static List<UnitePath[]> filteredOddUnitePaths;
        static UnitePath[] bestOfUnitePaths;

        static void Main()
        {
            CollectInitialInformation();
            FindAllOdd();
            if (oddVertices.Count % 2 == 1)
                Console.WriteLine("No solution.");
            else
            {
                GenerateAvailableOddUnitePaths();
                FilterOddUnitePaths();
                FindBestOfFilteredUnitePaths();
                WriteSolution();
            }

            Console.ReadLine();
        }

        static void CollectInitialInformation()
        {
            Console.Write("Vertex count: ");
            vertexCount = int.Parse(Console.ReadLine());
            Console.WriteLine("Edges: ");

                  edges = new Dictionary<Int32, Edge>();

            for (int i = 0; i < vertexCount; i++)
            {
                var input = Console.ReadLine().Split();
                for (int j = i + 1; j < vertexCount; j++)
                {
                    var edgeWeight = int.Parse(input[j]);
                    if (edgeWeight == 0) continue;
                    edges.Add(Pack(i, j), new Edge() { Weight = edgeWeight });
                }
            }
        }

        static void WriteSolution()
        {
            int totalWeight = 0;
            foreach (var path in bestOfUnitePaths)
            {
                totalWeight += path.PathWeight;
                Console.Write($"[{path.PathWeight}] ");
                foreach (var vertex in path.Vertices)
                    Console.Write($"{vertex} ");
                Console.WriteLine();
            }

            foreach (var edge in edges.Values)
                totalWeight += edge.Weight;

            Console.WriteLine($"Total weight: {totalWeight}");
        }

        static void FindAllOdd()
        {
            oddVertices = new List<Int32>();
            for (int i = 0; i < vertexCount; i++)
            {
                int neighboringVertices = 0;
                for (int j = 0; j < vertexCount; j++)
                {
                    if (i == j) continue;
                    if (edges.ContainsKey(Pack(i, j)))
                        neighboringVertices++;
                }

                if (neighboringVertices % 2 == 1)
                    oddVertices.Add(i);
            }
        }

        static void GenerateAvailableOddUnitePaths()
        {
            oddUnitePaths = new List<UnitePath>[oddVertices.Count];
            for (int i = 0; i < oddUnitePaths.Length; i++)
                oddUnitePaths[i] = new List<UnitePath>();

            for (int i = 0; i < oddVertices.Count; i++)
                RecursiveUnitePathSearch(new UnitePath() { Vertices = new List<Int32> { oddVertices[i] } });
        }

        static void RecursiveUnitePathSearch(UnitePath unitePath)
        {
            var lastVertex = unitePath.Vertices[unitePath.Vertices.Count - 1];
            if (unitePath.Vertices.Count > 1 && oddVertices.Contains(unitePath.Vertices[unitePath.Vertices.Count - 1]))
            {
                oddUnitePaths[oddVertices.IndexOf(unitePath.Vertices[0])].Add(new UnitePath(unitePath));
                return;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                if (i == unitePath.Vertices[unitePath.Vertices.Count - 1]) continue;
                if (unitePath.Vertices.Contains(i)) continue;
                int key = Pack(i, lastVertex); // change name
                if (!edges.ContainsKey(key)) continue;

                unitePath.PathWeight += edges[key].Weight;
                unitePath.Vertices.Add(i);

                RecursiveUnitePathSearch(unitePath);

                unitePath.PathWeight -= edges[key].Weight;
                unitePath.Vertices.Remove(i);
            }
        }

        static void FilterOddUnitePaths()
        {
            var allUniteCases = new List<UnitePath>();
            for (int i = 0; i < oddUnitePaths.Length; i++)
                allUniteCases.AddRange(oddUnitePaths[i]);

            filteredOddUnitePaths = new List<UnitePath[]>();
            FilterOddUnitePathRecursive(allUniteCases, new List<UnitePath>());
        }

        static void FilterOddUnitePathRecursive(List<UnitePath> allPaths, List<UnitePath> selectedPaths)
        {
            if (selectedPaths.Count == oddVertices.Count / 2)
                filteredOddUnitePaths.Add(selectedPaths.ToArray());

            for (int i = 0; i < allPaths.Count; i++)
            {
                var current = allPaths[i];
                var selected = new List<UnitePath>();
                for (int j = 0; j < allPaths.Count; j++)
                {
                    bool valid = true;
                    for (int k = 0; k < current.Vertices.Count; k++)
                        if (allPaths[j].Vertices.Contains(current.Vertices[k]))
                            valid = false;

                    if (valid) selected.Add(allPaths[j]);
                }

                selectedPaths.Add(current);
                FilterOddUnitePathRecursive(selected, selectedPaths);
                selectedPaths.Remove(current);
            }
        }

        static void FindBestOfFilteredUnitePaths()
        {
            int sum, minTotalWeight = int.MaxValue;
            foreach (var paths in filteredOddUnitePaths)
            {
                sum = 0;
                foreach (var path in paths)
                    sum += path.PathWeight;

                if (sum < minTotalWeight)
                {
                    minTotalWeight = sum;
                    bestOfUnitePaths = paths;
                }
            }
        }

        static int Pack(int a, int b) => a > b ? a << 16 | b : b << 16 | a;
        static int[] Unpack(int i) => new int[] { i >> 16, i & 0xFFFF };
    }

    class Edge
    {
        public int Weight;
    }

    class UnitePath
    {
        public List<Int32> Vertices;
        public int PathWeight;

        public UnitePath() { }

        public UnitePath(UnitePath clone)
        {
            Vertices = new List<Int32>(clone.Vertices);
            PathWeight = clone.PathWeight;
        }
    }
}

//0 1 0 2 0 0
//1 0 3 5 0 0
//0 3 0 0 6 2
//2 5 0 0 4 0
//0 0 6 4 0 1
//0 0 2 0 1 0

//0 5 9
//5 0 6
//9 6 0

//0 1 1 1 1 1 1
//1 0 1 1 1 1 1
//1 1 0 1 1 1 1
//1 1 1 0 1 1 1
//1 1 1 1 0 1 1
//1 1 1 1 1 0 1
//1 1 1 1 1 1 0

//0 10 0 11
//10 0 12 15
//0 12 0 9
//11 15 9 0

//0 42 0 0 0 0 45
//42 0 38 0 0 0 35
//0 38 0 70 0 28 0
//0 0 70 0 80 0 0
//0 0 0 80 0 62 0
//0 0 28 0 62 0 40
//45 35 0 0 0 40 0


//0 8 0 7 4
//8 0 9 0 5
//0 9 0 6 3
//7 0 6 0 2
//4 5 3 2 0

//BAD

//0 1 0 2 0
//1 0 3 5 0
//0 3 0 0 2
//2 5 4 0 0
//0 0 2 0 0

//0 1 2 0 0
//1 0 5 3 0
//2 5 0 4 0
//0 3 4 0 2
//0 0 0 2 0