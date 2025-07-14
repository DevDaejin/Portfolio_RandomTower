using Spawn;
using System;
using UnityEngine;

public class InGameEnemyHandler
{
    private InGameContext _context;
    private Action<string> _onSendReturnEnemy;
    public InGameEnemyHandler(InGameContext context, Action<string> onSendReturnEnemy)
    {
        _context = context;
        _onSendReturnEnemy = onSendReturnEnemy;
    }

    public void Initialize()
    {
        _context.Enemy.OnReward += OnReward;
        if (_context.Network.IsConnect)
        {
            _context.Enemy.OnSendSpawnPacket += (id, syncObject) => _context.Network.OnSendSpawnPacket<SpawnEnemyPacket>("enemy", id, _context.IDGenerator.Get(), syncObject);
            _context.Enemy.OnSendEnemyReturn += _onSendReturnEnemy;
        }
    }

    private void OnReward(int gold)
    {
        ResourceManager.ResourceType type = ResourceManager.ResourceType.Gold;
        _context.Resource.Earn(type, gold);
        _context.UI.SetGoldCount(_context.Resource.Get(type));
    }
}
