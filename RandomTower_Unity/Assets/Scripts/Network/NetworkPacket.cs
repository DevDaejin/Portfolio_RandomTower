using System.Collections.Generic;
using UnityEngine;

public class BasePacket
{
    public string type;
}

public class CreateRoomPacket : INetworkMessage
{
    public string type => "create_room";
    public string name;
}

public class JoinRoomPacket : INetworkMessage
{
    public string type => "join_room";
    public string room_id;
}

public class GetRoomInfoPacket : INetworkMessage
{
    public string type => "get_room_info";
    public string room_id;
}

public class ListRoomsPacket : INetworkMessage
{
    public string type => "list_rooms";
}

// --- 수신용 ---
public class RoomCreatedPacket : INetworkMessage
{
    public string type { get; set; }
    public string room_id;
    public string name;
}

public class RoomJoinedPacket : INetworkMessage
{
    public string type { get; set; }
    public string room_id;
    public string name;
}

public class RoomInfoPacket : INetworkMessage
{
    public string type { get; set; }
    public string room_id;
    public string name;
    public int client_count;
}

public class RoomListPacket : INetworkMessage
{
    public string type { get; set; }
    public List<RoomSummary> rooms;
}

public class RoomSummary
{
    public string id;
    public string name;
    public int client_count;
}
