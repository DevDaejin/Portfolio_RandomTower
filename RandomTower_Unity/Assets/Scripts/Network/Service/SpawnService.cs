using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine.Android;

public class SpawnService
{
    private NetworkClient _client;
    private Dictionary<string, List<SyncPacket>> _syncBuffer = new();
    private GenericPool<List<SyncPacket>> _bufferPool = new();

    public SpawnService(NetworkClient client)
    {
        _client = client;
    }

    public void Initialize(
        Action<string, SpawnEnemyPacket> OnReceivedEnemyPacket,
        Action<string, SpawnTowerPacket> OnReceivedTowerPacket,
        Action<string, SpawnProjectilePacket> OnReceivedProjectilePacket)
    {
        RegisterSpawnHandler(OnReceivedEnemyPacket);
        RegisterSpawnHandler(OnReceivedTowerPacket);
        RegisterSpawnHandler(OnReceivedProjectilePacket);
    }

    public void RegisterSpawnHandler<T>(Action<string, T> callback) where T : class, ISpawnPacket, new()
    {
        _client.RegisterHandler(new T().Type, json =>
        {
            T packet = JsonUtility.DeserializeObject<T>(json);
            if (packet == null || packet.OwnerID == _client.ClientID) return;

            callback?.Invoke(packet.GetSpawnID(), packet);
        });
    }

    public void AddSyncPacketBuffer(SyncPacket packet)
    {
        string id = packet.ObjectID;
        if (!_syncBuffer.ContainsKey(id))
        {
            _syncBuffer[id] = _bufferPool.Get();
        }

        _syncBuffer[id].Add(packet);
    }

    public void OnApplybufferWhenSpawned(ISyncObject syncObject)
    {
        string objectID = syncObject.ObjectID;
        if (_syncBuffer.TryGetValue(objectID, out List<SyncPacket> packets))
        {
            foreach (SyncPacket packet in packets)
            {
                syncObject.Receive(packet.SyncType, packet.Payload);
            }
            _bufferPool.Release(_syncBuffer[objectID]);
            _syncBuffer.Remove(objectID);
        }
    }

    public async Task SendSpawn<T>(string id, ISyncObject syncObject) where T : class, ISpawnPacket, new()
    {
        await _client?.SendSpawnPacket<T>(id, syncObject);
    }
}
