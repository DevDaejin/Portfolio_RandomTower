using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    private LobbyUI _ui;
    private NetworkManager _network;
    private List<Room> _roomList = null;

    private float _time = 0;
    private const float RoomlistUpdateInterval = 3f;

    void Awake()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.Lobby);
        _ui = GameManager.Instance.UI.Lobby;
        _ui.OnCreate = OnCreate;
        _ui.OnPlay = OnPlay;

        _network = GameManager.Instance.Network;
        _network.OnRoomListUpdated += (roomList) =>
        {
            Debug.Log($"[UI] RoomListUpdated called. Count: {roomList.Count}");
            _roomList = roomList;
            _ui.CreateRoomButton(_roomList, EnterRoom);
        };
    }

    private async void Update()
    {
        if (!_network.IsConnect) return;

        _time += Time.deltaTime;
        if (_time > RoomlistUpdateInterval)
        {
            Debug.Log("[Lobby] Requesting room list...");
            _time = 0;
            await _network.RequestRoomList();
        }
    }

    private void OnPlay()
    {
        if (_network.IsConnect)
        {
            _ui.ActiveRoomListPanel(true);
        }
        else
        {
            GameManager.Instance.LoadScene(GameManager.Scenes.Game);
        }
    }

    private async void OnCreate()
    {
        await _network.CreateRoom(_ui.InputedRoomName);
        GameManager.Instance.LoadScene(GameManager.Scenes.Game);
    }


    private async void EnterRoom(string roomID)
    {
        Debug.Log($"[Lobby] Joining room with ID: {roomID}");

        if (!_network.IsConnect)
        {
            Debug.LogWarning("[Lobby] Not connected to server.");
            return;
        }

        await _network.JoinRoom(roomID);
        GameManager.Instance.LoadScene(GameManager.Scenes.Game);
    }
}
 