public interface ISpawnPacket
{
    string ObjectId { get; set; }
    string OwnerId { get; set; }
    string RoomId { get; set; }
    string SpawnId { get; set; }
}