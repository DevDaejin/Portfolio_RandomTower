using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnService
{
    private NetworkClient _client;
    public Action<string, SpawnObjectPacket> OnReceivedSpawnPacket;

    public SpawnService(NetworkClient client)
    {
        _client = client;
        _client?.RegisterHandler("spawn_enemy", OnSpawnReceived);
    }

    private void OnSpawnReceived(string json)
    {
        SpawnObjectPacket packet = JsonConvert.DeserializeObject<SpawnObjectPacket>(json);

        if (packet == null || packet.OwnerID == _client?.ClientID) return;

        OnReceivedSpawnPacket?.Invoke(packet.EnemyID, packet);
    }

    public async Task SpawnNetworkObject(string id, ISyncObject syncObject)
    {
        await _client?.SpawnNetworkObject(id, syncObject);
    }
}
