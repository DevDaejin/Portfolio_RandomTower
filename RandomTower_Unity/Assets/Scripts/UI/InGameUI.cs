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
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _towerCountText;

    [Header("Bottom UI")]
    [SerializeField] private Button _waveButton;
    [SerializeField] private Button _spawnButton;
    [SerializeField] private Button _upgradeButton;

    [Header("Result")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _resultTitleText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _lobbyButton;

    public void Initialize(int maxWave, int maxEnemy, int maxTower, float time, int gold, bool isHost)
    {
        _resultPanel.SetActive(false);
        SetWave(0, maxWave);
        SetEnemyCount(0, maxEnemy);
        SetTimer(time);
        SetGoldCount(gold);
        SetTowerCount(0, maxTower);
        _waveButton.gameObject.SetActive(isHost);
    }

    public void SetWave(int current, int max)
    {
        _waveText.text = $"{current} / {max}";
    }

    public void SetTimer(float time)
    {
        int second = Mathf.CeilToInt(time);
        _timerText.text = $"Time\n{second:00}";
    }

    public void SetEnemyCount(int current, int max)
    {
        _enemyCountText.text = $"{current} / {max}";
    }

    public void SetGoldCount(int current)
    {
        _goldText.text = current.ToString("N0");
    }

    public void SetTowerCount(int current, int max)
    {
        _towerCountText.text = $"{current} / {max}";
    }

    public void SetResult(bool isSuccess)
    {
        _resultPanel.SetActive(true);
        _resultPanel.SetActive(true);
        _resultTitleText.text = isSuccess ? "<color=#F6CA3D>Success</color>" : "<color=#FF0000>Failed</color>";
    }

    public void ActiveWaveButton(bool isAct)
    {
        _waveButton.interactable = isAct;
    }

    public void SetWaveButton(UnityAction callback)
    {
        _waveButton.onClick.AddListener(callback);
    }

    public void ReleaseWaveButton(UnityAction callback)
    {
        _waveButton.onClick.RemoveListener(callback);
    }

    public void SetSpawnButton(UnityAction callback)
    {
        _spawnButton.onClick.AddListener(callback);
    }

    public void ReleaseSpawnButton(UnityAction callback)
    {
        _spawnButton.onClick.RemoveListener(callback);
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
