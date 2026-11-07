using System;

class Program
{
    static void Main()
    {
        const int N = 3;
        const int K = 6;
        const int X = 60;
        const int Y = 72;
        const int Missing = 121;

        Func<int, int, int> cmp = (x, y) => x.CompareTo(y);

        Random random = new Random(N);
        int[] data = new int[100000];

        for (int i = 0; i < data.Length; i++)
            data[i] = random.Next(0, 20 * K + 1);

        Array.Sort(data, (x, y) => cmp(x, y));

        Console.WriteLine("ЛАБОРАТОРНА РОБОТА №8");
        Console.WriteLine("Алгоритми пошуку та їх порівняння");
        Console.WriteLine($"N={N}, K={K}, a=0, b=3, c=3");
        Console.WriteLine($"Розмір масиву: {data.Length}");
        Console.WriteLine($"X={X}, Y={Y}, відсутнє значення={Missing}");

        Task1(data, cmp, X, Missing);
        Task2(data, cmp);
        Task3(data, cmp, X, Y);
        Task4(data, cmp, X, Missing);
        EdgeCases(cmp);

        Measurements.ComparisonTable();
        Measurements.TimeTable();

        Console.WriteLine("\nВиконання програми завершено.");
    }

    static void Task1(
        int[] data, Func<int, int, int> cmp,
        int x, int missing)
    {
        Console.WriteLine("\nЗАВДАННЯ 1. ЛІНІЙНИЙ ПОШУК");

        int first = data[0];
        int[] queries = { x, first, missing };

        foreach (int target in queries)
        {
            int index = Search.LinearSearch(
                data, target, cmp, out int comparisons);

            Console.WriteLine(
                $"Значення {target}: індекс={index}, порівнянь={comparisons}");
        }
    }

    static void Task2(int[] data, Func<int, int, int> cmp)
    {
        Console.WriteLine("\nЗАВДАННЯ 2. БІНАРНИЙ ПОШУК");

        int[] queries = { 60, data[0], 121 };

        foreach (int target in queries)
        {
            int index = Search.BinarySearch(
                data, target, cmp, out int comparisons);

            Console.WriteLine(
                $"Значення {target}: індекс={index}, порівнянь={comparisons}");
        }

        Console.WriteLine("\nПОЛОМКА 1: while (lo < hi)");

        int[] small = { 0, 1 };

        int brokenIndex = Broken.MissingLast(
            small, 1, cmp, out int brokenComparisons);

        Console.WriteLine(
            $"Масив [0, 1], шукаємо 1: індекс={brokenIndex}, " +
            $"порівнянь={brokenComparisons}. Очікується індекс 1.");

        Console.WriteLine("\nПОЛОМКА 2: lo = mid");

        int noProgressIndex = Broken.NoProgress(
            small, 1, cmp,
            out int noProgressComparisons,
            out int iterations,
            10);

        Console.WriteLine(
            $"Масив [0, 1], шукаємо 1: індекс={noProgressIndex}, " +
            $"порівнянь={noProgressComparisons}, ітерацій={iterations}.");

        Console.WriteLine(
            "Цикл зупинено обмеженням у 10 ітерацій. " +
            "Межа lo не змінюється.");

        Console.WriteLine("\nПОЛОМКА 3: mid = (lo + hi) / 2");

        long firstOverflowLength = 1073741825L;

        Console.WriteLine(
            $"Потенційне переповнення можливе при довжині масиву " +
            $"від {firstOverflowLength:N0} елементів.");

        Console.WriteLine(
            "Причина: сума lo + hi може перевищити int.MaxValue.");

        int overflowIndex = Broken.OverflowMid(
            data, 60, cmp, out int overflowComparisons);

        Console.WriteLine(
            $"На масиві з {data.Length} елементів: " +
            $"індекс={overflowIndex}, порівнянь={overflowComparisons}. " +
            "Помилка не проявляється.");

        Console.WriteLine("\nПОЛОМКА 4: невідсортований масив");

        int[] unsorted = (int[])data.Clone();
        Random random = new Random(123);

        for (int i = unsorted.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            int temp = unsorted[i];
            unsorted[i] = unsorted[j];
            unsorted[j] = temp;
        }

        int failed = 0;

        for (int i = 0; i < 100; i++)
        {
            int target = unsorted[random.Next(unsorted.Length)];

            int index = Broken.UnsortedSearch(
                unsorted, target, cmp, out _);

            if (index == -1)
                failed++;
        }

        Console.WriteLine(
            $"Із 100 запитів наявних значень не знайдено: {failed}");
    }

    static void Task3(
        int[] data, Func<int, int, int> cmp,
        int x, int y)
    {
        Console.WriteLine("\nЗАВДАННЯ 3. ПОШУК МЕЖІ");

        int lower = Search.BoundarySearch(
            data, value => value >= x,
            out int lowerComparisons);

        int upper = Search.BoundarySearch(
            data, value => value > x,
            out int upperComparisons);

        int countX = upper - lower;

        Console.WriteLine(
            $"LowerBound({x}) = {lower}, порівнянь={lowerComparisons}");

        Console.WriteLine(
            $"UpperBound({x}) = {upper}, порівнянь={upperComparisons}");

        Console.WriteLine($"Кількість елементів, рівних {x}: {countX}");

        int left = Search.BoundarySearch(
            data, value => value >= x, out int leftComparisons);

        int right = Search.BoundarySearch(
            data, value => value > y, out int rightComparisons);

        int rangeCount = right - left;

        Console.WriteLine(
            $"Кількість елементів у [{x}; {y}] = {rangeCount}");

        Console.WriteLine(
            $"Порівнянь пошуку меж: {leftComparisons + rightComparisons}");

        int linearCount = 0;
        int linearComparisons = 0;

        foreach (int value in data)
        {
            linearComparisons++;

            if (cmp(value, x) >= 0)
            {
                linearComparisons++;

                if (cmp(value, y) <= 0)
                    linearCount++;
            }
        }

        Console.WriteLine(
            $"Лінійний підрахунок діапазону: {linearCount}, " +
            $"порівнянь={linearComparisons}");

        Console.WriteLine("\nПеревірка неузгодженої умови: кратність 7");

        int invalidBoundary = Search.BoundarySearch(
            data, value => value % 7 == 0,
            out int invalidComparisons);

        int actualFirst = -1;

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] % 7 == 0)
            {
                actualFirst = i;
                break;
            }
        }

        bool correct = invalidBoundary == actualFirst;

        Console.WriteLine(
            $"BoundarySearch повернув індекс {invalidBoundary}, " +
            $"порівнянь={invalidComparisons}");

        Console.WriteLine(
            $"Справжній перший індекс кратного 7: {actualFirst}");

        Console.WriteLine(
            $"Результат правильний: {(correct ? "так" : "ні")}");
    }

    static void Task4(
        int[] data, Func<int, int, int> cmp,
        int x, int missing)
    {
        Console.WriteLine("\nЗАВДАННЯ 4. ЕКСПОНЕНЦІЙНИЙ ПОШУК");

        int[] queries = { x, data[0], missing };

        foreach (int target in queries)
        {
            int binaryIndex = Search.BinarySearch(
                data, target, cmp, out int binaryComparisons);

            int exponentialIndex = Search.ExponentialSearch(
                data, target, cmp, out int exponentialComparisons);

            Console.WriteLine(
                $"Значення {target}:");

            Console.WriteLine(
                $"  Бінарний: індекс={binaryIndex}, " +
                $"порівнянь={binaryComparisons}");

            Console.WriteLine(
                $"  Експоненційний: індекс={exponentialIndex}, " +
                $"порівнянь={exponentialComparisons}");
        }
    }

    static void EdgeCases(Func<int, int, int> cmp)
    {
        Console.WriteLine("\nПЕРЕВІРКА КРАЙОВИХ ВИПАДКІВ");

        int[] empty = { };
        int emptySearch = Search.BinarySearch(
            empty, 5, cmp, out int emptyComparisons);

        int emptyBoundary = Search.BoundarySearch(
            empty, value => value >= 5,
            out int emptyBoundaryComparisons);

        Console.WriteLine(
            $"Порожній масив: пошук={emptySearch}, " +
            $"межа={emptyBoundary}");

        int[] one = { 5 };
        int oneSearch = Search.BinarySearch(
            one, 5, cmp, out int oneComparisons);

        Console.WriteLine(
            $"Один елемент [5], шукаємо 5: індекс={oneSearch}, " +
            $"порівнянь={oneComparisons}");

        int[] two = { 1, 5 };
        int twoSearch = Search.BinarySearch(
            two, 5, cmp, out int twoComparisons);

        Console.WriteLine(
            $"Два елементи [1, 5], шукаємо 5: індекс={twoSearch}, " +
            $"порівнянь={twoComparisons}");

        int[] values = { 2, 4, 6, 8 };

        int below = Search.BoundarySearch(
            values, value => value >= 1, out int belowComparisons);

        int above = Search.BoundarySearch(
            values, value => value >= 10, out int aboveComparisons);

        Console.WriteLine(
            $"Межа для значення нижче всіх: {below} (очікується 0)");

        Console.WriteLine(
            $"Межа для значення вище всіх: {above} (очікується 4)");

        int[] equal = { 5, 5, 5, 5 };

        int equalLower = Search.BoundarySearch(
            equal, value => value >= 5, out int equalLowerComparisons);

        int equalUpper = Search.BoundarySearch(
            equal, value => value > 5, out int equalUpperComparisons);

        Console.WriteLine(
            $"Усі елементи рівні 5: перша межа={equalLower}, " +
            $"остання межа={equalUpper}");
    }
}