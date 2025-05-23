using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SyncView : MonoBehaviour, ISyncView
{
    public int SyncID => syncID;
    [SerializeField] private int syncID;
    public bool IsMine => isMine;
    [SerializeField] private bool isMine = true;
    
    private readonly List<ISyncable> _syncables = new();


    private void Awake()
    {
        NetworkManager.Instance.SyncRegistry.Register(this);
    }

    public void SetOwnership(bool mine)
    {
        isMine = mine;
    }

    public void Register(ISyncable syncable)
    {
        if (_syncables.Contains(syncable)) return;
        
        _syncables.Add(syncable);
    }

    public void SerializeDirty(BinaryWriter writer)
    {
        writer.Write(syncID);
        foreach (ISyncable syncable in _syncables)
        {
            if(syncable.IsDirty)
            {
                syncable.CollectData(writer);
                syncable.ResetDirty();
            }
        }
    }

    public void DeserializeAll(BinaryReader reader)
    {
        foreach(ISyncable syncable in _syncables)
        {
            syncable.ApplyData(reader);
        }
    }

    private void Update()
    {
        if (!IsMine) return;

        foreach (ISyncable syncable in _syncables)
        {
            if (syncable.IsDirty)
            {
                using MemoryStream stream = new MemoryStream();
                using BinaryWriter writer = new BinaryWriter(stream);
                SerializeDirty(writer);
                NetworkManager.Instance.Send(stream.ToArray());
                break;
            }
        }
    }
}
