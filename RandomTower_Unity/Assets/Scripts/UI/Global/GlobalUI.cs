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

    public enum MessageBoxOption { None, NetworkingWaiting, Quit};
    private Dictionary<MessageBoxOption, MessageBoxData> _messageBoxes = new();


    //[Header("Option")]
    //[SerializeField] private GameObject _optionPanel;
    //[SerializeField] private TMP_Dropdown _resolutionDropdown;
    //[SerializeField] private Button _optionConfirmButton;
    //[SerializeField] private Button _optionCancelButton;

    //[Header("Network error")]
    //[SerializeField] private GameObject _networkErrorPanel;
    //[SerializeField] private Button _confirmNetworkErrorButton;
    //public Action OnNetworkConfirmClicked;

    //[Header("Network waitting")]
    //[SerializeField] private GameObject _networkWattingPanel;
    //[SerializeField] private Button _confirmNetworkWattingButton;
    //public Action OnNetworkWaittingClicked;

    private void Start()
    {
        //_optionConfirmButton.onClick.AddListener(OnOptionConfirm);
        //_optionCancelButton.onClick.AddListener(OnOptionCancel);
        //_confirmNetworkErrorButton.onClick.AddListener(OnNetworkErrorConfirm);
        //_confirmNetworkWattingButton.onClick.AddListener(OnNetworkWaittingConfirm);
    }

    public void Show(GameObject target)
    {
        gameObject.SetActive(true);
        target.SetActive(true);
        _background.SetParent(target.transform);
        _background.SetAsFirstSibling();
    }

    public void ShowOption()
    {
        Show(_optionUI.Panel);
        _messageBoxUI.ActiveUI(false);
    }

    public void ShowQuit(Action positive, Action negative)
    {
        Show(_messageBoxUI.Panel);
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

    //private void OnOptionConfirm()
    //{
    //    //TODO: 적용 로직
    //    gameObject.SetActive(false);
    //}

    //private void OnOptionCancel()
    //{
    //    //TODO: 원복 로직
    //    gameObject.SetActive(false);
    //}

    //public void OnNetworkErrorConfirm()
    //{
    //    OnNetworkConfirmClicked?.Invoke();
    //}

    //public void OnNetworkWaittingConfirm()
    //{
    //    OnNetworkWaittingClicked?.Invoke();
    //}

    //public void SetQuitConfrimButton(Action callback)
    //{
    //    _quitConfirmButton.onClick.AddListener(callback.Invoke);
    //}

    //public void SetQuitCancelButton(Action callback)
    //{
    //    _quitCancelButton.onClick.AddListener(callback.Invoke);
    //}
}
