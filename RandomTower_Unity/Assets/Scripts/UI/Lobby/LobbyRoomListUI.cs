using Room;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomListUI : MonoBehaviour
{
    [SerializeField] private GameObject _roomButton;
    [SerializeField] private GameObject _roomListPanel;
    [SerializeField] private Transform _roomContainer;
    [SerializeField] private Button _roomListCancelButton;
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private TMP_InputField _createRoomNameInput;

    public string InputedRoomName => _createRoomNameInput.text;

    private GameObjectPool<RoomButton> _roomButtons;
    

    public void Initialize(Action OnCreateButtonClicked)
    {
        _roomButtons = new(_roomButton, _roomContainer);

        _createRoomButton.onClick.AddListener(() => OnCreateButtonClicked?.Invoke());
        _roomListCancelButton.onClick.AddListener(() => _roomListPanel.SetActive(false));
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

    public void ActiveUI(bool isAct)
    {
        _roomListPanel.SetActive(isAct);
    }
}
