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

    [Header("Result")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _resultTitleText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _lobbyButton;

    Dictionary<StringBuilderKey, StringBuilder> stringBuilderDict = new();

    private enum StringBuilderKey
    {
        Time, Wave, Enemy, Gold, TowerCount, Success
    }

    public void Initialize(int maxWave, int maxEnemy, int maxTower, float time, int gold)
    {
        _resultPanel.SetActive(false);
        SetWave(0, maxWave);
        SetEnemyCount(0, maxEnemy);
        SetTimer(time);
        SetGoldCount(gold);
        SetTowerCount(0, maxTower);
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

    public void SetResult(bool isSuceess)
    {
        _resultPanel.SetActive(true);

        StringBuilder stringBuilder = GetStringBuilder(StringBuilderKey.Success);
        if (isSuceess)
        {
            stringBuilder.Append("<color=#F6CA3D>Success</color>");
        }
        else
        {
            stringBuilder.Append("<color=#FF0000>Failed</color>");
        }
        _resultTitleText.text = stringBuilder.ToString();
    }


    public void SetSpawnButton(UnityAction callback)
    {
        _spawnButton?.onClick.AddListener(callback);
    }

    public void ReleaseSpawnButton(UnityAction callback)
    {
        _spawnButton?.onClick.RemoveListener(callback);
    }

    public void SetResultButtons(UnityAction onRetry, UnityAction onLobby)
    {
        _retryButton?.onClick.AddListener(onRetry);
        _lobbyButton?.onClick.AddListener(onLobby);
    }

    public void ReleaseResultButtons(UnityAction onRetry, UnityAction onLobby)
    {
        _retryButton?.onClick.RemoveListener(onRetry);
        _lobbyButton?.onClick.RemoveListener(onLobby);
    }
}
