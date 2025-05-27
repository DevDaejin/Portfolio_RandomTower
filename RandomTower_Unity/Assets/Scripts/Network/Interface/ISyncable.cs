public interface ISyncable
{
    string SyncType { get; }
    string Serialize();
    void Deserialize(string json);
    bool IsDirty();
    void ClearDirty();
}
