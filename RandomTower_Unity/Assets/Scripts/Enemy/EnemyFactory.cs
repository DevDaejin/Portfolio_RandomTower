using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory
{
    private readonly Transform _enemyGroup;
    private readonly Dictionary<int, Pool<BaseEnemy>> _pools = new();
    private const string EnemyGroupName = "EnemyGroup";

    public EnemyFactory()
    {
        _enemyGroup = new GameObject(EnemyGroupName).transform;
        _enemyGroup.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public BaseEnemy CreateEnemy(EnemyDataConfig config, Transform routeGroup)
    {
        Pool<BaseEnemy> pool = GetEnemyPool(config);
        BaseEnemy enemy = pool.Get();

        enemy.Initialize(config, routeGroup);

        return enemy;
    }

    private Pool<BaseEnemy> GetEnemyPool(EnemyDataConfig config)
    {
        int id = config.Data.ID;

        if (_pools.TryGetValue(id, out var pool)) return pool;

        pool = new Pool<BaseEnemy>(config.Data.Prefab, _enemyGroup);
        _pools[id] = pool;

        return pool;
    }

    public void Return(BaseEnemy enemy)
    {
        if(_pools.TryGetValue(enemy.Data.ID, out Pool<BaseEnemy> pool))
        {
            pool.Return(enemy);
        }
    }
}
