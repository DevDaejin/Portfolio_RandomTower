using Google.Protobuf;
using Net;
using Spawn;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SpawnService
{
    private NetworkClient _client;
    private Dictionary<string, List<SyncPacketData>> _syncBuffer = new();
    private GenericPool<List<SyncPacketData>> _bufferPool = new();

    public SpawnService(NetworkClient client)
    {
        _client = client;
    }

    public void Initialize(
        Action<string, SpawnEnemyPacket> onEnemy,
        Action<string, SpawnTowerPacket> onTower,
        Action<string, SpawnProjectilePacket> onProjectile)
    {
        _client.RegisterEnvelopeHandler("spawn_enemy", bytes =>
        {
            var packet = SpawnEnemyPacket.Parser.ParseFrom(bytes);
            if (packet.OwnerId == _client.ClientID) return;
            onEnemy?.Invoke(packet.SpawnId, packet);
        });

        _client.RegisterEnvelopeHandler("spawn_tower", bytes =>
        {
            var packet = SpawnTowerPacket.Parser.ParseFrom(bytes);
            if (packet.OwnerId == _client.ClientID) return;
            onTower?.Invoke(packet.SpawnId, packet);
        });

        _client.RegisterEnvelopeHandler("spawn_projectile", bytes =>
        {
            var packet = SpawnProjectilePacket.Parser.ParseFrom(bytes);
            if (packet.OwnerId == _client.ClientID) return;
            onProjectile?.Invoke(packet.SpawnId, packet);
        });
    }

    public void AddSyncPacketBuffer(SyncPacketData packet)
    {
        string id = packet.ObjectId;
        if (!_syncBuffer.ContainsKey(id))
        {
            _syncBuffer[id] = _bufferPool.Get();
        }

        _syncBuffer[id].Add(packet);
    }

    public void OnApplybufferWhenSpawned(ISyncObject syncObject)
    {
        string objectID = syncObject.ObjectID;
        if (_syncBuffer.TryGetValue(objectID, out List<SyncPacketData> packets))
        {
            foreach (SyncPacketData packet in packets)
            {
                syncObject.Receive(packet.SyncType, packet.Payload);
            }

            packets.Clear();
            _bufferPool.Release(packets);
            _syncBuffer.Remove(objectID);
        }
    }

    public async Task SendSpawn(string type, string spawnId, ISyncObject syncObject)
    {
        IMessage packet = CreateSpawnPacket(type, spawnId, syncObject);
        await _client.Send(type, packet);
    }

    private IMessage CreateSpawnPacket(string type, string spawnId, ISyncObject syncObject)
    {
        switch (type)
        {
            case "spawn_enemy":
                return new SpawnEnemyPacket
                {
                    ObjectId = syncObject.ObjectID,
                    OwnerId = syncObject.OwnerID,
                    RoomId = syncObject.RoomID,
                    SpawnId = spawnId
                };

            case "spawn_tower":
                return new SpawnTowerPacket
                {
                    ObjectId = syncObject.ObjectID,
                    OwnerId = syncObject.OwnerID,
                    RoomId = syncObject.RoomID,
                    SpawnId = spawnId
                };

            case "spawn_projectile":
                return new SpawnProjectilePacket
                {
                    ObjectId = syncObject.ObjectID,
                    OwnerId = syncObject.OwnerID,
                    RoomId = syncObject.RoomID,
                    SpawnId = spawnId
                };

            default:
                return null;
        }
    }
}