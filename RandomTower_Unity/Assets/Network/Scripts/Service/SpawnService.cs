using Google.Protobuf;
using Net;
using Spawn;
using System;
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

    public void OnReceive(SpawnPacketData packet)
    {
        switch(packet.SpawnType.ToLower())
        {
            case "enemy":
                var enemy = SpawnEnemyPacket.Parser.ParseFrom(packet.Payload);
                if (enemy.OwnerId == _client.ClientID) return;
                OnEnemySpawned?.Invoke(enemy);
                break;

            case "tower":
                var tower = SpawnTowerPacket.Parser.ParseFrom(packet.Payload);
                if (tower.OwnerId == _client.ClientID) return;
                OnTowerSpawned?.Invoke(tower);
                break;

            case "projectile":
                var projectile = SpawnProjectilePacket.Parser.ParseFrom(packet.Payload);
                if (projectile.OwnerId == _client.ClientID) return;
                OnProjectileSpawned?.Invoke(projectile);
                break;

            default:
                UnityEngine.Debug.LogWarning($"[Spawn] Unknown spawn type: {packet.SpawnType}");
                break;
        }
    }

    public async Task SendSpawn(string type, string spawnId, ISyncObject syncObject)
    {
        IMessage payload = type switch
        {
            "enemy" => new SpawnEnemyPacket
            {
                ObjectId = syncObject.ObjectID,
                OwnerId = syncObject.OwnerID,
                RoomId = syncObject.RoomID,
                SpawnId = spawnId
            },
            "tower" => new SpawnTowerPacket
            {
                ObjectId = syncObject.ObjectID,
                OwnerId = syncObject.OwnerID,
                RoomId = syncObject.RoomID,
                SpawnId = spawnId
            },
            "projectile" => new SpawnProjectilePacket
            {
                ObjectId = syncObject.ObjectID,
                OwnerId = syncObject.OwnerID,
                RoomId = syncObject.RoomID,
                SpawnId = spawnId
            },
            _ => null
        };

        if (payload == null) return;

        var packet = new SpawnPacketData
        {
            ObjectId = syncObject.ObjectID,
            SpawnType = type,
            Payload = payload.ToByteString()
        };

        await _client.Send("spawn", packet);
    }
}