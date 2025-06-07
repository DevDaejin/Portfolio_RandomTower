using Google.Protobuf;
using Net;
using System;
using System.IO;
using System.Text;
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
        _client.RegisterEnvelopeHandler("sync", HandleSyncEnvelope);
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

    private void HandleSyncEnvelope(byte[] data)
    {
        var packet = SyncPacketData.Parser.ParseFrom(data);

        var syncObject = SyncObjectManager.GetSyncObject(packet.ObjectId);

        if (syncObject != null)
        {
            syncObject?.Receive(packet.SyncType, packet.Payload);
        }
        else
        {
            SpawnService.AddSyncPacketBuffer(packet);
        }
    }

    public async Task SendEnvelope(string type, IMessage payload) => await _client.SendEnvelope(type, payload);
    public void CancelConnect() => _client.CancelConnect();
    public void Disconnect() => _client?.Disconnect();
}