using Room;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
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
    public Action OnCreate;
    public Action OnBack;

    private Func<TowerData> _onBuyingTower;
    private List<TowerButton> _towerButtons;
    public void Initialize()
    {
        _roomList.Initialize(OnCreate);
        
        _towerList.Initialize(UnlockTowerClicked, LockTowerClicked);
        _towerInfo.Initialize();
        _towerBuying.Initialize(_onBuyingTower);

        _playButton.onClick.AddListener(()=> OnPlay?.Invoke());
        _backButton.onClick.AddListener(() => OnBack?.Invoke());
    }

    public void CreateRoomButtons(List<RoomInfo> roomList, Action<string> onEnter)
    {
        _roomList.CreateRoomButtons(roomList, onEnter);
    }

    public void CreateTowerButtons(TowerDatabase database, Dictionary<string, TowerDataConfig> actived)
    {
        _towerButtons = _towerList.CreateTowerButtons(database, actived);
    }

    private void UnlockTowerClicked(TowerData data)
    {
        _onBuyingTower = () => data;
        _towerInfo.ActiveUI(true);
        _towerBuying.ActiveUI(false);
    }

    private void LockTowerClicked(TowerData data)
    {
        _towerInfo.ActiveUI(false);
        _towerBuying.ActiveUI(true);
        _towerBuying.UpdateBuyingPrice(data.GemCoast.ToString());
    }

    public void UpdateGem(int amount)
    {
        _gemTxt.text = amount.ToString();
    }

    public void ActiveRoomListPanel(bool isAct)
    {
        _roomList.ActiveUI(isAct);
    }
}
 