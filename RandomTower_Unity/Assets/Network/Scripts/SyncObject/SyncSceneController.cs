using NUnit.Framework.Constraints;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SyncSceneController : MonoBehaviour
{
    private SyncObject _syncObject;

    private const string ReservedObjectID = "scene_code_8765";

    private IEnumerator Start()
    {
        var network = GameManager.Instance.Network;
        yield return new WaitUntil(()=> network.IsConnect);

        gameObject.AddComponent<SyncScene>();
        _syncObject = gameObject.AddComponent<SyncObject>();
        yield return null;

        _syncObject.Initialize(
            ReservedObjectID,
            ReservedObjectID,
            network.RoomID);
    }
}
