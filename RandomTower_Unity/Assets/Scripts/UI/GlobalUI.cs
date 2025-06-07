using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalUI : MonoBehaviour
{
    public enum GlobalUIOption { None, Option, Quit, Error, Watting};

    [Header("Option")]
    [SerializeField] private GameObject _optionPanel;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private Button _optionConfirmButton;
    [SerializeField] private Button _optionCancelButton;

    [Header("Quit")]
    [SerializeField] private GameObject _quitPanel;
    [SerializeField] private Button _quitConfirmButton;
    [SerializeField] private Button _quitCancelButton;

    [Header("Network error")]
    [SerializeField] private GameObject _networkErrorPanel;
    [SerializeField] private Button _confirmNetworkErrorButton;
    public Action OnNetworkConfirmClicked;

    [Header("Network waitting")]
    [SerializeField] private GameObject _networkWattingPanel;
    [SerializeField] private Button _confirmNetworkWattingButton;
    public Action OnNetworkWaittingClicked;

    private void Start()
    {
        _optionConfirmButton.onClick.AddListener(OnOptionConfirm);
        _optionCancelButton.onClick.AddListener(OnOptionCancel);
        _confirmNetworkErrorButton.onClick.AddListener(OnNetworkErrorConfirm);
        _confirmNetworkWattingButton.onClick.AddListener(OnNetworkWaittingConfirm);
    }

    public void Set(GlobalUIOption option)
    {
        gameObject.SetActive(true);

        _optionPanel.SetActive(option == GlobalUIOption.Option);
        _quitPanel.SetActive(option == GlobalUIOption.Quit);
        _networkErrorPanel.SetActive(option == GlobalUIOption.Error);
        _networkWattingPanel.SetActive(option == GlobalUIOption.Watting);
    }

    private void OnOptionConfirm()
    {
        //TODO: 적용 로직
        gameObject.SetActive(false);
    }

    private void OnOptionCancel()
    {
        //TODO: 원복 로직
        gameObject.SetActive(false);
    }

    public void OnNetworkErrorConfirm()
    {
        OnNetworkConfirmClicked?.Invoke();
    }

    public void OnNetworkWaittingConfirm()
    {
        OnNetworkWaittingClicked?.Invoke();
    }

    public void SetQuitConfrimButton(Action callback)
    {
        _quitConfirmButton.onClick.AddListener(callback.Invoke);
    }

    public void SetQuitCancelButton(Action callback)
    {
        _quitCancelButton.onClick.AddListener(callback.Invoke);
    }
}
