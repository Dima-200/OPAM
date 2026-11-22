using System;
using System.Diagnostics;
using System.Text;

namespace Lab10;

static class Bench
{
    public static int Sink; // щоб JIT не викинув "марну" роботу

    // прогрівочний запуск + три заміри, береться найменший
    public static double Ms(Action a)
    {
        a();
        double best = double.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            var sw = Stopwatch.StartNew();
            a();
            sw.Stop();
            if (sw.Elapsed.TotalMilliseconds < best) best = sw.Elapsed.TotalMilliseconds;
        }
        return best;
    }
}

static class Report
{
    static readonly StringBuilder sb = new StringBuilder();
    public static void Line(string s = "")
    {
        Console.WriteLine(s);
        sb.AppendLine(s);
    }
    public static void Save(string path) => System.IO.File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
}
