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
    private float _syncInterval = 0.001f;

    public void Initialize(string objectID, string ownerID, string roomID)
    {
        ObjectID = objectID;
        OwnerID = ownerID;
        RoomID = roomID;

        _network = GameManager.Instance.Network;
        LocalClientID = _network.ClientID;

        _syncables = GetComponents<ISyncable>().ToList();
        _network.SyncService.Register(this);

        if (IsOwner)
        {
            foreach (var syncable in _syncables)
            {
                _ = Send(syncable);
            }

            _syncRoutine = StartCoroutine(SyncRoutine());
        }
        else
        {
            _network.SyncService.OnSyncObjectSpawned(this);
        }
    }

    private IEnumerator SyncRoutine()
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(_syncInterval);

        yield return new WaitUntil(() => !string.IsNullOrEmpty(ObjectID));

        while (true)
        {
            if (!gameObject.activeInHierarchy)
            {
                yield return wait;
                continue;
            }


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

    private void OnDisable()
    {
        if (_syncRoutine != null)
        {
            StopCoroutine(_syncRoutine);
            _syncRoutine = null;
        }
    }

    private void OnDestroy()
    {
        _network?.SyncService.Unregister(ObjectID);
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