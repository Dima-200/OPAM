
using System;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    const int N = 3;
    const int K = 6;

    static void PrintList(string title, List<int> list)
    {
        Console.WriteLine($"{title}: [{string.Join(", ", list)}]");
    }

    static List<int> FilterList(List<int> source)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < source.Count; i++)
        {
            if (source[i] > K % 7)
                result.Add(source[i]);
        }

        return result;
    }

    static List<int> RemoveIncorrectly(List<int> source, int target)
    {
        List<int> result = new List<int>(source);

        for (int i = 0; i < result.Count; i++)
        {
            if (result[i] == target)
                result.RemoveAt(i);
        }

        return result;
    }

    static List<int> RemoveFromEnd(List<int> source, int target)
    {
        List<int> result = new List<int>(source);

        for (int i = result.Count - 1; i >= 0; i--)
        {
            if (result[i] == target)
                result.RemoveAt(i);
        }

        return result;
    }

    static List<int> RemoveToNewList(List<int> source, int target)
    {
        List<int> result = new List<int>(source.Count);

        for (int i = 0; i < source.Count; i++)
        {
            if (source[i] != target)
                result.Add(source[i]);
        }

        return result;
    }

    static List<int> GenerateSortedList(int length, Random rnd)
    {
        List<int> result = new List<int>(length);

        if (length == 0)
            return result;

        result.Add(rnd.Next(1, 10));

        for (int i = 1; i < length; i++)
            result.Add(result[i - 1] + rnd.Next(1, 10));

        return result;
    }

    static List<int> MergeSortedLists(List<int> first, List<int> second)
    {
        List<int> result = new List<int>(first.Count + second.Count);

        int i = 0;
        int j = 0;

        while (i < first.Count && j < second.Count)
        {
            if (first[i] <= second[j])
                result.Add(first[i++]);
            else
                result.Add(second[j++]);
        }

        while (i < first.Count)
            result.Add(first[i++]);

        while (j < second.Count)
            result.Add(second[j++]);

        return result;
    }

    static bool IsSorted(List<int> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] < list[i - 1])
                return false;
        }

        return true;
    }

    static void SplitByIndex(
        List<int> source,
        out List<int> evenIndexes,
        out List<int> oddIndexes)
    {
        evenIndexes = new List<int>((source.Count + 1) / 2);
        oddIndexes = new List<int>(source.Count / 2);

        for (int i = 0; i < source.Count; i++)
        {
            if (i % 2 == 0)
                evenIndexes.Add(source[i]);
            else
                oddIndexes.Add(source[i]);
        }
    }
    static bool CheckEdgeCases()
    {
        List<int> empty = new List<int>();
        List<int> single = new List<int> { 5 };
        List<int> noneMatch = new List<int> { -3, 0, 2 };
        List<int> allMatch = new List<int> { 7, 8, 9 };

        bool task1Empty = FilterList(empty).Count == 0;
        bool task1Single = FilterList(single).Count == 0;
        bool task1None = FilterList(noneMatch).Count == 0;
        bool task1All = FilterList(allMatch).Count == 3;

        bool task2Empty = RemoveFromEnd(empty, 0).Count == 0;
        bool task2Single = RemoveFromEnd(single, 5).Count == 0;
        bool task2None = RemoveFromEnd(noneMatch, 100).Count == 3;
       bool task2All = RemoveFromEnd(
    new List<int> { 7, 7, 7 }, 7).Count == 0;

        SplitByIndex(empty, out List<int> e1, out List<int> o1);
        SplitByIndex(single, out List<int> e2, out List<int> o2);

        bool task4Empty = e1.Count + o1.Count == 0;
        bool task4Single = e2.Count + o2.Count == 1;

        List<int> merged1 = MergeSortedLists(empty, new List<int> { 1, 2 });
        List<int> merged2 = MergeSortedLists(new List<int> { 1, 2 }, empty);

        bool task3Empty = IsSorted(merged1) && IsSorted(merged2);

        return task1Empty && task1Single && task1None && task1All
            && task2Empty && task2Single && task2None && task2All
            && task4Empty && task4Single && task3Empty;
    }
    static double Measure(Action action)
    {
        action(); 

        Stopwatch sw = Stopwatch.StartNew();
        action();
        sw.Stop();

        return sw.Elapsed.TotalMilliseconds;
    }

    static void Main()
    
    {
        List<int> repeatTest = new List<int> { -4, -4, 2, -4 };

        PrintList("Тест із повтореннями", repeatTest);
        PrintList("Хибне видалення з повтореннями",RemoveIncorrectly(repeatTest, -4));
        PrintList("Правильне видалення з повтореннями",RemoveFromEnd(repeatTest, -4));
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Лабораторна робота №4");
        Console.WriteLine("Алгоритми над списками");
        Console.WriteLine($"N = {N}, K = {K}, a = {N % 3}, b = {N % 4}, c = {N % 5}");
        Console.WriteLine();

        Random rnd = new Random(N);
        List<int> numbers = new List<int>(20);

        for (int i = 0; i < 20; i++)
            numbers.Add(rnd.Next(-9, 10));

        PrintList("Основний список", numbers);

        List<int> filtered = FilterList(numbers);
        PrintList("Завдання 1", filtered);
        Console.WriteLine($"Кількість елементів: {filtered.Count}");

        int target = numbers.Count > 0 ? numbers[0] : 0;

        List<int> incorrect = RemoveIncorrectly(numbers, target);
        List<int> fromEnd = RemoveFromEnd(numbers, target);
        List<int> newList = RemoveToNewList(numbers, target);

        PrintList("Завдання 2 — хибний варіант", incorrect);
        PrintList("Завдання 2 — цикл з кінця", fromEnd);
        PrintList("Завдання 2 — новий список", newList);

        Console.WriteLine( $"Правильні варіанти однакові: {ListsEqual(fromEnd, newList)}");

        List<int> first = GenerateSortedList(10 + K % 5, new Random(N));
        List<int> second = GenerateSortedList(6 + K % 4, new Random(N + 100));

        PrintList("Перший впорядкований список", first);
        PrintList("Другий впорядкований список", second);

        List<int> merged = MergeSortedLists(first, second);
        PrintList("Результат злиття", merged);
        Console.WriteLine($"Результат впорядкований: {IsSorted(merged)}");

        SplitByIndex(numbers, out List<int> even, out List<int> odd);

        PrintList("Елементи на парних індексах", even);
        PrintList("Елементи на непарних індексах", odd);

        Console.WriteLine($"Перевірка кількості: {even.Count + odd.Count == numbers.Count}");

        const int size = 100000;

        double addWithoutCapacity = Measure(() =>
        {
            List<int> list = new List<int>();
            for (int i = 0; i < size; i++)
                list.Add(i);
        });

        double addWithCapacity = Measure(() =>
        {
            List<int> list = new List<int>(size);
            for (int i = 0; i < size; i++)
                list.Add(i);
        });

        double insertAtStart = Measure(() =>
        {
            List<int> list = new List<int>();
            for (int i = 0; i < size; i++)
                list.Insert(0, i);
        });

        List<int> containsList = new List<int>(size);
        for (int i = 0; i < size; i++)
            containsList.Add(i);

        double containsTime = Measure(() =>
        {
            for (int i = 0; i < 1000; i++)
                containsList.Contains(-1);
        });

        Console.WriteLine();
        Console.WriteLine("Вимірювання часу (мс):");
        Console.WriteLine($"100000 Add без місткості: {addWithoutCapacity:F3}");
        Console.WriteLine($"100000 Add з місткістю: {addWithCapacity:F3}");
        Console.WriteLine($"100000 Insert(0, x): {insertAtStart:F3}");
        Console.WriteLine($"1000 Contains відсутнього: {containsTime:F3}");

        Console.WriteLine();
        Console.WriteLine($"Перевірка крайових випадків: {CheckEdgeCases()}");
    }
    static bool ListsEqual(List<int> first, List<int> second)
{
    if (first.Count != second.Count)
        return false;

    for (int i = 0; i < first.Count; i++)
    {
        if (first[i] != second[i])
            return false;
    }

    return true;
}
}
