using System;

class Program
{
    const int N = 3;
    const int K = 6;
    const int A = N % 3, B = N % 4, C = N % 5;

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine($"N = {N}, K = {K}, a = {A}, b = {B}, c = {C}");

        Random rnd = new Random(N);
        int[] arr = CreateArray(rnd, 18, -3, 3);

        PrintArray("Масив", arr);
        PrintAggregate(arr);
        PrintArray("Елементи > " + (K % 5), Filter(arr, K % 5));
        PrintRun(arr);

        PrintArray("До перестановки", arr);
        MoveNonPositiveToFront(arr);
        PrintArray("Після перестановки", arr);

        int[,] m = CreateMatrix(rnd, 6, 3, -3, 3);
        PrintMatrix(m);
        PrintMatrixResults(m);

        Console.WriteLine("\n--- Крайові випадки ---");
        EdgeCases();
    }

    static int[] CreateArray(Random rnd, int size, int lo, int hi)
    {
        int[] a = new int[size];
        for (int i = 0; i < a.Length; i++) a[i] = rnd.Next(lo, hi + 1);
        return a;
    }

    static void PrintArray(string title, int[] a)
    {
        Console.Write(title + ": ");
        if (a.Length == 0) Console.Write("(порожній)");
        for (int i = 0; i < a.Length; i++) Console.Write(a[i] + " ");
        Console.WriteLine();
    }

    static bool Aggregate(int[] a, out long sum, out double avg,
        out int min, out int minIdx, out int max, out int maxIdx, out int zeros)
    {
        sum = 0; avg = 0; min = 0; minIdx = -1; max = 0; maxIdx = -1; zeros = 0;
        if (a.Length == 0) return false;

        min = max = a[0]; minIdx = maxIdx = 0;
        for (int i = 0; i < a.Length; i++)
        {
            sum += a[i];
            if (a[i] < min) { min = a[i]; minIdx = i; }
            if (a[i] > max) { max = a[i]; maxIdx = i; }
            if (a[i] == 0) zeros++;
        }
        avg = (double)sum / a.Length;
        return true;
    }

    static void PrintAggregate(int[] a)
    {
        long sum; double avg; int min, minIdx, max, maxIdx, zeros;
        if (!Aggregate(a, out sum, out avg, out min, out minIdx, out max, out maxIdx, out zeros))
        {
            Console.WriteLine("Завдання 1: результат не визначений (порожній масив)");
            return;
        }
        Console.WriteLine($"Сума: {sum}, середнє: {avg:F2}, мін: {min} (індекс {minIdx}), " +
                          $"макс: {max} (індекс {maxIdx}), нулів: {zeros}");
    }

    static int[] Filter(int[] a, int threshold)
    {
        int count = 0;
        for (int i = 0; i < a.Length; i++)
            if (a[i] > threshold) count++;

        int[] result = new int[count];
        int k = 0;
        for (int i = 0; i < a.Length; i++)
            if (a[i] > threshold) result[k++] = a[i];
        return result;
    }

    static bool LongestRun(int[] a, out int value, out int length, out int start)
    {
        value = 0; length = 0; start = -1;
        if (a.Length == 0) return false;

        int bestLen = 1, bestStart = 0, curLen = 1;
        for (int i = 1; i < a.Length; i++)
        {
            if (a[i] == a[i - 1]) curLen++; else curLen = 1;
            if (curLen > bestLen) { bestLen = curLen; bestStart = i - curLen + 1; }
        }
        value = a[bestStart]; length = bestLen; start = bestStart;
        return true;
    }

    static void PrintRun(int[] a)
    {
        int v, len, st;
        if (!LongestRun(a, out v, out len, out st))
            Console.WriteLine("Завдання 3: результат не визначений (порожній масив)");
        else
            Console.WriteLine($"Найдовша серія: значення {v}, довжина {len}, початок з індексу {st}");
    }

    static void MoveNonPositiveToFront(int[] a)
    {
        int pos = 0;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] <= 0)
            {
                int tmp = a[i];
                for (int j = i; j > pos; j--) a[j] = a[j - 1];
                a[pos] = tmp;
                pos++;
            }
        }
    }
    static int[,] CreateMatrix(Random rnd, int rows, int cols, int lo, int hi)
    {
        int[,] m = new int[rows, cols];
        for (int i = 0; i < m.GetLength(0); i++)
            for (int j = 0; j < m.GetLength(1); j++)
                m[i, j] = rnd.Next(lo, hi + 1);
        return m;
    }

    static void PrintMatrix(int[,] m)
    {
        Console.WriteLine("Матриця:");
        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
                Console.Write("{0,4}", m[i, j]);
            Console.WriteLine();
        }
    }

    static int[] RowSums(int[,] m)
    {
        int[] sums = new int[m.GetLength(0)];
        for (int i = 0; i < m.GetLength(0); i++)
            for (int j = 0; j < m.GetLength(1); j++)
                sums[i] += m[i, j];
        return sums;
    }

    static int[] ColumnMax(int[,] m)
    {
        int[] maxes = new int[m.GetLength(1)];
        for (int j = 0; j < m.GetLength(1); j++)
        {
            maxes[j] = m[0, j];
            for (int i = 1; i < m.GetLength(0); i++)
                if (m[i, j] > maxes[j]) maxes[j] = m[i, j];
        }
        return maxes;
    }

    static int IndexOfMax(int[] v)
    {
        int idx = 0;
        for (int i = 1; i < v.Length; i++)
            if (v[i] > v[idx]) idx = i;
        return idx;
    }

    static void PrintMatrixResults(int[,] m)
    {
        int[] sums = RowSums(m);
        int[] maxes = ColumnMax(m);
        PrintArray("Суми рядків", sums);
        PrintArray("Максимуми стовпців", maxes);
        Console.WriteLine("Рядок з найбільшою сумою: " + IndexOfMax(sums));
    }

    static void EdgeCases()
    {
        int[][] tests = { new int[0], new int[] { 5 }, new int[] { 2, 2, 2, 2 } };
        string[] names = { "порожній масив", "один елемент", "усі однакові" };

        for (int t = 0; t < tests.Length; t++)
        {
            int[] x = tests[t];
            Console.WriteLine("\n" + names[t]);
            PrintAggregate(x);
            PrintArray("Фільтр", Filter(x, K % 5));
            PrintRun(x);
            MoveNonPositiveToFront(x);
            PrintArray("Перестановка", x);
        }
    }
}