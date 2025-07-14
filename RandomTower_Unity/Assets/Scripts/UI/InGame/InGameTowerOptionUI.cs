using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameTowerOptionUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerOptionalMenuPanel;
    public Button MergeButton => _mergeButton;
    [SerializeField] private Button _mergeButton;

    public Button SellButton => _sellButton;
    [SerializeField] private Button _sellButton;

    public void Initialize(Action merge, Action sell)
    {
        MergeButton?.onClick.RemoveAllListeners();
        SellButton?.onClick.RemoveAllListeners();

        MergeButton.onClick.AddListener(() => merge?.Invoke());
        SellButton.onClick.AddListener(() => sell?.Invoke());
    }

    public void ActiveUI(bool isAct) => _towerOptionalMenuPanel.SetActive(isAct);
}
