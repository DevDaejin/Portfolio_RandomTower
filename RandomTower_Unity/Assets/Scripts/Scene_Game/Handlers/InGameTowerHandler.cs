using Spawn;
using System;

public class InGameTowerHandler
{
    private InGameContext _context;
    private int _maxTowerCount;
    private Action<string> _onSendReturnProejctile;

    public InGameTowerHandler( InGameContext context, int MaxTowerCount, Action<string> onSendReturnProjectile)
    {
        _context = context;
        _maxTowerCount = MaxTowerCount;
        _onSendReturnProejctile = onSendReturnProjectile;
    }

    public void Initialize()
    {
        _context.Tower.Initialize(_context.Enemy, _maxTowerCount);
        _context.Tower.OnTowerUpdated += _context.UI.SetTowerCount;
        
        if (_context.Network.IsConnect)
        {
            _context.Tower.OnSendSpawnTowerPacket = (id, sync) => _context.Network.OnSendSpawnPacket<SpawnTowerPacket>("tower", id, _context.IDGenerator.Get(), sync);
            _context.Tower.OnSendSpawnProjectilePacket = (id, sync) => _context.Network.OnSendSpawnPacket<SpawnProjectilePacket>("projectile", id, _context.IDGenerator.Get(), sync);
            _context.Tower.OnSendReturnProejctile = _onSendReturnProejctile;
        }
    }
}
