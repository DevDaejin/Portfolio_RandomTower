using Game;
using Google.Protobuf;
using Net;
using Sync;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class InGame : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stageConfigs;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private MultiEnviromentHandler _multiEnviromentHandler;

    private InGameContext _context;

    private InGameTowerHandler _towerHandler;
    private InGameEnemyHandler _enemyHandler;
    private InGameWaveHandler _waveHandler;
    private InGameNetworkHandler _networkHandler;
    private InGameUIHandler _uiHandler;

    private KeyValuePair<ResourceManager.ResourceType, int> _initialGold;
    private int _currentStage = 0;
    private int _maxWave = 0;

    private const int MaxTower = 20;
    private const int MaxEnemy = 20;
    private const float WaveDuration = 40;
    private const int InitialGoldAmount = 10;

    private void Awake()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.InGame);

        _maxWave = _stageConfigs[_currentStage].WaveData.SpawnList.Count;

        _context = new InGameContext(
            GetComponent<TowerManager>(),
            GetComponent<EnemyManager>(),
            new WaveController(_maxWave, MaxEnemy, WaveDuration, GetSpawningState, RefreshAlivedEnemyCount)
        );

        _networkHandler = new(_context, _multiEnviromentHandler);
        _towerHandler = new(_context, GameManager.Instance.TowerDB, MaxTower, OnSellTower, ForceReturnProjectile);
        _enemyHandler = new(_context, ForceReturnEnemy);
        _waveHandler = new(_context, _stageConfigs[_currentStage], OnWave);
        _uiHandler = new(_context, OnWave, OnSpawnTower, OnMergeTower, OnSellTower, OnRetry, OnGoToLobby);
    }

    private void Start()
    {
        _initialGold = new KeyValuePair<ResourceManager.ResourceType, int>(ResourceManager.ResourceType.Gold, InitialGoldAmount);
        _context.Resource.Initialize(_initialGold);
        _context.Resource.SetCallback(ResourceManager.ResourceType.Gold, _uiHandler.SetGold);

        _towerHandler.Initialize();
        _enemyHandler.Initialize();
        _networkHandler.Initialize();
        _waveHandler.Initialize();
        _uiHandler.Initialize();

        TowerGridSelectionHandler.OnSelect = grid =>
        {
            if (grid == null) return;

            var tower = grid?.GetTower();
            if (tower == null) return;

            _uiHandler.SelectTowerUI(tower, grid.IsMergeable);
        };
        TowerGridSelectionHandler.OnDeselect = () => _uiHandler.DeselectTowerUI();

        _navMeshSurface.BuildNavMesh();
        RefreshAlivedEnemyCount();
    }

    private void Update()
    {
        TowerSelecting();
        UpdateWave();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _ = _context.Network.RoomService.LeaveRoom();
            GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
        }
    }

    private void OnWave()
    {
        WaveController.WaveState state = _waveHandler.GetCurrentWaveState;
        int alive = _enemyHandler.GetAlivedEnemyCount;
        bool isSpawning = _enemyHandler.IsSpawningState;

        if (state == WaveController.WaveState.Idle)
        {
            SendWaveStarting();
            _waveHandler.StartWave();
        }

        else if (state == WaveController.WaveState.InProgress && !isSpawning && alive == 0)
        {
            _waveHandler.EndWave();
        }
    }

    private async void SendWaveStarting()
    {
        if (_networkHandler.IsConnected && _networkHandler.IsHost)
        {
            var packet = new GameStatePacket
            {
                State = GameStateType.StartWave,
                RoomId = _networkHandler.RoomID
            };

            await _networkHandler.SendEnvelope("game_state", packet);
        }
    }

    private void ForceReturnProjectile(string objectId)
    {
        var data = new SyncProjectileData
        {
            IsReturned = true
        };

        var packet = new SyncPacketData
        {
            ObjectId = objectId,
            SyncType = "projectile",
            Payload = data.ToByteString()
        };

        _ = _networkHandler.SendEnvelope("sync", packet);
    }

    private void ForceReturnEnemy(string objectId)
    {
        var data = new SyncHPData
        {
            Hp = -1
        };

        var packet = new SyncPacketData
        {
            ObjectId = objectId,
            SyncType = "hp",
            Payload = data.ToByteString()
        };

        _ = _networkHandler.SendEnvelope("sync", packet);
    }

    private void TowerSelecting()
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
    }

    private void UpdateWave()
    {
        bool isClearEnemiesInWave = RefreshAlivedEnemyCount() == 0 && !GetSpawningState();
        _uiHandler.SetInteractableWaveButton(isClearEnemiesInWave);

        if (_waveHandler.IsWaveStooped) return;

        _waveHandler.Update();
    }

    private int RefreshAlivedEnemyCount() => _uiHandler.RefreshAlivedEnemyCount();

    private bool GetSpawningState() => _enemyHandler.IsSpawningState;

    private void OnSpawnTower()
    {
        //TODO: 임시코드
        _towerHandler.OnSpawnTower(1);
    }

    private void OnMergeTower()
    {
        var grid = TowerGridSelectionHandler.Current;

        if (grid == null) return;

        _towerHandler.MergeTower(grid);
    }

    private void OnSellTower()
    {
        var grid = TowerGridSelectionHandler.Current;
        _towerHandler.SellTower(grid.GetTower());
        grid.RemoveTower();

        var tower = grid.GetTower();
        if (tower == null)
        {
            _uiHandler.DeselectTowerUI();
            TowerGridSelectionHandler.Clear();
        }
        else
        {
            tower.ShowRange(true);
        }

        _uiHandler.SetInteractableMergeButton(grid.IsMergeable);
        _uiHandler.RefreshInstalledTowerCount();
    }

    private void OnSellTower(int sellingPrice) => _context.Resource.Earn(ResourceManager.ResourceType.Gold, sellingPrice);

    private void OnGoToLobby() => GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);

    private void OnRetry()
    {
        _context.Resource.Reset();
        _enemyHandler.Reset();
        _towerHandler.Reset();
        _waveHandler.Reset();
        _uiHandler.Reset();
    }
}