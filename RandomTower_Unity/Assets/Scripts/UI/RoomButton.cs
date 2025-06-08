using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _buttonTxt;
    private string _roomID;
    private Action<string> _onClicked;
    private const int MaxUser = 2;

    private void Awake()
    {
        _button.onClick.RemoveListener(OnClick);
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        _onClicked?.Invoke(_roomID);
    }

    public void Set(string roomName, string roomID, int count, Action<string> buttonCallback)
    {
        _buttonTxt.text = roomName;
        _roomID = roomID;
        _button.interactable = count != MaxUser;

        _onClicked = buttonCallback;
        gameObject.SetActive(true);
    }
}
