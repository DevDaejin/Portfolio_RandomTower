using Game;
using Google.Protobuf;
using Net;
using Spawn;
using Sync;
using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class InGame : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stageConfigs;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private MultiEnviromentHandler _multiEnviromentHandler;

    private InGameContext _context;
    
    private InGameTowerHandler _towerHandler;
    private InGameEnemyHandler _enemyHandler;
    private InGameWaveHandler _waveHandler;
    private InGameNetworkHandler _networkHandler;
    private InGameUIHandler _uiHandler;

    private KeyValuePair<ResourceManager.ResourceType, int> _initialGold;
    private int _currentStage = 0;
    private int maxWave = 0;

    private const int MaxTower = 20;
    private const int MaxEnemy = 20;
    private const float WaveDuration = 40;
    private const int InitialGoldAmount = 10;

    private void Awake()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.InGame);

        maxWave = _stageConfigs[_currentStage].WaveData.SpawnList.Count;

        _context = new InGameContext(
            GetComponent<TowerManager>(),
            GetComponent<EnemyManager>(),
            new WaveController(maxWave, MaxEnemy, WaveDuration, GetSpawningState, GetEnemyCount)
        );

        _networkHandler = new(_context, _multiEnviromentHandler);
        _towerHandler = new(_context, MaxTower, ForceReturnProjectile);
        _enemyHandler = new(_context, ForceReturnEnemy);
        _waveHandler = new(_context, _stageConfigs[_currentStage], OnWave);
        _uiHandler = new(_context, OnWave, OnSpawnTower);
    }

    private void Start()
    {
        _towerHandler.Initialize();
        _enemyHandler.Initialize();
        _networkHandler.Initialize();
        _waveHandler.Initialize();
        _uiHandler.Initialize();

        navMeshSurface.BuildNavMesh();
        //InitNetwork();
        //InitTowerManager();
        //InitEnemyManager();
        //InitWave();
        //InitUI();

        _initialGold = new KeyValuePair<ResourceManager.ResourceType, int>(ResourceManager.ResourceType.Gold, InitialGoldAmount);
        _context.Resource.Initialize(_initialGold);

        GetEnemyCount();
    }

    //private void InitNetwork()
    //{
    //    _network = GameManager.Instance.Network;

    //    if (_network.IsConnect)
    //    {
    //        GetComponent<MultiEnviromentHandler>().Initialize(_network.IsHost);

    //        _idGenerator = new(_network.ClientID);

    //        _network.SpawnService.OnEnemySpawned += packet => OnReceivedEnemyPacket(packet.SpawnId, packet);
    //        _network.SpawnService.OnTowerSpawned += packet => OnReceivedTowerPacket(packet.SpawnId, packet);
    //        _network.SpawnService.OnProjectileSpawned += packet => OnReceivedProjectilePacket(packet.SpawnId, packet);

    //        _network.GameStateService.OnWaveStart += () =>
    //        {
    //            if (!_network.IsHost) _wave.StartWave();
    //        };

    //        _globalUI.Set(GlobalUI.GlobalUIOption.Watting);
    //    }
    //}

    //private void InitTowerManager()
    //{
    //    _tower.Initialize(_enemy, MaxTower);

    //    _tower.OnTowerUpdated += _ui.SetTowerCount;
    //    _tower._onSendSpawnTowerPacket += (id, syncObject) => OnSendSpawnPacket<SpawnTowerPacket>("tower", id, syncObject);
    //    _tower._onSendSpawnProjectilePacket += (id, syncObject) => OnSendSpawnPacket<SpawnProjectilePacket>("projectile", id, syncObject);
    //    if(_network.IsConnect) _tower._onSendReturnProejctile += ForceReturnProjectile;
    //}
    //private void InitEnemyManager()
    //{
    //    _enemy.OnReward += OnReward;
    //    _enemy.OnSendSpawnPacket += (id, syncObject) => OnSendSpawnPacket<SpawnEnemyPacket>("enemy", id, syncObject);
    //    if (_network.IsConnect) _enemy.OnSendEnemyReturn += ForceReturnEnemy;
    //}
    //private void InitWave()
    //{
    //    _wave.OnTimeChanged += _ui.SetTimer;
    //    _wave.OnWaveChanged += _ui.SetWave;
    //    _wave.OnEnemyCountChanged += _ui.SetEnemyCount;
    //    _wave.OnStageResult += Result;
    //    _wave.OnWaveEnded += OnWave;
    //    _wave.OnWaveStarted += OnWaveStarted;
    //    _wave.Initialize();
    //}
    //private void InitUI()
    //{
    //    _ui.Initialize(maxWave, MaxEnemy, MaxTower, WaveDuration, 0, _network.IsHost);
    //    _ui.SetWaveButton(OnWave);
    //    _ui.SetSpawnButton(OnSpawnTower);
    //    _ui.SetResultButtons(Retry, GoToLobby);
    //}

    //public async void OnSendSpawnPacket<T>(string type, int id, ISyncObject syncObject)
    //{
    //    if (!_context.Network.IsConnect) return;

    //    syncObject.Initialize(_idGenerator.Get(), _context.Network.ClientID, _context.Network.RoomID);
    //    await _context.Network.SpawnService.SendSpawn(type, id.ToString(), syncObject);
    //}

    //private async void OnSendSpawnPacket<T>(string type, int id, ISyncObject syncObject)
    //{
    //    if (!_network.IsConnect) return;

    //    syncObject.Initialize(_idGenerator.Get(), _network.ClientID, _network.RoomID);
    //    await _network.SpawnService.SendSpawn(type, id.ToString(), syncObject);
    //}

    //private void OnReceivedEnemyPacket(string id, SpawnEnemyPacket packet)
    //{
    //    EnemyData data = _enemy.GetEnemyDataWithID(int.Parse(id));
    //    BaseEnemy enemy = _enemy.GetEnemy(data);

    //    ISyncObject syncObject = enemy.GetComponent<ISyncObject>();
    //    syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

    //    _enemy.AddSpawnedEnemy(enemy);
    //    _network.SyncService.OnSyncObjectSpawned(syncObject);
    //}

    //private void OnReceivedTowerPacket(string id, SpawnTowerPacket packet)
    //{
    //    TowerData data = _tower.TowerDatabase.GetTowerByID(int.Parse(id)).Data;
    //    ITower tower = _tower.CreateTower(data, Vector3.down, null, null, 1);

    //    ISyncObject syncObject = tower.Transform.GetComponent<ISyncObject>();
    //    syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

    //    _network.SyncService.OnSyncObjectSpawned(syncObject);
    //}

    //private void OnReceivedProjectilePacket(string id, SpawnProjectilePacket packet)
    //{
    //    TowerData data = _tower.TowerDatabase.GetTowerByID(int.Parse(id)).Data;

    //    IProjectilePool pool = _tower.GetProjectilePool(data);
    //    Projectile projectile = pool.Get(null, Vector3.down, 0, data.ProjectileSpeed, null);

    //    ISyncObject syncObject = projectile.GetComponent<ISyncObject>();
    //    syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

    //    _network.SyncService.OnSyncObjectSpawned(syncObject);
    //}

    private void OnWave()
    {
        WaveController.WaveState state = _context.Wave.CurrentState;
        int alive = _context.Enemy.GetAlivedEnemyCount;
        bool isSpawning = _context.Enemy.IsSpawningState();
        bool isFinal = _context.Wave.IsFinalWave;

        if (isFinal) return;

        if (state == WaveController.WaveState.Idle)
        {
            SendWaveStarting();
            _context.Wave.StartWave();
        }
        else if (state == WaveController.WaveState.InProgress && !isSpawning && alive == 0)
        {
            _context.Wave.ForceTimeUp();
        }
    }

    private async void SendWaveStarting()
    {
        if (_context.Network.IsConnect
            && _context.Network.IsHost)
        {
            var packet = new GameStatePacket
            {
                State = GameStateType.StartWave,
                RoomId = _context.Network.RoomID
            };

            await _context.Network.SendEnvelope("game_state", packet);
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

        _ = _context.Network.SendEnvelope("sync", packet);
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

        _ = _context.Network.SendEnvelope("sync", packet);
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

        if (_wave == null ||
            _wave.CurrentState == WaveController.WaveState.Failed ||
            _wave.CurrentState == WaveController.WaveState.Cleared)
        {
            return;
        }

        _wave.Update();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _ = _network.RoomService.LeaveRoom();
            GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
        }
    }

    private int GetEnemyCount()
    {
        int count = _enemy.GetCurrentEnemyCount();
        _ui.SetEnemyCount(count, MaxEnemy);
        return count;
    }

    private bool GetSpawningState()
    {
        return _enemy.IsSpawningState();
    }

    






    private void OnSpawnTower()
    {
        //TODO: 임시코드
        _context.Tower.SpawnTower(1);
    }

    
    private void GoToLobby()
    {
        GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
    }

    private void Retry()
    {
        _enemy.ReleaseAll();
        _tower.ReleaseAll();
        _resource.Initialize(_initialGold);
        _wave.Initialize();
        _ui.Initialize(maxWave, MaxEnemy, MaxTower, WaveDuration, 0, _network.IsHost);
    }
}