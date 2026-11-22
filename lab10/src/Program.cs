using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Lab10;

static class Program
{
    static string? FindProse(out string baseDir)
    {
        baseDir = Directory.GetCurrentDirectory();
        string[] starts = { AppContext.BaseDirectory, Directory.GetCurrentDirectory() };
        foreach (string s in starts)
        {
            string? dir = s;
            for (int up = 0; up < 8 && dir != null; up++)
            {
                string cand = Path.Combine(dir, "data", "prose.txt");
                if (File.Exists(cand)) { baseDir = dir; return cand; }
                dir = Path.GetDirectoryName(dir);
            }
        }
        return null;
    }

    static int Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture; // лише для крапки в числах

        string? path = FindProse(out string baseDir);
        if (path == null)
        {
            Console.WriteLine("Не знайдено data/prose.txt. Покладіть український текст (>= 50 000 символів) у data/prose.txt.");
            return 1;
        }
        string prose = File.ReadAllText(path, Encoding.UTF8).Replace("\r", "");
        if (prose.Length < 50_000)
        {
            Console.WriteLine($"У prose.txt лише {prose.Length} символів, потрібно щонайменше 50 000.");
            return 1;
        }

        Report.Line("# Результати вимірювань");
        Report.Line();
        Report.Line("N = 3, K = 6, a = 0, b = 3, c = 3");
        Report.Line();
#if DEBUG
        Report.Line("**УВАГА: збірка Debug — заміри недійсні. Запускайте: dotnet run -c Release**");
        Report.Line();
#endif
        Report.Line($"Проза: {prose.Length} символів. Кожен замір: прогрів + 3 запуски, береться найменший.");
        Report.Line();

        Traps.Run(prose);
        LongestSubstring.Run(prose);
        SubstringSearch.Run(prose);
        RodCutting.Run();
        ExtraBench.Run();
        EdgeCases.Run();

        string outPath = Path.Combine(baseDir, "results.md");
        Report.Save(outPath);
        Console.WriteLine($"Збережено: {outPath}");
        return 0;
    }
}
