using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
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

    [SerializeField] private LocalizedString _localizedUnlock;
    [SerializeField] private LocalizedString _localizedUpgrade;
    [SerializeField] private LocalizedString _localizedPrice;

    [SerializeField] private LocalizedString _localizedGrade;
    [SerializeField] private LocalizedString _localizedLevel;
    [SerializeField] private LocalizedString _localizedDamage;
    [SerializeField] private LocalizedString _localizedRange;
    [SerializeField] private LocalizedString _localizedFirerate;

    private object[] _priceArgs = new object[1];
    private object[] _infoArgs = new object[1];

    private Action _onUnlock;
    private Action _onUpgrade;
    private Action _buttonCallback;
    private TMP_Text _purchaseButtonText;

    public enum ButtonType { Unlock, Upgrade };
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

        _infoArgs[0] = data.Grade;
        _grade.text = GetLocalizedStringToText(_localizedGrade, _infoArgs);

        _infoArgs[0] = data.Level;
        _level.text = GetLocalizedStringToText(_localizedLevel, _infoArgs);

        _infoArgs[0] = data.Damage;
        _damage.text = GetLocalizedStringToText(_localizedDamage, _infoArgs);

        _infoArgs[0] = data.Range;
        _range.text = GetLocalizedStringToText(_localizedRange, _infoArgs);

        _infoArgs[0] = data.FireRate;
        _firerate.text = GetLocalizedStringToText(_localizedFirerate, _infoArgs);

        _purchaseButton.interactable = data.IsUpgradeable;
    }

    private void UpdatePurchase(ButtonType type, int unlockPrice, int upgradePrice)
    {
        _purchaseButtonText ??= _purchaseButton.GetComponentInChildren<TMP_Text>();

        string buttonText = string.Empty;
        string price = string.Empty;
        Action callback = null;

        switch (type)
        {
            case ButtonType.Unlock:
                buttonText = _localizedUnlock.GetLocalizedString();
                price = unlockPrice.ToString();
                callback = _onUnlock;
                break;

            case ButtonType.Upgrade:
                buttonText = _localizedUpgrade.GetLocalizedString();
                price = upgradePrice.ToString();
                callback = _onUpgrade;
                break;
        }

        _priceArgs[0] = price;
        _localizedPrice.Arguments = _priceArgs;
        _priceText.text = _localizedPrice.GetLocalizedString();
        _purchaseButtonText.text = buttonText;
        _buttonCallback = callback;
    }

    private string GetLocalizedStringToText(LocalizedString localized, object[] objects)
    {
        localized.Arguments = objects;
        return localized.GetLocalizedString();
    }

    public void ActiveUI(bool isAct)
    {
        _towerInfoPanel.SetActive(isAct);
    }
}
