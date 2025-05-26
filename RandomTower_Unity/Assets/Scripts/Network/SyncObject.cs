using UnityEngine;

public abstract class SyncObject : MonoBehaviour
{
    public string ObjectID { get; private set; }
    public string OwnerID { get; private set; }
    public string RoomID { get; private set; }
    public string SceneID { get; private set; }
    public string LocalClientID { get; private set; }


    public bool IsOwner => OwnerID == LocalClientID;

    public void Initialize(string objectID, string ownerID, string roomID, string sceneID, string localClientID)
    {
        ObjectID = objectID;
        OwnerID = ownerID;
        RoomID = roomID;
        SceneID = sceneID;
        LocalClientID = localClientID;

        GameManager.Instance.Network.Register(this);
    }

    public abstract string Serialize();
    public abstract void Deserialize(string json);
}
