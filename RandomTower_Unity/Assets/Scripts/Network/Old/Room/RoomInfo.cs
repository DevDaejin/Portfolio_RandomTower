using System;
using System.IO;
using UnityEngine;

[Serializable]
public class RoomInfo : ISyncable
{
    public int ID { get; private set; }
    public string Name { get; private set; }
    public bool IsDirty { get; private set; }

    public RoomInfo(int id, string name)
    {
        ID = id;
        Name = name;
        IsDirty = true;
    }

    public void ApplyData(BinaryReader reader)
    {
        ID = reader.ReadInt32();
        Name = reader.ReadString();
    }

    public void CollectData(BinaryWriter writer)
    {
        writer.Write(ID);
        writer.Write(Name);
    }

    public void ResetDirty()
    {
        IsDirty = false;
    }

    public void SetRoom(string name)
    {
        Name = name;
        IsDirty = true;
    }
}
