using System.IO;
using UnityEngine;

public interface ISyncView
{
    int SyncID { get; }
    bool IsMine { get; }
    void Register(ISyncable syncable);
    void SerializeDirty(BinaryWriter writer);
    void DeserializeAll(BinaryReader reader);
}
