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
        _towerInfo.Initialize(OnBought, OnUpgrade);

        _playButton.onClick.RemoveAllListeners();
        _playButton.onClick.AddListener(() => OnPlay?.Invoke());

        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(() => OnBack?.Invoke());

        _optionButton.onClick.RemoveAllListeners();
        _optionButton.onClick.AddListener(() => GameManager.Instance.UI.Global.ShowMenu(OnBack));
    }

    public void UnlockTowerClicked(TowerData data)
    {
        CurrentTowerData = data;
        _towerInfo.UpdateInfoUI(data, LobbyTowerInfoUI.ButtonType.Upgrade);
    }

    private void LockTowerClicked(TowerData data)
    {
        CurrentTowerData = data;
        _towerInfo.UpdateInfoUI(data, LobbyTowerInfoUI.ButtonType.Unlock);
    }

    public void UpdateTowerInfoPanel(TowerData data) => _towerInfo.UpdatePanel(data);
    public void CreateRoomButtons(List<RoomInfo> roomList, Action<string> onEnter) => _roomList.CreateRoomButtons(roomList, onEnter);
    public void CreateTowerButtons(TowerDatabase database) => _towerList.CreateTowerButtons(database);
    public void UpdateOwnedGem(int amount) => _gemTxt.text = amount.ToString();
    public void ActiveRoomListPanel(bool isAct) => _roomList.ActiveUI(isAct);
    public void RefreshUnlockedButton(TowerData data) => _towerList.RefreshUnlockedTowerButton(data);
}
