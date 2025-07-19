using Google.Protobuf;
using Spawn;
using System.Threading.Tasks;
using UnityEngine;

public class InGameNetworkHandler
{
    private readonly InGameContext _context;
    private readonly MultiEnviromentHandler _multiEnviromentHandler;

    public InGameNetworkHandler(InGameContext context, MultiEnviromentHandler multiEnviromentHandler)
    {
        _context = context;
        _multiEnviromentHandler = multiEnviromentHandler;
    }

    public void Initialize()
    {
        if (_context.Network.IsConnect)
        {
            _multiEnviromentHandler.Initialize(_context.Network.IsHost);

            _context.Network.SpawnService.OnEnemySpawned = OnReceivedEnemyPacket;
            _context.Network.SpawnService.OnTowerSpawned = OnReceivedTowerPacket;
            _context.Network.SpawnService.OnProjectileSpawned =  OnReceivedProjectilePacket;
            _context.Network.GameStateService.OnWaveStart += OnWaveStart;
        }
    }

    public bool IsConnected => _context.Network.IsConnect;
    public bool IsHost => _context.Network.IsHost;
    public string RoomID => _context.Network.RoomID;

    public async Task SendEnvelope(string type, IMessage payload) => await _context.Network.SendEnvelope(type, payload);

    private void OnReceivedEnemyPacket(SpawnEnemyPacket packet)
    {
        EnemyData data = _context.Enemy.GetEnemyDataWithID(int.Parse(packet.SpawnId));
        BaseEnemy enemy = _context.Enemy.GetEnemy(data);

        ISyncObject syncObject = enemy.GetComponent<ISyncObject>();
        syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

        _context.Enemy.AddSpawnedEnemy(enemy);
        _context.Network.SyncService.OnSyncObjectSpawned(syncObject);
    }

    private void OnReceivedTowerPacket(SpawnTowerPacket packet)
    {
        TowerData data = _context.Tower.TowerDB.GetTowerByID(int.Parse(packet.SpawnId)).Data;
        BaseTower tower = _context.Tower.CreateTower(data, Vector3.down, null, null);

        ISyncObject syncObject = tower.Transform.GetComponent<ISyncObject>();
        syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

        _context.Network.SyncService.OnSyncObjectSpawned(syncObject);
    }

    private void OnReceivedProjectilePacket(SpawnProjectilePacket packet)
    {
        TowerData data = _context.Tower.TowerDB.GetTowerByID(int.Parse(packet.SpawnId)).Data;

        IProjectilePool pool = _context.Tower.GetProjectilePool(data);
        Projectile projectile = pool.Get(null, Vector3.down, 0, data.ProjectileSpeed, null);

        ISyncObject syncObject = projectile.GetComponent<ISyncObject>();
        syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

        _context.Network.SyncService.OnSyncObjectSpawned(syncObject);
    }

    private void OnWaveStart()
    {
        if (!_context.Network.IsHost) _context.Wave.StartWave();
    }

}
