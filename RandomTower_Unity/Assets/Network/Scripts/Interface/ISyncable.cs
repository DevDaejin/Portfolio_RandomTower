using Google.Protobuf;

public interface ISyncable
{
    string SyncType { get; }
    string ObjectID { get; }
    IMessage Serialize();
    void Deserialize(ByteString payload);
    bool IsDirty();
    void ClearDirty();
}
