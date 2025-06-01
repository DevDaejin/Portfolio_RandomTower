using System;
using System.Collections.Generic;
using UnityEngine;

public class GenericPool<T> where T : new()
{
    private readonly Stack<T> _pooled = new();
    private readonly List<T> _actived = new();

    public T Get()
    {
        T instance = _pooled.Count > 0 ? _pooled.Pop() : new();
        _actived.Add(instance);

        return instance;
    }

    public void Release(T Target)
    {
        _actived.Remove(Target);
        _pooled.Push(Target);
    }

    public void ReleaseAll()
    {
        foreach(T t in _actived)
        {
            Release(t);
        }
    }
}
