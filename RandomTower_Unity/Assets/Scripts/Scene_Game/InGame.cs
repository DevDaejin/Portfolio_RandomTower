using Game;
using Google.Protobuf;
using Net;
using Spawn;
using Sync;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class InGame : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stageConfigs;
    [SerializeField] private NavMeshSurface navMeshSurface;

    private TowerManager _towerManager;
    private EnemyManager _enemyManager;
    private WaveController _waveController;
    private ResourceManager _resourceManager;
    private NetworkManager _networkManager;
    private IDGenerator _idGenerator;
    private InGameUI _ui;
    private GlobalUI _globalUI;

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
        _globalUI = GameManager.Instance.UI.Global;

        _resourceManager = new ResourceManager();

        maxWave = _stageConfigs[_currentStage].WaveData.SpawnList.Count;
        _waveController = new WaveController(maxWave, MaxEnemy, WaveDuration, GetSpawningState, GetEnemyCount);
    }

    private void Start()
    {
        InitNetwork();
        navMeshSurface.BuildNavMesh();

        InitTowerManager();
        InitEnemyManager();
        InitWave();
        InitUI();

        GetEnemyCount();
    }

    private void InitNetwork()
    {
        _networkManager = GameManager.Instance.Network;

        if (_networkManager.IsConnect)
        {
            GetComponent<MultiEnviromentHandler>().Initialize(_networkManager.IsHost);

            _idGenerator = new(_networkManager.ClientID);

            _networkManager.SpawnService.OnEnemySpawned += packet => OnReceivedEnemyPacket(packet.SpawnId, packet);
            _networkManager.SpawnService.OnTowerSpawned += packet => OnReceivedTowerPacket(packet.SpawnId, packet);
            _networkManager.SpawnService.OnProjectileSpawned += packet => OnReceivedProjectilePacket(packet.SpawnId, packet);

            _networkManager.GameStateService.OnWaveStart += () =>
            {
                if (!_networkManager.IsHost) _waveController.StartWave();
            };

            _globalUI.Set(GlobalUI.GlobalUIOption.Watting);
        }
    }
    private void InitTowerManager()
    {
        _towerManager.OnTowerUpdated += _ui.SetTowerCount;
        _towerManager.OnSendSpawnTowerPacket += (id, syncObject) => OnSendSpawnPacket<SpawnTowerPacket>("tower", id, syncObject);
        _towerManager.OnSendSpawnProjectilePacket += (id, syncObject) => OnSendSpawnPacket<SpawnProjectilePacket>("projectile", id, syncObject);
        _towerManager.OnSendProejctileReturn += ForceProjectileReturn;
    }
    private void InitEnemyManager()
    {
        _enemyManager.OnReward += OnReward;
        _enemyManager.OnSendSpawnPacket += (id, syncObject) => OnSendSpawnPacket<SpawnEnemyPacket>("enemy", id, syncObject);
        _enemyManager.OnSendEnemyReturn += ForceEnemyReturn;
    }
    private void InitWave()
    {
        _waveController.OnTimeChanged += _ui.SetTimer;
        _waveController.OnWaveChanged += _ui.SetWave;
        _waveController.OnEnemyCountChanged += _ui.SetEnemyCount;
        _waveController.OnStageResult += Result;
        _waveController.OnWaveEnded += OnWave;
        _waveController.OnWaveStarted += OnWaveStarted;
        _waveController.Initialize();
    }
    private void InitUI()
    {
        _ui.Initialize(maxWave, MaxEnemy, MaxTower, WaveDuration, 0);
        _ui.SetWaveButton(OnWave);
        _ui.SetSpawnButton(SpawnTower);
        _ui.SetResultButtons(Retry, GoToLobby);
    }

    private async void OnSendSpawnPacket<T>(string type, int id, ISyncObject syncObject)
    {
        if (!_networkManager.IsConnect) return;

        syncObject.Initialize(_idGenerator.Get(), _networkManager.ClientID, _networkManager.RoomID);
        await _networkManager.SpawnService.SendSpawn(type, id.ToString(), syncObject);
    }

    private void OnReceivedEnemyPacket(string id, SpawnEnemyPacket packet)
    {
        EnemyData data = _enemyManager.GetEnemyDataWithID(int.Parse(id));
        BaseEnemy enemy = _enemyManager.GetEnemy(data);

        ISyncObject syncObject = enemy.GetComponent<ISyncObject>();
        syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

        _enemyManager.AddSpawnedEnemy(enemy);
        _networkManager.SyncService.OnSyncObjectSpawned(syncObject);
    }

    private void OnReceivedTowerPacket(string id, SpawnTowerPacket packet)
    {
        TowerData data = _towerManager.TowerDatabase.GetTowerByID(int.Parse(id));
        ITower tower = _towerManager.CreateTower(data, Vector3.down, null, null, 1);

        ISyncObject syncObject = tower.Transform.GetComponent<ISyncObject>();
        syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

        _networkManager.SyncService.OnSyncObjectSpawned(syncObject);
    }

    private void OnReceivedProjectilePacket(string id, SpawnProjectilePacket packet)
    {
        TowerData data = _towerManager.TowerDatabase.GetTowerByID(int.Parse(id));

        IProjectilePool pool = _towerManager.GetProjectilePool(data);
        Projectile projectile = pool.Get(null, Vector3.down, 0, data.ProjectileSpeed, null);

        ISyncObject syncObject = projectile.GetComponent<ISyncObject>();
        syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

        _networkManager.SyncService.OnSyncObjectSpawned(syncObject);
    }

    private void ForceProjectileReturn(Projectile projectile, ISyncObject syncObject)
    {
        var data = new SyncProjectileData
        {
            IsReturned = true
        };

        var packet = new SyncPacketData
        {
            ObjectId = syncObject.ObjectID,
            SyncType = "projectile",
            Payload = data.ToByteString()
        };

        _ = _networkManager.SendEnvelope("sync", packet);
    }

    private void ForceEnemyReturn(BaseEnemy enemy, ISyncObject syncObject)
    {
        var data = new SyncHPData
        {
            Hp = -1
        };

        var packet = new SyncPacketData
        {
            ObjectId = syncObject.ObjectID,
            SyncType = "hp",
            Payload = data.ToByteString()
        };

        _ = _networkManager.SendEnvelope("sync", packet);
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

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _ = _networkManager.RoomService.LeaveRoom();
            GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
        }
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
            SendWaveStarting();
            _waveController.StartWave();
        }
        else if (state == WaveController.WaveState.InProgress && !isSpawning && alive == 0)
        {
            _waveController.ForceTimeUp();
        }
    }

    private async void SendWaveStarting()
    {
        if (_networkManager.IsConnect 
            && _networkManager.IsHost)
        {
            var packet = new GameStatePacket
            {
                State = GameStateType.StartWave,
                RoomId = _networkManager.RoomID
            };

            await _networkManager.SendEnvelope("game_state", packet);
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
        _enemyManager.ReleaseAll();
        _towerManager.ReleaseAll();
        _resourceManager.Initialize();
        _waveController.Initialize();
        _ui.Initialize(maxWave, MaxEnemy, MaxTower, WaveDuration, 0);
    }
}