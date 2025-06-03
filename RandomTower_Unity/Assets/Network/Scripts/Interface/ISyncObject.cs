using Google.Protobuf;
using System.Buffers.Text;
using UnityEngine;

public interface ISyncObject
{
    string ObjectID { get; }
    string OwnerID { get; }
    string RoomID { get; }

    void Initialize(string objectID, string ownerID, string roomID);
    void Receive(string syncType, ByteString payload);
}
