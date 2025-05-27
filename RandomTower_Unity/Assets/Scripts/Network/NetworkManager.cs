using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class NetworkManager : MonoBehaviour
{
    private SyncObjectManager _syncObjectManager;
    private NetworkClient _client;
    public bool IsConnect { get; private set; } = false;

    public Action OnConnected;
    public Action<List<Room>> OnRoomListUpdated;

    private void Awake()
    {
        _syncObjectManager = new SyncObjectManager();
    }

    private void Update()
    {
        _client.DispatchMessages();
    }

    public async void Connect(string ip, string port)
    {
        
        _client = new($"{ip}:{port}");
        _client.OnConnected = OnConnectComplete;
        RegisterDefaultHandlers();

        await _client.Connect();
    }

    private void OnConnectComplete()
    {
        OnConnected.Invoke();
        IsConnect = true;
    }

    public void CancelConnect()
    {
        _client.CancelConnect();
    }

    public void Disconnect()
    {
        _client?.Disconnect();
    }

    private void RegisterDefaultHandlers()
    {
        _client.RegisterHandler("room_created", OnRoomCreated);
        _client.RegisterHandler("room_joined", OnRoomJoined);
        _client.RegisterHandler("room_list", OnRoomListReceived);
        _client.RegisterHandler("room_left", OnRoomLeft);
        _client.RegisterHandler("spawn", OnSpawnReceived);
    }

    public void RegistSyncObject(SyncObject syncObject)
    {
        _syncObjectManager.Register(syncObject);
    }

    public void UnregistSyncObject(SyncObject syncObject)
    {
        _syncObjectManager.Unregister(syncObject.ObjectID);
    }

    public async Task Send(string json)
    {
        await _client.Send(json);
    }

    public async Task CreateRoom(string roomName)
    {
        await _client.CreateRoom(roomName);
    }

    public async Task JoinRoom(string roomId)
    {
        await _client.JoinRoom(roomId);
    }

    public async Task LeaveRoom()
    {
        await _client.LeaveRoom();
    }

    public async Task RequestRoomList()
    {
        await _client.RequestRoomList();
    }

    public async Task SpawnNetworkObjectawn(string addressableName)
    {
        await _client.SpawnNetworkObject(addressableName);
    }

    private void OnRoomCreated(string json)
    {
        var packet = JsonConvert.DeserializeObject<ReceiveRoomCreatedPacket>(json);
        Debug.Log($"[RoomCreated] ID: {packet.RoomID}, Name: {packet.Name}");
    }

    private void OnRoomJoined(string json)
    {
        var packet = JsonConvert.DeserializeObject<ReceiveRoomJoinedPacket>(json);
        Debug.Log($"[RoomJoined] RoomID: {packet.RoomID}, Name: {packet.Name}");

        _client.RoomID = packet.RoomID;
        _client.ClientID = packet.ClientID; // ⬅ 서버에서 받은 값 저장
    }

    private void OnRoomListReceived(string json)
    {
        Debug.Log("aa");
        var packet = JsonConvert.DeserializeObject<ReceiveRoomListPacket>(json);
        OnRoomListUpdated?.Invoke(packet.Rooms);
        foreach (var room in packet.Rooms)
        {
            Debug.Log($"[RoomList] ID: {room.RoomID}, Name: {room.Name}, Count: {room.ClientCount}");
        }
    }

    private void OnRoomLeft(string json)
    {
        Debug.Log("[RoomLeft] You have left the room.");
    }

    private void OnSpawnReceived(string json)
    {
        var packet = JsonConvert.DeserializeObject<SpawnObjectPacket>(json);

        Addressables.LoadAssetAsync<GameObject>(packet.PrefabName).Completed += handle =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[Spawn] Failed: {packet.PrefabName}");
                return;
            }

            GameObject obj = Instantiate(handle.Result);

            var syncObject = obj.GetComponent<SyncObject>();
            if (syncObject != null)
            {
                syncObject.Initialize(
                    packet.ObjectID,
                    packet.OwnerID,
                    packet.RoomID,
                    packet.SceneID,
                    _client.ClientID
                );

                _syncObjectManager.Register(syncObject);
            }
        };
    }

}