using System;
using System.Collections.Generic;
using UnityEngine;

public class InGame : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stageConfigs;

    private TowerManager _towerSpawner;
    private EnemyManager _enemyManager;
    private WaveController _waveController;
    private InGameUI _ui;
   
    private int _currentStage = 0;

    private const int MaxEnemies = 80;
    private const float WaveTime = 40;

    private void Awake()
    {
        _enemyManager = GetComponent<EnemyManager>();
        _towerSpawner = GetComponent<TowerManager>();
        _towerSpawner.Initialize(_enemyManager);

        GameManager.Instance.UIManager.Initialize(typeof(InGameUI));
        _ui = GameManager.Instance.UIManager.InGame;

        _waveController = new WaveController(
            _stageConfigs[_currentStage].WaveData.SpawnList.Count,
            MaxEnemies,
            WaveTime,
            ()=>_enemyManager.GetCurrentEnemyCount());
    }

    private void Start()
    {
        _waveController.OnTimeChanged += _ui.SetTimer;
        _waveController.OnWaveChanged += _ui.SetWave;
        _waveController.OnStageFailed += StageFailed;
        _waveController.OnStageCleared += StageSuccess;
        _waveController.OnWaveEnded += TryStartWave;
        _waveController.OnWaveStarted += OnWaveStarted;
    }

    private void Update()
    {
        if (_waveController == null ||
            _waveController.CurrentState == WaveController.WaveState.Failed ||
            _waveController.CurrentState == WaveController.WaveState.Cleared)
        {
            return;
        }

        _waveController.Update();

        #region Test Code
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnTower();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryStartWave();
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            _waveController.TestCode();
        }
        #endregion
    }

    private void OnDestroy()
    {
        if (_waveController == null) return;

        _waveController.OnTimeChanged -= _ui.SetTimer;
        _waveController.OnWaveChanged -= _ui.SetWave;
        _waveController.OnStageFailed -= StageFailed;
        _waveController.OnStageCleared -= StageSuccess;
        _waveController.OnWaveEnded -= TryStartWave;
        _waveController.OnWaveStarted -= OnWaveStarted;
    }

    private void OnWaveStarted()
    {
        _enemyManager.SpawnWave(_stageConfigs[_currentStage], _waveController.CurrentWaveIndex);
    }

    private void TryStartWave()
    {
        _waveController?.StartWave();
    }


    private void SpawnTower()
    {
        //TODO: 임시코드
        _towerSpawner.SpawnTower(1);
    }

    private void StageFailed()
    {
        Debug.Log("Failed");
    }

    private void StageSuccess()
    {
        Debug.Log("Success");
    }
}