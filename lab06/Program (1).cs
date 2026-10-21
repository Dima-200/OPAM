using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OpamLab06;

internal static class Program
{
    private const int N = 3;
    private const int K = 6;

    private const string SampleText =
        @"Алгоритм описує послідовність дій. Алгоритм можна записати словами,
а можна записати кодом. Кодом алгоритм стає тоді, коли його
виконує машина, а не людина. Одна задача — багато алгоритмів.";

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("N = 3, K = 6, a = 0, b = 3, c = 3");

        RunTask1();
        RunTask2();
        RunTask3();
        RunTask4();
        RunEdgeCases();
        Benchmarks.Run();
    }

    private static void AddWord(Dictionary<string, int> counts, StringBuilder word)
    {
        if (word.Length == 0)
        {
            return;
        }

        string key = word.ToString();
        word.Clear();

        if (counts.TryGetValue(key, out int current))
        {
            counts[key] = current + 1;
        }
        else
        {
            counts[key] = 1;
        }
    }

    private static Dictionary<string, int> CountWords(string text)
    {
        var counts = new Dictionary<string, int>();
        var word = new StringBuilder();

        foreach (char ch in text)
        {
            if (char.IsLetter(ch))
            {
                word.Append(char.ToLowerInvariant(ch));
            }
            else
            {
                AddWord(counts, word);
            }
        }

        AddWord(counts, word); 
        return counts;
    }

    private static void PrintFrequentWords(Dictionary<string, int> counts, int threshold)
    {
        int total = 0;
        foreach (KeyValuePair<string, int> pair in counts)
        {
            total += pair.Value;
        }

        Console.WriteLine($"  Усього слів: {total}, різних слів: {counts.Count}");
        Console.WriteLine($"  Слова з частотою не менше {threshold}:");

        bool any = false;
        foreach (KeyValuePair<string, int> pair in counts)
        {
            if (pair.Value >= threshold)
            {
                Console.WriteLine($"    {pair.Key}: {pair.Value}");
                any = true;
            }
        }

        if (!any)
        {
            Console.WriteLine("    (таких слів немає)");
        }
    }

    private static void RunTask1()
    {
        Console.WriteLine();
        Console.WriteLine("=== Завдання 1. Частоти слів ===");

        int threshold = K % 3 + 2; // 2
        PrintFrequentWords(CountWords(SampleText), threshold);
    }

    private static List<Student> GetStudents()
    {
        return new List<Student>
        {
            new Student { Name = "Коваленко", Group = "КН-3/1", Score = 88 },
            new Student { Name = "Мельник", Group = "КН-3/2", Score = 64 },
            new Student { Name = "Іщенко", Group = "КН-3/1", Score = 93 },
            new Student { Name = "Бондар", Group = "КН-3/1", Score = 71 },
            new Student { Name = "Савченко", Group = "КН-3/2", Score = 55 },
            new Student { Name = "Ткачук", Group = "КН-3/2", Score = 90 },
            new Student { Name = "Кравець", Group = "КН-3/1", Score = 76 },
            new Student { Name = "Іванів", Group = "КН-3/1", Score = 48 },
            new Student { Name = "Мороз", Group = "КН-3/2", Score = 82 },
            new Student { Name = "Шевчук", Group = "КН-3/1", Score = 67 },
            new Student { Name = "Бойко", Group = "КН-3/2", Score = 95 },
            new Student { Name = "Кушнір", Group = "КН-3/2", Score = 73 },
        };
    }

    private static Dictionary<int, List<Student>> GroupBySurnameLength(List<Student> students)
    {
        var groups = new Dictionary<int, List<Student>>();

        foreach (Student student in students)
        {
            int key = student.Name.Length;

            if (!groups.TryGetValue(key, out List<Student>? list))
            {
                list = new List<Student>();
                groups[key] = list;
            }

            list.Add(student);
        }

        return groups;
    }

    private static void PrintGroups(Dictionary<int, List<Student>> groups)
    {
        if (groups.Count == 0)
        {
            Console.WriteLine("  (груп немає)");
            return;
        }

        var pairs = new List<KeyValuePair<int, List<Student>>>(groups);
        pairs.Sort((x, y) => x.Key.CompareTo(y.Key));

        foreach (KeyValuePair<int, List<Student>> pair in pairs)
        {
            double sum = 0;
            foreach (Student student in pair.Value)
            {
                sum += student.Score;
            }

            double average = pair.Value.Count > 0 ? sum / pair.Value.Count : 0;
            string averageText = average.ToString("F2", CultureInfo.InvariantCulture);

            Console.WriteLine(
                $"  {pair.Key} літер: студентів {pair.Value.Count}, середній бал {averageText}");
        }
    }

    private static void RunTask2()
    {
        Console.WriteLine();
        Console.WriteLine("=== Завдання 2. Групування за кількістю літер у прізвищі ===");

        PrintGroups(GroupBySurnameLength(GetStudents()));
    }

    private static List<int> GenerateNumbers(int count, int min, int maxInclusive, int seed)
    {
        var random = new Random(seed);
        var numbers = new List<int>(count);

        for (int i = 0; i < count; i++)
        {
            numbers.Add(random.Next(min, maxInclusive + 1));
        }

        return numbers;
    }

    private static List<int> DistinctUnordered(List<int> source)
    {
        var set = new HashSet<int>(source);
        return new List<int>(set);
    }
    private static List<int> DistinctOrdered(List<int> source)
    {
        var seen = new HashSet<int>();
        var result = new List<int>();

        foreach (int x in source)
        {
            if (seen.Add(x))
            {
                result.Add(x);
            }
        }

        return result;
    }

    private static void PrintDistinct(List<int> source)
    {
        Console.WriteLine($"  Вихідний список ({source.Count}): {string.Join(", ", source)}");

        List<int> unordered = DistinctUnordered(source);
        Console.WriteLine($"  Спосіб 1, HashSet ({unordered.Count}): {string.Join(", ", unordered)}");

        List<int> ordered = DistinctOrdered(source);
        Console.WriteLine($"  Спосіб 2, зі збереженням порядку ({ordered.Count}): {string.Join(", ", ordered)}");
    }

    private static void RunTask3()
    {
        Console.WriteLine();
        Console.WriteLine("=== Завдання 3. Дублікати ===");

        int max = K % 10 + 6; // 12
        List<int> numbers = GenerateNumbers(25, 1, max, N);
        PrintDistinct(numbers);
    }

    private static (bool IsSubset, List<int> Extra) CompareWithSets(List<int> first, List<int> second)
    {
        var firstSet = new HashSet<int>(first);
        bool isSubset = firstSet.IsSubsetOf(second);

        var extraSet = new HashSet<int>(first); // копія
        extraSet.ExceptWith(second);

        var extra = new List<int>(extraSet);
        extra.Sort();
        return (isSubset, extra);
    }

    private static (bool IsSubset, List<int> Extra) CompareWithLoops(List<int> first, List<int> second)
    {
        var extra = new List<int>();

        foreach (int x in first)
        {
            bool inSecond = false;
            foreach (int y in second)
            {
                if (x == y)
                {
                    inSecond = true;
                    break;
                }
            }

            if (inSecond)
            {
                continue;
            }

            bool alreadyAdded = false;
            foreach (int e in extra)
            {
                if (e == x)
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (!alreadyAdded)
            {
                extra.Add(x);
            }
        }

        extra.Sort();
        return (extra.Count == 0, extra);
    }

    private static bool SameValues(List<int> a, List<int> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }

        return true;
    }

    private static void PrintSubsetResult(List<int> first, List<int> second)
    {
        var withSets = CompareWithSets(first, second);
        var withLoops = CompareWithLoops(first, second);

        string verdict = withSets.IsSubset ? "так" : "ні";
        Console.WriteLine($"  Перший набір — підмножина другого: {verdict}");
        Console.WriteLine($"  Зайвих елементів: {withSets.Extra.Count}" +
                          (withSets.Extra.Count > 0 ? $": {string.Join(", ", withSets.Extra)}" : ""));

        bool match = withSets.IsSubset == withLoops.IsSubset && SameValues(withSets.Extra, withLoops.Extra);
        Console.WriteLine($"  Перевірка: результати HashSet і вкладених циклів {(match ? "збігаються" : "НЕ ЗБІГАЮТЬСЯ")}");
    }

    private static void RunTask4()
    {
        Console.WriteLine();
        Console.WriteLine("=== Завдання 4. Порівняння двох наборів ===");

        int max = 20 + K; // 26
        List<int> first = GenerateNumbers(30, 1, max, N);
        List<int> second = GenerateNumbers(30, 1, max, N + 100);

        Console.WriteLine($"  Перший набір:  {string.Join(", ", first)}");
        Console.WriteLine($"  Другий набір:  {string.Join(", ", second)}");
        PrintSubsetResult(first, second);
    }


    private static void RunEdgeCases()
    {
        Console.WriteLine();
        Console.WriteLine("=== Крайові випадки ===");

        int threshold = K % 3 + 2;

        Console.WriteLine("  Завдання 1, порожній текст:");
        PrintFrequentWords(CountWords(""), threshold);

        Console.WriteLine("  Завдання 1, усі слова різні («один два три»):");
        PrintFrequentWords(CountWords("один два три"), threshold);

        Console.WriteLine("  Завдання 2, усі студенти в одній групі (Бондар, Ткачук, Іванів):");
        var sameGroup = new List<Student>
        {
            new Student { Name = "Бондар", Group = "КН-3/1", Score = 71 },
            new Student { Name = "Ткачук", Group = "КН-3/2", Score = 90 },
            new Student { Name = "Іванів", Group = "КН-3/1", Score = 48 },
        };
        PrintGroups(GroupBySurnameLength(sameGroup));

        Console.WriteLine("  Завдання 3, дублікатів немає (5, 2, 9):");
        PrintDistinct(new List<int> { 5, 2, 9 });

        Console.WriteLine("  Завдання 4, набори не перетинаються ({1, 2, 3} і {4, 5, 6}):");
        PrintSubsetResult(new List<int> { 1, 2, 3 }, new List<int> { 4, 5, 6 });

        Console.WriteLine("  Завдання 4, набори повністю однакові ({1, 2, 3} і {3, 2, 1}):");
        PrintSubsetResult(new List<int> { 1, 2, 3 }, new List<int> { 3, 2, 1 });
    }
}
