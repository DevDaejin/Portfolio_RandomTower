using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameTowerOptionUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerOptionalMenuPanel;
    [SerializeField] private Button _mergeButton;
    [SerializeField] private Button _sellButton;

    private Camera _camera;

    public void Initialize(Action merge, Action sell)
    {
        _mergeButton?.onClick.RemoveAllListeners();
        _mergeButton.onClick.AddListener(() => merge?.Invoke());

        _sellButton?.onClick.RemoveAllListeners();
        _sellButton.onClick.AddListener(() => sell?.Invoke());
    }

    public void ActiveUI(bool isAct) => _towerOptionalMenuPanel.SetActive(isAct);

    public void SetInterableMergeButton(bool isAct) => _mergeButton.interactable = isAct;

    public void MoveUI(Vector3 position)
    {
        _camera ??= Camera.main;
        var newPosition = _camera.WorldToScreenPoint(position);
        _towerOptionalMenuPanel.transform.position = newPosition;
    }
}
