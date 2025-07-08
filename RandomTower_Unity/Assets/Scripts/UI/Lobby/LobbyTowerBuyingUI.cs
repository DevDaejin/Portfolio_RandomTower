using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTowerBuyingUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerBuyingPanel;
    [SerializeField] private TMP_Text _towerBuyingPrice;
    [SerializeField] private Button _towerBuyingButton;
    
    private TowerData currentData;
    private const string CoastText = "Coast : ";

    public void Initialize(Func<TowerData> OnBuyingTower)
    {
        _towerBuyingButton.onClick.AddListener(() => currentData = OnBuyingTower?.Invoke());
        _towerBuyingPanel.SetActive(false);
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
