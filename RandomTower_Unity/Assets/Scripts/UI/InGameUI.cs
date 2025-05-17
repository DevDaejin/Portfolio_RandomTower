using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [Header("Upper UI")]
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _enemyCountText;

    [Header("Bottom UI")]
    [SerializeField] private Button _spawnButton;
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _towerCountText;

    Dictionary<StringBuilderKey, StringBuilder> stringBuilderDict = new();

    private enum StringBuilderKey
    {
        Time, Wave, Enemy, Gold, TowerCount
    }

    private StringBuilder GetStringBuilder(StringBuilderKey key)
    {
        if(!stringBuilderDict.ContainsKey(key))
        {
            stringBuilderDict.Add(key, new StringBuilder());
        }

        StringBuilder stringBuilder = stringBuilderDict[key];
        stringBuilder.Clear();

        return stringBuilder;
    }

    public void SetWave(int current, int max)
    {
        StringBuilder stringBuilder = GetStringBuilder(StringBuilderKey.Wave);
        stringBuilder.Append("Wave\n").Append(current).Append(" / ").Append(max);
        _waveText.text = stringBuilder.ToString();
    }

    public void SetTimer(float time)
    {
        int second = Mathf.CeilToInt(time);
        StringBuilder stringBuilder = GetStringBuilder(StringBuilderKey.Time);
        stringBuilder.Append("Time\n").Append($"{second:00}");
        _timerText.text = stringBuilder.ToString();
    }

    public void SetEnemyCount(int current, int max)
    {
        StringBuilder stringBuilder = GetStringBuilder(StringBuilderKey.Enemy);
        stringBuilder.Append("Enemy\n").Append(current).Append(" / ").Append(max);
        _enemyCountText.text = stringBuilder.ToString();
    }

    public void SetGoldCount (int current)
    {
        StringBuilder stringBuilder = GetStringBuilder(StringBuilderKey.Gold);
        stringBuilder.Append(current.ToString("N0"));
        _goldText.text = stringBuilder.ToString();
    }

    public void SetTowerCount(int current, int max)
    {
        StringBuilder stringBuilder = GetStringBuilder(StringBuilderKey.TowerCount);
        stringBuilder.Append(current).Append(" / ").Append(max);
        _towerCountText.text = stringBuilder.ToString();
    }



    public void SetSpawnButton(UnityAction callback)
    {
        _spawnButton.onClick.AddListener(callback);
    }

    public void ReleaseSpawnButton(UnityAction callback)
    {
        _spawnButton.onClick.RemoveListener(callback);
    }
}
