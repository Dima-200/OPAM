using System;

namespace OpamLab09;

/// <summary>
/// Власні сортування. Усі — узагальнені за типом, з порівнянням, переданим параметром.
/// Порівняння рахується на кожен виклик cmp. Переставляння — це кожен запис елемента в масив
/// (обмін двох елементів = 2 записи; обмін елемента з самим собою не робиться і не рахується).
/// Тут немає Array.Sort, List.Sort, OrderBy, Reverse та LINQ.
/// Примітка: лічильники int; на n ≈ 100 000 вибір переповнить int (n²/2 > 2·10⁹) — це не впливає на
/// час, а таблиця лічильників будується лише для n ≤ 10 000.
/// </summary>
public static class Sorts
{
    // ======================= Завдання 1: сортування вибором =======================
    public static void SelectionSort<T>(T[] a, Func<T, T, int> cmp,
        out int comparisons, out int moves)
    {
        int c = 0, m = 0;
        int n = a.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int min = i;                          // індекс найменшого в решті a[i..n-1]
            for (int j = i + 1; j < n; j++)
            {
                c++;
                if (cmp(a[j], a[min]) < 0) min = j;
            }
            if (min != i)
            {
                T tmp = a[i];
                a[i] = a[min];                    // запис 1
                a[min] = tmp;                     // запис 2
                m += 2;
            }
        }
        comparisons = c;
        moves = m;
    }

    // ======================= Завдання 2: швидке сортування, опорний — медіана трьох =======================
    public static void QuickSort<T>(T[] a, Func<T, T, int> cmp,
        out int comparisons, out int moves)
    {
        int c = 0, m = 0;
        QuickRec(a, 0, a.Length - 1, cmp, ref c, ref m);
        comparisons = c;
        moves = m;
    }

    private static void QuickRec<T>(T[] a, int lo, int hi, Func<T, T, int> cmp,
        ref int c, ref int m)
    {
        while (lo < hi)
        {
            int j = HoarePartition(a, lo, hi, cmp, ref c, ref m);   // частини: [lo..j] і [j+1..hi]
            // у рекурсію йде менша частина, більша — циклом: глибина стека ≤ log2 n
            if (j - lo + 1 <= hi - j)
            {
                QuickRec(a, lo, j, cmp, ref c, ref m);
                lo = j + 1;
            }
            else
            {
                QuickRec(a, j + 1, hi, cmp, ref c, ref m);
                hi = j;
            }
        }
    }

    /// <summary>
    /// Розділення за Хоаром: два вказівники йдуть назустріч і зупиняються на елементах, рівних опорному.
    /// Завдяки цьому на однакових елементах ділянка ділиться навпіл (а не відщеплює по одному елементу).
    /// Опорне значення — медіана трьох (перший, середній, останній), його обирає окрема функція.
    /// </summary>
    private static int HoarePartition<T>(T[] a, int lo, int hi, Func<T, T, int> cmp,
        ref int c, ref int m)
    {
        int mid = lo + (hi - lo) / 2;
        int med = MedianOfThree(a, lo, mid, hi, cmp, ref c);
        T pivot = a[med];
        int i = lo - 1, j = hi + 1;
        while (true)
        {
            do { i++; c++; } while (cmp(a[i], pivot) < 0);
            do { j--; c++; } while (cmp(a[j], pivot) > 0);
            if (i >= j) return j;
            T tmp = a[i];
            a[i] = a[j];                          // запис 1
            a[j] = tmp;                           // запис 2
            m += 2;
        }
    }

    /// <summary>Індекс медіани з a[i], a[j], a[k]. Кожне порівняння тут теж рахується в c.</summary>
    private static int MedianOfThree<T>(T[] a, int i, int j, int k, Func<T, T, int> cmp, ref int c)
    {
        c++;
        if (cmp(a[i], a[j]) < 0)
        {
            c++;
            if (cmp(a[j], a[k]) < 0) return j;            // a[i] < a[j] < a[k]
            c++;
            return cmp(a[i], a[k]) < 0 ? k : i;           // a[k] <= a[j]: медіана — більший з a[i], a[k]
        }
        else
        {
            c++;
            if (cmp(a[i], a[k]) < 0) return i;            // a[j] <= a[i] < a[k]
            c++;
            return cmp(a[j], a[k]) < 0 ? k : j;           // a[k] <= a[i]: медіана — більший з a[j], a[k]
        }
    }

    // ---- Експеримент до завдання 2: «класична» схема Ломуто (елементи < опорного ліворуч, решта праворуч).
    // ---- Основним алгоритмом НЕ є. Потрібна, щоб власними лічильниками побачити виродження в n²/2
    // ---- на однакових елементах і дублікатах (див. README).
    public static void QuickSortLomuto<T>(T[] a, Func<T, T, int> cmp,
        out int comparisons, out int moves)
    {
        int c = 0, m = 0;
        QuickRecLomuto(a, 0, a.Length - 1, cmp, ref c, ref m);
        comparisons = c;
        moves = m;
    }

    private static void QuickRecLomuto<T>(T[] a, int lo, int hi, Func<T, T, int> cmp,
        ref int c, ref int m)
    {
        while (lo < hi)
        {
            int p = LomutoPartition(a, lo, hi, cmp, ref c, ref m);   // опорний опинився на місці p
            if (p - lo < hi - p)
            {
                QuickRecLomuto(a, lo, p - 1, cmp, ref c, ref m);
                lo = p + 1;
            }
            else
            {
                QuickRecLomuto(a, p + 1, hi, cmp, ref c, ref m);
                hi = p - 1;
            }
        }
    }

    private static int LomutoPartition<T>(T[] a, int lo, int hi, Func<T, T, int> cmp,
        ref int c, ref int m)
    {
        int mid = lo + (hi - lo) / 2;
        int med = MedianOfThree(a, lo, mid, hi, cmp, ref c);
        Swap(a, med, hi, ref m);                  // опорний — в кінець ділянки
        T pivot = a[hi];
        int store = lo;
        for (int j = lo; j < hi; j++)
        {
            c++;
            if (cmp(a[j], pivot) < 0)
            {
                Swap(a, store, j, ref m);
                store++;
            }
        }
        Swap(a, store, hi, ref m);
        return store;
    }

    private static void Swap<T>(T[] a, int i, int j, ref int m)
    {
        if (i == j) return;
        T tmp = a[i];
        a[i] = a[j];
        a[j] = tmp;
        m += 2;
    }

    // ======================= Завдання 3: сортування Шелла =======================
    /// <summary>Вставки з кроком, що зменшується (послідовність Кнута: ..., 40, 13, 4, 1).</summary>
    public static void ShellSort<T>(T[] a, Func<T, T, int> cmp,
        out int comparisons, out int moves)
    {
        int c = 0, m = 0;
        int n = a.Length;
        int gap = 1;
        while (gap < n / 3) gap = 3 * gap + 1;
        while (gap >= 1)
        {
            for (int i = gap; i < n; i++)
            {
                T tmp = a[i];
                int j = i;
                while (j >= gap)
                {
                    c++;
                    if (cmp(a[j - gap], tmp) > 0)
                    {
                        a[j] = a[j - gap];        // зсув більшого вперед на gap — один запис
                        m++;
                        j -= gap;
                    }
                    else break;
                }
                if (j != i)
                {
                    a[j] = tmp;                   // ставимо елемент на місце — один запис
                    m++;
                }
            }
            gap /= 3;
        }
        comparisons = c;
        moves = m;
    }

    /// <summary>
    /// Прості вставки — довідковий алгоритм для порівняння з Шеллом (вимога c = 3 у завданні 3).
    /// Те саме, що Шелл з єдиним кроком 1.
    /// </summary>
    public static void InsertionSort<T>(T[] a, Func<T, T, int> cmp,
        out int comparisons, out int moves)
    {
        int c = 0, m = 0;
        for (int i = 1; i < a.Length; i++)
        {
            T tmp = a[i];
            int j = i;
            while (j >= 1)
            {
                c++;
                if (cmp(a[j - 1], tmp) > 0)
                {
                    a[j] = a[j - 1];
                    m++;
                    j--;
                }
                else break;
            }
            if (j != i)
            {
                a[j] = tmp;
                m++;
            }
        }
        comparisons = c;
        moves = m;
    }
}
