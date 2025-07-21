using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private InGameStatusUI _statusUI;
    [SerializeField] private InGameResultUI _resultUI;
    [SerializeField] private InGameTowerOptionUI _towerOptionUI;

    public Button WaveButton => _waveButton;
    [SerializeField] private Button _waveButton;
    public Button SpawnButton => _spawnButton;
    [SerializeField] private Button _spawnButton;
    public Button UpgradeButton => _upgradeButton;
    [SerializeField] private Button _upgradeButton;

    public void Initialize(int maxWave, int maxEnemy, int maxTower, float time, int gold, bool isHost, Action merge, Action sell)
    {
        _resultUI.Intialize();
        _towerOptionUI.Initialize(merge, sell);        

        SetWave(0, maxWave);
        SetEnemyCount(0, maxEnemy);
        SetTimer(time);
        SetGoldCount(gold);
        SetTowerCount(0, maxTower);
        _resultUI.ActiveUI(false);

        UpgradeButton.onClick.RemoveAllListeners();
        SpawnButton.onClick.RemoveAllListeners();
        WaveButton.onClick.RemoveAllListeners();
        WaveButton.gameObject.SetActive(isHost);
    }

    public void SetInteractableWaveButton(bool isAct) => WaveButton.interactable = isAct;
    public void ActiveTowerOptionMenuUI(bool isAct) => _towerOptionUI.ActiveUI(isAct);
    public void MoveTowerOptionMenuUI(Vector3 position) => _towerOptionUI.MoveUI(position);
    public void SetInterableMergeButton(bool isAct) => _towerOptionUI.SetInterableMergeButton(isAct);
    public void SetWave(int current, int max) => _statusUI.SetWave(current, max);
    public void SetGoldCount(int current) => _statusUI.SetGoldCount(current);
    public void SetTimer(float time) => _statusUI.SetTimer(time);
    public void SetEnemyCount(int current, int max) => _statusUI.SetEnemyCount(current, max);
    public void SetTowerCount(int current, int max) => _statusUI.SetTowerCount(current, max);
    public void SetResult(bool isSuccess) => _resultUI.SetResult(isSuccess);
    public void SetResultButtons(Action onRetry, Action onLobby) => _resultUI.SetResultButtons(onRetry, onLobby);
}