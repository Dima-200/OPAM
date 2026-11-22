using System;

namespace OpamLab09;

public static class Program
{
    // Запуск (обов'язково Release!):  dotnet run -c Release            — усе
    //                                 dotnet run -c Release -- tables  — лише лічильники
    //                                 dotnet run -c Release -- time    — час і межа прийнятності
    public static void Main(string[] args)
    {
#if DEBUG
        Console.WriteLine("УВАГА: Debug-збірка. Для замірів часу запускайте: dotnet run -c Release (інакше −1 бал).");
#endif
        string what = args.Length > 0 ? args[0] : "all";
        Console.WriteLine("N = 3, K = 6, a = 0, b = 3, c = 3");

        if (what is "all" or "trace") Experiments.Trace();
        if (what is "all" or "edge") Experiments.EdgeCases();
        if (what is "all" or "tables")
        {
            Experiments.CounterTable("Вибір (зав. 1)", Sorts.SelectionSort);
            Experiments.CounterTable("Швидке, медіана трьох (зав. 2)", Sorts.QuickSort);
            Experiments.CounterTable("Шелл (зав. 3)", Sorts.ShellSort);
            Experiments.CounterTable("Прості вставки (довідково для зав. 3)", Sorts.InsertionSort);
            Experiments.AllEqualExperiment();
        }
        if (what is "all" or "stability") Experiments.Stability();
        if (what is "all" or "time")
        {
            Experiments.TimeTable();
            Experiments.AcceptabilityLimit();
        }
    }
}
