using System;

namespace OpamLab05;

public class IntStack
{
    private int[] _items;
    private int _count;

    public IntStack(int capacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Місткість має бути не менше 1.");
        }

        _items = new int[capacity];
        _count = 0;
    }

    public int Count => _count;
    public int Capacity => _items.Length;

    public void Push(int x)
    {
        if (_count == _items.Length)
        {
            Grow();
        }

        _items[_count] = x;
        _count++;
    }

    public int Pop()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("Стек порожній: Pop неможливий.");
        }

        _count--;
        return _items[_count];
    }

    public int Peek()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("Стек порожній: Peek неможливий.");
        }

        return _items[_count - 1];
    }

    private void Grow()
    {
        int[] bigger = new int[_items.Length * 2];
        for (int i = 0; i < _count; i++)
        {
            bigger[i] = _items[i];
        }

        _items = bigger;
    }
}
