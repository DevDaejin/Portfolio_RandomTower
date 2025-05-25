using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SyncRegistry
{
    private readonly Dictionary<int, ISyncView> _views = new();
    
    public void Register(ISyncView view)
    {
        if (view == null)
        {
            Debug.Log("SyncView is null.");
            return;
        }

        if (_views.ContainsKey(view.SyncID))
        {
            Debug.Log($"{view.SyncID} already registred");
            return;
        }

        _views.Add(view.SyncID, view);
    }

    public void Receive(byte[] data)
    {
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        int id = reader.ReadInt32();
        ISyncView view = Get(id);
        if (view == null)
        {
            Debug.Log("SyncView is null.");
            return;
        }
        view?.DeserializeAll(reader);
    }

    public ISyncView Get(int id) => _views.TryGetValue(id, out ISyncView view) ? view : null;
}
