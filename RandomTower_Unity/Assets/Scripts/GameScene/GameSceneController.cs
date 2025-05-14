using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stageConfigs;
    private TowerManager _towerSpawner;
    private EnemyManager _enemyManager;
    
    private int _spawnedCount = 0;
    private float _currentWaveTime = 0;
    private int _currentStage = 0;
    private int _currentWave = 0;

    private bool _isWaveInProgress = false;
    private bool _isStageEnded = false;

    private const int MaxEnemines = 80;
    private const float WaveTime = 40;

    private void Awake()
    {
        _enemyManager = GetComponent<EnemyManager>();
        _towerSpawner = GetComponent<TowerManager>();

        _towerSpawner.Initialize(_enemyManager);
    }

    private void Update()
    {
        if (_isStageEnded) return;

        //Wave();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnTower();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryStartWave();
        }
    }

    private void TryStartWave()
    {
        if (_isWaveInProgress || _spawnedCount >= MaxEnemines) return;

        List<SpawnInfo> waveList = _stageConfigs[_currentStage].WaveData.SpawnList;

        if (_currentWave >= waveList.Count) return;

        _enemyManager.SpawnWave(_stageConfigs[_currentStage], _currentWave);
        _spawnedCount += waveList[_currentWave].Count;

        _currentWaveTime = WaveTime;
        _isWaveInProgress = true;
    }

    private void Wave()
    {
        _currentWaveTime = Mathf.Max(0f, _currentWaveTime - Time.deltaTime);

        int alive = _enemyManager.GetCurrentEnemyCount();
        List<SpawnInfo> waveList = _stageConfigs[_currentStage].WaveData.SpawnList;

        bool waveFinished = _isWaveInProgress && alive == 0;

        if ((_currentWaveTime <= 0 && alive > 0 && _isWaveInProgress) || MaxEnemines < alive)
        {
            StageFailed();
            return;
        }

        if (_currentWave >= waveList.Count && alive == 0)
        {
            StageSuccess();
            return;
        }

        if(waveFinished)
        {
            _isWaveInProgress = false;
            _currentWave++;

            if(_currentWave < waveList.Count)
            {
                NextWave();
            }
        }
    }

    private void SpawnTower()
    {
        //TODO: 임시코드
        _towerSpawner.SpawnTower(1);
    }

    private void StageFailed()
    {
        Debug.Log("Failed");
        _isStageEnded = true;
    }

    private void StageSuccess()
    {
        Debug.Log("Success");
        _isStageEnded = true;
    }

    private void NextWave()
    {
        Debug.Log("Next Wave");
    }
}