using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizedDropdown : MonoBehaviour
{
    private TMP_Dropdown _dropdown;
    private Dictionary<int, string> _textDict = new();
    
    public void Initialzie()
    {
        LocalizationSettings.SelectedLocaleChanged += _ => RefreshAll();
        _dropdown ??= GetComponent<TMP_Dropdown>();

        for (int i = 0; i < _dropdown.options.Count; i++)
        {
            var data = _dropdown.options[i];
            RegisterText(i, data.text);
        }
    }

    private void RegisterText(int index, string key)
    {
        _textDict[index] = key;
        RefreshText(index, key);
    }

    private void RefreshAll()
    {
        foreach(var pair in _textDict)
        {
            RefreshText(pair.Key, pair.Value);
        }
    }

    private void RefreshText(int index, string key)
    {
        LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UI", key).Completed += handle =>
        {
            _dropdown.options[index].text = handle.Result;
            _dropdown.RefreshShownValue();
        };
    }
}
