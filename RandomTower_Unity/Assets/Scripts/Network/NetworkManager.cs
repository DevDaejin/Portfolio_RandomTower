using System;
using System.Threading.Tasks;
using UnityEngine;


public class NetworkManager : MonoBehaviour
{
    private NetworkClient _client;
    public SyncObjectManager SyncObjectManager { get; private set; }
    public RoomService RoomService {get; private set; }
    public SpawnService SpawnService { get; private set; }
    public bool IsConnect { get; private set; } = false;
    public string ClientID => _client?.ClientID ?? string.Empty;
    public string RoomID => _client?.RoomID ?? string.Empty;

    public Action OnSceneLoad;

    private void Update()
    {
        _client?.DispatchMessages();
    }

    private void OnDestroy()
    {
        RoomService?.LeaveRoom();
        Disconnect();
    }

    public async void Connect(string ip, string port)
    {
        _client = new($"{ip}:{port}");
        _client.RegisterHandler("sync", OnSyncReceived);
        _client.OnConnected += Connected;
        await _client.Connect();
    }

    private void Connected()
    {
        SyncObjectManager = new SyncObjectManager();
        RoomService = new RoomService(_client);
        SpawnService = new SpawnService(_client);
        IsConnect = true;
        OnSceneLoad.Invoke();
    }

    private void OnSyncReceived(string json)
    {
        var packet = JsonUtility.DeserializeObject<SyncPacket>(json);
        var syncObject = SyncObjectManager.GetSyncObject(packet.ObjectID);

        if(syncObject != null)
        {
            syncObject?.Receive(packet.SyncType, packet.Payload);
        }
        else
        {
            SpawnService.AddSyncPacketBuffer(packet);
        }
    }

    public void CancelConnect() => _client.CancelConnect();
    public void Disconnect() => _client?.Disconnect();

    public async Task Send(string json) => await _client.Send(json);
}