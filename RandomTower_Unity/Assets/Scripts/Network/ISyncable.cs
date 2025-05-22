using System;

public interface ISyncable
{
    void TrySync(Action<string> sendAction);
    void SetDirty();
    bool IsDirty();
    void ClearDirty();
    string ToJson();
}
