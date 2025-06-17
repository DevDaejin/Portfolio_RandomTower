using Google.Protobuf;
using UnityEngine;

public abstract class BaseSync<TData> : MonoBehaviour, ISyncable where TData : IMessage<TData>, new()
{
    protected TData _currentData;
    protected TData _receivedData;
    protected TData _temp;

    public abstract string SyncType { get; }
    public string ObjectID { get; private set; }

    protected abstract void FillData(TData target);
    protected abstract void ApplyData(TData data);
    protected abstract bool Equals(TData a, TData b);

    protected virtual void Awake()
    {
        _currentData = new TData();
        _receivedData = new TData();
        _temp = new TData();
    }

    protected virtual void Start()
    {
        ObjectID = GetComponent<SyncObject>().ObjectID;
        FillData(_currentData);
    }

    public IMessage Serialize()
    {
        FillData(_temp);
        return _temp;
    }

    public void Deserialize(ByteString payload)
    {
        _temp.MergeFrom(payload);
        if (Equals(_currentData, _temp)) return;

        _receivedData.MergeFrom(_temp);
        ApplyData(_receivedData);
    }

    public bool IsDirty()
    {
        FillData(_temp);
        return !Equals(_currentData, _temp);
    }

    public void ClearDirty()
    {
        FillData(_temp);
        _currentData.MergeFrom(_temp);
    }
}
