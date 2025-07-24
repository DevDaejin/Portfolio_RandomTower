using Despawn;
using Google.Protobuf;
using System;
using System.Threading.Tasks;

public class DespawnService
{
    private NetworkClient _client;

    public Action<DespawnTowerPacket> OnTowerDespawned;

    public DespawnService(NetworkClient client)
    {
        _client = client;
    }

    public void OnReceive(DespawnPacketData packet)
    {
        switch (packet.DespawnType.ToLower())
        {
            case NetworkConst.Tower:
                var tower = DespawnTowerPacket.Parser.ParseFrom(packet.Payload);
                OnTowerDespawned?.Invoke(tower);
                break;

            default:
                UnityEngine.Debug.LogWarning($"[Despawn] Unknown type: {packet.DespawnType}");
                break;
        }
    }

    public async Task SendDespawn(string type, string dataId, ISyncObject syncObject)
    {
        IMessage payload = type switch
        {
            NetworkConst.Tower => new DespawnTowerPacket
            {
                ObjectId = syncObject.ObjectID,
                OwnerId = syncObject.OwnerID,
                RoomId = syncObject.RoomID,
                DespawnId = dataId,
            },
            _ => null
        };

        if (payload == null) return;

        var packet = new DespawnPacketData
        {
            ObjectId = syncObject.ObjectID,
            DespawnType = type,
            Payload = payload.ToByteString()
        };

        await _client.Send(NetworkConst.Despawn, packet);
    }
}
