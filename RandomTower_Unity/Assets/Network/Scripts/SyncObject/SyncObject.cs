using Google.Protobuf;
using Net;
using System.Collections;
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

    private Coroutine _syncRoutine;
    private float _syncInterval = 0.1f;

    public void Initialize(string objectID, string ownerID, string roomID)
    {
        ObjectID = objectID;
        OwnerID = ownerID;
        RoomID = roomID;

        _network = GameManager.Instance.Network;
        LocalClientID = _network.ClientID;

        _syncables = GetComponents<ISyncable>().ToList();
        _network.SyncObjectManager.Register(this);

        if (!IsOwner) return;

        _syncRoutine = StartCoroutine(SyncRoutine());
    }

    private IEnumerator SyncRoutine()
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(_syncInterval);
        
        yield return new WaitUntil(() => !string.IsNullOrEmpty(ObjectID));

        while (true)
        {
            foreach (ISyncable syncable in _syncables)
            {
                if (!syncable.IsDirty()) continue;

                _ = Send(syncable);
            }

            yield return wait;
        }
    }

    private async Task Send(ISyncable syncable)
    {
        SyncPacketData syncPacket = new SyncPacketData
        {
            ObjectId = this.ObjectID,
            SyncType = syncable.SyncType,
            Payload = ByteString.CopyFrom(syncable.Serialize().ToByteArray())
        };

        await _network.SendEnvelope("sync", syncPacket);
        syncable.ClearDirty();
    }

    private void OnDestroy()
    {
        _network?.SyncObjectManager.Unregister(ObjectID);
    }

    public void Receive(string syncType, ByteString payload)
    {
        foreach (ISyncable syncable in _syncables)
        {
            if (syncable.SyncType == syncType)
            {
                syncable.Deserialize(payload);
                break;
            }
        }
    }
}