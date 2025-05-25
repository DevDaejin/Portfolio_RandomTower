
using UnityEngine;
using NativeWebSocket;
using System;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

public class NetworkClient
{
    private WebSocket _socket;
    private readonly string _url;

    private string _currentRoomId = null;
    public bool IsInRoom => !string.IsNullOrEmpty(_currentRoomId);

    private event Action<INetworkMessage> _onMessageReceived;

    public NetworkClient(string url)
    {
        _url = url;
    }

    public async Task Connect()
    {
        _socket = new WebSocket($"ws://{_url}");

        _socket.OnOpen += () =>
        {
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
            HandlePacket(basePacket.type, json);
        };

        await _socket.Connect();
    }

    public async void CreateRoom(string roomName)
    {
        if (IsInRoom)
        {
            Debug.LogWarning("[Client] Already in a room. Cannot create a new one.");
            return;
        }

        var packet = new CreateRoomPacket { name = roomName };
        await Send(packet);
    }

    public async void JoinRoom(string roomID)
    {
        if (IsInRoom)
        {
            Debug.LogWarning("[Client] Already in a room. Cannot join another.");
            return;
        }

        var packet = new JoinRoomPacket { room_id = roomID };
        await Send(packet);
    }

    public async void RequestRoomList()
    {
        var packet = new ListRoomsPacket();
        await Send(packet);
    }

    public void LeaveRoom()
    {
        if (!IsInRoom)
        {
            Debug.LogWarning("[Client] Not in a room.");
            return;
        }

        Debug.Log($"[Client] Leaving room: {_currentRoomId}");
        _currentRoomId = null;
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

    private void HandlePacket(string type, string json)
    {
        INetworkMessage packet = type switch
        {
            "room_list" => JsonConvert.DeserializeObject<RoomListPacket>(json),
            "room_created" => JsonConvert.DeserializeObject<RoomCreatedPacket>(json),
            "room_joined" => JsonConvert.DeserializeObject<RoomJoinedPacket>(json),
            "room_info" => JsonConvert.DeserializeObject<RoomInfoPacket>(json),
            _ => null
        };

        if (packet != null)
        {
            Debug.Log($"[Recv] {type}: {json}");

            if (packet is RoomCreatedPacket created)
                _currentRoomId = created.room_id;

            else if (packet is RoomJoinedPacket joined)
                _currentRoomId = joined.room_id;

            else if (packet is RoomListPacket list)
            {
                Debug.Log($"[RoomList] 총 {list.rooms.Count}개");

                foreach (var room in list.rooms)
                {
                    Debug.Log($"[Room] ID: {room.id}, Name: {room.name}, Clients: {room.client_count}");
                }
            }
            _onMessageReceived?.Invoke(packet);
        }
        else
        {
            Debug.LogWarning($"[Recv] Unknown packet type: {type}");
        }
    }
}
