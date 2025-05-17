using System.Collections.Generic;
using UnityEngine;

public class InGame : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stageConfigs;

    private TowerManager _towerManager;
    private EnemyManager _enemyManager;
    private WaveController _waveController;
    private ResourceManager _resourceManager;
    private InGameUI _ui;
   
    private int _currentStage = 0;

    private const int MaxTowers = 20;
    private const int MaxEnemies = 80;
    private const float WaveDuration = 40;

    private void Awake()
    {
        _enemyManager = GetComponent<EnemyManager>();
        _towerManager = GetComponent<TowerManager>();
        _towerManager.Initialize(_enemyManager, MaxTowers);

        GameManager.Instance.UIManager.Initialize(typeof(InGameUI));
        _ui = GameManager.Instance.UIManager.InGame;

        _resourceManager = new ResourceManager();

        _waveController = new WaveController(
            _stageConfigs[_currentStage].WaveData.SpawnList.Count,
            MaxEnemies,
            WaveDuration,
            GetEnemyCount);
    }

    private void Start()
    {
        _towerManager.OnTowerUpdated += _ui.SetTowerCount;
        _enemyManager.OnReward += OnReward;

        _waveController.OnTimeChanged += _ui.SetTimer;
        _waveController.OnWaveChanged += _ui.SetWave;
        _waveController.OnEnemyCountChanged += _ui.SetEnemyCount;
        _waveController.OnStageFailed += StageFailed;
        _waveController.OnStageCleared += StageSuccess;
        _waveController.OnWaveEnded += TryStartWave;
        _waveController.OnWaveStarted += OnWaveStarted;
        _waveController.Initialize();

        _ui.SetSpawnButton(SpawnTower);

        GetEnemyCount();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridSelectionHandler.TryDeselectOnEmptyClick(Input.mousePosition);
        }

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                GridSelectionHandler.TryDeselectOnEmptyClick(touch.position);
            }
        }


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
            _waveController.StartWave();
        }
        #endregion
    }

    private void OnDestroy()
    {
        _ui?.ReleaseSpawnButton(SpawnTower);

        _enemyManager.OnReward -= OnReward;
        _towerManager.OnTowerUpdated -= _ui.SetTowerCount;

        _waveController.OnTimeChanged -= _ui.SetTimer;
        _waveController.OnWaveChanged -= _ui.SetWave;
        _waveController.OnStageFailed -= StageFailed;
        _waveController.OnStageCleared -= StageSuccess;
        _waveController.OnWaveEnded -= TryStartWave;
        _waveController.OnWaveStarted -= OnWaveStarted;
    }



    private int GetEnemyCount()
    {
        int count = _enemyManager.GetCurrentEnemyCount();
        _ui.SetEnemyCount(count, MaxEnemies);
        return count;
    }

    private void OnWaveStarted()
    {
        _enemyManager.SpawnWave(_stageConfigs[_currentStage], _waveController.CurrentWaveIndex);
    }

    private void TryStartWave()
    {
        _waveController?.StartWave();
    }

    private void OnReward(int gold)
    {
        _resourceManager.EarnGold(gold);
        _ui.SetGoldCount(_resourceManager.Gold);
    }


    private void SpawnTower()
    {
        //TODO: 임시코드
        _towerManager.SpawnTower(1);
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