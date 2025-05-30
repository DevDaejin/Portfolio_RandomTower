using UnityEngine;

public interface ISyncObject
{
    string ObjectID { get; }
    string OwnerID { get; }
    string RoomID { get; }

    void Initialize(string objectID, string ownerID, string roomID);

    //TODO: 버퍼 적용로직
}
