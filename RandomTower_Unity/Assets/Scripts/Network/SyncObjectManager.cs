using System;
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

    public void Unregister(string objectID)
    {
        _syncObjects.Remove(objectID);
    }

    public SyncObject GetSyncObject(string objectID)
    {
        return _syncObjects.TryGetValue(objectID, out SyncObject sync) ? sync : null;
    }
}
