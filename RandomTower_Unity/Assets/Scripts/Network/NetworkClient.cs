
using NativeWebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkClient
{
    
    private WebSocket _socket;
    private readonly string _url;

    public string RoomID { get; set; }
    public string ClientID { get; set; }
    public bool IsInRoom => !string.IsNullOrEmpty(RoomID);

    public event Action OnConnected;
    private readonly Dictionary<string, Action<string>> _packetHandlers = new();


    public NetworkClient(string url)
    {
        _url = url;
    }

    public void RegisterHandler(string type, Action<string> handler)
    {
        _packetHandlers[type] = handler;
    }

    public async Task Connect()
    {
        _socket = new WebSocket($"ws://{_url}");

        _socket.OnOpen += () =>
        {
            OnConnected.Invoke();
            Debug.Log("[WebSocket] Connected to server");
        };

        _socket.OnError += (e) =>
        {
            Debug.LogError($"[WebSocket] Error: {e}");
        };

        _socket.OnClose += (e) =>
        {
            Debug.LogWarning($"[WebSocket] Closed with code {e}");
        };

        _socket.OnMessage += (bytes) =>
        {
            string json = Encoding.UTF8.GetString(bytes);
            var basePacket = JsonConvert.DeserializeObject<BasePacket>(json);
            HandlePacket(basePacket.Type, json);
        };

        await _socket.Connect();
    }
    public async Task Disconnect()
    {
        if (_socket != null)
        {
            await _socket.Close();
        }
    }

    public void DispatchMessages()
    {
        _socket?.DispatchMessageQueue();
    }

    public async Task Send<T>(T packet) where T : INetworkMessage
    {
        if (_socket == null || _socket.State != WebSocketState.Open)
        {
            Debug.LogWarning("[Send] WebSocket not connected.");
            return;
        }

        string json = JsonConvert.SerializeObject(packet);
        await _socket.SendText(json);
    }

    public async void SendRaw(string json)
    {
        if (_socket == null || _socket.State != WebSocketState.Open)
        {
            Debug.LogWarning("[SendRaw] WebSocket not connected.");
            return;
        }

        await _socket.SendText(json);
    }

    private void HandlePacket(string type, string json)
    {
        Debug.Log(json);

        if(_packetHandlers.TryGetValue(type, out Action<string> hanlder))
        {
            hanlder(json);
        }
        else
        {
            Debug.LogWarning($"[NetworkClient] Unknown packet type: {type}");
        }
    }

    public async Task CreateRoom(string name)
    {
        if (IsInRoom)
        {
            Debug.LogWarning("[CreateRoom] Already in a room.");
            return;
        }

        var packet = new CreateRoomPacket { Name = name };
        await Send(packet);
    }

    public async Task JoinRoom(string roomId)
    {
        if (IsInRoom)
        {
            Debug.LogWarning("[JoinRoom] Already in a room.");
            return;
        }

        var packet = new JoinRoomPacket { RoomID = roomId };
        await Send(packet);
    }

    public async Task RequestRoomList()
    {
        var packet = new RoomListPacket();
        await Send(packet);
    }

    public async Task LeaveRoom()
    {
        if (!IsInRoom)
        {
            Debug.LogWarning("[LeaveRoom] Not in a room.");
            return;
        }

        var packet = new LeaveRoomPacket { RoomID = RoomID };
        await Send(packet);

        RoomID = null;
    }
}
