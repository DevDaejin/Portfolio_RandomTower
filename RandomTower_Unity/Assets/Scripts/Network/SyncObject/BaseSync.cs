using Newtonsoft.Json;
using UnityEngine;

public abstract class BaseSync<TData> : MonoBehaviour, ISyncable
{
    protected TData _syncedData;

    public abstract string SyncType { get; }
    protected abstract TData GetCurrentData();
    protected abstract void ApplyData(TData data);
    protected abstract bool Equals(TData a, TData b);

    public string Serialize()
    {
        return JsonUtility.SerializeObject(GetCurrentData());
    }

    public void Deserialize(string json)
    {
        TData data = JsonUtility.DeserializeObject<TData>(json);
        ApplyData(data);
    }

    public bool IsDirty()
    {
        return !Equals(GetCurrentData(), _syncedData);
    }

    public void ClearDirty()
    {
        _syncedData = GetCurrentData();
    }
}
