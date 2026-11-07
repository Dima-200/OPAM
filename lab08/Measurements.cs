using System;
using System.Diagnostics;

public static class Measurements
{
    private static long checksumSink;

    public static void ComparisonTable()
    {
        int[] sizes = { 1000, 10000, 100000, 1000000 };
        Random random = new Random(3);
        Func<int, int, int> cmp = (x, y) => x.CompareTo(y);

        Console.WriteLine("\nТАБЛИЦЯ 1. СЕРЕДНЯ КІЛЬКІСТЬ ПОРІВНЯНЬ");
        Console.WriteLine("n\tLinear\tBinary\tRatio");

        foreach (int n in sizes)
        {
            int[] data = new int[n];

            for (int i = 0; i < n; i++)
                data[i] = random.Next(0, 121);

            Array.Sort(data, (x, y) => cmp(x, y));

            long linearTotal = 0;
            long binaryTotal = 0;

            for (int q = 0; q < 100; q++)
            {
                int target = data[random.Next(n)];

                Search.LinearSearch(data, target, cmp, out int lc);
                Search.BinarySearch(data, target, cmp, out int bc);

                linearTotal += lc;
                binaryTotal += bc;
            }

            double linearAverage = linearTotal / 100.0;
            double binaryAverage = binaryTotal / 100.0;
            double ratio = linearAverage / binaryAverage;

            Console.WriteLine(
                $"{n}\t{linearAverage:F2}\t{binaryAverage:F2}\t{ratio:F2}");
        }
    }

    public static void TimeTable()
    {
        const int n = 100000;

        int[] original = new int[n];
        Random random = new Random(3);

        for (int i = 0; i < n; i++)
            original[i] = random.Next(0, 121);

        int[] queries = new int[100];

        for (int i = 0; i < queries.Length; i++)
            queries[i] = original[random.Next(n)];

        Func<int, int, int> cmp = (x, y) => x.CompareTo(y);

        int[] qValues = { 1, 5, 10, 20, 30, 50, 100 };

        RunLinear(original, queries, 100, cmp);
        RunSortedBinary(original, queries, 100, cmp);

        Console.WriteLine("\nТАБЛИЦЯ 2. ВИМІРЮВАННЯ ЧАСУ");
        Console.WriteLine("q\tLinear (ms)\tSort + Binary (ms)\tFaster");

        foreach (int q in qValues)
        {
            double linearTime = MeasureMinimum(
                () => RunLinear(original, queries, q, cmp));

            double sortedTime = MeasureMinimum(
                () => RunSortedBinary(original, queries, q, cmp));

            string faster = linearTime < sortedTime
                ? "Linear"
                : sortedTime < linearTime
                    ? "Sort + Binary"
                    : "Equal";

            Console.WriteLine(
                $"{q}\t{linearTime:F3}\t\t{sortedTime:F3}\t\t{faster}");
        }
    }

    private static void RunLinear(
        int[] data, int[] queries, int q,
        Func<int, int, int> cmp)
    {
        long checksum = 0;

        for (int i = 0; i < q; i++)
        {
            int index = Search.LinearSearch(
                data, queries[i], cmp, out int comparisons);

            checksum += index;
            checksum += comparisons;
        }

        checksumSink = checksum;
    }

    private static void RunSortedBinary(
        int[] data, int[] queries, int q,
        Func<int, int, int> cmp)
    {
        int[] sorted = (int[])data.Clone();

        Array.Sort(sorted, (x, y) => cmp(x, y));

        long checksum = 0;

        for (int i = 0; i < q; i++)
        {
            int index = Search.BinarySearch(
                sorted, queries[i], cmp, out int comparisons);

            checksum += index;
            checksum += comparisons;
        }

        checksumSink = checksum;
    }

    private static double MeasureMinimum(Action action)
    {
        double minimum = double.MaxValue;

        for (int repetition = 0; repetition < 3; repetition++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            action();

            stopwatch.Stop();

            if (stopwatch.Elapsed.TotalMilliseconds < minimum)
                minimum = stopwatch.Elapsed.TotalMilliseconds;
        }

        return minimum;
    }
}