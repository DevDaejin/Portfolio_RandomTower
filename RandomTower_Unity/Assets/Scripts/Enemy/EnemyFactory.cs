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

    public BaseEnemy CreateEnemy(EnemyData data, Transform routeGroup)
    {
        Pool<BaseEnemy> pool = GetEnemyPool(data);
        BaseEnemy enemy = pool.Get();

        enemy.Initialize(data, routeGroup);

        return enemy;
    }

    private Pool<BaseEnemy> GetEnemyPool(EnemyData data)
    {
        int id = data.ID;

        if (_pools.TryGetValue(id, out var pool)) return pool;

        pool = new Pool<BaseEnemy>(data.Prefab, _enemyGroup);
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
