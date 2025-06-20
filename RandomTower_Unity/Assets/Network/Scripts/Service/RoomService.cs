using Room;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RoomService
{
    private readonly NetworkClient _client;
    public Action<List<RoomInfo>> OnRoomListUpdated;

    public RoomService(NetworkClient client)
    {
        _client = client;
    }

    public void HandleRoomPacket(RoomPacket packet)
    {
        switch (packet.PayloadCase)
        {
            case RoomPacket.PayloadOneofCase.RoomCreated:
                _client.RoomID = packet.RoomCreated.RoomId;
                _client.ClientID = packet.RoomCreated.ClientId;
                break;

            case RoomPacket.PayloadOneofCase.RoomJoined:
                _client.RoomID = packet.RoomJoined.RoomId;
                _client.ClientID = packet.RoomJoined.ClientId;
                _client.RoomOwnerID = packet.RoomJoined.OwnerId;
                break;

            case RoomPacket.PayloadOneofCase.RoomList:
                OnRoomListUpdated?.Invoke(new List<RoomInfo>(packet.RoomList.Rooms));
                break;
        }
    }

    public async Task CreateRoom(string name) =>
        await _client.SendEnvelope("room", new RoomPacket { CreateRoom = new CreateRoomRequest { Name = name } });

    public async Task JoinRoom(string roomId) =>
        await _client.SendEnvelope("room", new RoomPacket { JoinRoom = new JoinRoomRequest { RoomId = roomId } });

    public async Task LeaveRoom() =>
        await _client.SendEnvelope("room", new RoomPacket { LeaveRoom = new LeaveRoomRequest { RoomId = _client.RoomID } });

    public async Task RequestRoomList() =>
        await _client.SendEnvelope("room", new RoomPacket { ListRoom = new ListRoomRequest() });
}
