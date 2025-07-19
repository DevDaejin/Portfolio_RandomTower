using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTowerInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerInfoPanel;
    [SerializeField] private Image _picture;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _grade;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _damage;

    [SerializeField] private TMP_Text _range;
    [SerializeField] private TMP_Text _firerate;

    [SerializeField] private Button _upgradeButton;

    public void Initialize(Action OnUpgradeClicked)
    {
        _towerInfoPanel.SetActive(false);

        _upgradeButton.onClick.RemoveAllListeners();
        _upgradeButton.onClick.AddListener(() => OnUpgradeClicked?.Invoke());
    }

    public void UpdatePanel(TowerData data)
    {
        _picture.sprite = data.TowerSprite;
        _name.text = data.TowerName;
        _grade.text = $"Grade: {data.Grade}";
        _level.text = $"Level: {data.Level}";
        _damage.text = $"Damage: {data.Damage}";
        _range.text = $"Range: {data.Range}";
        _firerate.text = $"Firerate: {data.FireRate}";
        _upgradeButton.interactable = data.IsUpgradeable;
    }

    public void ActiveUI(bool isAct)
    {
        _towerInfoPanel.SetActive(isAct);
    }
}
