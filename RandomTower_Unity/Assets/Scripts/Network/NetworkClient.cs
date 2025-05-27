
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
    public Action<string> OnMessage;

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
        Debug.Log($"[NetClient] Raw JSON: {json}");

        BasePacket basePacket = JsonConvert.DeserializeObject<BasePacket>(json);
        Debug.Log($"[NetClient] Parsed type: {basePacket?.Type}");

        if (basePacket.Type == "sync")
        {
            OnMessage?.Invoke(json);
        }
        else
        {
            if (_packetHandlers.TryGetValue(basePacket.Type, out var handler))
            {
                Debug.Log($"[NetClient] Found handler for: {basePacket.Type}");
                handler.Invoke(json);
            }
            else
            {
                Debug.LogWarning($"[NetClient] No handler registered for type: {basePacket.Type}");
            }
        }
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

    public async Task CreateRoom(string name)
    {
        string packet = JsonConvert.SerializeObject(new SendCreateRoomPacket { Name = name });
        await Send(packet);
    }

    public async Task JoinRoom(string roomId)
    {
        string packet = JsonConvert.SerializeObject(new SendJoinRoomPacket { RoomID = roomId });
        await Send(packet);
    }

    public async Task LeaveRoom()
    {
        string packet = JsonConvert.SerializeObject(new SendLeaveRoomPacket { RoomID = RoomID });
        await Send(packet);
    }

    public async Task RequestRoomList()
    {
        string packet = JsonConvert.SerializeObject(new SendListRoomsRequest());
        await Send(packet);
    }

    public async Task SpawnNetworkObject(string name)
    {
        var packet = new
        {
            type = "spawn",
            prefab_name = name,
            room_id = RoomID
        };

        await Send(JsonConvert.SerializeObject(packet));
    }
}