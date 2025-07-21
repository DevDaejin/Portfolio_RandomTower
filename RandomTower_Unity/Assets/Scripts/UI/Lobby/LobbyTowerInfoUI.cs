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

    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Button _purchaseButton;

    private Action _onUnlock;
    private Action _onUpgrade;

    private Action _buttonCallback;

    private TMP_Text _purchaseButtonText;

    public enum ButtonType { Unlock, Upgrade };

    private const string CoastText = "Coast : ";
    private const string UnlockText = "Unlock";
    private const string UpgradeText = "Upgrade";
    private const string GemText = "Gem";

    public void Initialize(Action onUnlock, Action onUpgrade)
    {
        _towerInfoPanel.SetActive(false);

        _onUnlock = onUnlock;
        _onUpgrade = onUpgrade;

        _purchaseButton.onClick.RemoveAllListeners();
        _purchaseButton.onClick.AddListener(() => _buttonCallback?.Invoke());
    }

    public void UpdateInfoUI(TowerData data, ButtonType type)
    {
        ActiveUI(true);
        UpdatePanel(data);
        UpdatePurchase(type, data.BuyingCoast, data.UpgradeCost);
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
        _purchaseButton.interactable = data.IsUpgradeable;
    }

    private void UpdatePurchase(ButtonType type, int unlockPrice, int upgradePrice)
    {
        _purchaseButtonText ??= _purchaseButton.GetComponentInChildren<TMP_Text>();

        string buttonText = string.Empty;
        string price = string.Empty;
        Action callback = null;

        switch(type)
        {
            case ButtonType.Unlock:
                buttonText = UnlockText;
                price = unlockPrice.ToString();
                callback = _onUnlock;
                break;

            case ButtonType.Upgrade:
                buttonText = UpgradeText;
                price = upgradePrice.ToString();
                callback = _onUpgrade;
                break;
        }

        _priceText.text = $"{CoastText}{price}{GemText}";
        _purchaseButtonText.text = buttonText;
        _buttonCallback = callback;
    }

    public void ActiveUI(bool isAct)
    {
        _towerInfoPanel.SetActive(isAct);
    }
}
