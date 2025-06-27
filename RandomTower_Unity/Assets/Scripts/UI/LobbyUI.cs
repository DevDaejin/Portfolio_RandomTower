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

    [Header("Tower")]
    [SerializeField] private GameObject _towerButtonPrefab;
    [SerializeField] private Transform _towerContainer;

    [Header("Tower buying")]
    [SerializeField] private GameObject _towerBuyingPanel;
    [SerializeField] private TMP_Text _sekectedTowerBuyingPrice;
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

    private TowerButton[] _towerButtonArray;
    private GameObjectPool<RoomButton> _roomButtons;

    private void Awake()
    {
        _roomButtons = new(_roomButton, _roomContainer);
    }

    private void Start()
    {
        _createRoomButton.onClick.AddListener(OnCreateButton);
        _roomListCancelButton.onClick.AddListener(() => _roomListPanel.SetActive(false));

        _towerInfoPanel.SetActive(false);
        _towerBuyingPanel.SetActive(false);

        _playButton.onClick.AddListener(OnPlayButton);
        _backButton.onClick.AddListener(OnBack.Invoke);
    }

    public void CreateTowerButtons(TowerDatabase database)
    {
        List<TowerButton> towers = new();
        foreach(var tower in database._towers)
        {
            var towerButton = Instantiate(_towerButtonPrefab, _towerContainer).GetComponent<TowerButton>();
            towerButton.Initialize(tower.Data, ActiveTowerInfoPanel, ActiveTowerBuyingPanel);
            towers.Add(towerButton);
        }

        _towerButtonArray = towers.ToArray();
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
 