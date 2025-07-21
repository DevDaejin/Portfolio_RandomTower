using Spawn;
using System;

public class InGameTowerHandler
{
    private InGameContext _context;
    private TowerDatabase _towerDB;
    private int _maxTowerCount;
    private Action<int> _onSellTower;
    private Action<string> _onSendReturnProejctile;

    public InGameTowerHandler( InGameContext context, TowerDatabase towerDB, int MaxTowerCount, Action<int> onSellTower, Action<string> onSendReturnProjectile)
    {
        _context = context;
        _towerDB = towerDB;
        _maxTowerCount = MaxTowerCount;
        _onSellTower = onSellTower;
        _onSendReturnProejctile = onSendReturnProjectile;
    }

    public void Initialize()
    {
        _context.Tower.Initialize(_towerDB, _context.Enemy, _maxTowerCount);
        _context.Tower.OnTowerUpdated += _context.UI.SetTowerCount;
        
        if (_context.Network.IsConnect)
        {
            _context.Tower.OnSendSpawnTowerPacket = (id, sync) => _context.Network.OnSendSpawnPacket<SpawnTowerPacket>("tower", id, _context.IDGenerator.Get(), sync);
            _context.Tower.OnSendSpawnProjectilePacket = (id, sync) => _context.Network.OnSendSpawnPacket<SpawnProjectilePacket>("projectile", id, _context.IDGenerator.Get(), sync);
            _context.Tower.OnSendReturnProejctile = _onSendReturnProejctile;
        }
    }

    public void SellTower(BaseTower tower) => _context.Tower.SellTower(tower, _onSellTower);
    public void MergeTower(TowerGrid grid) => _context.Tower.MergeTower(grid);
    public void OnSpawnTower(int towerSpawnChancePassiveLevel) => _context.Tower.SpawnTower(towerSpawnChancePassiveLevel);
    public bool IsUpgradeMax(int level) => _context.Tower.GetHighestLevel() > level;
    public int[] GetProbability(int level) => _context.Tower.GetProbability(level);
    public void Reset() => _context.Tower.ReleaseAll();
}
