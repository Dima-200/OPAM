using System;

namespace Lab10;

static class EdgeCases
{
    static string Q(string s) => "`\"" + s + "\"`";

    public static void Run()
    {
        Report.Line("## Крайові випадки");
        Report.Line();
        Report.Line("| Вхід | Завдання | Очікувано | Отримано |");
        Report.Line("|---|---|---|---|");

        T2("", "довжина 0");
        T2("a", "довжина 1");
        T2("aaaa", "довжина 1");
        T2("  abc  d  ", "довжина 4, початок 1 (\" abc\")");

        // Завдання 3
        int r = SubstringSearch.IndexOf("ab", "abc", out int c);
        Report.Line($"| шаблон довший за текст | 3 | -1, 0 порівнянь | {r}, {c} порівнянь |");
        r = SubstringSearch.IndexOf("abc", "", out c);
        Report.Line($"| порожній шаблон | 3 | 0, 0 порівнянь | {r}, {c} порівнянь |");
        r = SubstringSearch.IndexOf("", "", out c);
        Report.Line($"| порожній текст і порожній шаблон | 3 | 0 | {r}, {c} порівнянь |");

        // Завдання 4
        int[] none = Array.Empty<int>();
        Report.Line($"| нуль елементів | 4 | використано 0, відхід {RodCutting.Rod} | жадібно {RodCutting.Greedy(none, RodCutting.Rod)}, перебір {RodCutting.Brute(none, RodCutting.Rod)} |");
        int[] one = { 10 };
        Report.Line($"| один елемент (10) | 4 | використано 10 | жадібно {RodCutting.Greedy(one, RodCutting.Rod)}, перебір {RodCutting.Brute(one, RodCutting.Rod)} |");
        int[] big = { 40 };
        Report.Line($"| один елемент, що не влазить (40) | 4 | використано 0 | жадібно {RodCutting.Greedy(big, RodCutting.Rod)}, перебір {RodCutting.Brute(big, RodCutting.Rod)} |");
        Report.Line();
    }

    static void T2(string s, string expected)
    {
        var n = LongestSubstring.Naive(s);
        var f = LongestSubstring.Fast(s);
        Report.Line($"| {Q(s)} | 2 | {expected} | наївно (початок {n.start}, довжина {n.len}); швидко (початок {f.start}, довжина {f.len}) |");
    }
}
