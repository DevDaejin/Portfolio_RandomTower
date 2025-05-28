using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SyncObject : MonoBehaviour, ISyncObject
{
    public string ObjectID { get; private set; }
    public string OwnerID { get; private set; }
    public string RoomID { get; private set; }
    public string LocalClientID { get; private set; }
    public bool IsOwner => OwnerID == LocalClientID;

    private List<ISyncable> _syncables = new();
    private NetworkManager _network;

    public void Initialize(string objectID, string ownerID, string roomID)
    {
        ObjectID = objectID;
        OwnerID = ownerID;
        RoomID = roomID;

        _network = GameManager.Instance.Network;
        LocalClientID = _network.ClientID;

        _syncables = GetComponents<ISyncable>().ToList();
        _network.SyncObjectManager.Register(this);
    }

    private void Update()
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

            _ = _network.Send(JsonConvert.SerializeObject(packet));

            syncable.ClearDirty();
        }
    }

    private void OnDestroy()
    {
        _network.SyncObjectManager.Unregister(ObjectID);
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