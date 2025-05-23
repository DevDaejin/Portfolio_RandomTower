//using NativeWebSocket;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

//public class SyncController
//{
//    private readonly ConcurrentDictionary<int, ISyncable> _syncables = new();
//    private readonly List<int> keys = new();
//    private readonly WebSocket _socket;
//    private readonly IDGenerator _idGenerator;

//    public SyncController(WebSocket socket, IDGenerator idGenerator)
//    {
//        _socket = socket;
//        _idGenerator = idGenerator;
//    }

//    public void Register(ISyncable syncable)
//    {
//        if (syncable.ID < 0)
//        {
//            int id = _idGenerator.Get();
//            syncable.AssignID(id);
//            if (_syncables.TryAdd(id, syncable))
//            {
//                keys.Add(id);
//            }
//        }
//    }

//    public void Update()
//    {
//        foreach (int key in keys)
//        {
//            if (_syncables.TryGetValue(key, out ISyncable syncable) && 
//                syncable.IsDirty)
//            {
//                SendToServer(syncable);
//            }
//        }
//    }

//    private async void SendToServer(ISyncable syncable)
//    {
//        using MemoryStream stream = new();
//        using BinaryWriter writer = new(stream);
//        syncable.Serialize(writer);

//        byte[] data = stream.ToArray();

//        if (_socket.State == WebSocketState.Open)
//        {
//            await _socket.Send(data);
//        }
//    }
//}
