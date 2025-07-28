using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class InGameResultUI : MonoBehaviour
{
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _resultTitleText;
    [SerializeField] private TMP_Text _resultDescriptionText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _lobbyButton;

    [SerializeField] private LocalizedString _localizedSuccessTitle;
    [SerializeField] private LocalizedString _localizedSuccessDescription;
    [SerializeField] private LocalizedString _localizedFailedTitle;
    [SerializeField] private LocalizedString _localizedFailedDescription;

    private Func<int> OnSuccessReward;
    private Func<int> OnFailedReward;

    private const string SuccessColor = "#F6CA3D";
    private const string FailedColor = "#FF0000";

    public void Intialize(Func<int> onSuccessReward, Func<int> onFailedReward)
    {
        OnSuccessReward = onSuccessReward;
        OnFailedReward = onFailedReward;

        _retryButton.onClick.RemoveAllListeners();
        _lobbyButton.onClick.RemoveAllListeners();
    }

    public void ActiveUI(bool isAct)
    {
        _resultPanel.SetActive(isAct);
    }

    public void SetResult(bool isSuccess, bool isMulti)
    {
        _retryButton.gameObject.SetActive(!isMulti);
        ActiveUI(true);

        _resultTitleText.text = isSuccess
            ? _localizedSuccessTitle.GetLocalizedString()
            : _localizedFailedTitle.GetLocalizedString();

        if (ColorUtility.TryParseHtmlString(isSuccess ? SuccessColor : FailedColor, out var color))
        {
            _resultTitleText.color = color;
        }

        _resultDescriptionText.text = isSuccess
            ? _localizedSuccessDescription.GetLocalizedString()
            : _localizedFailedDescription.GetLocalizedString();

        string reward = (isSuccess ? OnSuccessReward?.Invoke() : OnFailedReward?.Invoke()).ToString();

        _rewardText.text = reward;
    }

    public void SetResultButtons(Action onRetry, Action onLobby)
    {
        _retryButton?.onClick.RemoveAllListeners();
        _retryButton?.onClick.AddListener(() => onRetry?.Invoke());

        _lobbyButton?.onClick.RemoveAllListeners();
        _lobbyButton?.onClick.AddListener(() => onLobby?.Invoke());
    }
}
