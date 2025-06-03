using Google.Protobuf;

public interface ISyncable
{
    string SyncType { get; }
    IMessage Serialize();
    void Deserialize(ByteString payload);
    bool IsDirty();
    void ClearDirty();
}
