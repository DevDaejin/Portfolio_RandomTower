using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGame : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stageConfigs;

    private TowerManager _towerManager;
    private EnemyManager _enemyManager;
    private WaveController _waveController;
    private ResourceManager _resourceManager;
    private InGameUI _ui;

    private int _currentStage = 0;

    private int maxWave = 0;
    private const int MaxTower = 20;
    private const int MaxEnemy = 20;
    private const float WaveDuration = 40;

    private void Awake()
    {
        _enemyManager = GetComponent<EnemyManager>();
        _towerManager = GetComponent<TowerManager>();
        _towerManager.Initialize(_enemyManager, MaxTower);

        GameManager.Instance.UI.Initialize(UIManager.UIType.InGame);
        _ui = GameManager.Instance.UI.InGame;

        _resourceManager = new ResourceManager();

        maxWave = _stageConfigs[_currentStage].WaveData.SpawnList.Count;
        _waveController = new WaveController(maxWave, MaxEnemy, WaveDuration, GetSpawningState, GetEnemyCount);
    }

    private void Start()
    {
        _ui.Initialize(maxWave, MaxEnemy, MaxTower, WaveDuration, 0);

        _towerManager.OnTowerUpdated += _ui.SetTowerCount;
        _enemyManager.OnReward += OnReward;

        _waveController.OnTimeChanged += _ui.SetTimer;
        _waveController.OnWaveChanged += _ui.SetWave;
        _waveController.OnEnemyCountChanged += _ui.SetEnemyCount;
        _waveController.OnStageResult += Result;
        _waveController.OnWaveEnded += OnWave;
        _waveController.OnWaveStarted += OnWaveStarted;

        _waveController.Initialize();

        _ui.SetWaveButton(OnWave);
        _ui.SetSpawnButton(SpawnTower);
        _ui.SetResultButtons(Retry, GoToLobby);

        GetEnemyCount();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TowerGridSelectionHandler.TryDeselectOnEmptyClick(Input.mousePosition);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                TowerGridSelectionHandler.TryDeselectOnEmptyClick(touch.position);
            }
        }

        _ui.ActiveWaveButton(GetEnemyCount() == 0 && !GetSpawningState());

        if (_waveController == null ||
            _waveController.CurrentState == WaveController.WaveState.Failed ||
            _waveController.CurrentState == WaveController.WaveState.Cleared)
        {
            return;
        }

        _waveController.Update();
    }

    private void OnDestroy()
    {
        _ui?.ReleaseSpawnButton(SpawnTower);

        _enemyManager.OnReward -= OnReward;
        _towerManager.OnTowerUpdated -= _ui.SetTowerCount;

        _waveController.OnTimeChanged -= _ui.SetTimer;
        _waveController.OnWaveChanged -= _ui.SetWave;
        _waveController.OnStageResult -= Result;
        _waveController.OnWaveEnded -= OnWave;
        _waveController.OnWaveStarted -= OnWaveStarted;
    }



    private int GetEnemyCount()
    {
        int count = _enemyManager.GetCurrentEnemyCount();
        _ui.SetEnemyCount(count, MaxEnemy);
        return count;
    }

    private bool GetSpawningState()
    {
        return _enemyManager.IsSpawningState();
    }

    private void OnWaveStarted()
    {
        _enemyManager.SpawnWave(_stageConfigs[_currentStage], _waveController.CurrentWaveIndex);
    }

    private void OnWave()
    {
        WaveController.WaveState state = _waveController.CurrentState;
        int alive = _enemyManager.GetCurrentEnemyCount();
        bool isSpawning = _enemyManager.IsSpawningState();
        bool isFinal = _waveController.CurrentWaveIndex == maxWave;

        if (isFinal) return;

        if (state == WaveController.WaveState.Idle)
        {
            _waveController.StartWave();
        }
        else if(state == WaveController.WaveState.InProgress && !isSpawning && alive == 0)
        {
            _waveController.ForceTimeUp();
        }
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

    private void Result(bool isSuccess)
    {
        if (isSuccess)
        {
            StageSuccess();
        }
        else
        {
            StageFailed();
        }
    }
    private void StageFailed()
    {
        _ui.SetResult(false);
    }

    private void StageSuccess()
    {
        _ui.SetResult(true);
    }

    private void GoToLobby()
    {
        GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
    }

    private void Retry()
    {
        _enemyManager.ReturnAll();
        _towerManager.ReturnAll();
        _resourceManager.Initialize();
        _waveController.Initialize();
        _ui.Initialize(maxWave, MaxEnemy, MaxTower, WaveDuration, 0);
    }
}