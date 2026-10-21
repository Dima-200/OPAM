using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
 
namespace OpamLab05;
 
internal static class Program
{
    private const int N = 3;
    private const int K = 6;
 
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("N = 3, K = 6, a = 0, b = 3, c = 3");
 
        RunTask1();
        RunTask2();
        RunTask3();
        RunTask4();
        RunEdgeCases();
    }
 
    private static bool IsOpen(char c) => c == '(' || c == '[' || c == '{';
 
    private static bool IsClose(char c) => c == ')' || c == ']' || c == '}';
 
    private static bool Matches(char open, char close) =>
        (open == '(' && close == ')') ||
        (open == '[' && close == ']') ||
        (open == '{' && close == '}');
 
    /// <summary>
    /// </summary>
    private static (bool Ok, int ErrorIndex) CheckBrackets(string s)
    {
        var stack = new Stack<(char Ch, int Index)>();
 
        for (int i = 0; i < s.Length; i++)
        {
            char ch = s[i];
 
            if (IsOpen(ch))
            {
                stack.Push((ch, i));
            }
            else if (IsClose(ch))
            {
                if (stack.Count == 0)
                {
                    return (false, i);
                }
 
                var top = stack.Pop();
                if (!Matches(top.Ch, ch))
                {
                    return (false, i);
                }
            }
        }
 
        if (stack.Count > 0)
        {
            int first = int.MaxValue;
            foreach (var item in stack)
            {
                if (item.Index < first)
                {
                    first = item.Index;
                }
            }
 
            return (false, first);
        }
 
        return (true, -1);
    }
 
    private static void PrintBrackets(string s)
    {
        var (ok, index) = CheckBrackets(s);
        string verdict = ok ? "збалансований" : "незбалансований";
        Console.WriteLine($"  \"{s}\" -> {verdict}, індекс помилки: {index}");
    }
 
    private static void RunTask1()
    {
        Console.WriteLine();
        Console.WriteLine("=== Завдання 1. Дужки ===");
 
        string[] tests = { "(a[b]{c})", "([)]", ")(", "((", "" };
        foreach (string test in tests)
        {
            PrintBrackets(test);
        }
 
        string special = new string('(', K) + new string(')', K - 1);
        PrintBrackets(special);
    }
 
    private static bool IsOperator(string token) =>
        token == "+" || token == "-" || token == "*" || token == "/" ||
        token == "min" || token == "max";
 
    private static bool TryEvalRpn(string expression, out double result, out string error)
    {
        result = 0;
        error = "";
 
        var stack = new Stack<double>();
        string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
 
        if (tokens.Length == 0)
        {
            error = "порожній вираз";
            return false;
        }
 
        foreach (string token in tokens)
        {
            if (IsOperator(token))
            {
                if (stack.Count < 2)
                {
                    error = $"не вистачає чисел для операції '{token}'";
                    return false;
                }
 
                double right = stack.Pop();
                double left = stack.Pop();
 
                switch (token)
                {
                    case "+":
                        stack.Push(left + right);
                        break;
                    case "-":
                        stack.Push(left - right);
                        break;
                    case "*":
                        stack.Push(left * right);
                        break;
                    case "/":
                        if (right == 0)
                        {
                            error = "ділення на нуль";
                            return false;
                        }
 
                        stack.Push(left / right);
                        break;
                    case "min":
                        stack.Push(Math.Min(left, right));
                        break;
                    case "max":
                        stack.Push(Math.Max(left, right));
                        break;
                }
            }
            else if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            {
                stack.Push(number);
            }
            else
            {
                error = $"невідомий токен '{token}'";
                return false;
            }
        }
 
        if (stack.Count != 1)
        {
            error = "забагато чисел: бракує операцій";
            return false;
        }
 
        result = stack.Pop();
        return true;
    }
 
    private static void PrintRpn(string expression)
    {
        if (TryEvalRpn(expression, out double result, out string error))
        {
            Console.WriteLine($"  \"{expression}\" -> {result.ToString(CultureInfo.InvariantCulture)}");
        }
        else
        {
            Console.WriteLine($"  \"{expression}\" -> помилка: {error}");
        }
    }
 
    private static void RunTask2()
    {
        Console.WriteLine();
        Console.WriteLine("=== Завдання 2. Зворотний польський запис ===");
 
        string[] tests = { "3 4 + 2 *", "5 3 -", "8 2 /", "2 3 4 * +", "5 0 /" };
        foreach (string test in tests)
        {
            PrintRpn(test);
        }
 
        Console.WriteLine("  Власний вираз (результат має бути K = 6):");
        PrintRpn("8 2 / 5 max 1 +");
 
        Console.WriteLine("  Некоректний вхід:");
        PrintRpn("3 abc +");
        PrintRpn("1 2");
        PrintRpn("1 +");
    }
    private static void SimulateService(int count, int seed)
    {
        if (count <= 0)
        {
            Console.WriteLine("  Заявок немає, моделювати нічого.");
            return;
        }
 
        var random = new Random(seed);
        var queue = new Queue<(int Id, int Weight)>();
        int maxQueueLength = 0;
 
        for (int id = 1; id <= count; id++)
        {
            queue.Enqueue((id, random.Next(1, 10)));
            if (queue.Count > maxQueueLength)
            {
                maxQueueLength = queue.Count;
            }
        }
 
        int[] freeAt = new int[2]; 
 
        while (queue.Count > 0)
        {
            int server = freeAt[0] <= freeAt[1] ? 0 : 1;
 
            var job = queue.Dequeue();
            int start = freeAt[server];
            int end = start + job.Weight; 
            freeAt[server] = end;
 
            Console.WriteLine(
                $"  Заявка {job.Id}: вага {job.Weight}, обслуговувач {server + 1}, початок {start} хв, завершення {end} хв");
        }
 
        Console.WriteLine($"  Максимальна довжина черги: {maxQueueLength}");
    }
 
    private static void RunTask3()
    {
        Console.WriteLine();
        Console.WriteLine("=== Завдання 3. Модель обслуговування ===");
 
        int count = 8 + K % 5;
        Console.WriteLine($"  Кількість заявок: {count}");
        SimulateService(count, N);
    }
 
    private static (bool Ok, int ErrorIndex) CheckBracketsOnIntStack(string s)
    {
        var chars = new IntStack(K % 4 + 2);
        var indices = new IntStack(K % 4 + 2);
 
        for (int i = 0; i < s.Length; i++)
        {
            char ch = s[i];
 
            if (IsOpen(ch))
            {
                chars.Push(ch);
                indices.Push(i);
            }
            else if (IsClose(ch))
            {
                if (chars.Count == 0)
                {
                    return (false, i);
                }
 
                char open = (char)chars.Pop();
                indices.Pop();
                if (!Matches(open, ch))
                {
                    return (false, i);
                }
            }
        }
 
        if (chars.Count > 0)
        {
            int first = -1;
            while (indices.Count > 0)
            {
                first = indices.Pop();
            }
 
            return (false, first);
        }
 
        return (true, -1);
    }
 
    private static void RunTask4()
    {
        Console.WriteLine();
        Console.WriteLine("=== Завдання 4. Власний стек ===");
 
        int capacity = K % 4 + 2; // 4
        int pushes = K % 4 + 3;   // 5
        var stack = new IntStack(capacity);
        Console.WriteLine($"  Початкова місткість: {stack.Capacity}");
 
        for (int i = 1; i <= pushes; i++)
        {
            stack.Push(i * 10);
            Console.WriteLine($"  Push({i * 10}): Count = {stack.Count}, Capacity = {stack.Capacity}");
        }
 
        Console.WriteLine($"  Peek = {stack.Peek()}, Count після Peek = {stack.Count}");
 
        Console.WriteLine("  Завдання 1 на власному стеку (порівняння зі стандартним):");
        string[] tests = { "(a[b]{c})", "([)]", ")(", "((", "", new string('(', K) + new string(')', K - 1) };
        foreach (string test in tests)
        {
            var standard = CheckBrackets(test);
            var custom = CheckBracketsOnIntStack(test);
            string same = standard == custom ? "збігається" : "НЕ ЗБІГАЄТЬСЯ";
            Console.WriteLine($"  \"{test}\": стандартний {standard}, власний {custom} -> {same}");
        }
    }
 
   
    private static void RunEdgeCases()
    {
        Console.WriteLine();
        Console.WriteLine("=== Крайові випадки ===");
 
        Console.WriteLine("  Завдання 1:");
        PrintBrackets("");
        PrintBrackets("abc");
 
        Console.WriteLine("  Завдання 2:");
        PrintRpn("");
        PrintRpn("7");
        PrintRpn("5 0 /");
 
        Console.WriteLine("  Завдання 3:");
        SimulateService(0, N);
 
        Console.WriteLine("  Завдання 4:");
        try
        {
            new IntStack(K % 4 + 2).Pop();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  Pop на порожньому стеку -> виняток перехоплено: {ex.Message}");
        }
    }
}