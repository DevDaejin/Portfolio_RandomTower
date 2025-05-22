using Newtonsoft.Json;
using System;
using UnityEngine;

public abstract class Syncable<T> : ISyncable where T : Syncable<T>, new()
{
    private bool _isDirty = false;

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
    public static T FromJson(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
    public void SetDirty()
    {
        _isDirty = true;
    }
    public bool IsDirty()
    {
        return _isDirty;
    }
    public void ClearDirty()
    {
        _isDirty = false;
    }
    public void TrySync(Action<string> sendAction)
    {
        if(_isDirty)
        {
            string json = ToJson();
            sendAction?.Invoke(json);
            ClearDirty();
        }
    }
}
