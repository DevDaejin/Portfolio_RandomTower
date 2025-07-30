using System;
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

    public enum MessageBoxOption { None, NetworkingWaiting, Quit, NetworkingError, Reset, WaitForCompanion };
    private MessageBoxOption _currentOption = MessageBoxOption.None;
    private Dictionary<MessageBoxOption, MessageBoxData> _messageBoxes = new();

    public void Initialize(OptionSetting setting, OptionSaveData data)
    {
        _optionUI.Initialize(setting, data);
        _menuUI.Initilaize(
            () => ShowOption(),
            () => ShowQuit(Application.Quit, null)
        );

        _optionUI.ActiveUI(false);
        _menuUI.ActiveUI(false);
        _messageBoxUI.ActiveUI(false);

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void MoveBackground(GameObject target)
    {
        _background.SetParent(target.transform);
        _background.anchoredPosition = Vector2.zero;
        _background.gameObject.SetActive(true);
        _background.SetAsFirstSibling();
    }

    public void ShowOption(bool includeResetButton = false)
    {
        gameObject.SetActive(true);
        _menuUI.ActiveUI(true);
        _optionUI.ActiveUI(true, includeResetButton);
        _messageBoxUI.ActiveUI(false);
    }

    public void ShowQuit(Action positive, Action negative)
    {
        gameObject.SetActive(true);
        _messageBoxUI.ActiveUI(true);
        _optionUI.ActiveUI(false);

        var data = GetMessageBoxData(MessageBoxOption.Quit);
        data.OnPositiveButtonClick = positive;
        data.OnNegativeButtonClick = negative;

        _messageBoxUI.ShowContext(data, _background);
    }

    public void ShowNetworkWaiting()
    {
        gameObject.SetActive(true);
        _optionUI.ActiveUI(false);
        var data = GetMessageBoxData(MessageBoxOption.NetworkingWaiting);
        _messageBoxUI.ShowContext(data, _background);
    }

    public void ShowWaitForCompanion(Action cancael)
    {
        gameObject.SetActive(true);
        _optionUI.ActiveUI(false);
        var data = GetMessageBoxData(MessageBoxOption.WaitForCompanion);
        data.OnNegativeButtonClick = cancael;
        _messageBoxUI.ShowContext(data, _background);
    }

    public void ShowNetworkError(Action confirm)
    {
        gameObject.SetActive(true);
        _optionUI.ActiveUI(false);
        var data = GetMessageBoxData(MessageBoxOption.NetworkingError);
        data.OnPositiveButtonClick = confirm;
        _messageBoxUI.ShowContext(data, _background);
    }

    public void ShowReset(Action resetCallback)
    {
        _currentOption = MessageBoxOption.None;
        gameObject.SetActive(true);
        var data = GetMessageBoxData(MessageBoxOption.Reset);
        data.OnPositiveButtonClick = resetCallback;
        _messageBoxUI.ShowContext(data, _background);
    }

    public void ShowMenu(Action backCallback)
    {
        _currentOption = MessageBoxOption.None;
        gameObject.SetActive(true);
        MoveBackground(_menuUI.Panel);
        _menuUI.ShowContent(backCallback);
    }

    public void CloseMessageBox()
    {
        MoveBackground(_messageBoxUI.transform.parent.gameObject);
        _messageBoxUI.ActiveUI(false);
    }


    private MessageBoxData GetMessageBoxData(MessageBoxOption option)
    {
        if (!_messageBoxes.ContainsKey(option))
        {
            CreateMessageBoxData(option);
        }

        _currentOption = option;
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

            if (key == _currentOption && _messageBoxUI.gameObject.activeInHierarchy)
            {
                _messageBoxUI.ShowContext(GetMessageBoxData(key), _background);
            }
        }
    }

    private string GetLocalized(string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("MessageBox", key);
    }
}
