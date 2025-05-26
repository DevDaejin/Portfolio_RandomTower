using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject _roomButton;

    [Header("Lobby")]
    [SerializeField] private Button _playButton;

    public Action OnPlay;

    [Header("RoomList")]
    [SerializeField] private GameObject _roomListPanel;
    [SerializeField] private Transform _container;
    [SerializeField] private Button _roomListCancelButton;
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private TMP_InputField _createRoomNameInput;
    public string InputedRoomName => _createRoomNameInput.text;

    public Action OnCreate;

    private Pool<RoomButton> _roomButtons;
    private Transform _roomButtonGroup;

    private const string RoomButtonGroupName = "RoomButtons";

    private void Awake()
    {
        _roomButtonGroup = new GameObject(RoomButtonGroupName).transform;
        _roomButtons = new(_roomButton, _roomButtonGroup);
    }

    private void Start()
    {
        _createRoomButton.onClick.AddListener(OnCreateButton);
        _playButton.onClick.AddListener(OnPlayButton);
        _roomListCancelButton.onClick.AddListener(() => _roomListPanel.SetActive(false));
    }

    public void CreateRoomButton(List<Room> roomList, Action<string> onEnter)
    {
        for (int index = 0; index < roomList.Count; index++)
        {
            RoomButton roomButton = _roomButtons.Get();
            roomButton.transform.SetParent(_container, false);
            roomButton.Set(
                roomList[index].Name,
                roomList[index].ID,
                onEnter
            );
        }
    }

    public void ActiveRoomListPanel(bool isAct)
    {
        _roomListPanel.SetActive(isAct);
    }

    public void ClearAll()
    {
        _roomButtons.ReturnAll();
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
 