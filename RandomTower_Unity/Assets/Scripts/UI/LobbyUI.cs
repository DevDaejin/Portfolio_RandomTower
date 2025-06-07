using Room;
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
    [SerializeField] private Button _backButton;

    public Action OnPlay;

    [Header("RoomList")]
    [SerializeField] private GameObject _roomListPanel;
    [SerializeField] private Transform _container;
    [SerializeField] private Button _roomListCancelButton;
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private TMP_InputField _createRoomNameInput;
    public string InputedRoomName => _createRoomNameInput.text;

    public Action OnCreate;
    public Action OnBack;

    private GameObjectPool<RoomButton> _roomButtons;
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
        _backButton.onClick.AddListener(OnBack.Invoke);
        _roomListCancelButton.onClick.AddListener(() => _roomListPanel.SetActive(false));
    }

    public void CreateRoomButtons(List<RoomInfo> roomList, Action<string> onEnter)
    {
        _roomButtons.ReleaseAll();

        for (int index = 0; index < roomList.Count; index++)
        {
            RoomButton target = _roomButtons.Get();
            target.transform.SetParent(_container, false);

            target.transform.SetSiblingIndex(index);
            target.Set(
                roomList[index].Name,
                roomList[index].RoomId,
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
 