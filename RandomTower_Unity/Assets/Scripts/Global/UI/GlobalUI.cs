using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class GlobalUI : MonoBehaviour
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private GlobalMessageBoxUI _messageBoxUI;
    [SerializeField] private GlobalOptionUI _optionUI;
    [SerializeField] private GlobalMenuUI _menuUI;

    private Stack<GameObject> _uiStack = new();

    public enum MessageBoxOption { None, NetworkingWaiting, Quit, NetworkingError, Reset, WaitForCompanion };
    //private MessageBoxOption _currentOption = MessageBoxOption.None;
    private Dictionary<MessageBoxOption, MessageBoxData> _messageBoxes = new();

    public void Initialize(OptionSetting setting, OptionSaveData data)
    {
        _optionUI.Initialize(setting, data, Close);
        _menuUI.Initilaize(
            () => ShowOption(),
            () => ShowQuit(Application.Quit, Close),
            Close);
        _messageBoxUI.Initialize(Close);

        _optionUI.ActiveUI(false);
        _menuUI.ActiveUI(false);
        _messageBoxUI.ActiveUI(false);

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void SetBackground(GameObject target)
    {
        _background.SetParent(target.transform);
        _background.anchoredPosition = Vector2.zero;
        _background.gameObject.SetActive(true);
        _background.SetAsFirstSibling();
    }

    public void ShowOption(bool includeResetButton = false)
    {
        Open(_optionUI.Panel);
        _optionUI.ActiveUI(true, includeResetButton);
    }

    public void ShowQuit(Action positive, Action negative)
    {
        var data = GetMessageBoxData(MessageBoxOption.Quit);
        data.OnPositiveButtonClick = positive;
        data.OnNegativeButtonClick = negative;

        Open(_messageBoxUI.Panel);
        _messageBoxUI.ShowContext(data);
    }

    public void ShowNetworkWaiting()
    {
        var data = GetMessageBoxData(MessageBoxOption.NetworkingWaiting);

        Open(_messageBoxUI.Panel);
        _messageBoxUI.ShowContext(data);
    }

    public void ShowWaitForCompanion(Action cancael)
    {
        var data = GetMessageBoxData(MessageBoxOption.WaitForCompanion);
        data.OnNegativeButtonClick = cancael;

        Open(_messageBoxUI.Panel);
        _messageBoxUI.ShowContext(data);
    }

    public void ShowNetworkError(Action confirm)
    {
        var data = GetMessageBoxData(MessageBoxOption.NetworkingError);
        data.OnPositiveButtonClick = confirm;

        Open(_messageBoxUI.Panel);
        _messageBoxUI.ShowContext(data);
    }

    public void ShowReset(Action resetCallback)
    {
        //_currentOption = MessageBoxOption.None;
        var data = GetMessageBoxData(MessageBoxOption.Reset);
        data.OnPositiveButtonClick = resetCallback;

        Open(_messageBoxUI.Panel);
        _messageBoxUI.ShowContext(data);
    }

    public void ShowMenu(Action backCallback)
    {
        Open(_menuUI.Panel);
        _menuUI.ShowContent(backCallback);
        //_currentOption = MessageBoxOption.None;
    }

    public void Open(GameObject target)
    {
        gameObject.SetActive(true);
        _uiStack.Push(target);
        SetBackground(target);
    }

    public void Close()
    {
        if (_uiStack.Count > 0)
        {
            var old = _uiStack.Pop();
            var target = _uiStack.Count == 0 ? old : _uiStack.Peek();
            SetBackground(target);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    private MessageBoxData GetMessageBoxData(MessageBoxOption option)
    {
        if (!_messageBoxes.ContainsKey(option))
        {
            CreateMessageBoxData(option);
        }

        //_currentOption = option;
        return _messageBoxes[option];
    }

    private void CreateMessageBoxData(MessageBoxOption option, bool overWirte = false)
    {
        if (!overWirte && _messageBoxes.ContainsKey(option)) return;

        MessageBoxData data = null;

        switch (option)
        {
            case MessageBoxOption.NetworkingWaiting:
                data = new()
                {
                    Title = GetLocalized("네트워크 연결"),
                    Description = GetLocalized("서버와 연결 중입니다. 잠시만 기다려 주세요."),
                    NegativeButtonText = GetLocalized("취소"),
                };
                break;

            case MessageBoxOption.NetworkingError:
                data = new()
                {
                    Title = GetLocalized("네트워크 에러"),
                    Description = GetLocalized("네트워크 연결이 끊겼습니다."),
                    PositiveButtonText = GetLocalized("확인"),
                };
                break;

            case MessageBoxOption.WaitForCompanion:
                data = new()
                {
                    Title = GetLocalized("동료 대기 중.."),
                    Description = GetLocalized("동료 입장 시 시작 됩니다."),
                    NegativeButtonText = GetLocalized("취소"),
                };
                break;

            case MessageBoxOption.Quit:
                data = new()
                {
                    Title = GetLocalized("게임 종료"),
                    Description = GetLocalized("정말로 게임을 종료하시겠습니까?"),
                    PositiveButtonText = GetLocalized("확인"),
                    NegativeButtonText = GetLocalized("취소"),
                };
                break;

            case MessageBoxOption.Reset:
                data = new()
                {
                    Title = GetLocalized("데이터 초기화"),
                    Description = GetLocalized("게임 데이터 초기화하시겠습니까?"),
                    PositiveButtonText = GetLocalized("확인"),
                    NegativeButtonText = GetLocalized("취소"),
                };
                break;
        }

        _messageBoxes[option] = data;
    }

    private void OnLocaleChanged(Locale locale)
    {
        var keys = new List<MessageBoxOption>(_messageBoxes.Keys);

        foreach (var key in keys)
        {
            CreateMessageBoxData(key, true);

            if (/*key == _currentOption && */_messageBoxUI.gameObject.activeInHierarchy)
            {
                _messageBoxUI.ShowContext(GetMessageBoxData(key));
            }
        }
    }

    private string GetLocalized(string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("MessageBox", key);
    }
}
