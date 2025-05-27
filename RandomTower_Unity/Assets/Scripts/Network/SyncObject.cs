using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SyncObject : MonoBehaviour
{
    public string ObjectID { get; private set; }
    public string OwnerID { get; private set; }
    public string RoomID { get; private set; }
    public string SceneID { get; private set; }
    public string LocalClientID { get; private set; }
    public bool IsOwner => OwnerID == LocalClientID;

    private List<ISyncable> _syncables = new();

    public void Initialize(string objectId, string ownerId, string roomId, string sceneId, string localClientId)
    {
        ObjectID = objectId;
        OwnerID = ownerId;
        RoomID = roomId;
        SceneID = sceneId;
        LocalClientID = localClientId;

        _syncables = GetComponents<ISyncable>().ToList();
        GameManager.Instance.Network.RegistSyncObject(this);
    }

    private async void Update()
    {
        if (!IsOwner) return;

        foreach(ISyncable syncable in _syncables)
        {
            if (!syncable.IsDirty()) continue;

            string json = syncable.Serialize();
            SyncPacket packet = new SyncPacket
            {
                ObjectID = ObjectID,
                SyncType = syncable.SyncType,
                Payload = json
            };

            await GameManager.Instance.Network.Send(JsonConvert.SerializeObject(packet));
            syncable.ClearDirty();
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.Network.UnregistSyncObject(this);
    }

    public void Receive(string syncType, string json)
    {
        foreach(ISyncable syncable in _syncables)
        {
            if (syncable.SyncType == syncType)
            {
                syncable.Deserialize(json);
                break;
            }
        }
    }
}