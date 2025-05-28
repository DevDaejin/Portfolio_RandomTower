using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class NetworkManager : MonoBehaviour
{
    private NetworkClient _client;
    public SyncObjectManager SyncObjectManager { get; private set; }
    public RoomService RoomService {get; private set; }
    public SpawnService SpawnService { get; private set; }
    public bool IsConnect { get; private set; } = false;
    public string ClientID => _client?.ClientID ?? string.Empty;
    public string RoomID => _client?.RoomID ?? string.Empty;

    public Action OnConnected;

    private void Update()
    {
        _client?.DispatchMessages();
    }

    private void OnDestroy()
    {
        _client?.LeaveRoom();
        _client?.Disconnect();
    }

    public async void Connect(string ip, string port)
    {
        _client = new($"{ip}:{port}");
        _client.OnConnected = OnConnectComplete;
        await _client.Connect();
    }

    private void OnConnectComplete()
    {
        SyncObjectManager = new SyncObjectManager();
        RoomService = new RoomService(_client);
        SpawnService = new SpawnService(_client);

        _client.RegisterHandler("sync", OnSyncReceived);

        OnConnected.Invoke();
        IsConnect = true;
    }

    private void OnSyncReceived(string json)
    {
        var packet = JsonConvert.DeserializeObject<SyncPacket>(json);
        var syncObject = SyncObjectManager.GetSyncObject(packet.ObjectID);
        syncObject?.Receive(packet.SyncType, packet.Payload);
    }

    public void CancelConnect() => _client.CancelConnect();
    public void Disconnect() => _client?.Disconnect();

    public async Task Send(string json) => await _client.Send(json);
}