using System;
using System.Collections.Generic;

public class ResourceManager
{
    public enum ResourceType { Gem, Gold }
    private Dictionary<ResourceType, Resource> _resourceDict = new();

    public void Initialize(params KeyValuePair<ResourceType, int>[] pairs)
    {
        foreach (var pair in pairs)
        {
            _resourceDict[pair.Key] = new(pair.Value);
        }
    }

    public void SetCallback(ResourceType type, Action<int> callback)
    {
        _resourceDict[type].ValueChanged -= callback;
        _resourceDict[type].ValueChanged += callback;
    }

    public int Get(ResourceType type) => _resourceDict[type].Get();
    public void Earn(ResourceType type, int amount) => _resourceDict[type].Earn(amount);
    public bool Spend(ResourceType type, int amount) => _resourceDict[type].Spend(amount);
}
