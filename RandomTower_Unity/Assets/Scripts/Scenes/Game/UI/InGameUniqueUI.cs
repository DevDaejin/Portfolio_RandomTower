using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUniqueUI : MonoBehaviour
{
    [SerializeField] private GameObject _contianer;
    [SerializeField] private GameObject _uniqueButtonPrefab;

    private Dictionary<int, Button> _uniqueDict = new();

    public void Initialize()
    {
        ActiveUI(false);
    }

    public void ActiveButton(int id, Sprite sprite, Action onSpawnUnique)
    {
        if (_uniqueDict.ContainsKey(id))
        {
            var button = _uniqueDict[id];
            button.gameObject.SetActive(true);
            button.transform.SetAsFirstSibling();
            return;
        }

        _uniqueDict[id] = CreateButton(sprite, onSpawnUnique);
    }

    private Button CreateButton(Sprite sprite, Action onSpawnUnique)
    {
        var button = Instantiate(_uniqueButtonPrefab, _contianer.transform).GetComponent<Button>();
        button.GetComponent<Image>().sprite = sprite;
        button.onClick.AddListener(() => onSpawnUnique.Invoke());
        return button;
    }

    public void DeactiveButton(int id)
    {
        _uniqueDict[id].gameObject.SetActive(false);

        if(_contianer.transform.childCount == 0)
        {
            ActiveUI(false);
        }
    }

    public void ActiveUI(bool isAct) => _contianer.SetActive(isAct);
}
