using Google.Protobuf;
using Newtonsoft.Json;
using UnityEngine;

public abstract class BaseSync<TData> : MonoBehaviour, ISyncable where TData : IMessage<TData>, new()
{
    protected TData _currentData;
    protected TData _receivedData;

    public abstract string SyncType { get; }
    public string ObjectID { get; private set; }
    
    protected abstract TData GetCurrentData();
    protected abstract void ApplyData(TData data);
    protected abstract bool Equals(TData a, TData b);

    protected virtual void Awake()
    {
        _currentData = new TData();
        _receivedData = new TData();
    }

    protected virtual void Start()
    {
        ObjectID = GetComponent<SyncObject>().ObjectID;
        _currentData.MergeFrom(GetCurrentData());
    }

    public IMessage Serialize()
    {
        return GetCurrentData();
    }

    public void Deserialize(ByteString payload)
    {
        _receivedData.MergeFrom(payload);
        ApplyData(_receivedData);
    }

    public bool IsDirty()
    {
        return !Equals(_currentData, _receivedData);
    }

    public void ClearDirty()
    {
        _currentData = GetCurrentData();
    }
}
