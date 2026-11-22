using System;

namespace OpamLab09;

/// <summary>Параметри варіанта та генерація чотирьох типів входу.</summary>
public static class Data
{
    public const int N = 3;          // номер у списку
    public const int K = 6;          // кількість випадкових обмінів у «майже відсортованому»
    public const int MaxValue = 120; // числа з [0; 120]

    public static readonly Func<int, int, int> IntCmp = (x, y) => x.CompareTo(y);

    public static int[] Generate(int n)
    {
        var rnd = new Random(N);
        var a = new int[n];
        for (int i = 0; i < n; i++) a[i] = rnd.Next(0, MaxValue + 1);
        return a;
    }

    // Підготовка входів — власним швидким сортуванням (Array.Sort дозволений лише як еталон і для перевірки).
    public static int[] Sorted(int[] src)
    {
        var a = (int[])src.Clone();
        Sorts.QuickSort(a, IntCmp, out _, out _);
        return a;
    }

    public static int[] ReverseSorted(int[] src)
    {
        int[] a = Sorted(src);
        for (int i = 0, j = a.Length - 1; i < j; i++, j--)   // ручний розворот, без Array.Reverse
        {
            (a[i], a[j]) = (a[j], a[i]);
        }
        return a;
    }

    public static int[] NearlySorted(int[] src)
    {
        int[] a = Sorted(src);
        var rnd = new Random(N);
        for (int t = 0; t < K && a.Length > 1; t++)
        {
            int i = rnd.Next(a.Length), j = rnd.Next(a.Length);
            (a[i], a[j]) = (a[j], a[i]);
        }
        return a;
    }

    public static readonly string[] KindNames = { "випадковий", "відсортований", "навпаки", "майже" };

    /// <summary>Чотири версії входу з основного масиву: випадковий, відсортований, навпаки, майже відсортований.</summary>
    public static int[][] FourInputs(int n)
    {
        int[] g = Generate(n);
        return new[] { g, Sorted(g), ReverseSorted(g), NearlySorted(g) };
    }
}

/// <summary>Запис зі Лабораторної 6. Клас: у IsPermutationOf порівнюється за посиланням.</summary>
public sealed class Student
{
    public string Name { get; }
    public string Group { get; }
    public int Score { get; }

    public Student(string name, string group, int score)
    {
        Name = name;
        Group = group;
        Score = score;
    }

    public override string ToString() => $"{Name} ({Group}, {Score})";
}
