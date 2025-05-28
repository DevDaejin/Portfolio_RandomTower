using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

public class SpawnService
{
    private NetworkClient _client;
    public Action<string, SpawnEnemyPacket> OnReceivedEnemyPacket;
    public Action<string, SpawnTowerPacket> OnReceivedTowerPacket;

    public SpawnService(NetworkClient client)
    {
        _client = client;
        _client?.RegisterHandler("spawn_enemy", OnEnemySpawnReceived);
        _client?.RegisterHandler("spawn_tower", OnTowerSpawnReceived);
    }

    private void OnEnemySpawnReceived(string json)
    {
        SpawnEnemyPacket packet = JsonConvert.DeserializeObject<SpawnEnemyPacket>(json);

        if (packet == null || packet.OwnerID == _client?.ClientID) return;

        OnReceivedEnemyPacket?.Invoke(packet.EnemyID, packet);
    }

    public async Task SendEnemySpawn(string id, ISyncObject syncObject)
    {
        await _client?.SendEnemySpawn(id, syncObject);
    }

    private void OnTowerSpawnReceived(string json)
    {
        SpawnTowerPacket packet = JsonConvert.DeserializeObject<SpawnTowerPacket>(json);

        if (packet == null || packet.OwnerID == _client?.ClientID) return;

        OnReceivedTowerPacket?.Invoke(packet.TowerID, packet);
    }

    public async Task SendTowerSpawn(string id, ISyncObject syncObject)
    {
        await _client?.SendTowerSpawn(id, syncObject);
    }
}
