using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Graph_Isomorphism {
  class Program {
    static void Main(string[] args) {
      Console.Write("Enter the number of vertices: ");
      int v1 = int.Parse(Console.ReadLine());
      Console.Write("Enter the number of ribs: ");
      int e1 = int.Parse(Console.ReadLine());
      int[, ] m1 = new int[e1, 2];
      for (int i = 0; i < e1; i++) {
        Console.Write("\nEnter the number of first a vertex for an " + (i + 1) + " rib: ");
        m1[i, 0] = int.Parse(Console.ReadLine()) - 1;
        Console.Write("\nEnter the number of second a vertex for an " + (i + 1) + " rib: ");
        m1[i, 1] = int.Parse(Console.ReadLine()) - 1;
      }
      //Ввдоми вершини і ребра для першого графа
      bool[, ] a = new bool[v1, v1],
        b = new bool[v1, v1];
      for (int i = 0; i < e1; i++) {
        a[m1[i, 0], m1[i, 1]] = true;
        a[m1[i, 1], m1[i, 0]] = true;
      }
      //Побудавали матрицю суміжності для першого графа
      Console.Write("Enter the number of vertices: ");
      int v2 = int.Parse(Console.ReadLine());
      Console.Write("Enter the number of ribs: ");
      int e2 = int.Parse(Console.ReadLine());
      int[, ] m2 = new int[e2, 2];
      for (int i = 0; i < e2; i++) {
        Console.Write("\nEnter the number of first a vertex for an " + (i + 1) + " rib: ");
        m2[i, 0] = int.Parse(Console.ReadLine()) - 1;
        Console.Write("\nEnter the number of second a vertex for an " + (i + 1) + " rib: ");
        m2[i, 1] = int.Parse(Console.ReadLine()) - 1;
      }
      for (int i = 0; i < e2; i++) {
        b[m2[i, 0], m2[i, 1]] = true;
        b[m2[i, 1], m2[i, 0]] = true;
      }
      //////Заповнюємо матрицю суміжностей
      int[] mA = new int[v1], mB = new int[v2];
      mA = Schet(a);
      mB = Schet(b);
      for (int i = 0; i < mA.Length && i < mB.Length; i++)
        if (mA[i] != mB[i]) {
          Console.WriteLine("Not Isomorphism");
          break;
        }
      int f = 0;
      while (f < v1 && f < v2) {
        int s = 0;
        for (int j = 0; j < v1 && j < v2; j++)
          if (a[f, j] != b[f, j])
            if (s < Fact(v1)) {
              bool q = false;
              for (int w = 0; w < mA.Length; w++) {
                for (int g = 0; g < mB.Length; g++) {
                  if (mA[w] == mB[g]) {
                    bool t;
                    for (int h = 0; h < b.Rank; h++) {
                      t = b[h, w];
                      b[h, w] = b[h, g];
                      b[h, g] = t;
                    }
                    for (int h = 0; h < b.Rank; h++) {
                      t = b[w, h];
                      b[w, h] = b[g, h];
                      b[g, h] = t;
                    }
                    q = true;
                  }
                  if (q) break;
                }
                if (q) break;
              }
              f = 0;
              s++;
              break;
            }
        else {
          Console.WriteLine("Not Ispmorphism");
          break;
        }
        f++;

      }
      if ((f >= v1 - 1 || f >= v2 - 1))
        Console.WriteLine("Ispmorphism");
      Console.Read();
    }
    static void Izomorf(int i, int j, bool[, ] a, bool[, ] b) {}
    static public int Fact(int x) {
      var y = 1;
      for (int i = 1; i < x; i++)
        y = y * i;
      return y;
    }
    static int[] Scan(bool[, ] x) {
      int[] mX = new int[x.Rank];
      for (int j = 0; j < (x.Rank); j++) {
        int xx = 0;
        for (int i = 0; i < (x.Rank); i++)
          if (x[i, j])
            xx++;
        mX[j] = xx;
      }
      return mX;
    }
    static int[] Schet(bool[, ] x) {
      int[] mX = new int[x.Rank];
      for (int j = 0; j < (x.Rank); j++) {
        int xx = 0;
        for (int i = 0; i < x.Rank; i++)
          if (x[i, j])
            xx++;
        mX[j] = xx;
      }
      for (int j = 1; j < mX.Length; j++) {
        int min = mX[j], temp = j, ind = j;
        for (int i = j; i < mX.Length; i++)
          if (min > mX[i]) {
            min = mX[i];
            ind = i;
          }
        temp = mX[j];
        mX[j] = min;
        mX[ind] = temp;
      }
      return mX;
    }
  }
}