using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class LobbyTowerListUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerButtonPrefab;
    [SerializeField] private Transform _towerContainer;

    private List<TowerButton> createdButtons = new();
    private event Action<TowerData> _onUnlockTowerClicked;
    private event Action<TowerData> _onLockTowerClicked;

    public void Initialize(Action<TowerData> onUnlockTowerClicked, Action<TowerData> onLockTowerClicked)
    {
        _onUnlockTowerClicked = onUnlockTowerClicked;
        _onLockTowerClicked = onLockTowerClicked;
    }

    public void CreateTowerButtons(TowerDatabase database)
    {
        var children = _towerContainer.GetComponentsInChildren<Transform>().ToList();
        children.Remove(_towerContainer);
        foreach(var child in children)
        {
            if(child != null && child.parent == _towerContainer)
            {
                Destroy(child.gameObject);
            }
        }

        Dictionary<int, TowerDataConfig> actived = new();
        foreach (var tower in database.ActiveTowers)
        {
            actived[tower.Data.ID] = tower;
        }

        createdButtons.Clear();
        foreach (var tower in database.Towers)
        {
            var towerButton = CreateTowerButton(
                actived.ContainsKey(tower.Data.ID),
                tower.Data
            );

            createdButtons.Add(towerButton);
        }
    }

    private TowerButton CreateTowerButton(bool isUnlock, TowerData data)
    {
        var towerButton = Instantiate(_towerButtonPrefab, _towerContainer).GetComponent<TowerButton>();
        Action<TowerData> unlockCallback = isUnlock ? null : _onLockTowerClicked;
        towerButton.Initialize(data, _onUnlockTowerClicked, unlockCallback);
        return towerButton;
    }

    public void RefreshUnlockedTowerButton(TowerData data)
    {
        foreach (var button in createdButtons)
        {
            if(button.Data.TowerName == data.TowerName)
            {
                button.Initialize(data, _onUnlockTowerClicked, null);
                return;
            }
        }
    }
}
