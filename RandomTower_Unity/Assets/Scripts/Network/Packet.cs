using System.Collections.Generic;
using Newtonsoft.Json;

public class BasePacket : ITypePacket
{
    public string Type { get; set; }
}

public interface ITypePacket
{
    public string Type { get; set; }
}

public interface IOwnerIDPacket
{
    public string OwnerID { get; set; }
}
public interface IClientIDPacket
{
    public string ClientID { get; set; }
}

public interface IObjectIDPacket
{
    public string ObjectID { get; set; }
}

public interface INamePacket
{
    public string Name { get; set; }
}

public interface IRoomIDPacket
{ 
    public string RoomID { get; set; }
}

public class SyncPacket : ITypePacket, IObjectIDPacket
{
    public string Type { get; set; } = "sync";

    public string ObjectID { get; set; }

    public string SyncType;
    public string Payload;
}

public class SendCreateRoomPacket : ITypePacket, INamePacket
{
    public string Type { get; set; } = "create_room";
    public string Name { get; set; }
}

public class SendJoinRoomPacket : ITypePacket, IRoomIDPacket
{
    public string Type { get; set; } = "join_room";
    public string RoomID { get; set; }
}

public class SendLeaveRoomPacket : ITypePacket, IRoomIDPacket
{
    public string Type { get; set; } = "leave_room";
    public string RoomID { get; set; }
}

public class SendListRoomsRequest : ITypePacket
{
    public string Type { get; set; } = "room_list";
}


public interface ISpawnPacket : ITypePacket, IObjectIDPacket, IOwnerIDPacket, IRoomIDPacket
{
    void SetSpawnID(string id);
    string GetSpawnID();
}

public class SpawnEnemyPacket : ISpawnPacket
{
    public string Type { get; set; } = "spawn_enemy";
    public string EnemyID { get; set; }
    public string ObjectID { get; set; }
    public string OwnerID { get; set; }
    public string RoomID { get; set; }
    public void SetSpawnID(string id) => EnemyID = id;
    public string GetSpawnID() => EnemyID;

}

public class SpawnTowerPacket : ISpawnPacket
{
    public string Type { get; set; } = "spawn_tower";
    public string TowerID { get; set; }
    public string ObjectID { get; set; }
    public string OwnerID { get; set; }
    public string RoomID { get; set; }
    public void SetSpawnID(string id) => TowerID = id;
    public string GetSpawnID() => TowerID;
}

public class SpawnProjectilePacket : ISpawnPacket
{
    public string Type { get; set; } = "spawn_projectile";
    public string ProjectileID { get; set; }
    public string ObjectID { get; set; }
    public string OwnerID { get; set; }
    public string RoomID { get; set; }
    public void SetSpawnID(string id) => ProjectileID = id;
    public string GetSpawnID() => OwnerID;
}

public class ReceiveRoomCreatedPacket : ITypePacket, IRoomIDPacket, INamePacket, IClientIDPacket
{
    public string Type { get; set; }
    public string RoomID { get; set; }
    public string Name { get; set; }
    public string ClientID { get; set; }
}

public class ReceiveRoomJoinedPacket : ITypePacket, IRoomIDPacket, INamePacket, IClientIDPacket
{
    public string Type { get; set; }
    public string RoomID { get; set; }
    public string Name { get; set; }
    public string ClientID { get; set; }
}
public class ReceiveRoomLeftPacket : ITypePacket, IRoomIDPacket
{
    public string Type { get; set; }
    public string RoomID { get; set; }
}

public class ReceiveRoomListPacket : ITypePacket
{
    public string Type { get; set; }
    public List<Room> Rooms;
}

public class Room : IRoomIDPacket, INamePacket
{
    public string RoomID { get; set; }
    public string Name { get; set; }
    public int ClientCount;
}

public class ReceiveErrorPacket : ITypePacket
{
    public string Type { get; set; }
    public string Message;
}