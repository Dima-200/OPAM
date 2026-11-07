using System;

public static class Broken
{
    public static int MissingLast<T>(
        T[] a, T target,
        Func<T, T, int> cmp,
        out int comparisons)
    {
        comparisons = 0;
        int lo = 0;
        int hi = a.Length - 1;

        while (lo < hi)
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

    public static int NoProgress<T>(
        T[] a, T target,
        Func<T, T, int> cmp,
        out int comparisons,
        out int iterations,
        int maxIterations = 20)
    {
        comparisons = 0;
        iterations = 0;

        int lo = 0;
        int hi = a.Length - 1;

        while (lo <= hi && iterations < maxIterations)
        {
            iterations++;

            int mid = lo + (hi - lo) / 2;
            comparisons++;

            int result = cmp(a[mid], target);

            if (result == 0)
                return mid;

            if (result < 0)
                lo = mid;
            else
                hi = mid - 1;
        }

        return -1;
    }

    public static int OverflowMid<T>(
        T[] a, T target,
        Func<T, T, int> cmp,
        out int comparisons)
    {
        comparisons = 0;
        int lo = 0;
        int hi = a.Length - 1;

        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
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

    public static int UnsortedSearch<T>(
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
}