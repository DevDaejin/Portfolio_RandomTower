using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameTowerOptionUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerOptionalMenuPanel;
    [SerializeField] private Button _mergeButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Button _closeButton;

    public void Initialize(Action merge, Action sell)
    {
        _mergeButton?.onClick.RemoveAllListeners();
        _mergeButton.onClick.AddListener(() => merge?.Invoke());

        _sellButton?.onClick.RemoveAllListeners();
        _sellButton.onClick.AddListener(() => sell?.Invoke());

        _closeButton?.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(() => ActiveUI(false));

        ActiveUI(false);
    }

    public void ActiveUI(bool isAct) => _towerOptionalMenuPanel.SetActive(isAct);

    public void SetInterableMergeButton(bool isAct) => _mergeButton.interactable = isAct;
}
