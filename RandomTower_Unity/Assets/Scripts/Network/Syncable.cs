using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public abstract class Syncable<T> : ISyncable
{
    protected readonly T Target;
    private Action<ISyncable> _onDirtyCallback;

    public bool IsDirty => _isDirty;
    private bool _isDirty;

    public int ID => _id;
    private readonly int _id;

    protected Syncable(T target, int id)
    {
        Target = target;
        _id = id;
    }
    public void SetDirty()
    {
        _isDirty = true;
    }

    public void SetOnDirtyCallback(Action<ISyncable> callback)
    {
        _onDirtyCallback = callback;
    }

    protected void ForceDirty()
    {
        _isDirty = true;
        _onDirtyCallback?.Invoke(this);
    }

    public virtual void Initialize()
    {
        NetworkManager.Instance.SyncController.Register(this);
    }
    public abstract void Deserialize(BinaryReader reader);
    public abstract void Serialize(BinaryWriter writer);
}
