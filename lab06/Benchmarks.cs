using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace OpamLab06;


internal static class Benchmarks
{
    private const int Seed = 3;
    private const int Scale = 1;

    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("=== Завдання 5. Замір виграшу ===");
#if DEBUG
        Console.WriteLine("  УВАГА: це збірка Debug, заміри будуть неточні. Запускай: dotnet run -c Release");
#endif
        Console.WriteLine($"  Усі кількості поділено на {Scale}. Кожен замір — один прогрівальний прогін і один вимірюваний.");
        Console.WriteLine();
        Console.WriteLine("| Що міряємо | Список, мс | Словник або множина, мс | У скільки разів |");
        Console.WriteLine("|---|---|---|---|");

        RunLookups();
        RunFrequencies();
        RunDistinct();
        RunIntersection();
    }

    private static (double Milliseconds, int Result) Measure(Func<int> action)
    {
        action();

        var stopwatch = Stopwatch.StartNew();
        int result = action();
        stopwatch.Stop();

        return (stopwatch.Elapsed.TotalMilliseconds, result);
    }

    private static string Format(double milliseconds) =>
        milliseconds.ToString("F3", CultureInfo.InvariantCulture);

    private static void PrintRow(string name, Func<int> listVersion, Func<int> setVersion)
    {
        (double listMs, int listResult) = Measure(listVersion);
        (double setMs, int setResult) = Measure(setVersion);

        string ratio = setMs > 0
            ? (listMs / setMs).ToString("F1", CultureInfo.InvariantCulture)
            : "—";
        string warning = listResult == setResult ? "" : " (РЕЗУЛЬТАТИ НЕ ЗБІГАЮТЬСЯ!)";

        Console.WriteLine($"| {name} | {Format(listMs)} | {Format(setMs)} | {ratio}{warning} |");
    }

    private static void RunLookups()
    {
        int setSize = 100_000 / Scale;
        int lookups = 10_000 / Scale;

        var random = new Random(Seed);
        var list = new List<int>(setSize);
        for (int i = 0; i < setSize; i++)
        {
            list.Add(i * 2);
        }

        var set = new HashSet<int>(list);

        int[] queries = new int[lookups];
        for (int i = 0; i < lookups; i++)
        {
            queries[i] = random.Next(0, setSize * 2);
        }

        PrintRow(
            $"{lookups} перевірок наявності в наборі з {setSize} елементів",
            () =>
            {
                int found = 0;
                foreach (int q in queries)
                {
                    if (list.Contains(q))
                    {
                        found++;
                    }
                }

                return found;
            },
            () =>
            {
                int found = 0;
                foreach (int q in queries)
                {
                    if (set.Contains(q))
                    {
                        found++;
                    }
                }

                return found;
            });
    }

    private static void RunFrequencies()
    {
        int wordCount = 50_000 / Scale;
        const int vocabularySize = 5_000;

        var random = new Random(Seed);
        var vocabulary = new string[vocabularySize];
        for (int i = 0; i < vocabularySize; i++)
        {
            vocabulary[i] = "слово" + i;
        }

        var words = new List<string>(wordCount);
        for (int i = 0; i < wordCount; i++)
        {
            words.Add(vocabulary[random.Next(vocabularySize)]);
        }

        PrintRow(
            $"підрахунок частот {wordCount} слів",
            () =>
            {
                var keys = new List<string>();
                var counts = new List<int>();

                foreach (string word in words)
                {
                    int index = -1;
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (keys[i] == word)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index >= 0)
                    {
                        counts[index]++;
                    }
                    else
                    {
                        keys.Add(word);
                        counts.Add(1);
                    }
                }

                return keys.Count;
            },
            () =>
            {
                var counts = new Dictionary<string, int>();

                foreach (string word in words)
                {
                    if (counts.TryGetValue(word, out int current))
                    {
                        counts[word] = current + 1;
                    }
                    else
                    {
                        counts[word] = 1;
                    }
                }

                return counts.Count;
            });
    }

    private static void RunDistinct()
    {
        int count = 20_000 / Scale;

        var random = new Random(Seed);
        var numbers = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            numbers.Add(random.Next(0, 50_000));
        }

        PrintRow(
            $"вилучення дублікатів із {count} чисел",
            () =>
            {
                var result = new List<int>();
                foreach (int x in numbers)
                {
                    if (!result.Contains(x))
                    {
                        result.Add(x);
                    }
                }

                return result.Count;
            },
            () =>
            {
                var seen = new HashSet<int>();
                var result = new List<int>();
                foreach (int x in numbers)
                {
                    if (seen.Add(x))
                    {
                        result.Add(x);
                    }
                }

                return result.Count;
            });
    }

    private static void RunIntersection()
    {
        int size = 5_000 / Scale;

        var random = new Random(Seed);
        var first = new List<int>(size);
        var second = new List<int>(size);
        for (int i = 0; i < size; i++)
        {
            first.Add(random.Next(0, 10_000));
            second.Add(random.Next(0, 10_000));
        }

        PrintRow(
            $"спільні елементи двох наборів по {size} елементів",
            () =>
            {
                var common = new List<int>();

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

                    if (!inSecond)
                    {
                        continue;
                    }

                    bool alreadyAdded = false;
                    foreach (int c in common)
                    {
                        if (c == x)
                        {
                            alreadyAdded = true;
                            break;
                        }
                    }

                    if (!alreadyAdded)
                    {
                        common.Add(x);
                    }
                }

                return common.Count;
            },
            () =>
            {
                var common = new HashSet<int>(first);
                common.IntersectWith(second);
                return common.Count;
            });
    }
}
