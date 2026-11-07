using System;

public static class Search
{
    // Завдання 1. Лінійний пошук
    public static int LinearSearch<T>(
        T[] a, T target,
        Func<T, T, int> cmp,
        out int comparisons)
    {
        comparisons = 0;

        for (int i = 0; i < a.Length; i++)
        {
            comparisons++;

            if (cmp(a[i], target) == 0)
                return i;
        }

        return -1;
    }

    // Завдання 2. Правильний бінарний пошук
    public static int BinarySearch<T>(
        T[] a, T target,
        Func<T, T, int> cmp,
        out int comparisons)
    {
        comparisons = 0;
        int lo = 0;
        int hi = a.Length - 1;

        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            comparisons++;

            int result = cmp(a[mid], target);

            if (result == 0)
                return mid;

            if (result < 0)
                lo = mid + 1;
            else
                hi = mid - 1;
        }

        return -1;
    }

    // Завдання 3. Один узагальнений метод пошуку межі
    public static int BoundarySearch<T>(
        T[] a,
        Func<T, bool> condition,
        out int comparisons)
    {
        comparisons = 0;
        int lo = 0;
        int hi = a.Length;

        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            comparisons++;

            if (condition(a[mid]))
                hi = mid;
            else
                lo = mid + 1;
        }

        return lo;
    }

    // Завдання 4. Експоненційний пошук
    public static int ExponentialSearch<T>(
        T[] a, T target,
        Func<T, T, int> cmp,
        out int comparisons)
    {
        comparisons = 0;

        if (a.Length == 0)
            return -1;

        comparisons++;

        if (cmp(a[0], target) == 0)
            return 0;

        int bound = 1;

        while (bound < a.Length)
        {
            comparisons++;

            if (cmp(a[bound], target) >= 0)
                break;

            bound *= 2;
        }

        int lo = bound / 2;
        int hi = Math.Min(bound, a.Length - 1);

        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            comparisons++;

            int result = cmp(a[mid], target);

            if (result == 0)
                return mid;

            if (result < 0)
                lo = mid + 1;
            else
                hi = mid - 1;
        }

        return -1;
    }
}