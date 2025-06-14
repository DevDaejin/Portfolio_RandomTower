using Room;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    private LobbyUI _ui;
    private NetworkManager _network;
    private List<RoomInfo> _roomList = null;

    private float _time = 0;
    private const float RoomlistUpdateInterval = 3f;

    void Awake()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.Lobby);
        _ui = GameManager.Instance.UI.Lobby;
        _network = GameManager.Instance.Network;
    }

    private void Start()
    {
        _ui.OnCreate = OnCreate;
        _ui.OnPlay = OnPlay;
        _ui.OnBack = OnBack;
    }

    private async void Update()
    {
        if (!_network.IsConnect) return;

        _time += Time.deltaTime;
        if (_time > RoomlistUpdateInterval)
        {
            Debug.Log("[Lobby] Requesting room list...");
            _time = 0;
            await _network.RoomService.RequestRoomList();
        }
    }

    private async void OnPlay()
    {
        if (_network.IsConnect)
        {
            _network.RoomService.OnRoomListUpdated ??= UpdateRoomList;
            await _network.RoomService.RequestRoomList();
            _ui.ActiveRoomListPanel(true);
        }
        else
        {
            GameManager.Instance.LoadScene(GameManager.Scenes.Game);
        }
    }

    private async void OnCreate()
    {
        await _network.RoomService.CreateRoom(_ui.InputedRoomName);
        GameManager.Instance.LoadScene(GameManager.Scenes.Game);
    }


    private void OnBack()
    {
        GameManager.Instance.LoadScene(GameManager.Scenes.Main);
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        Debug.Log($"[UI] RoomListUpdated called. Count: {roomList.Count}");
        _roomList = roomList;
        _ui.CreateRoomButtons(_roomList, EnterRoom);
    }

    private async void EnterRoom(string roomID)
    {
        Debug.Log($"[Lobby] Joining room with ID: {roomID}");

        if (!_network.IsConnect)
        {
            Debug.LogWarning("[Lobby] Not connected to server.");
            return;
        }

        await _network.RoomService.JoinRoom(roomID);
        GameManager.Instance.LoadScene(GameManager.Scenes.Game);
    }
}
 