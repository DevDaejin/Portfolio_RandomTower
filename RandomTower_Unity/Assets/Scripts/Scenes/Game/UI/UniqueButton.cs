using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UniqueButton : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private EventTrigger _trigger;

    private TowerCombinationData _data;

    public void Initialize(TowerCombinationData data, Action<TowerCombinationData> onButtonClick, Action<List<TowerData>> OnButtonEnter, Action onButtonExit)
    {
        _data = data;
        _image.sprite = _data.ResultTower.Data.TowerSprite;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            onButtonClick.Invoke(_data);
            gameObject.SetActive(false);
        });

        if (_trigger.triggers.Capacity != 0) return;

        List<TowerData> requiredDatas = new();
        foreach (var required in _data.RequiredTowers)
        {
            requiredDatas.Add(required.Data);
        }

        EventTrigger.Entry enter = new() { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener(_ => OnButtonEnter.Invoke(requiredDatas));
        _trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new() { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ => onButtonExit.Invoke());
        _trigger.triggers.Add(exit);
    }
}
