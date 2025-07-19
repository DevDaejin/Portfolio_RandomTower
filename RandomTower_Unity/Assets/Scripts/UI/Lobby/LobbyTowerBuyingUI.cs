using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTowerBuyingUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerBuyingPanel;
    [SerializeField] private TMP_Text _towerBuyingPrice;
    [SerializeField] private Button _towerBuyingButton;
    
    public TowerData CurrentData { get; private set; }
    private Action _onBought;
    private const string CoastText = "Coast : ";

    public void Initialize(Action OnBought)
    {
        _onBought = OnBought;

        _towerBuyingButton.onClick.RemoveAllListeners();
        _towerBuyingButton.onClick.AddListener(Buy);

        _towerBuyingPanel.SetActive(false);
    }

    private void Buy()
    {
        _onBought?.Invoke();
    }

    public void UpdateBuyingPrice(string coast)
    {
        _towerBuyingPrice.text = CoastText + coast;
    }

    public void ActiveUI(bool isAct)
    {
        _towerBuyingPanel.SetActive(isAct);
    }
}
