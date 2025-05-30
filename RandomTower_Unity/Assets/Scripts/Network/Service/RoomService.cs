using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoomService
{
    private readonly NetworkClient _client;
    public Action<List<Room>> OnRoomListUpdated;
    public RoomService(NetworkClient client)
    {
        _client = client;

        _client?.RegisterHandler("room_created", OnRoomCreated);
        _client?.RegisterHandler("room_joined", OnRoomJoined);
        _client?.RegisterHandler("room_list", OnRoomListReceived);
        _client?.RegisterHandler("room_left", OnRoomLeft);
    }

    public Task CreateRoom(string name) => _client?.CreateRoom(name);
    public Task JoinRoom(string roomId) => _client?.JoinRoom(roomId);
    public Task LeaveRoom() => _client?.LeaveRoom();
    public Task RequestRoomList() => _client.RequestRoomList();

    private void OnRoomCreated(string json)
    {
        var packet = JsonConvert.DeserializeObject<ReceiveRoomCreatedPacket>(json);
        _client.RoomID = packet.RoomID;
        _client.ClientID = packet.ClientID;
    }

    private void OnRoomJoined(string json)
    {
        var packet = JsonConvert.DeserializeObject<ReceiveRoomJoinedPacket>(json);

        _client.RoomID = packet.RoomID;
        _client.ClientID = packet.ClientID;
    }

    private void OnRoomListReceived(string json)
    {
        var packet = JsonConvert.DeserializeObject<ReceiveRoomListPacket>(json);
        OnRoomListUpdated?.Invoke(packet.Rooms);
    }

    private void OnRoomLeft(string json){ }
}
