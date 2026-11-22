using System;

namespace Lab10;

// Завдання 4 (b = 3): розрізати стрижень на задані деталі з найменшим відходом.
// Припущення: кожну деталь з набору можна використати щонайбільше один раз.
// Відхід = довжина стрижня − сума вибраних деталей.
static class RodCutting
{
    public const int Rod = 36;

    // Жадібно: брати найдовшу деталь, що влазить. Повертає використану довжину.
    public static int Greedy(int[] parts, int rod)
    {
        int[] p = (int[])parts.Clone();
        Array.Sort(p);
        Array.Reverse(p);
        int used = 0;
        for (int i = 0; i < p.Length; i++)
            if (used + p[i] <= rod) used += p[i];
        return used;
    }

    // Повний перебір 2^n підмножин (еталон). Повертає найбільшу використану довжину.
    public static int Brute(int[] parts, int rod) => Rec(parts, 0, 0, rod);

    static int Rec(int[] p, int i, int sum, int rod)
    {
        if (i == p.Length) return sum;
        int best = Rec(p, i + 1, sum, rod);
        if (sum + p[i] <= rod)
        {
            int t = Rec(p, i + 1, sum + p[i], rod);
            if (t > best) best = t;
        }
        return best;
    }

    static bool FindCounter(int rod, int n, int[] cur, int idx, int min, ref int[]? found)
    {
        if (idx == n)
        {
            if (Greedy(cur, rod) < Brute(cur, rod)) { found = (int[])cur.Clone(); return true; }
            return false;
        }
        for (int v = min; v <= rod; v++)
        {
            cur[idx] = v;
            if (FindCounter(rod, n, cur, idx + 1, v, ref found)) return true;
        }
        return false;
    }

    public static void Run()
    {
        var rnd = new Random(3);
        int optimal = 0, worstG = 0, worstB = 0;
        double worstGap = 0;
        const int tests = 100;
        for (int t = 0; t < tests; t++)
        {
            int n = rnd.Next(1, 16);
            int[] p = new int[n];
            for (int i = 0; i < n; i++) p[i] = rnd.Next(3, 21);
            int g = Greedy(p, Rod), b = Brute(p, Rod);
            if (g == b) optimal++;
            else
            {
                double gap = (b - g) * 100.0 / b;
                if (gap > worstGap) { worstGap = gap; worstG = g; worstB = b; }
            }
        }

        // найменший контрприклад: спершу найменше n, потім найменший стрижень
        int[]? found = null;
        int foundRod = 0;
        for (int n = 1; n <= 6 && found == null; n++)
            for (int rod = 1; rod <= 15 && found == null; rod++)
                FindCounter(rod, n, new int[n], 0, 1, ref found);
        if (found != null)
        {
            // знайти rod ще раз для виводу
            for (int rod = 1; rod <= 15; rod++)
                if (Greedy(found, rod) < Brute(found, rod)) { foundRod = rod; break; }
        }

        var r = new Random(3);
        int[] a15 = new int[15], a20 = new int[20];
        for (int i = 0; i < 15; i++) a15[i] = r.Next(3, 21);
        for (int i = 0; i < 20; i++) a20[i] = r.Next(3, 21);
        double t15 = Bench.Ms(() => Bench.Sink += Brute(a15, Rod));
        double t20 = Bench.Ms(() => Bench.Sink += Brute(a20, Rod));

        Report.Line("## Завдання 4: жадібно проти перебору (стрижень 36 см)");
        Report.Line();
        Report.Line("| Показник | Значення |");
        Report.Line("|---|---|");
        Report.Line($"| Тестів, де жадібне дало оптимум | {optimal} із {tests} |");
        Report.Line(worstGap > 0
            ? $"| Найгірше відставання | {worstGap:F1}% (жадібно використано {worstG} см, оптимум {worstB} см) |"
            : "| Найгірше відставання | 0% (на цих тестах розбіжностей немає) |");
        Report.Line(found != null
            ? $"| Найменший контрприклад | стрижень {foundRod}, деталі [{string.Join(", ", found)}]: жадібно {Greedy(found, foundRod)}, оптимум {Brute(found, foundRod)} |"
            : "| Найменший контрприклад | не знайдено в пошуку n ≤ 6, стрижень ≤ 15 |");
        Report.Line($"| Час перебору | 15 елементів: {t15:F3} мс; 20 елементів: {t20:F3} мс (у {t20 / Math.Max(t15, 0.0001):F0} раз(ів) більше) |");
        Report.Line();
    }
}
