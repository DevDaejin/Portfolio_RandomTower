
using NativeWebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkClient
{
    private WebSocket _socket;
    private readonly string _url;

    private Dictionary<string, Action<string>> _packetHandlers = new();

    public string ClientID { get; set; }
    public string RoomID { get; set; }

    public Action OnConnected;

    public NetworkClient(string address)
    {
        _url = $"ws://{address}";
    }

    public async Task Connect()
    {
        _socket = new WebSocket(_url);

        _socket.OnOpen += () =>
        {
            Debug.Log("[WebSocket] Connected");
            OnConnected?.Invoke();
        };

        _socket.OnMessage += ProcessMessage;

        await _socket.Connect();

    }
    public void RegisterHandler(string type, Action<string> callback)
    {
        _packetHandlers[type] = callback;
    }

    private void ProcessMessage(byte[] bytes)
    {
        string json = System.Text.Encoding.UTF8.GetString(bytes);

        BasePacket basePacket = JsonUtility.DeserializeObject<BasePacket>(json);

        if (!_packetHandlers.TryGetValue(basePacket.Type, out var handler)) return;

        handler.Invoke(json);
    }


    public async Task Send(string json)
    {
        await _socket.SendText(json);
    }
    public void DispatchMessages()
    {
        _socket?.DispatchMessageQueue();
    }
    public void CancelConnect()
    {
        _socket?.CancelConnection();
    }
    public void Disconnect()
    {
        _socket?.Close();
    }

    public async Task SendPacket<T>(T packet) where T : ITypePacket
    {
        await Send(JsonUtility.SerializeObject(packet));
    }

    public async Task SendSpawnPacket<T>(string id, ISyncObject syncObject) where T : ISpawnPacket, new()
    {
        var packet = new T
        {
            ObjectID = syncObject.ObjectID,
            RoomID = syncObject.RoomID,
            OwnerID = syncObject.OwnerID,
        };

        packet.SetSpawnID(id);
        await SendPacket(packet);
    }
}