using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static (int Count, long Size) ScanDirectory(string path)
    {
        int count = 0;
        long size = 0;

        try
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file).Equals(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                    size += new FileInfo(file).Length;
                }
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                var result = ScanDirectory(dir);
                count += result.Count;
                size += result.Size;
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Немає доступу: {path}");
        }
        catch (IOException)
        {
            Console.WriteLine($"Помилка читання: {path}");
        }

        return (count, size);
    }

    static int MaxRecursive(int[] a, int lo, int hi)
    {
        if (lo == hi)
            return a[lo];

        int mid = (lo + hi) / 2;
        int left = MaxRecursive(a, lo, mid);
        int right = MaxRecursive(a, mid + 1, hi);

        return left > right ? left : right;
    }

    static int MaxDepth(int lo, int hi)
    {
        if (lo == hi)
            return 1;

        int mid = (lo + hi) / 2;
        int left = MaxDepth(lo, mid);
        int right = MaxDepth(mid + 1, hi);

        return 1 + Math.Max(left, right);
    }

    static int FindPaths(int row, int col, int size, string path, List<string> paths)
    {
        if (row == size - 1 && col == size - 1)
        {
            paths.Add(path);
            return 1;
        }

        int count = 0;

        if (col + 1 < size)
            count += FindPaths(row, col + 1, size, path + "R", paths);

        if (row + 1 < size)
            count += FindPaths(row + 1, col, size, path + "D", paths);

        return count;
    }

    static int TraceMax(int[] a, int lo, int hi, int depth)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}enter Max({lo}..{hi})");

        if (lo == hi)
        {
            Console.WriteLine($"{indent}exit Max({lo}..{hi}) -> {a[lo]}");
            return a[lo];
        }

        int mid = (lo + hi) / 2;
        int left = TraceMax(a, lo, mid, depth + 1);
        int right = TraceMax(a, mid + 1, hi, depth + 1);
        int result = left > right ? left : right;

        Console.WriteLine($"{indent}exit Max({lo}..{hi}) -> {result}");
        return result;
    }

    static int MaxIterative(int[] a)
    {
        int max = a[0];

        for (int i = 1; i < a.Length; i++)
            if (a[i] > max)
                max = a[i];

        return max;
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Лабораторна робота №3");
        Console.WriteLine("N = 3, K = 6, a = 0, b = 3, c = 3");

        Console.WriteLine("\nЗавдання 1");
        Console.Write("Введіть шлях до папки: ");
        string? folder = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
        {
            var result = ScanDirectory(folder);
            Console.WriteLine($"Кількість .cs файлів: {result.Count}");
            Console.WriteLine($"Сумарний розмір: {result.Size} байт");
        }
        else
        {
            Console.WriteLine("Папка не існує.");
        }

        Console.WriteLine("\nЗавдання 2");
        Random random = new Random(3);
        int[] numbers = new int[16];

        for (int i = 0; i < numbers.Length; i++)
            numbers[i] = random.Next(-20, 21);

        Console.WriteLine("Масив: " + string.Join(", ", numbers));

        int recursiveMax = MaxRecursive(numbers, 0, numbers.Length - 1);
        Console.WriteLine($"Максимум: {recursiveMax}");
        Console.WriteLine($"Глибина рекурсії: {MaxDepth(0, numbers.Length - 1)}");
        Console.WriteLine($"log2(16) = {Math.Log2(16)}");

        Console.WriteLine("\nЗавдання 3");
        List<string> paths = new List<string>();
        int pathCount = FindPaths(0, 0, 3, "", paths);

        foreach (string path in paths)
            Console.WriteLine(path);

        Console.WriteLine($"Кількість шляхів: {pathCount}");

        Console.WriteLine("\nЗавдання 4");
        int[] small = { 7, -2, 15, 4 };
        int tracedMax = TraceMax(small, 0, small.Length - 1, 0);
        Console.WriteLine($"Максимум: {tracedMax}");

        Console.WriteLine("\nЗавдання 5");
        int iterativeMax = MaxIterative(numbers);
        Console.WriteLine($"Рекурсивно: {recursiveMax}");
        Console.WriteLine($"Циклом: {iterativeMax}");
        Console.WriteLine($"Результати збігаються: {recursiveMax == iterativeMax}");
    }
}