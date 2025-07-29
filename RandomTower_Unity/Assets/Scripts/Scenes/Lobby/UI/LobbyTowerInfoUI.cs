using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
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

    private LocalizedString _localizedName;

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

        _localizedName?.Clear();
        _localizedName = new LocalizedString("TowerName", data.TowerName);
        BindLocalizedText(_localizedName, _name);

        _infoArgs[0] = data.Grade;
        BindLocalizedText(_localizedGrade, _grade, _infoArgs);

        _infoArgs[0] = data.Level;
        BindLocalizedText(_localizedLevel, _level, _infoArgs);

        _infoArgs[0] = data.Damage;
        BindLocalizedText(_localizedDamage, _damage, _infoArgs);

        _infoArgs[0] = data.Range;
        BindLocalizedText(_localizedRange, _range, _infoArgs);

        _infoArgs[0] = data.FireRate;
        BindLocalizedText(_localizedFirerate, _firerate, _infoArgs);

        _purchaseButton.interactable = data.IsUpgradeable;
    }

    private void UpdatePurchase(ButtonType type, int unlockPrice, int upgradePrice)
    {
        _purchaseButtonText ??= _purchaseButton.GetComponentInChildren<TMP_Text>();

        LocalizedString localized = type switch
        {
            ButtonType.Unlock => _localizedUnlock,
            ButtonType.Upgrade => _localizedUpgrade,
            _ => null
        };

        BindLocalizedText(localized, _purchaseButtonText);

        _priceArgs[0] = type == ButtonType.Unlock ? unlockPrice.ToString() : upgradePrice.ToString();
        BindLocalizedText(_localizedPrice, _priceText, _priceArgs);

        _buttonCallback = type == ButtonType.Unlock ? _onUnlock : _onUpgrade;
    }


    private void BindLocalizedText(LocalizedString localized, TMP_Text target, object[] argument = null)
    {
        localized.Arguments = argument;
        localized.StringChanged += (v) => target.text = v;
        localized.RefreshString();
    }

    public void ActiveUI(bool isAct)
    {
        _towerInfoPanel.SetActive(isAct);
    }
}
