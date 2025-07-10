using Room;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header("Lobby")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _backButton;

    [Header("Topbar")]
    [SerializeField] private TMP_Text _gemTxt;
    [SerializeField] private Button _optionButton;

    [Header("SubUI")]
    [SerializeField] private LobbyRoomListUI _roomList;
    [SerializeField] private LobbyTowerListUI _towerList;
    [SerializeField] private LobbyTowerInfoUI _towerInfo;
    [SerializeField] private LobbyTowerBuyingUI _towerBuying;

    public string InputedRoomName => _roomList.InputedRoomName;

    public Action OnPlay;
    public Action OnCreated;
    public Action OnBack;

    public Action OnUpgrade;
    public Action OnBought;

    public TowerData CurrentTowerData { get; private set; }

    public void Initialize()
    {
        _roomList.Initialize(OnCreated);

        _towerList.Initialize(UnlockTowerClicked, LockTowerClicked);
        _towerInfo.Initialize(()=> OnUpgrade?.Invoke());
        _towerBuying.Initialize(() => OnBought?.Invoke());

        _playButton.onClick.AddListener(() => OnPlay?.Invoke());
        _backButton.onClick.AddListener(() => OnBack?.Invoke());
    }

    
    public void UnlockTowerClicked(TowerData data)
    {
        CurrentTowerData = data;
        _towerBuying.ActiveUI(false);
        _towerInfo.ActiveUI(true);
        UpdateTowerInfoPanel(data);
    }

    private void LockTowerClicked(TowerData data)
    {
        CurrentTowerData = data;
        _towerInfo.ActiveUI(false);
        _towerBuying.ActiveUI(true);
        _towerBuying.UpdateBuyingPrice(data.BuyingCoast.ToString());
    }

    public void UpdateTowerInfoPanel(TowerData data) => _towerInfo.UpdatePanel(data);
    public void CreateRoomButtons(List<RoomInfo> roomList, Action<string> onEnter) => _roomList.CreateRoomButtons(roomList, onEnter);
    public void CreateTowerButtons(TowerDatabase database, Dictionary<int, TowerDataConfig> actived) => _towerList.CreateTowerButtons(database, actived);
    public void UpdateOwnedGem(int amount) => _gemTxt.text = amount.ToString();
    public void ActiveRoomListPanel(bool isAct) => _roomList.ActiveUI(isAct);
    public void RefreshUnlockedButton(TowerData data) => _towerList.RefreshUnlockedTowerButton(data);
}
