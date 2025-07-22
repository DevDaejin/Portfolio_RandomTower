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

    private StageConfig _stageConfig;

    private int _currentStage = 0;
    private int _currentChanceLevel = 1;

    private int _maxWave = 0;
    private int _spawnCount = 0;


    private const int StartSpawnPrice = 10;
    private const int SpawnPriceWeight = 2;

    private const int StartUpgradePrice = 50;
    private const int UpgradePriceWeight = 50;

    private const int MaxTower = 20;
    private const int MaxEnemy = 20;
    private const float WaveDuration = 40;
    private const int InitialGoldAmount = 50;

    private void Awake()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.InGame);

        _stageConfig = _stageConfigs[_currentStage];
        _maxWave = _stageConfig.WaveData.SpawnList.Count;

        _context = new InGameContext(
            GetComponent<TowerManager>(),
            GetComponent<EnemyManager>(),
            new WaveController(_maxWave, MaxEnemy, WaveDuration, GetSpawningState, RefreshAlivedEnemyCount)
        );

        _networkHandler = new(_context, _multiEnviromentHandler);
        _towerHandler = new(_context, GameManager.Instance.TowerDB, MaxTower, OnSellTower, ForceReturnProjectile);
        _enemyHandler = new(_context, ForceReturnEnemy);
        _waveHandler = new(_context, _stageConfigs[_currentStage], OnWave);
        _uiHandler = new(_context, new InGameUISetting()
        {
            OnWave = OnWave,
            OnSpawnTower = OnSpawnTower, 
            OnUpgrade = OnUpgrade,
            OnMerge = OnMergeTower, 
            OnSell = OnSellTower,
            OnRetry = OnRetry, 
            OnMenu = OnMenu,
            OnGoToLobby = OnGoToLobby,
            OnSpawnPrice = OnSpawnPrice, 
            OnUpgradePrice = OnUpgradePrice, 
            OnUpgradeProbability = OnUpgradeProbabilty,
            OnSuccessReward = OnSuccessReward, 
            OnFailedReward = OnFailedReward
        });
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

        TowerGridSelectionHandler.OnSelect = SelectGrid;
        TowerGridSelectionHandler.OnDeselect = _uiHandler.DeselectTowerUI;

        _navMeshSurface.BuildNavMesh();
        RefreshAlivedEnemyCount();
    }

    private void Update()
    {
        TowerSelecting();
        UpdateWave();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _ = _networkHandler.LeaveRoom();
            GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
        }
    }

    private void SelectGrid(TowerGrid grid)
    {
        if (grid == null) return;

        var tower = grid?.GetTower();
        if (tower == null) return;

        _uiHandler.SelectTowerUI(tower, grid.IsMergeable);
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
        var price = OnSpawnPrice();

        if (_context.Resource.Spend(ResourceManager.ResourceType.Gold, price))
        {
            _towerHandler.OnSpawnTower(_currentChanceLevel);
        }
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

    private void OnMenu()
    {
        GameManager.Instance.UI.Global.ShowMenu(() =>
        {
            GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
            _ = _networkHandler.LeaveRoom();
        });
    }

    private void OnGoToLobby() => GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);

    private void OnRetry()
    {
        _context.Resource.Reset();
        _enemyHandler.Reset();
        _towerHandler.Reset();
        _waveHandler.Reset();
        _uiHandler.Reset();
    }

    private void OnUpgrade()
    {
        bool condition = !_towerHandler.IsUpgradeMax(_currentChanceLevel);

        if (condition) _currentChanceLevel++;

        _uiHandler.SetInteractableUpgradeButton(condition);
    }

    private int OnUpgradePrice()
    {
        var price = StartUpgradePrice + ((_currentChanceLevel - 1) * UpgradePriceWeight);

        if (_towerHandler.IsUpgradeMax(_currentChanceLevel)) price = 0;

        return price;
    }
    private int OnSpawnPrice() => StartSpawnPrice + (_spawnCount * SpawnPriceWeight);
    private int[] OnUpgradeProbabilty()
    {
        return _towerHandler.GetProbability(_currentChanceLevel);
    }
    private int OnSuccessReward()
    {
        var reward = _stageConfig.ClearReward;
        _context.Resource.Earn(ResourceManager.ResourceType.Gem, reward);
        return reward;
    }
    private int OnFailedReward()
    {
        var reward = _stageConfig.FailedReward;
        _context.Resource.Earn(ResourceManager.ResourceType.Gem, reward);
        return reward;
    }
}