using System;

namespace Lab10;

// Завдання 3: наївний пошук підрядка з лічильником порівнянь символів
static class SubstringSearch
{
    public static int IndexOf(string text, string pat, out int comparisons)
    {
        comparisons = 0;
        if (pat.Length == 0) return 0;
        if (pat.Length > text.Length) return -1;
        for (int i = 0; i <= text.Length - pat.Length; i++)
        {
            int j = 0;
            while (j < pat.Length)
            {
                comparisons++;
                if (text[i + j] != pat[j]) break;
                j++;
            }
            if (j == pat.Length) return i;
        }
        return -1;
    }

    static string WordNear(string text, int pos)
    {
        int i = text.IndexOf(' ', pos);
        if (i < 0) i = pos;
        while (i < text.Length && !char.IsLetter(text[i])) i++;
        int j = i;
        while (j < text.Length && char.IsLetter(text[j])) j++;
        return text.Substring(i, j - i);
    }

    public static void Run(string prose)
    {
        string text = prose.Substring(0, 50_000);
        string present = WordNear(text, 45_000);
        string absent = "ґґґґґґ";
        if (text.IndexOf(absent, StringComparison.Ordinal) >= 0) absent = "#####";

        string rep = new string('Д', 10_000);
        string repPat = new string('Д', 959) + "Е"; // 960 символів

        Report.Line("## Завдання 3: наївний пошук підрядка");
        Report.Line();
        Report.Line("| Вхід | Шаблон | Знайдено на | Порівнянь | Час, мс | string.IndexOf, мс |");
        Report.Line("|---|---|---|---|---|---|");
        Row("проза, 50 тис.", text, present, "слово з кінця тексту");
        Row("проза, 50 тис.", text, absent, "якого в тексті немає");
        Row("повтори, 10 тис. Д", rep, repPat, "959×Д + Е");
        Report.Line();
        Report.Line($"Для порівняння: теоретично (10000 − 960 + 1) × 960 = {(10_000 - 960 + 1) * 960} порівнянь для третього рядка.");
        Report.Line();
    }

    static void Row(string input, string text, string pat, string label)
    {
        int idx = IndexOf(text, pat, out int cmp);
        int idx2 = text.IndexOf(pat, StringComparison.Ordinal);
        if (idx != idx2) Report.Line("!!! ПОМИЛКА: власний пошук і string.IndexOf дали різні відповіді");
        double t1 = Bench.Ms(() => Bench.Sink += SubstringSearch.IndexOf(text, pat, out int _));
        double t2 = Bench.Ms(() => Bench.Sink += text.IndexOf(pat, StringComparison.Ordinal));
        string shown = pat.Length > 12 ? label : $"«{pat}»";
        Report.Line($"| {input} | {shown} | {idx} | {cmp} | {t1:F4} | {t2:F4} |");
    }
}
