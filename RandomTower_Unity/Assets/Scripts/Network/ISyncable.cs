using System;
using System.IO;

public interface ISyncable
{
    void Initialize();
    void Serialize(BinaryWriter writer);
    void Deserialize(BinaryReader reader);
    void SetDirty();
    bool IsDirty { get; }
    int ID { get; }
}
