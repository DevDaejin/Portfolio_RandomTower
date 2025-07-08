using System;
using System.Collections.Generic;
using UnityEngine;

public class LobbyTowerListUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerButtonPrefab;
    [SerializeField] private Transform _towerContainer;

    private event Action<TowerData> _onUnlockTowerClicked;
    private event Action<TowerData> _onLockTowerClicked;

    public void Initialize(Action<TowerData> onUnlockTowerClicked, Action<TowerData> onLockTowerClicked)
    {
        _onUnlockTowerClicked = onUnlockTowerClicked;
        _onLockTowerClicked = onLockTowerClicked;
    }

    public List<TowerButton> CreateTowerButtons(TowerDatabase database, Dictionary<string, TowerDataConfig> actived)
    {
        while (_towerContainer.childCount > 0)
        {
            Destroy(_towerContainer.GetChild(0).gameObject);
        }

        List<TowerButton> towerButtons = new();
        foreach (var tower in database.Towers)
        {
            var towerButton = CreateTowerButton(
                actived.ContainsKey(tower.Data.TowerName),
                tower.Data
            );

            towerButtons.Add(towerButton);
        }
        return towerButtons;
    }

    private TowerButton CreateTowerButton(bool isUnlock, TowerData data)
    {
        var towerButton = Instantiate(_towerButtonPrefab, _towerContainer).GetComponent<TowerButton>();
        Action<TowerData> unlockCallback = isUnlock ? null : _onLockTowerClicked;
        Debug.Log($"{data.TowerName} = {isUnlock},{_onLockTowerClicked == null},{unlockCallback == null}");
        towerButton.Initialize(data, _onUnlockTowerClicked, unlockCallback);
        return towerButton;
    }
}
