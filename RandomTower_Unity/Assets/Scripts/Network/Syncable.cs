//using Newtonsoft.Json;
//using System;
//using System.IO;
//using UnityEngine;

//public abstract class Syncable<T> : ISyncable
//{
//    protected readonly T Target;
//    private Action<ISyncable> _onDirtyCallback;

//    public bool IsDirty => _isDirty;
//    private bool _isDirty;

//    public int ID => _id;
//    private int _id = -1;
//    private bool _isAssigned = false;

//    protected Syncable(T target)
//    {
//        Target = target;
//    }

//    public void AssignID(int id)
//    {
//        if (_isAssigned)
//        {
//#if UNITY_EDITOR
//            Debug.LogError($"ID already assigned to {id}");
//#endif
//            return;
//        }

//        _id = id;
//        _isAssigned = true;
//    }

//    public void SetDirty()
//    {
//        _isDirty = true;
//    }

//    public void SetOnDirtyCallback(Action<ISyncable> callback)
//    {
//        _onDirtyCallback = callback;
//    }

//    public virtual void Initialize()
//    {
//        NetworkManager.Instance.SyncController.Register(this);
//    }
//    public virtual void Deserialize(BinaryReader reader)
//    {
//        if (reader.ReadInt32() != ID) return;
//    }
//    public virtual void Serialize(BinaryWriter writer)
//    {
//        writer.Write(ID);
//    }

//    public abstract bool CheckDirty();
//}
