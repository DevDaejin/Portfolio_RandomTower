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

        _client.RegisterEnvelopeHandler("room", HandleRoomEnvelope);
    }

    private void HandleRoomEnvelope(byte[] bytes)
    {
        var packet = RoomPacket.Parser.ParseFrom(bytes);

        switch (packet.PayloadCase)
        {
            case RoomPacket.PayloadOneofCase.RoomCreated:
                _client.RoomID = packet.RoomCreated.RoomId;
                _client.ClientID = packet.RoomCreated.ClientId;
                break;

            case RoomPacket.PayloadOneofCase.RoomJoined:
                _client.RoomID = packet.RoomJoined.RoomId;
                _client.ClientID = packet.RoomJoined.ClientId;
                break;

            case RoomPacket.PayloadOneofCase.RoomList:
                OnRoomListUpdated?.Invoke(new List<RoomInfo>(packet.RoomList.Rooms));
                break;

            case RoomPacket.PayloadOneofCase.RoomLeft:
                break;
        }
    }
    public async Task CreateRoom(string name)
    {
        var wrapper = new RoomPacket { CreateRoom = new CreateRoomRequest { Name = name } };
        await _client.SendEnvelope("room", wrapper);
    }

    public async Task JoinRoom(string roomId)
    {
        var wrapper = new RoomPacket { JoinRoom = new JoinRoomRequest { RoomId = roomId } };
        await _client.SendEnvelope("room", wrapper);
    }

    public async Task LeaveRoom()
    {
        var wrapper = new RoomPacket { LeaveRoom = new LeaveRoomRequest { RoomId = _client?.RoomID } };
        await _client.SendEnvelope("room", wrapper);
    }

    public async Task RequestRoomList()
    {
        var wrapper = new RoomPacket { ListRoom = new ListRoomRequest() };
        await _client.SendEnvelope("room", wrapper);
    }
}
