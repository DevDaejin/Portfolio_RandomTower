using Despawn;
using Game;
using Google.Protobuf;
using Net;
using Room;
using Spawn;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private NetworkClient _client;
    public RoomService RoomService { get; private set; }
    public SpawnService SpawnService { get; private set; }
    public DespawnService DespawnService { get; private set; }
    public SyncService SyncService { get; private set; }
    public GameStateService GameStateService { get; private set; }
    public bool IsConnect { get; private set; } = false;

    public bool IsHost => _client?.ClientID == _client?.RoomOwnerID;
    public string ClientID => _client?.ClientID ?? string.Empty;
    public string RoomID => _client?.RoomID ?? string.Empty;

    public Action OnSceneLoad;
    public event Action OnConnectFailed;
    public event Action OnError;
    public event Action OnClose;

    private void Update() => _client?.DispatchMessages();

    private void OnDestroy()
    {
        if (IsConnect)
        {
            if (!string.IsNullOrWhiteSpace(RoomID))
            {
                RoomService?.LeaveRoom();
            }
            Disconnect();
        }
    }

    public async void Connect(string ip, string port)
    {
        _client = new NetworkClient($"{ip}:{port}");

        RoomService = new RoomService(_client);
        SpawnService = new SpawnService(_client);
        DespawnService = new DespawnService(_client);
        SyncService = new SyncService();
        GameStateService = new GameStateService();

        _client.RegisterEnvelopeHandler(NetworkConst.Room, HandleRoomEnvelope);
        _client.RegisterEnvelopeHandler(NetworkConst.Spawn, HandleSpawn);
        _client.RegisterEnvelopeHandler(NetworkConst.Despawn, HandleDespawn);
        _client.RegisterEnvelopeHandler(NetworkConst.Sync, HandleSync);
        _client.RegisterEnvelopeHandler(NetworkConst.GameState, HandleGameState);

        _client.OnError = OnError;
        _client.OnClose = OnClose;
        _client.OnConnectFailed = OnConnectFailed;
        _client.OnConnected = Connected;

        await _client.Connect();
    }

    private void Connected()
    {
        IsConnect = true;
        OnSceneLoad?.Invoke();
    }

    public async void OnSendSpawnPacket<T>(string type, int dataId, string objectId, ISyncObject syncObject)
    {
        if (!IsConnect) return;

        syncObject.Initialize(objectId, ClientID, RoomID);
        await SpawnService.SendSpawn(type, dataId.ToString(), syncObject);
    }

    public async void OnSendDespawnPacket<T>(string type, int dataId, ISyncObject syncObject)
    {
        await DespawnService.SendDespawn(type, dataId.ToString(), syncObject);
    }

    private void HandleRoomEnvelope(byte[] bytes)
    {
        var packet = RoomPacket.Parser.ParseFrom(bytes);
        RoomService.HandleRoomPacket(packet);
    }

    private void HandleSync(byte[] bytes)
    {
        var packet = SyncPacketData.Parser.ParseFrom(bytes);
        SyncService.HandleSync(packet);
    }

    private void HandleSpawn(byte[] bytes)
    {
        var packet = SpawnPacketData.Parser.ParseFrom(bytes);
        SpawnService.OnReceive(packet);
    }

    private void HandleDespawn(byte[] bytes)
    {
        var packet = DespawnPacketData.Parser.ParseFrom(bytes);
        DespawnService.OnReceive(packet);
    }

    private void HandleGameState(byte[] bytes)
    {
        var packet = GameStatePacket.Parser.ParseFrom(bytes);
        GameStateService.OnReceive(packet);
    }

    public async Task SendEnvelope(string type, IMessage payload) => await _client.SendEnvelope(type, payload);
    public void CancelConnect() => _client?.CancelConnect();
    public void Disconnect() => _client?.Disconnect();
}
