using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyFactory
{
    private readonly Transform _enemyGroup;
    public Dictionary<int, GameObjectPool<BaseEnemy>> Pools { get; private set; } = new();
    private const string EnemyGroupName = "EnemyGroup";

    public EnemyFactory()
    {
        _enemyGroup = new GameObject(EnemyGroupName).transform;
        _enemyGroup.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public BaseEnemy CreateEnemy(EnemyData data, Transform routeGroup)
    {
        GameObjectPool<BaseEnemy> pool = GetEnemyPool(data);
        BaseEnemy enemy = pool.Get();

        enemy.Initialize(data, routeGroup);

        return enemy;
    }

    private GameObjectPool<BaseEnemy> GetEnemyPool(EnemyData data)
    {
        int id = data.ID;

        if (Pools.TryGetValue(id, out var pool)) return pool;

        pool = new GameObjectPool<BaseEnemy>(data.Prefab, _enemyGroup);
        Pools[id] = pool;

        return pool;
    }

    public void Release(BaseEnemy enemy)
    {
        if(Pools.TryGetValue(enemy.Data.ID, out GameObjectPool<BaseEnemy> pool))
        {
            pool.Release(enemy);
        }
    }

    public void ReleaseAll()
    {
        foreach(GameObjectPool<BaseEnemy> pool in Pools.Values)
        {
            pool.ReleaseAll();
        }
    }
}
