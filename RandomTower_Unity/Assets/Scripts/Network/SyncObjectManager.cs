using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncObjectManager
{
    private Dictionary<string, SyncObject> _syncObjects = new();

    public void Register(SyncObject syncObject)
    {
        _syncObjects[syncObject.ObjectID] = syncObject;
    }

    public bool TryGet(string id, out SyncObject syncObject) => _syncObjects.TryGetValue(id, out syncObject);

    public IEnumerable<SyncObject> GetSyncTargets(string roomID, string sceneID)
    {
        List<SyncObject> results = new();

        foreach (var obj in _syncObjects.Values)
        {
            if (obj.RoomID == roomID && obj.SceneID == sceneID)
            {
                results.Add(obj);
            }
        }

        return results;
    }

    public void Clear()
    {
        _syncObjects.Clear();
    }
}
