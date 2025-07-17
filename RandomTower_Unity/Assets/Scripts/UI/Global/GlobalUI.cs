using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalUI : MonoBehaviour
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private GlobalMessageBoxUI _messageBoxUI;
    [SerializeField] private GlobalOptionUI _optionUI;

    public enum MessageBoxOption { None, NetworkingWaiting, Quit, NetworkingError };
    private Dictionary<MessageBoxOption, MessageBoxData> _messageBoxes = new();

    public void Initialize(OptionData optionData)
    {
        _optionUI.Initialize(optionData);
    }

    private void MoveBackground(GameObject target)
    {
        _background.SetParent(target.transform);
        _background.SetAsFirstSibling();
    }

    public void ShowMessage(MessageBoxData data) => _messageBoxUI.ShowContext(data);

    public void ShowOption(bool includeResetButton = false)
    {
        gameObject.SetActive(true);
        _optionUI.ActiveUI(true, includeResetButton);
        MoveBackground(_optionUI.Panel);
        _messageBoxUI.ActiveUI(false);
    }

    public void ShowQuit(Action positive, Action negative)
    {
        gameObject.SetActive(true);
        _messageBoxUI.ActiveUI(true);
        MoveBackground(_messageBoxUI.Panel);
        _optionUI.ActiveUI(false);

        if(!_messageBoxes.ContainsKey(MessageBoxOption.Quit))
        {
            _messageBoxes.Add(MessageBoxOption.Quit, new()
            {
                Title = "게임 종료",
                Description = "정말로 게임을 종료하시겠습니까?",
                PositiveButtonText = "네",
                NegativeButtonText = "아니요",
                OnPositiveButtonClick = positive,
                OnNegativeButtonClick = negative,
            });
        }

        _messageBoxUI.ShowContext(_messageBoxes[MessageBoxOption.Quit]);
    }

    public void ShowNetworkWaiting()
    {
        _optionUI.ActiveUI(false);
        if(!_messageBoxes.ContainsKey(MessageBoxOption.NetworkingWaiting))
        {
            _messageBoxes.Add(MessageBoxOption.NetworkingWaiting, new()
            {
                Title = "네트워크 연결",
                Description = "서버와 연결 중입니다. 잠시만 기다려 주세요.",
                NegativeButtonText = "취소",
            });
        }

        _messageBoxUI.ShowContext(_messageBoxes[MessageBoxOption.NetworkingWaiting]);
    }
    public void ShowNetworkError()
    {
        _optionUI.ActiveUI(false);
        if (!_messageBoxes.ContainsKey(MessageBoxOption.NetworkingError))
        {
            _messageBoxes.Add(MessageBoxOption.NetworkingError, new()
            {
                Title = "네트워크 에러",
                Description = "네트워크 연결이 끊겼습니다.",
                PositiveButtonText = "확인",
            });
        }

        _messageBoxUI.ShowContext(_messageBoxes[MessageBoxOption.NetworkingError]);
    }
}
