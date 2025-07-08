using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTowerInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerInfoPanel;
    [SerializeField] private Image _selectedTowerPicture;
    [SerializeField] private TMP_Text _selectedTowerName;
    [SerializeField] private TMP_Text _selectedTowerGrade;
    [SerializeField] private TMP_Text _selectedTowerLevel;
    [SerializeField] private TMP_Text _selectedTowerDamage;

    [SerializeField] private TMP_Text _selectedTowerRange;
    [SerializeField] private TMP_Text _selectedTowerFirerate;

    [SerializeField] private Button _selectedTowerUpgradeButton;

    public void Initialize()
    {
        _towerInfoPanel.SetActive(false);
    }

    public void ActiveUI(bool isAct)
    {
        _towerInfoPanel.SetActive(isAct);
    }
}
