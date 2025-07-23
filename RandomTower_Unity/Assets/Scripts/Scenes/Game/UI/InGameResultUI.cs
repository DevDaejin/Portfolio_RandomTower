using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InGameResultUI : MonoBehaviour
{
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _resultTitleText;
    [SerializeField] private TMP_Text _resultDescriptionText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _lobbyButton;

    private Func<int> OnSuccessReward;
    private Func<int> OnFailedReward;

    private const string SuccessText = "<color=#F6CA3D>Success</color>";
    private const string FailedText = "<color=#FF0000>Failed</color>";

    private const string SeuccessDescription = "Great job! You did it!";
    private const string FailedDescription = "You’re getting better every time, keep going!";

    public void Intialize(Func<int> onSuccessReward, Func<int> onFailedReward)
    {
        OnSuccessReward = onSuccessReward;
        OnFailedReward = onFailedReward;

        _retryButton.onClick.RemoveAllListeners();
        _lobbyButton.onClick.RemoveAllListeners();
    }

    public void ActiveUI(bool isAct) => _resultPanel.SetActive(isAct);

    public void SetResult(bool isSuccess)
    {
        ActiveUI(true);
        _resultTitleText.text = isSuccess ? SuccessText : FailedText;
        _resultDescriptionText.text = isSuccess ? SeuccessDescription : FailedDescription;
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
