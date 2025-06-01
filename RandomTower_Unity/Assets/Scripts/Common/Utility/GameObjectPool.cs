using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : Component
{
    private readonly Stack<T> _pooled = new();
    private readonly List<T> _actived = new();
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    public GameObjectPool(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public T Get()
    {
        T instance = _pooled.Count > 0 ? _pooled.Pop() : UnityEngine.Object.Instantiate(_prefab).GetComponent<T>();
        instance.transform.SetParent(_parent);
        instance.gameObject.SetActive(true);
  
        _actived.Add(instance);
        return instance;
    }

    public void Release(T target)
    {
        target.gameObject.SetActive(false);
        
        _actived.Remove(target);
        _pooled.Push(target);
    }

    public void ReleaseAll()
    {
        for (int i = _actived.Count - 1; i >= 0; i--)
        {
            Release(_actived[i]);
        }
    }

    public void Clear()
    {
        while(_pooled.Count > 0)
        {
            UnityEngine.Object.Destroy(_pooled.Pop().gameObject);
        }

        _pooled.Clear();
    }

    public int CountActived()
    {
        int count = 0;

        foreach(T element in _pooled)
        {
            if(element.gameObject.activeInHierarchy)
            {
                count++;
            }
        }

        return count;
    }
}
