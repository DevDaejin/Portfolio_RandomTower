using System.Collections.Generic;
using Newtonsoft.Json;

public class SyncPacket
{
    [JsonProperty("type")]
    public string Type => "sync";

    [JsonProperty("objectId")]
    public string ObjectID;

    [JsonProperty("syncType")]
    public string SyncType;

    [JsonProperty("payload")]
    public string Payload;
}
public class BasePacket
{
    [JsonProperty("type")]
    public string Type;
}

public interface INetworkMessage
{
    string Type { get; }
}

public class SendCreateRoomPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "create_room";

    [JsonProperty("name")]
    public string Name;
}

public class SendJoinRoomPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "join_room";

    [JsonProperty("room_id")]
    public string RoomID;
}

public class SendLeaveRoomPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "leave_room";

    [JsonProperty("room_id")]
    public string RoomID;
}

public class SendListRoomsRequest : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "room_list";
}
public class SpawnObjectPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "spawn";

    [JsonProperty("prefab_name")]
    public string PrefabName;

    [JsonProperty("object_id")]
    public string ObjectID;

    [JsonProperty("room_id")]
    public string RoomID;

    [JsonProperty("scene_id")]
    public string SceneID;

    [JsonProperty("owner_id")]
    public string OwnerID;
}

public class ReceiveRoomCreatedPacket
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("room_id")]
    public string RoomID;

    [JsonProperty("name")]
    public string Name;
}

public class ReceiveRoomJoinedPacket
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("room_id")]
    public string RoomID;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("client_id")]
    public string ClientID;
}

public class ReceiveRoomLeftPacket
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("room_id")]
    public string RoomID;
}

public class ReceiveRoomListPacket
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("rooms")]
    public List<Room> Rooms;
}

public class Room
{
    [JsonProperty("room_id")]
    public string RoomID;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("client_count")]
    public int ClientCount;
}

public class ReceiveErrorPacket
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("message")]
    public string Message;
}