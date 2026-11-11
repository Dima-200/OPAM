using System;
using System.Diagnostics;
using System.Linq;   // LINQ тут лише як ЕТАЛОН OrderBy у стабільності та для виведення; у власних алгоритмах його немає.
using System.Text;

namespace OpamLab09;

public delegate void SortFn<T>(T[] a, Func<T, T, int> cmp, out int comparisons, out int moves);

/// <summary>Вимірювання: таблиці лічильників, час, межа прийнятності, стабільність, крайові випадки.</summary>
public static class Experiments
{
    // ---------- спільне: запуск з обома перевірками ----------
    public static (int c, int m) RunChecked(SortFn<int> sort, int[] input, string label)
    {
        int[] a = (int[])input.Clone();
        sort(a, Data.IntCmp, out int c, out int m);
        if (!Checks.IsSorted(a, Data.IntCmp)) throw new Exception($"{label}: НЕ відсортовано");
        if (!Checks.IsPermutationOf(a, input)) throw new Exception($"{label}: НЕ перестановка вхідних даних");
        return (c, m);
    }

    // ---------- Завдання 4: таблиця «порівняння / переставляння» ----------
    public static void CounterTable(string name, SortFn<int> sort)
    {
        Console.WriteLine($"\n### {name}\n");
        Console.WriteLine("| n | випадковий | відсортований | навпаки | майже |");
        Console.WriteLine("|---|---|---|---|---|");
        foreach (int n in new[] { 100, 1000, 10000 })
        {
            int[][] inputs = Data.FourInputs(n);
            var cells = new string[4];
            for (int k = 0; k < 4; k++)
            {
                var (c, m) = RunChecked(sort, inputs[k], $"{name} n={n} {Data.KindNames[k]}");
                cells[k] = $"{c} / {m}";
            }
            Console.WriteLine($"| {n} | {string.Join(" | ", cells)} |");
        }
    }

    /// <summary>Усі елементи однакові: Хоар проти Ломуто. На Ломуто видно виродження в n²/2.</summary>
    public static void AllEqualExperiment()
    {
        Console.WriteLine("\n### Усі елементи однакові (швидке)\n");
        Console.WriteLine("| n | Хоар (власне швидке), порівн. / перест. | Ломуто, порівн. / перест. | n(n-1)/2 |");
        Console.WriteLine("|---|---|---|---|");
        foreach (int n in new[] { 1000, 10000 })
        {
            int[] eq = Enumerable.Repeat(7, n).ToArray();
            var h = RunChecked(Sorts.QuickSort, eq, "Хоар all-equal");
            var l = RunChecked(Sorts.QuickSortLomuto, eq, "Ломуто all-equal");
            Console.WriteLine($"| {n} | {h.c} / {h.m} | {l.c} / {l.m} | {(long)n * (n - 1) / 2} |");
        }
    }

    // ---------- Завдання 5: час ----------
    /// <summary>Прогрівальний запуск, потім 3 заміри; беремо найменший (мілісекунди).</summary>
    public static double TimeMs(Action<int[]> sort, int[] input)
    {
        sort((int[])input.Clone());                       // прогрів
        double best = double.MaxValue;
        for (int r = 0; r < 3; r++)
        {
            int[] a = (int[])input.Clone();               // копія — поза секундоміром
            var sw = Stopwatch.StartNew();
            sort(a);
            sw.Stop();
            best = Math.Min(best, sw.Elapsed.TotalMilliseconds);
        }
        return best;
    }

    public static void TimeTable()
    {
        Console.WriteLine("\n### Час на випадковому вході, мс (найменший із трьох)\n");
        Console.WriteLine("| n | Вибір (зав. 1) | Швидке (зав. 2) | Шелл (зав. 3) | Array.Sort |");
        Console.WriteLine("|---|---|---|---|---|");
        foreach (int n in new[] { 1000, 10000, 100000, 1000000 })
        {
            int[] g = Data.Generate(n);
            string sel = n <= 100000 ? TimeMs(a => Sorts.SelectionSort(a, Data.IntCmp, out _, out _), g).ToString("F2") : "—";
            string qs = TimeMs(a => Sorts.QuickSort(a, Data.IntCmp, out _, out _), g).ToString("F2");
            string sh = TimeMs(a => Sorts.ShellSort(a, Data.IntCmp, out _, out _), g).ToString("F2");
            string bs = TimeMs(a => Array.Sort(a), g).ToString("F2");   // еталон
            Console.WriteLine($"| {n} | {sel} | {qs} | {sh} | {bs} |");
        }
    }

    /// <summary>Межа прийнятності: перший n, на якому вибір не вкладається в 1 с. Подвоєння + двійковий пошук.</summary>
    public static void AcceptabilityLimit()
    {
        Console.WriteLine("\n### Межа прийнятності простого сортування (вибір, випадковий вхід, 1 с)\n");
        double Ms(int n) => TimeMs(a => Sorts.SelectionSort(a, Data.IntCmp, out _, out _), Data.Generate(n));
        int lo = 1000, hi = 1000;
        while (Ms(hi) < 1000) { lo = hi; hi *= 2; }
        while (hi - lo > Math.Max(100, lo / 50))
        {
            int mid = (lo + hi) / 2;
            if (Ms(mid) < 1000) lo = mid; else hi = mid;
        }
        Console.WriteLine($"Ще вкладається: n = {lo}; вже ні: n = {hi}. Межа ≈ {hi} елементів.");
    }

    // ---------- Завдання 5: стабільність ----------
    static readonly string[] Surnames =
    {
        "Бондаренко","Ковальчук","Шевченко","Коваленко","Бойко","Мельник","Ткаченко","Кравченко",
        "Олійник","Шевчук","Поліщук","Бондар","Ткачук","Мороз","Марченко","Лисенко","Савченко",
        "Руденко","Петренко","Гончаренко","Павленко","Клименко","Кузьменко","Давиденко",
        "Захарченко","Карпенко","Левченко","Мартинюк","Назаренко","Онищенко","Панченко",
        "Романюк","Семенюк","Тимошенко","Федоренко","Харченко","Цимбалюк","Чумак","Яковенко",
        "Гриценко","Дяченко","Остапенко","Приходько","Сидоренко","Тарасенко","Василенко",
        "Юрченко","Іваненко"
    };

    public static Student[] MakeStudents()
    {
        var rnd = new Random(Data.N);
        var list = new Student[Surnames.Length];
        for (int i = 0; i < list.Length; i++)
            list[i] = new Student(Surnames[i], "ІПЗ-" + (1 + rnd.Next(3)), rnd.Next(0, 10));   // бал 0..9: багато збігів
        return list;
    }

    static readonly Func<Student, Student, int> ByName = (x, y) => string.CompareOrdinal(x.Name, y.Name);
    static readonly Func<Student, Student, int> ByScore = (x, y) => x.Score.CompareTo(y.Score);

    /// <summary>Перший контрприклад: двоє з однаковим балом, у яких прізвища пішли не за алфавітом.</summary>
    static string FirstCounterexample(Student[] a)
    {
        for (int i = 1; i < a.Length; i++)
            if (a[i - 1].Score == a[i].Score && string.CompareOrdinal(a[i - 1].Name, a[i].Name) > 0)
                return $"{a[i - 1].Name} ({a[i - 1].Score}) стоїть перед {a[i].Name} ({a[i].Score})";
        return "—";
    }

    public static void Stability()
    {
        Student[] byName = MakeStudents();
        Sorts.QuickSort(byName, ByName, out _, out _);   // упорядкований за прізвищем (власним сортуванням)
        if (!Checks.IsSorted(byName, ByName)) throw new Exception("вхід стабільності не впорядкований за прізвищем");

        Console.WriteLine("\n### Стабільність (Student[] упорядкований за прізвищем → сортуємо за балом)\n");
        Console.WriteLine("| Алгоритм | Фактично | Перший контрприклад |");
        Console.WriteLine("|---|---|---|");

        void Row(string name, Student[] result)
        {
            if (!Checks.IsSorted(result, ByScore)) throw new Exception(name + ": не відсортовано за балом");
            if (!Checks.IsPermutationOf(result, byName)) throw new Exception(name + ": не перестановка");
            string ce = FirstCounterexample(result);
            Console.WriteLine($"| {name} | {(ce == "—" ? "стабільне" : "нестабільне")} | {ce} |");
        }

        foreach (var (name, fn) in new (string, SortFn<Student>)[]
                 { ("Вибір", Sorts.SelectionSort), ("Швидке (медіана трьох)", Sorts.QuickSort), ("Шелл", Sorts.ShellSort) })
        {
            var a = (Student[])byName.Clone();
            fn(a, ByScore, out _, out _);
            Row(name, a);
        }

        var arr = (Student[])byName.Clone();
        Array.Sort(arr, (x, y) => x.Score.CompareTo(y.Score));
        Row("Array.Sort", arr);
        Row("OrderBy", byName.OrderBy(s => s.Score).ToArray());
    }

    // ---------- Крайові випадки ----------
    public static void EdgeCases()
    {
        var two = new int[1000];
        for (int i = 0; i < two.Length; i++) two[i] = i < 500 ? 3 : 9;
        var rnd = new Random(Data.N);
        for (int i = two.Length - 1; i > 0; i--) { int j = rnd.Next(i + 1); (two[i], two[j]) = (two[j], two[i]); }

        var cases = new (string, int[])[]
        {
            ("порожній масив", new int[0]),
            ("один елемент", new[] { 5 }),
            ("два елементи в порядку", new[] { 1, 2 }),
            ("два елементи навпаки", new[] { 2, 1 }),
            ("усі елементи однакові", Enumerable.Repeat(7, 1000).ToArray()),
            ("два різних значення по пів масиву", two),
        };
        Console.WriteLine("\n### Крайові випадки (IsSorted + IsPermutationOf)\n");
        Console.WriteLine("| Вхід | Вибір | Швидке | Шелл |");
        Console.WriteLine("|---|---|---|---|");
        foreach (var (name, data) in cases)
        {
            string R(SortFn<int> f)
            {
                try { RunChecked(f, data, name); return "OK"; }
                catch (Exception e) { return "ПОМИЛКА: " + e.Message; }
            }
            Console.WriteLine($"| {name} | {R(Sorts.SelectionSort)} | {R(Sorts.QuickSort)} | {R(Sorts.ShellSort)} |");
        }
    }

    // ---------- Трасування вибором для перших 7 елементів — для звірки з ручним ----------
    public static void Trace()
    {
        int[] a = Data.Generate(7);
        Console.WriteLine("\n### Трасування (вибір), перші 7 елементів\n");
        Console.WriteLine("Початок: " + string.Join(" ", a));
        for (int i = 0; i < a.Length - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < a.Length; j++) if (a[j] < a[min]) min = j;
            if (min != i) (a[i], a[min]) = (a[min], a[i]);
            Console.WriteLine($"Прохід {i + 1}: мінімум на позиції {min + 1} → {string.Join(" ", a)}");
        }
    }
}
