using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Lab10;

// Замір із таблиці C (c = 3): Split усього файлу проти читання по рядку StreamReader
static class ExtraBench
{
    public static void Run()
    {
        string path = Path.Combine(Path.GetTempPath(), "lab10_100k_lines.txt");
        var sb = new StringBuilder();
        for (int i = 0; i < 100_000; i++)
            sb.Append("рядок ").Append(i).Append(": якийсь текст для виміру швидкості читання файлу\n");
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        sb = null!;
        GC.Collect();

        long fileBytes = new FileInfo(path).Length;

        // 1) по рядку (першим, щоб PeakWorkingSet відображав саме цей варіант)
        long alloc1 = 0;
        double t1 = Bench.Ms(() =>
        {
            long before = GC.GetAllocatedBytesForCurrentThread();
            long total = 0;
            using var sr = new StreamReader(path, Encoding.UTF8);
            string? line;
            while ((line = sr.ReadLine()) != null) total += line.Length;
            Bench.Sink += (int)total;
            alloc1 = GC.GetAllocatedBytesForCurrentThread() - before;
        });
        var p = Process.GetCurrentProcess();
        p.Refresh();
        long peak1 = p.PeakWorkingSet64;

        // 2) Split усього файлу
        long alloc2 = 0;
        string[]? keep = null;
        double t2 = Bench.Ms(() =>
        {
            long before = GC.GetAllocatedBytesForCurrentThread();
            string all = File.ReadAllText(path, Encoding.UTF8);
            string[] lines = all.Split('\n');
            long total = 0;
            for (int i = 0; i < lines.Length; i++) total += lines[i].Length;
            Bench.Sink += (int)total;
            keep = lines;
            alloc2 = GC.GetAllocatedBytesForCurrentThread() - before;
        });
        p.Refresh();
        long peak2 = p.PeakWorkingSet64;
        Bench.Sink += keep!.Length;

        File.Delete(path);

        Report.Line("## Замір C (c = 3): Split усього файлу проти StreamReader");
        Report.Line();
        Report.Line($"Файл: 100 000 рядків, {fileBytes / 1024} КБ.");
        Report.Line();
        Report.Line("| Спосіб | Час, мс | Виділено за прохід, МБ | Піковий working set процесу після способу, МБ |");
        Report.Line("|---|---|---|---|");
        Report.Line($"| StreamReader.ReadLine | {t1:F2} | {alloc1 / 1048576.0:F1} | {peak1 / 1048576.0:F1} |");
        Report.Line($"| ReadAllText + Split | {t2:F2} | {alloc2 / 1048576.0:F1} | {peak2 / 1048576.0:F1} |");
        Report.Line();
        Report.Line("Примітка: PeakWorkingSet64 лише зростає, тому по рядку міряється першим; ріст піку на другому рядку — це вартість тримати весь файл і масив рядків в пам'яті.");
        Report.Line();
    }
}
