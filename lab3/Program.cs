using System;
using System.Linq;

using System.Collections;

public class GFG
{
    public static int N = 6;

    public static int[] final_path = new int[GFG.N + 1];

    public static bool[] visited = new bool[GFG.N];
    public static int final_res = int.MaxValue;

    public static void copyToFinal(int[] curr_path)
    {
        for (int i = 0; i < GFG.N; i++)
        {
            GFG.final_path[i] = curr_path[i];
        }
        GFG.final_path[GFG.N] = curr_path[0];
    }

    public static int firstMin(int[,] adj, int i)
    {
        var min = int.MaxValue;
        for (int k = 0; k < GFG.N; k++)
        {
            if (adj[i,k] < min && i != k)
            {
                min = adj[i,k];
            }
        }
        return min;
    }

    public static int secondMin(int[,] adj, int i)
    {
        var first = int.MaxValue;
        var second = int.MaxValue;
        for (int j = 0; j < GFG.N; j++)
        {
            if (i == j)
            {
                continue;
            }
            if (adj[i,j] <= first)
            {
                second = first;
                first = adj[i,j];
            }
            else
            if (adj[i,j] <= second && adj[i,j] != first)
            {
                second = adj[i,j];
            }
        }
        return second;
    }

    public static void TSPRec(int[,] adj, int curr_bound, int curr_weight, int level, int[] curr_path)
    {

        if (level == GFG.N)
        {

            if (adj[curr_path[level - 1],curr_path[0]] != 0)
            {

                var curr_res = curr_weight + adj[curr_path[level - 1],curr_path[0]];

                if (curr_res < GFG.final_res)
                {
                    GFG.copyToFinal(curr_path);
                    GFG.final_res = curr_res;
                }
            }
            return;
        }

        for (int i = 0; i < GFG.N; i++)
        {

            if (adj[curr_path[level - 1],i] != 0 && GFG.visited[i] == false)
            {
                var temp = curr_bound;
                curr_weight += adj[curr_path[level - 1],i];

                if (level == 1)
                {
                    curr_bound -= ((int)((GFG.firstMin(adj, curr_path[level - 1]) + GFG.firstMin(adj, i)) / 2));
                }
                else {
                    curr_bound -= ((int)((GFG.secondMin(adj, curr_path[level - 1]) + GFG.firstMin(adj, i)) / 2));
                }

                if (curr_bound + curr_weight < GFG.final_res)
                {
                    curr_path[level] = i;
                    GFG.visited[i] = true;

                    GFG.TSPRec(adj, curr_bound, curr_weight, level + 1, curr_path);
                }

                curr_weight -= adj[curr_path[level - 1],i];
                curr_bound = temp;
                System.Array.Fill(GFG.visited,false);
                for (int j = 0; j <= level - 1; j++)
                {
                    GFG.visited[curr_path[j]] = true;
                }
            }
        }
    }
    public static void TSP(int[,] adj)
    {
        int[] curr_path = new int[GFG.N + 1];

        var curr_bound = 0;
        System.Array.Fill(curr_path,-1);
        System.Array.Fill(GFG.visited,false);
        for (int i = 0; i < GFG.N; i++)
        {
            curr_bound += (GFG.firstMin(adj, i) + GFG.secondMin(adj, i));
        }
        curr_bound = (curr_bound == 1) ? (int)(curr_bound / 2) + 1 : (int)(curr_bound / 2);

        GFG.visited[0] = true;
        curr_path[0] = 0;

        GFG.TSPRec(adj, curr_bound, 0, 1, curr_path);
    }
    public static void Main(String[] args)
    {
        int[,] adj =
        {
        {0, 0, 88, 0, 71, 16},
        {0, 0, 26, 21, 87, 55},
        {88, 26, 0, 13, 91, 0},
        {0, 21, 13, 0, 0, 89},
        {71, 87, 91, 0, 0, 37},
        {16, 55, 0, 89, 37, 0}
        };
        GFG.TSP(adj);
        Console.Write("Minimum cost : {0}\n",GFG.final_res);
        Console.Write("Path Taken : ");
        for (int i = 0; i <= GFG.N; i++)
        {
            Console.Write("{0} ",GFG.final_path[i]);
        }
    }
}