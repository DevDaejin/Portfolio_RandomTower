using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    private void OnRoomCreated(string json)
    {
        var packet = JsonUtility.DeserializeObject<ReceiveRoomCreatedPacket>(json);
        _client.RoomID = packet.RoomID;
        _client.ClientID = packet.ClientID;
    }

    private void OnRoomJoined(string json)
    {
        var packet = JsonUtility.DeserializeObject<ReceiveRoomJoinedPacket>(json);

        _client.RoomID = packet.RoomID;
        _client.ClientID = packet.ClientID;
    }

    private void OnRoomListReceived(string json)
    {
        var packet = JsonUtility.DeserializeObject<ReceiveRoomListPacket>(json);
        OnRoomListUpdated?.Invoke(packet.Rooms);
    }

    private void OnRoomLeft(string json){ }

    public async Task CreateRoom(string name)
    {
        string packet = JsonUtility.SerializeObject(new SendCreateRoomPacket { Name = name });
        await _client?.Send(packet);
    }

    public async Task JoinRoom(string roomID)
    {
        string packet = JsonUtility.SerializeObject(new SendJoinRoomPacket { RoomID = roomID });
        await _client?.Send(packet);
    }

    public async Task LeaveRoom()
    {
        string packet = JsonUtility.SerializeObject(new SendLeaveRoomPacket { RoomID = _client?.RoomID });
        await _client?.Send(packet);
    }

    public async Task RequestRoomList()
    {
        string packet = JsonUtility.SerializeObject(new SendListRoomsRequest());
        await _client?.Send(packet);
    }
}
