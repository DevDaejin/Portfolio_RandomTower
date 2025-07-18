using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUI : MonoBehaviour
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private GlobalMessageBoxUI _messageBoxUI;
    [SerializeField] private GlobalOptionUI _optionUI;
    [SerializeField] private GlobalMenuUI _menuUI;

    public enum MessageBoxOption { None, NetworkingWaiting, Quit, NetworkingError, Reset };
    private Dictionary<MessageBoxOption, MessageBoxData> _messageBoxes = new();

    public void Initialize(OptionData optionData)
    {
        _optionUI.Initialize(optionData);
        _menuUI.Initilaize(
            ()=>ShowOption(), 
            ()=>ShowQuit(Application.Quit, null)
        );
    }

    private void MoveBackground(GameObject target)
    {
        _background.SetParent(target.transform);
        _background.SetAsFirstSibling();
    }

    public void ShowMessage(MessageBoxData data)
    {
        gameObject.SetActive(true);
        _messageBoxUI.ShowContext(data);
    }

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

        var data = GetMessageBoxData(MessageBoxOption.Quit);
        data.OnPositiveButtonClick = positive;
        data.OnNegativeButtonClick = negative;

        _messageBoxUI.ShowContext(data);
    }

    public void ShowNetworkWaiting()
    {
        gameObject.SetActive(true);
        _optionUI.ActiveUI(false);
        var data = GetMessageBoxData(MessageBoxOption.NetworkingWaiting);
        _messageBoxUI.ShowContext(data);
    }
    public void ShowNetworkError()
    {
        gameObject.SetActive(true);
        _optionUI.ActiveUI(false);
        var data = GetMessageBoxData(MessageBoxOption.NetworkingError);
        _messageBoxUI.ShowContext(data);
    }

    public void ShowReset(Action resetCallback)
    {
        gameObject.SetActive(true);
        var data = GetMessageBoxData(MessageBoxOption.Reset);
        data.OnPositiveButtonClick = resetCallback;
        _messageBoxUI.ShowContext(data);
    }

    public void ShowMenu(Action backCallback)
    {
        gameObject.SetActive(true);
        MoveBackground(_menuUI.Panel);
        _menuUI.ShowContent(backCallback);
    }

    private MessageBoxData GetMessageBoxData(MessageBoxOption option)
    {
        if (!_messageBoxes.ContainsKey(option))
        {
            CreateMessageBoxData(option);
        }

        return _messageBoxes[option];
    }

    private void CreateMessageBoxData(MessageBoxOption option)
    {
        MessageBoxData data = null;

        switch (option)
        {
            case MessageBoxOption.NetworkingWaiting:
                data = new()
                {
                    Title = "네트워크 연결",
                    Description = "서버와 연결 중입니다. 잠시만 기다려 주세요.",
                    NegativeButtonText = "취소",
                };
                break;
            case MessageBoxOption.NetworkingError:
                data = new()
                {
                    Title = "네트워크 에러",
                    Description = "네트워크 연결이 끊겼습니다.",
                    PositiveButtonText = "확인",
                };
                break;

            case MessageBoxOption.Quit:
                data = new()
                {
                    Title = "게임 종료",
                    Description = "정말로 게임을 종료하시겠습니까?",
                    PositiveButtonText = "네",
                    NegativeButtonText = "아니요",
                };
                break;

            case MessageBoxOption.Reset:
                data = new()
                {
                    Title = "데이터 초기화",
                    Description = "게임 데이터 초기화하시겠습니까?",
                    PositiveButtonText = "예",
                    NegativeButtonText = "아니요",
                };
                break;
        }

        _messageBoxes.Add(option, data);
    }
}
