using Game;
using Google.Protobuf;
using Net;
using Sync;
using System.Collections.Generic;
using UnityEngine;
using ResourceType = ResourceManager.ResourceType;

public class InGame : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stageConfigs;
    [SerializeField] private MultiEnviromentHandler _multiEnviromentHandler;
    [SerializeField] private AudioClip _bgmClip;

    private InGameContext _context;

    private InGameTowerHandler _towerHandler;
    private InGameEnemyHandler _enemyHandler;
    private InGameWaveHandler _waveHandler;
    private InGameNetworkHandler _networkHandler;
    private InGameUIHandler _uiHandler;

    private SoundManager _sound => GameManager.Instance.Sound;
    private InputController _inputController;

    private KeyValuePair<ResourceType, int> _initialGold;

    private StageConfig _stageConfig;

    private int _currentStage = 0;
    private int _currentChanceLevel = 1;

    private int _maxWave = 0;
    private int _spawnCount = 0;

    private int _currentCount = 0;

    private const int StartSpawnPrice = 10;
    private const int SpawnPriceWeight = 2;

    private const int StartUpgradePrice = 50;
    private const int UpgradePriceWeight = 50;

    private const int MaxTower = 20;
    private const int MaxEnemy = 20;

    private const float WaveDuration = 40;
    private const int InitialGoldAmount = 500;

    private void Awake()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.InGame);
        _stageConfig = _stageConfigs[_currentStage];
        _maxWave = _stageConfig.WaveData.SpawnList.Count;

        _inputController = new();

        _context = new InGameContext(
            GetComponent<TowerManager>(),
            GetComponent<EnemyManager>(),
            new WaveController(WaveDuration)
        );

        _networkHandler = new(_context, _multiEnviromentHandler);
        _towerHandler = new(_context, GameManager.Instance.TowerDB, MaxTower, OnSellTower, ForceReturnProjectile);
        _enemyHandler = new(_context, ForceReturnEnemy);

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

        _waveHandler = new(_context, new WaveSetting
        {
            StageConfig = _stageConfigs[_currentStage],
            MaxWave = _maxWave,
            MaxEnemies = MaxEnemy,
            WaveDuration = WaveDuration,
            OnWaveEnded = OnWave,
            GetSpawningState = GetSpawningState,
            GetAliveEnemyCount = RefreshAlivedEnemyCount
        });
    }

    private void Start()
    {
        _inputController.OnDragEnd = OnSwapTower;
        _inputController.OnSelect = SelectGrid;
        _inputController.OnDeselect = _uiHandler.DeselectTowerUI;

        _initialGold = new KeyValuePair<ResourceType, int>(ResourceType.Gold, InitialGoldAmount);

        _context.Resource.Initialize(_initialGold);
        _context.Resource.SetCallback(ResourceType.Gold, _uiHandler.SetGold);

        _towerHandler.Initialize();
        _enemyHandler.Initialize();
        _networkHandler.Initialize();
        _waveHandler.Initialize();
        _uiHandler.Initialize();

        RefreshAlivedEnemyCount();

        _sound.PlayBGM(_bgmClip);

        if (_networkHandler.IsConnected)
        {
            _networkHandler.SetUserCountCallback(UpdateUserCount);

            if (_networkHandler.IsHost) _uiHandler.ShowWaitForCompanion(GoToLobby);
        }
    }

    private void Update()
    {
        UpdateWave();
        _inputController.Raycast();
    }

    private void SelectGrid(ISelect selected)
    {
        if (selected is TowerGrid grid)
        {
            _uiHandler.SelectTowerUI(grid.GetTower(), grid.IsMergeable);
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

            await _networkHandler.SendEnvelope(NetworkConst.GameState, packet);
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
            SyncType = NetworkConst.Projectile,
            Payload = data.ToByteString()
        };

        _ = _networkHandler.SendEnvelope(NetworkConst.Sync, packet);
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

        _ = _networkHandler.SendEnvelope(NetworkConst.Sync, packet);
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

        if (_context.Resource.Spend(ResourceType.Gold, price))
        {
            _towerHandler.OnSpawnTower(_currentChanceLevel, () => _context.Resource.Earn(ResourceType.Gold, price));
        }
        RefreshUniqueUI();
    }

    private void OnMergeTower()
    {
        var selected = _inputController.SelectTarget;
        if (selected is TowerGrid grid)
        {
            _towerHandler.MergeTower(grid);
        }
        RefreshUniqueUI();
    }

    private void RefreshUniqueUI()
    {
        List<TowerCombinationData> results = _towerHandler.GetAvailableCombinations();
        _uiHandler.RefreshUnique(results, OnSpawnUnique);
    }

    private void OnSpawnUnique(TowerCombinationData data) => _towerHandler.TryCombineTowers(data);

    private void OnSellTower()
    {
        var selected = _inputController.SelectTarget;
        if (selected is TowerGrid grid)
        {
            _towerHandler.SellTower(grid.GetTower());
            var tower = grid.GetTower();
            if (tower == null)
            {
                _uiHandler.DeselectTowerUI();
            }
            else
            {
                tower.ShowRange(true);
            }

            _uiHandler.SetInteractableMergeButton(grid.IsMergeable);
            _uiHandler.RefreshInstalledTowerCount();
        }
    }

    private void OnSwapTower(Vector3 position1, Vector3 position2)
    {
        _towerHandler.SwapTower(position1, position2);
    }

    private void OnSellTower(int sellingPrice) => _context.Resource.Earn(ResourceType.Gold, sellingPrice);

    private void OnMenu()
    {
        GameManager.Instance.UI.Global.ShowMenu(GoToLobby);
    }

    private void OnGoToLobby() => GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);

    private void OnRetry()
    {
        _context.Resource.Initialize(_initialGold);
        _enemyHandler.Reset();
        _towerHandler.Reset();
        _waveHandler.Reset();
        _uiHandler.Reset();
    }

    private void OnUpgrade()
    {
        bool condition = !_towerHandler.IsUpgradeMax(_currentChanceLevel);
        var price = OnUpgradePrice();

        if (_context.Resource.Spend(ResourceType.Gold, price))
        {
            if (condition) _currentChanceLevel++;
        }

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
        _context.Resource.Earn(ResourceType.Gem, reward);

        _towerHandler.Reset();
        _enemyHandler.Reset();

        return reward;
    }
    private int OnFailedReward()
    {
        var reward = _stageConfig.FailedReward;
        _context.Resource.Earn(ResourceType.Gem, reward);

        _towerHandler.Reset();
        _enemyHandler.Reset();

        return reward;
    }

    private void UpdateUserCount(int count)
    {
        if (_networkHandler.IsHost && count > 1)
        {
            _uiHandler.CloseUI();
        }

        if (_currentCount == 2 && count <= 1)
        {
            _uiHandler.ShowNetworkError(GoToLobby);
        }

        _currentCount = count;
    }

    private void GoToLobby()
    {
        GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
        _ = _networkHandler.LeaveRoom();
        _uiHandler.CloseUI();
    }
}