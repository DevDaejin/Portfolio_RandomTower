using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUniqueUI : MonoBehaviour
{
    [SerializeField] private GameObject _contianer;
    [SerializeField] private UniqueButton _uniqueButtonPrefab;
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private Image[] _requiredTowerImage;

    private Dictionary<int, UniqueButton> _uniqueDict = new();

    public void Initialize()
    {
        ActiveUI(false);
        _infoPanel.SetActive(false);
    }

    private void Update()
    {
        if(_contianer.activeInHierarchy)
        {
            bool active = false;
            foreach(Transform child in _contianer.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    active = true;
                    break;
                }
            }

            ActiveUI(active);
        }
    }

    public void ActiveButton(TowerCombinationData data, Action<TowerCombinationData> onSpawnUnique)
    {
        var id = data.ResultTower.Data.ID;

        if (_uniqueDict.ContainsKey(id))
        {
            var button = _uniqueDict[id];
            button.gameObject.SetActive(true);
            button.transform.SetAsFirstSibling();
            return;
        }

        onSpawnUnique += _ => _infoPanel.SetActive(false);

        _uniqueDict[id] = CreateButton(data, onSpawnUnique);
        ActiveUI(true);
    }

    private UniqueButton CreateButton(TowerCombinationData data, Action<TowerCombinationData> onSpawnUnique)
    {
        var button = Instantiate(_uniqueButtonPrefab, _contianer.transform);
        button.Initialize(data, onSpawnUnique, OnEnter, OnExit);
        return button;
    }

    private void OnEnter(List<TowerData> towers)
    {
        for (int i = 0; i < _requiredTowerImage.Length; i++)
        {
            var target = _requiredTowerImage[i];
            if (towers.Count <= i)
            {
                target.gameObject.SetActive(false);
            }
            else
            {
                
                target.gameObject.SetActive(true);
                target.sprite = towers[i].TowerSprite;
            }
        }
        
        _infoPanel.SetActive(true);
    }

    private void OnExit() => _infoPanel.SetActive(false);

    public void DeactiveButton(int id)
    {
        _uniqueDict[id].gameObject.SetActive(false);
    }

    public void ActiveUI(bool isAct)
    {
        _contianer.SetActive(isAct);
    }
}
