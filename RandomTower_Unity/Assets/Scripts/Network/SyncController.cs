using NativeWebSocket;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SyncController
{
    private readonly List<ISyncable> _syncables = new();
    private readonly WebSocket _socket;

    public SyncController(WebSocket socket)
    {
        _socket = socket;
    }

    public void Register(ISyncable syncable)
    {
        if(!_syncables.Contains(syncable))
        {
            _syncables.Add(syncable);
        }
    }

    public void Update()
    {
        foreach(ISyncable syncable in _syncables)
        {
            if(syncable.IsDirty)
            {
                SendToServer(syncable);
            }
        }
    }

    private async void SendToServer(ISyncable syncable)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);
        syncable.Serialize(writer);

        byte[] data = stream.ToArray();
        await _socket.Send(data);
    }
}
