using System.Collections.Generic;
using UnityEngine;

public class Pool<T>
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    private readonly List<T> _actived = new();
    private readonly Queue<T> _pooled = new();

    public IReadOnlyList<T> Actived => _actived;

    public Pool(GameObject prefab, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public T Get()
    {
        T item;

        if (_pooled.Count > 0)
        {
            item = _pooled.Dequeue();
        }
        else
        {
            GameObject obj = Object.Instantiate(_prefab, _parent);

            if (typeof(T) == typeof(GameObject))
            {
                item = (T)(object)obj;
            }
            else
            {
                item = obj.GetComponent<T>();
            }
        }

        Activate(item);
        _actived.Add(item);

        return item;
    }

    public void ReturnAll()
    {
        foreach (var item in _actived)
        {
            Deactivate(item);
            _pooled.Enqueue(item);
        }
        _actived.Clear();
    }

    public void Return(T item)
    {
        if (_actived.Remove(item))
        {
            Deactivate(item);
            _pooled.Enqueue(item);
        }
    }

    private void Activate(T item)
    {
        if (item is GameObject go)
        {
            go.SetActive(true);
        }
        else if (item is Component comp)
        {
            comp.gameObject.SetActive(true);
        }
    }

    private void Deactivate(T item)
    {
        if (item is GameObject go)
        {
            go.SetActive(false);
        }
        else if (item is Component comp)
        {
            comp.gameObject.SetActive(false);
        }
    }
}
