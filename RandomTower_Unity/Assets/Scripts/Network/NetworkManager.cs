using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private string _roomID;
    private string _roomName = "room id";
    private string _address = "127.0.0.1";
    private string _port = "8765";

    private NetworkClient _client;
    private SyncObjectManager _syncObjectManager;

    public event Action<List<Room>> OnRoomListUpdated;

    public string ClientID => _client.ClientID;
    private async void Update()
    {
        _client?.DispatchMessages();

        if (Input.GetKeyDown(KeyCode.F1))
            await _client.CreateRoom("Test");

        if (Input.GetKeyDown(KeyCode.F2))
            await _client.JoinRoom(_roomID);

        if (Input.GetKeyDown(KeyCode.F3))
            await _client.RequestRoomList();

        if (Input.GetKeyDown(KeyCode.F4))
            await _client.LeaveRoom();

        if (Input.GetKeyDown(KeyCode.Space))
            SpawnNetworkObject("TestCube");
    }

    private async void OnApplicationQuit()
    {
        await _client?.Disconnect();
    }

    public async void Connect(string ip, string port)
    {
        _address = ip;
        _port = port;

        _client = new($"{_address}:{_port}");
        _client.OnConnected += OnConnected;

        _client.RegisterHandler("room_list", OnRoomListReceived);
        _client.RegisterHandler("room_created", OnRoomCreated);
        _client.RegisterHandler("room_joined", OnRoomJoined);
        _client.RegisterHandler("spawn", json =>
        {
            var packet = JsonConvert.DeserializeObject<SpawnObjectPacket>(json);
            SpawnFromPacket(packet, false);
        });
        _client.RegisterHandler("sync", OnReceiveSync);

        await _client.Connect();
    }

    private void OnConnected()
    {
        _syncObjectManager ??= new SyncObjectManager();
    }

    public void Register(SyncObject syncObject)
    {
        _syncObjectManager.Register(syncObject);
    }

    public void SendRaw(string json)
    {
        _client.SendRaw(json);
    }

    public void SpawnNetworkObject(string prefabName)
    {
        string objectID = Guid.NewGuid().ToString();


        SpawnObjectPacket packet = new()
        {
            PrefabName = prefabName,
            ObjectID = objectID,
            RoomID = _client.RoomID,
            SceneID = GameManager.Instance.SceneLoader.CurrentScene
        };

        SendRaw(JsonConvert.SerializeObject(packet));

        SpawnFromPacket(packet, true);
    }

    public void SpawnFromPacket(SpawnObjectPacket packet, bool isOwner = false)
    {
        Debug.Log(packet);

        Addressables.LoadAssetAsync<GameObject>(packet.PrefabName).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject gameObject = Instantiate(handle.Result);
                SyncObject syncObject = gameObject.GetComponent<SyncObject>();
                if (syncObject != null)
                {
                    syncObject.Initialize(packet.ObjectID, isOwner ? _client.ClientID : "remote", packet.RoomID, packet.SceneID);
                    _syncObjectManager.Register(syncObject);
                }
                else
                {
                    Debug.LogError($"SyncObject component not found on prefab {packet.PrefabName}");
                }
            }
            else
            {
                Debug.LogError($"Failed to load prefab {packet.PrefabName}: {handle.OperationException}");
            }
        };
    }

    public void OnReceiveSync(string json)
    {
        SyncObjectHeader packet = JsonConvert.DeserializeObject<SyncObjectHeader>(json);

        if (_syncObjectManager.TryGet(packet.ObjectID, out SyncObject syncObject))
        {
            if (!syncObject.IsOwner &&
                syncObject.RoomID == _client.RoomID &&
                syncObject.SceneID == GameManager.Instance.SceneLoader.CurrentScene)
            {
                syncObject.Deserialize(json);
            }
        }
    }

    private void OnRoomListReceived(string json)
    {
        var packet = JsonConvert.DeserializeObject<RoomListPacket>(json);
        Debug.Log($"[RoomList] {packet.Rooms.Count} rooms received");
        foreach (Room room in packet.Rooms)
        {
            Debug.Log($"[Room] {room.ID} - {room.Name} ({room.ClientCount})");
        }

        OnRoomListUpdated?.Invoke(packet.Rooms);
    }

    private void OnRoomCreated(string json)
    {
        var packet = JsonConvert.DeserializeObject<RoomCreatedPacket>(json);
        Debug.Log($"[RoomCreated] RoomID: {packet.RoomID}");
        _client.RoomID = packet.RoomID;
    }

    private void OnRoomJoined(string json)
    {
        var packet = JsonConvert.DeserializeObject<RoomJoinedPacket>(json);
        Debug.Log($"[RoomJoined] RoomID: {packet.RoomID}");
        _client.RoomID = packet.RoomID;
    }

    public async void LeaveRoom()
    {
        if (!_client.IsInRoom)
        {
            Debug.LogWarning("[NetworkManager] Cannot leave room: not currently in any room.");
            return;
        }

        await _client.LeaveRoom();
        Debug.Log($"[RoomJoined] Leave room");
        GameManager.Instance.LoadScene(GameManager.Scenes.Main);
    }
}