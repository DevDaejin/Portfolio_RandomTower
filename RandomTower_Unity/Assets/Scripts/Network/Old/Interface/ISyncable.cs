using System;
using System.IO;

public interface ISyncable
{
    bool IsDirty { get; }
    void CollectData(BinaryWriter writer);
    void ApplyData(BinaryReader reader);
    void ResetDirty();
}
