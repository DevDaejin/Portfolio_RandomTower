using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    private LobbyUI _ui;
    private NetworkManager _network;
    private List<Room> _roomList = null;
    void Awake()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.Lobby);
        _ui = GameManager.Instance.UI.Lobby;
        _ui.OnCreate = OnCreate;
        _ui.OnPlay = OnPlay;

        _network = GameManager.Instance.Network;
        _network.OnRoomListUpdated += (roomList)=> _roomList = roomList;
    }

    private async void OnPlay()
    {
        if (_network.IsMulti)
        {
            _ui.ActiveRoomListPanel(true);
            await _network.RequestRoomList();
            await UpdateRoomListButtons();
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

    private async Task UpdateRoomListButtons()
    {
        _ui.ClearAll();
        await WaitUntilAsync(() => _roomList != null);
        _ui.CreateRoomButton(_roomList, EnterRoom);
        _roomList = null;
    }


    private async void EnterRoom(string roomID)
    {
        Debug.Log($"[Lobby] Joining room with ID: {roomID}");

        if (!_network.IsMulti)
        {
            Debug.LogWarning("[Lobby] Not connected to server.");
            return;
        }

        if (_network.IsInRoom)
        {
            Debug.LogWarning("[Lobby] Already in a room.");
            return;
        }

        await _network.JoinRoom(roomID);
        GameManager.Instance.LoadScene(GameManager.Scenes.Game);
    }

    private async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 100)
    {
        while (!condition())
        {
            await Task.Delay(checkIntervalMs);
        }
    }


}
 