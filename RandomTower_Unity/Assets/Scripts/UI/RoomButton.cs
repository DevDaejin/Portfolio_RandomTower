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


    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        _onClicked?.Invoke(_roomID);
    }

    public void Set(string roomName, string roomID, Action<string> buttonCallback)
    {
        _buttonTxt.text = roomName;
        _roomID = roomID;
        _onClicked = buttonCallback;
    }
}
