using Spawn;
using UnityEngine;

public class InGameNetworkHandler : IInGameHandler
{
    private InGameContext _context;
    private readonly MultiEnviromentHandler _multiEnviromentHandler;
    private IDGenerator _idGenerator;

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

            _idGenerator = new(_context.Network.ClientID);

            _context.Network.SpawnService.OnEnemySpawned += packet => OnReceivedEnemyPacket(packet.SpawnId, packet);
            _context.Network.SpawnService.OnTowerSpawned += packet => OnReceivedTowerPacket(packet.SpawnId, packet);
            _context.Network.SpawnService.OnProjectileSpawned += packet => OnReceivedProjectilePacket(packet.SpawnId, packet);

            _context.Network.GameStateService.OnWaveStart += () =>
            {
                if (!_context.Network.IsHost) _context.Wave.StartWave();
            };

            _context.GlobalUI.Set(GlobalUI.GlobalUIOption.Watting);
        }
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }

    public async void OnSendSpawnPacket<T>(string type, int id, ISyncObject syncObject)
    {
        if (!_context.Network.IsConnect) return;

        syncObject.Initialize(_idGenerator.Get(), _context.Network.ClientID, _context.Network.RoomID);
        await _context.Network.SpawnService.SendSpawn(type, id.ToString(), syncObject);
    }

    private void OnReceivedEnemyPacket(string id, SpawnEnemyPacket packet)
    {
        EnemyData data = _context.Enemy.GetEnemyDataWithID(int.Parse(id));
        BaseEnemy enemy = _context.Enemy.GetEnemy(data);

        ISyncObject syncObject = enemy.GetComponent<ISyncObject>();
        syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

        _context.Enemy.AddSpawnedEnemy(enemy);
        _context.Network.SyncService.OnSyncObjectSpawned(syncObject);
    }

    private void OnReceivedTowerPacket(string id, SpawnTowerPacket packet)
    {
        TowerData data = _context.Tower.TowerDatabase.GetTowerByID(int.Parse(id)).Data;
        ITower tower = _context.Tower.CreateTower(data, Vector3.down, null, null, 1);

        ISyncObject syncObject = tower.Transform.GetComponent<ISyncObject>();
        syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

        _context.Network.SyncService.OnSyncObjectSpawned(syncObject);
    }

    private void OnReceivedProjectilePacket(string id, SpawnProjectilePacket packet)
    {
        TowerData data = _context.Tower.TowerDatabase.GetTowerByID(int.Parse(id)).Data;

        IProjectilePool pool = _context.Tower.GetProjectilePool(data);
        Projectile projectile = pool.Get(null, Vector3.down, 0, data.ProjectileSpeed, null);

        ISyncObject syncObject = projectile.GetComponent<ISyncObject>();
        syncObject.Initialize(packet.ObjectId, packet.OwnerId, packet.RoomId);

        _context.Network.SyncService.OnSyncObjectSpawned(syncObject);
    }

}
