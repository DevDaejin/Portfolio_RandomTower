using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public interface INetworkMessage
{
    string Type { get; }
}

public class BasePacket
{
    [JsonProperty("type")]
    public string Type;
}

public class CreateRoomPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "create_room";

    [JsonProperty("name")]
    public string Name;
}

public class JoinRoomPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "join_room";

    [JsonProperty("room_id")]
    public string RoomID;
}

public class GetRoomInfoPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "get_room_info";

    [JsonProperty("room_id")]
    public string RoomID;
}

public class ListRoomsPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "list_rooms";
}

public class RoomCreatedPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("room_id")]
    public string RoomID;

    [JsonProperty("name")]
    public string Name;
}

public class RoomJoinedPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("room_id")]
    public string RoomID;

    [JsonProperty("name")]
    public string Name;
}

public class ListRoomsRequest : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "list_rooms";
}

public class RoomInfoPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("room_id")]
    public string RoomID;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("client_count")]
    public int ClientCount;
}

public class RoomListPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("rooms")]
    public List<Room> Rooms;
}

public class LeaveRoomPacket : INetworkMessage
{
    [JsonProperty("type")]
    public string Type => "leave_room";

    [JsonProperty("room_id")]
    public string RoomID;
}

public class Room
{
    [JsonProperty("id")]
    public string ID;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("client_count")]
    public int ClientCount;
}

public class SyncObjectHeader
{
    [JsonProperty("object_id")]
    public string ObjectID;

    [JsonProperty("room_id")]
    public string RoomId;

    [JsonProperty("scene_id")]
    public string SceneId;
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
}