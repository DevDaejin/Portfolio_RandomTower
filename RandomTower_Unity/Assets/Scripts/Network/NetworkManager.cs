using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NetworkManager : MonoBehaviour
{
    //TODO: for the test;
    [SerializeField] private string _roomID;
    private string _roomName = "room id";

    private string _address = "127.0.0.1";
    private string _port = "8765";

    private NetworkClient _client;
    private SyncObjectManager _syncObjectManager;

    public bool IsMulti { get; private set; } = false;
    public bool IsInRoom => _client.IsInRoom;

    public event Action OnConnected;
    public event Action<List<Room>> OnRoomListUpdated;

    public string ClientID => _client.ClientID;

    private void Awake()
    {
        _syncObjectManager ??= new SyncObjectManager();
    }

    private void Start()
    {
        OnConnected += () => IsMulti = true;
    }

    private void Update()
    {
        _client?.DispatchMessages();
    }
    
    private async void OnDestroy()
    {
        await _client?.LeaveRoom();
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

        try
        {
            await _client.Connect();
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("[NetworkManager] Connection cancelled.");
            return;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[NetworkManager] Connection failed: {ex.Message}");
            return;
        }
    }

    public void Disconnect() => _client?.Disconnect();

    public void CancelConnect() => _client?.CancelConnect();

    public void Register(SyncObject syncObject) => _syncObjectManager.Register(syncObject);

    public async Task RequestRoomList() => await _client.RequestRoomList();

    public async Task JoinRoom(string roomID) => await _client.JoinRoom(roomID);

    public async Task CreateRoom(string roomName) => await _client.CreateRoom(roomName);

    public async Task SpawnNetworkObject(string prefabName)
    {
        string objectID = Guid.NewGuid().ToString();
        Debug.Log($"[SnedSpawningNetworkObject] Sending prefab: {prefabName}, id: {objectID}");

        SpawnObjectPacket packet = new()
        {
            PrefabName = prefabName,
            ObjectID = objectID,
            RoomID = _client.RoomID,
            SceneID = GameManager.Instance.SceneLoader.CurrentScene
        };

        await _client.Send(packet);
        //SpawnFromPacket(packet, true);
    }

    public void SpawnFromPacket(SpawnObjectPacket packet, bool isOwner = false)
    {
        Debug.Log($"[SpawnFromPacket] Start - isOwner: {isOwner}, prefab: {packet.PrefabName}");

        Addressables.LoadAssetAsync<GameObject>(packet.PrefabName).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject gameObject = Instantiate(handle.Result);
                Debug.Log($"[SpawnFromPacket] Instantiated prefab: {packet.PrefabName}");

                SyncObject syncObject = gameObject.GetComponent<SyncObject>();
                if (syncObject != null)
                {
                    syncObject.Initialize(
                        packet.ObjectID, 
                        isOwner ? _client.ClientID : "remote", 
                        packet.RoomID, 
                        packet.SceneID,
                        GameManager.Instance.Network.ClientID);
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

    public void OnRoomListReceived(string json)
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

        Debug.Log($"[NetworkManager] Left room");

        GameManager.Instance.LoadScene(GameManager.Scenes.Main);
    }
}