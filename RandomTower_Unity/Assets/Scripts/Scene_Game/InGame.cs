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
            new WaveController(maxWave, MaxEnemy, WaveDuration, GetSpawningState, RefreshAlivedEnemyCount)
        );

        _networkHandler = new(_context, _multiEnviromentHandler);
        _towerHandler = new(_context, MaxTower, ForceReturnProjectile);
        _enemyHandler = new(_context, ForceReturnEnemy);
        _waveHandler = new(_context, _stageConfigs[_currentStage], OnWave);
        _uiHandler = new(_context, OnWave, OnSpawnTower, null, null, Retry, GoToLobby);
    }

    private void Start()
    {
        _initialGold = new KeyValuePair<ResourceManager.ResourceType, int>(ResourceManager.ResourceType.Gold, InitialGoldAmount);
        _context.Resource.Initialize(_initialGold);

        _towerHandler.Initialize();
        _enemyHandler.Initialize();
        _networkHandler.Initialize();
        _waveHandler.Initialize();
        _uiHandler.Initialize();

        TowerGridSelectionHandler.OnSelect += _uiHandler.SelectTowerUI;
        TowerGridSelectionHandler.OnDeselect += _uiHandler.DeselectTowerUI;

        navMeshSurface.BuildNavMesh();
        RefreshAlivedEnemyCount();
    }
    private void OnWave()
    {
        if (_waveHandler.IsFinalWave) return;

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
            _waveHandler.ForceTimeUp();
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
        _uiHandler.InteractableWaveButton(isClearEnemiesInWave);

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

    private void GoToLobby() => GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);

    private void Retry()
    {
        _enemyHandler.Reset();
        _towerHandler.Reset();
        _waveHandler.Reset();
        _uiHandler.Reset();
        _context.Resource.Initialize(_initialGold);
    }
}