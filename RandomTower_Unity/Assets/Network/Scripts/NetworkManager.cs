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
    public SyncService SyncService { get; private set; }
    public bool IsConnect { get; private set; } = false;

    public string ClientID => _client?.ClientID ?? string.Empty;
    public string RoomID => _client?.RoomID ?? string.Empty;

    public Action OnSceneLoad;
    public event Action OnConnectFailed;
    public event Action OnError;
    public event Action OnClose;

    private void Update() => _client?.DispatchMessages();

    private void OnDestroy()
    {
        RoomService?.LeaveRoom();
        Disconnect();
    }

    public async void Connect(string ip, string port)
    {
        _client = new NetworkClient($"{ip}:{port}");

        RoomService = new RoomService(_client);
        SpawnService = new SpawnService(_client);
        SyncService = new SyncService();

        _client.RegisterEnvelopeHandler("room", HandleRoomEnvelope);
        _client.RegisterEnvelopeHandler("spawn_enemy", HandleSpawnEnemy);
        _client.RegisterEnvelopeHandler("spawn_tower", HandleSpawnTower);
        _client.RegisterEnvelopeHandler("spawn_projectile", HandleSpawnProjectile);
        _client.RegisterEnvelopeHandler("sync", HandleSync);

        _client.OnError = () => OnError?.Invoke();
        _client.OnClose = () => OnClose?.Invoke();
        _client.OnConnectFailed = () => OnConnectFailed?.Invoke();
        _client.OnConnected = Connected;

        await _client.Connect();
    }

    private void Connected()
    {
        IsConnect = true;
        OnSceneLoad?.Invoke();
    }

    private void HandleRoomEnvelope(byte[] bytes)
    {
        var packet = RoomPacket.Parser.ParseFrom(bytes);
        RoomService.HandleRoomPacket(packet);
    }

    private void HandleSpawnEnemy(byte[] bytes) => SpawnService.OnReceiveEnemy(SpawnEnemyPacket.Parser.ParseFrom(bytes));
    private void HandleSpawnTower(byte[] bytes) => SpawnService.OnReceiveTower(SpawnTowerPacket.Parser.ParseFrom(bytes));
    private void HandleSpawnProjectile(byte[] bytes) => SpawnService.OnReceiveProjectile(SpawnProjectilePacket.Parser.ParseFrom(bytes));

    private void HandleSync(byte[] bytes)
    {
        var packet = SyncPacketData.Parser.ParseFrom(bytes);
        SyncService.HandleSync(packet);
    }

    public async Task SendEnvelope(string type, IMessage payload) => await _client.SendEnvelope(type, payload);
    public void CancelConnect() => _client?.CancelConnect();
    public void Disconnect() => _client?.Disconnect();
}
