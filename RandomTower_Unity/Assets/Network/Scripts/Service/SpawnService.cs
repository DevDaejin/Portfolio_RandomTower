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

    public Action<SpawnEnemyPacket> OnEnemySpawned;
    public Action<SpawnTowerPacket> OnTowerSpawned;
    public Action<SpawnProjectilePacket> OnProjectileSpawned;

    public SpawnService(NetworkClient client)
    {
        _client = client;
    }

    public void OnReceiveEnemy(SpawnEnemyPacket packet)
    {
        if (packet.OwnerId == _client.ClientID) return;
        OnEnemySpawned?.Invoke(packet);
    }

    public void OnReceiveTower(SpawnTowerPacket packet)
    {
        if (packet.OwnerId == _client.ClientID) return;
        OnTowerSpawned?.Invoke(packet);
    }

    public void OnReceiveProjectile(SpawnProjectilePacket packet)
    {
        if (packet.OwnerId == _client.ClientID) return;
        OnProjectileSpawned?.Invoke(packet);
    }

    public async Task SendSpawn(string type, string spawnId, ISyncObject syncObject)
    {
        IMessage packet = type switch
        {
            "spawn_enemy" => new SpawnEnemyPacket
            {
                ObjectId = syncObject.ObjectID,
                OwnerId = syncObject.OwnerID,
                RoomId = syncObject.RoomID,
                SpawnId = spawnId
            },
            "spawn_tower" => new SpawnTowerPacket
            {
                ObjectId = syncObject.ObjectID,
                OwnerId = syncObject.OwnerID,
                RoomId = syncObject.RoomID,
                SpawnId = spawnId
            },
            "spawn_projectile" => new SpawnProjectilePacket
            {
                ObjectId = syncObject.ObjectID,
                OwnerId = syncObject.OwnerID,
                RoomId = syncObject.RoomID,
                SpawnId = spawnId
            },
            _ => null
        };

        if (packet != null) await _client.Send(type, packet);
    }
}