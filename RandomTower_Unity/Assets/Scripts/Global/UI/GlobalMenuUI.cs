using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalMenuUI : MonoBehaviour
{
    public GameObject Panel => _menuPanel;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _closeButton;

    private Action _onBack;
    private Action _onClose;

    public void Initilaize(Action onOption, Action onQuit, Action onClose)
    {
        _onClose = onClose;

        _optionButton.onClick.RemoveAllListeners();
        _optionButton.onClick.AddListener(() => onOption.Invoke());

        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(() => _onBack?.Invoke());

        _quitButton.onClick.RemoveAllListeners();
        _quitButton.onClick.AddListener(() => onQuit?.Invoke());

        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(() =>
        {
            ActiveUI(false);
            _onClose.Invoke();
        });
    }

    public void ShowContent(Action backButtonCallback)
    {
        _onBack = backButtonCallback;
        ActiveUI(true);
    }

    public void ActiveUI(bool isAct) => _menuPanel.SetActive(isAct);
}
