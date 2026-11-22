using System;
using System.Collections.Generic;

namespace Lab10;

// Завдання 2 (a = 0): найдовший підрядок без повторів символів
static class LongestSubstring
{
    // Наївно: для кожного початку i розширюємо кінець j, перевіряючи кожен підрядок.
    // Розширення обривається на першому повторі (довший підрядок теж буде з повтором).
    public static (int start, int len) Naive(string s)
    {
        int bs = 0, bl = 0;
        for (int i = 0; i < s.Length; i++)
        {
            var seen = new HashSet<char>();
            for (int j = i; j < s.Length; j++)
            {
                if (!seen.Add(s[j])) break;
                if (j - i + 1 > bl) { bl = j - i + 1; bs = i; }
            }
        }
        return (bs, bl);
    }

    // Швидко: ковзне вікно [l, r] зі змінною довжиною + HashSet
    public static (int start, int len) Fast(string s)
    {
        var set = new HashSet<char>();
        int l = 0, bs = 0, bl = 0;
        for (int r = 0; r < s.Length; r++)
        {
            while (!set.Add(s[r]))
            {
                set.Remove(s[l]);
                l++;
            }
            if (r - l + 1 > bl) { bl = r - l + 1; bs = l; }
        }
        return (bs, bl);
    }

    // Окрема функція перевірки: обидва алгоритми мають давати однакову відповідь
    public static bool Verify()
    {
        var rnd = new Random(3);
        for (int t = 0; t < 2000; t++)
        {
            int len = rnd.Next(0, 41);
            int alpha = rnd.Next(1, 9);
            var chars = new char[len];
            for (int i = 0; i < len; i++) chars[i] = (char)('a' + rnd.Next(alpha));
            string s = new string(chars);
            if (Naive(s) != Fast(s)) return false;
        }
        return true;
    }

    public static void Run(string prose)
    {
        Report.Line("## Завдання 2: найдовший підрядок без повторів");
        Report.Line();
        Report.Line("Перевірка збігу відповідей (2000 випадкових рядків): " + (Verify() ? "OK" : "ПОМИЛКА — відповіді різняться!"));
        Report.Line();

        Report.Line("### Проза (обрізана)");
        Report.Line("| Символів | Довжина відповіді | Наївно, мс | Швидко, мс | У скільки разів |");
        Report.Line("|---|---|---|---|---|");
        foreach (int size in new[] { 1_000, 10_000, 50_000 })
        {
            string s = prose.Substring(0, Math.Min(size, prose.Length));
            int len = Fast(s).len;
            double tn = Bench.Ms(() => Bench.Sink += Naive(s).len);
            double tf = Bench.Ms(() => Bench.Sink += Fast(s).len);
            Report.Line($"| {size} | {len} | {tn:F3} | {tf:F3} | {tn / Math.Max(tf, 0.0001):F1} |");
        }
        Report.Line();

        Report.Line("### Контрольний вхід: усі символи різні (найгірший випадок для наївного)");
        Report.Line("| Символів | Наївно, мс | Швидко, мс | У скільки разів |");
        Report.Line("|---|---|---|---|");
        foreach (int size in new[] { 1_000, 5_000, 20_000 })
        {
            var chars = new char[size];
            for (int i = 0; i < size; i++) chars[i] = (char)(0x4E00 + i);
            string s = new string(chars);
            double tn = Bench.Ms(() => Bench.Sink += Naive(s).len);
            double tf = Bench.Ms(() => Bench.Sink += Fast(s).len);
            Report.Line($"| {size} | {tn:F3} | {tf:F3} | {tn / Math.Max(tf, 0.0001):F1} |");
        }
        Report.Line();
    }
}
