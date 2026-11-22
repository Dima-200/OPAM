using System;
using System.Globalization;
using System.Text;

namespace Lab10;

// Завдання 1: три пастки
static class Traps
{
    public static void Run(string prose)
    {
        Report.Line("## Завдання 1: пастки");
        Report.Line();

        // --- Пастка 1 ---
        Report.Line("### Пастка 1: s += c проти StringBuilder.Append");
        Report.Line("| Додавань | += , мс | StringBuilder, мс | У скільки разів |");
        Report.Line("|---|---|---|---|");
        foreach (int n in new[] { 10_000, 100_000 })
        {
            double t1 = Bench.Ms(() =>
            {
                string s = "";
                for (int i = 0; i < n; i++) s += (char)('a' + i % 26);
                Bench.Sink += s.Length;
            });
            double t2 = Bench.Ms(() =>
            {
                var sb = new StringBuilder();
                for (int i = 0; i < n; i++) sb.Append((char)('a' + i % 26));
                Bench.Sink += sb.ToString().Length;
            });
            Report.Line($"| {n} | {t1:F2} | {t2:F2} | {t1 / Math.Max(t2, 0.001):F0} |");
        }
        Report.Line();

        // --- Пастка 2 ---
        string[] words = prose.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        int m = Math.Min(words.Length, 5000);
        const int pairs = 100_000;
        double a = Bench.Ms(() =>
        {
            int eq = 0;
            for (int i = 0; i < pairs; i++)
            {
                string x = words[i % m], y = words[(i * 7 + 1) % m];
                if (x.ToLower() == y.ToLower()) eq++; // навмисно: це і є "пастка"
            }
            Bench.Sink += eq;
        });
        double b = Bench.Ms(() =>
        {
            int eq = 0;
            for (int i = 0; i < pairs; i++)
            {
                string x = words[i % m], y = words[(i * 7 + 1) % m];
                if (string.Equals(x, y, StringComparison.OrdinalIgnoreCase)) eq++;
            }
            Bench.Sink += eq;
        });
        Report.Line("### Пастка 2: ToLower() == ToLower() проти string.Equals(OrdinalIgnoreCase)");
        Report.Line("| Порівнянь | ToLower, мс | OrdinalIgnoreCase, мс | У скільки разів |");
        Report.Line("|---|---|---|---|");
        Report.Line($"| {pairs} | {a:F2} | {b:F2} | {a / Math.Max(b, 0.001):F1} |");
        Report.Line();

        // --- Пастка 3 ---
        string[] src =
        {
            "ґанок", "ґудзик", "ґрунт", "ґатунок", "ґава", "ґвалт", "ґрати", "ґумка", "ґедзь", "ґніт",
            "гарбуз", "гора", "голуб", "глина", "гість", "грім", "гвинт", "гума", "дім", "ящик"
        };
        string[] ord = (string[])src.Clone();
        Array.Sort(ord, StringComparer.Ordinal);
        string[] cul = (string[])src.Clone();
        Array.Sort(cul, StringComparer.Create(new CultureInfo("uk-UA"), false));
        bool differ = false;
        for (int i = 0; i < ord.Length; i++)
            if (!string.Equals(ord[i], cul[i], StringComparison.Ordinal)) differ = true;

        Report.Line("### Пастка 3: ординальне проти культурного (uk-UA) порівняння");
        Report.Line("| № | Ordinal | Культурне uk-UA |");
        Report.Line("|---|---|---|");
        for (int i = 0; i < ord.Length; i++)
        {
            string mark = string.Equals(ord[i], cul[i], StringComparison.Ordinal) ? "" : " ⟵ різниця";
            Report.Line($"| {i + 1} | {ord[i]} | {cul[i]}{mark} |");
        }
        Report.Line();
        Report.Line(differ
            ? "Порядок РІЗНИЙ: ординально ґ (U+0491) стоїть за кодом після всієї кирилиці, культурно — одразу після г."
            : "УВАГА: порядок однаковий. Імовірно, вимкнено ICU (InvariantGlobalization) — перевірте налаштування середовища.");
        Report.Line();
    }
}
