using Room;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    private LobbyUI _ui => GameManager.Instance.UI.Lobby;
    private ResourceManager _resource => GameManager.Instance.Resource;
    private NetworkManager _network => GameManager.Instance.Network;
    private LocalDataManager _data => GameManager.Instance.Data;
    private List<RoomInfo> _roomList = null;

    private float _time = 0;
    private const float RoomlistUpdateInterval = 3f;

    void Awake()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.Lobby);
    }

    private void Start()
    {
        InitializeUI();
        InitializeUICallbacks();

        _resource.SetCallback(ResourceManager.ResourceType.Gem, GemChangedCallback);
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

    private void InitializeUI()
    {
        _ui.Initialize();
        _ui.UpdateOwnedGem(GameManager.Instance.Data.Loaded.Gem);
        _ui.CreateTowerButtons(GameManager.Instance.TowerDB, GameManager.Instance.ActivedTowers);
        InitializeUICallbacks();
    }

    private void InitializeUICallbacks()
    {
        _ui.OnCreated = OnCreated;
        _ui.OnPlay = OnPlay;
        _ui.OnBack = OnBack;
        _ui.OnBought = OnBought;
        _ui.OnUpgrade = OnUpgrade;
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

    private async void OnCreated()
    {
        await _network.RoomService.CreateRoom(_ui.InputedRoomName);
        GameManager.Instance.LoadScene(GameManager.Scenes.Game);
    }

    private void OnBack()
    {
        GameManager.Instance.LoadScene(GameManager.Scenes.Main);
    }

    private void OnBought()
    {
        var data = _ui.CurrentTowerData;
        var gem = _resource.Get(ResourceManager.ResourceType.Gem);
        if (data.BuyingCoast <= gem)
        {
            _ui.UnlockTowerClicked(data);
            _ui.RefreshUnlockedButton(data);
            _resource.Spend(ResourceManager.ResourceType.Gem, data.BuyingCoast);
            _data.AddGainedTowerID(data.ID);
            _data.Save();
        }
    }

    private void OnUpgrade()
    {
        if(_ui.CurrentTowerData.LevelUp())
        {
            if (_resource.Spend(ResourceManager.ResourceType.Gem, _ui.CurrentTowerData.UpgradeCost))
            {
                _ui.UpdateTowerInfoPanel(_ui.CurrentTowerData);
            }
        }
    }

    private void GemChangedCallback(int gem)
    {
        _ui.UpdateOwnedGem(gem);
        _data.UpdateGem(gem);
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
 