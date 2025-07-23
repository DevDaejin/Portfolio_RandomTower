using Net;
using System.Collections.Generic;
using UnityEngine;

public class SyncService
{
    public SyncObjectManager SyncObjectManager { get; } = new();

    private readonly Dictionary<string, List<SyncPacketData>> _syncBuffer = new();
    private readonly GenericPool<List<SyncPacketData>> _bufferPool = new();

    public void HandleSync(SyncPacketData packet)
    {
        var syncObject = SyncObjectManager.GetSyncObject(packet.ObjectId);

        if (syncObject != null)
        {
            syncObject.Receive(packet.SyncType, packet.Payload);
        }
        else
        {
            if (!_syncBuffer.TryGetValue(packet.ObjectId, out var list))
            {
                list = _bufferPool.Get();
                _syncBuffer[packet.ObjectId] = list;
            }

            list.Add(packet);
        }
    }

    public void OnSyncObjectSpawned(ISyncObject syncObject)
    {
        string objectId = syncObject.ObjectID;

        if (_syncBuffer.TryGetValue(objectId, out var packets))
        {
            foreach (var packet in packets)
            {
                syncObject.Receive(packet.SyncType, packet.Payload);
            }

            packets.Clear();
            _bufferPool.Release(packets);
            _syncBuffer.Remove(objectId);
        }
    }

    public void Register(SyncObject syncObject)
    {
        SyncObjectManager.Register(syncObject);
    }

    public void Unregister(string objectId)
    {
        SyncObjectManager.Unregister(objectId);
    }
}
