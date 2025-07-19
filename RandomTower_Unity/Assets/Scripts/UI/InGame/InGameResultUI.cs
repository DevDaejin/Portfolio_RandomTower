using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InGameResultUI : MonoBehaviour
{
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _resultTitleText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _lobbyButton;

    private const string SuccessColor = "<color=#F6CA3D>Success</color>";
    private const string FailedColor = "<color=#FF0000>Failed</color>";

    public void Intialize()
    {
        _retryButton.onClick.RemoveAllListeners();
        _lobbyButton.onClick.RemoveAllListeners();
    }

    public void ActiveUI(bool isAct) => _resultPanel.SetActive(isAct);

    public void SetResult(bool isSuccess)
    {
        _resultPanel.SetActive(true);
        _resultPanel.SetActive(true);
        _resultTitleText.text = isSuccess ? SuccessColor : FailedColor;
    }

    public void SetResultButtons(Action onRetry, Action onLobby)
    {
        _retryButton?.onClick.RemoveAllListeners();
        _retryButton?.onClick.AddListener(() => onRetry?.Invoke());

        _lobbyButton?.onClick.RemoveAllListeners();
        _lobbyButton?.onClick.AddListener(() => onLobby?.Invoke());
    }
}
