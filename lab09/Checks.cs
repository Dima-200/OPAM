using System;
using System.Collections.Generic;

namespace OpamLab09;

/// <summary>Перевірки результату сортування. Обидві мають пройти на кожному запуску.</summary>
public static class Checks
{
    /// <summary>Чи впорядкований масив: жодна пара сусідів не стоїть у зворотному порядку.</summary>
    public static bool IsSorted<T>(T[] a, Func<T, T, int> cmp)
    {
        for (int i = 1; i < a.Length; i++)
            if (cmp(a[i - 1], a[i]) > 0) return false;
        return true;
    }

    /// <summary>
    /// Чи є result перестановкою original: ті самі елементи з тією самою кількістю повторів.
    /// Ловить загублені елементи (наприклад, забуті хвости в сортуванні злиттям), яких IsSorted не бачить.
    /// Для int — порівняння за значенням, для класів (Student) — за посиланням: сортування лише переставляє
    /// ті самі об'єкти.
    /// </summary>
    public static bool IsPermutationOf<T>(T[] result, T[] original) where T : notnull
    {
        if (result.Length != original.Length) return false;

        var counts = new Dictionary<T, int>();
        foreach (T x in original)
        {
            counts[x] = counts.TryGetValue(x, out int have) ? have + 1 : 1;
        }
        foreach (T x in result)
        {
            if (!counts.TryGetValue(x, out int left) || left == 0) return false;
            counts[x] = left - 1;
        }
        return true;
    }
}
