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

    [Header("Tower")]
    [SerializeField] private GameObject _towerButtonPrefab;
    [SerializeField] private Transform _towerContainer;

    [Header("Tower buying")]
    [SerializeField] private GameObject _towerBuyingPanel;
    [SerializeField] private TMP_Text _selectedTowerBuyingPrice;
    [SerializeField] private Button _selectedTowerBuyingButton;

    [Header("Tower info")]
    [SerializeField] private GameObject _towerInfoPanel;
    [SerializeField] private Image _selectedTowerPicture;
    [SerializeField] private TMP_Text _selectedTowerName;
    [SerializeField] private TMP_Text _selectedTowerGrade;
    [SerializeField] private TMP_Text _selectedTowerLevel;
    [SerializeField] private TMP_Text _selectedTowerDamage;   

    [SerializeField] private TMP_Text _selectedTowerRange;
    [SerializeField] private TMP_Text _selectedTowerFirerate;
    [SerializeField] private TMP_Text _selectedTowerDescription;

    [SerializeField] private Button _selectedTowerUpgradeButton;

    [Header("Room")]
    [SerializeField] private GameObject _roomButton;
    [SerializeField] private GameObject _roomListPanel;
    [SerializeField] private Transform _roomContainer;
    [SerializeField] private Button _roomListCancelButton;
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private TMP_InputField _createRoomNameInput;
    
    public string InputedRoomName => _createRoomNameInput.text;

    public Action OnPlay;
    public Action OnCreate;
    public Action OnBack;

    public Action OnBuyingTower;

    private List<TowerButton> _towerButtons;
    private GameObjectPool<RoomButton> _roomButtons;

    private void Awake()
    {
        _roomButtons = new(_roomButton, _roomContainer);
    }

    private void Start()
    {
        _createRoomButton.onClick.AddListener(OnCreateButton);
        _roomListCancelButton.onClick.AddListener(() => _roomListPanel.SetActive(false));

        _selectedTowerBuyingButton.onClick.AddListener(() => OnBuyingTower?.Invoke());

        _towerInfoPanel.SetActive(false);
        _towerBuyingPanel.SetActive(false);

        _playButton.onClick.AddListener(OnPlayButton);
        _backButton.onClick.AddListener(OnBack.Invoke);
    }

    public void CreateTowerButtons(TowerDatabase database, Dictionary<string, TowerDataConfig> actived)
    {
        while(_towerContainer.childCount > 0)
        {
            Destroy(_towerContainer.GetChild(0).gameObject);
        }

        List<TowerButton> towers = new();
        foreach (var tower in database.Towers)
        {
            var towerButton = CreateTowerButton(
                actived.ContainsKey(tower.Data.TowerName),
                tower.Data
            );

            towers.Add(towerButton);
        }
        _towerButtons = towers;
    }

    private TowerButton CreateTowerButton(bool isUnlock, TowerData data)
    {
        var towerButton = Instantiate(_towerButtonPrefab, _towerContainer).GetComponent<TowerButton>();
        Action<TowerData> unlockCallback = isUnlock ? null : ActiveTowerBuyingPanel;
        towerButton.Initialize(data, ActiveTowerInfoPanel, unlockCallback);
        return towerButton;
    }

    private void ActiveTowerInfoPanel(TowerData data)
    {
        
        _towerInfoPanel.SetActive(true);
        _towerBuyingPanel.SetActive(false);
    }

    private void ActiveTowerBuyingPanel(TowerData data)
    {
        _towerInfoPanel.SetActive(false);
        _towerBuyingPanel.SetActive(true);

        _selectedTowerBuyingPrice.text = data.GemCoast.ToString();
    }

    public void CreateRoomButtons(List<RoomInfo> roomList, Action<string> onEnter)
    {
        _roomButtons.ReleaseAll();

        for (int index = 0; index < roomList.Count; index++)
        {
            RoomButton target = _roomButtons.Get();

            target.transform.SetSiblingIndex(index);
            target.Set(
                roomList[index].Name,
                roomList[index].RoomId,
                roomList[index].ClientCount,
                onEnter
            );
        }
    }

    public void UpdateGem(int amount)
    {
        _gemTxt.text = amount.ToString();
    }

    public void ActiveRoomListPanel(bool isAct)
    {
        _roomListPanel.SetActive(isAct);
    }

    private void OnPlayButton()
    {
        OnPlay?.Invoke();
    }

    private void OnCreateButton()
    {
        OnCreate?.Invoke();
    }
}
 