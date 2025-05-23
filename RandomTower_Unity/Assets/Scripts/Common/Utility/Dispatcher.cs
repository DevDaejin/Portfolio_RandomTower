using System;
using System.Collections.Concurrent;
using UnityEngine;

public class Dispatcher : MonoBehaviour, IDispatcher
{
    private readonly ConcurrentQueue<Action> _queue = new();
    public void Enqueue(Action action)
    {
        if (action == null) return;
        _queue.Enqueue(action);
    }

    private void Update()
    {
        while (_queue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}
