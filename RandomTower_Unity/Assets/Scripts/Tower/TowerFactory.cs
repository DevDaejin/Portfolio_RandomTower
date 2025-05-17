using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerFactory
{
    private readonly TowerDatabase _database;
    private readonly  Transform _towerGroup;

    private readonly Dictionary<int, Pool<BaseTower>> _towerPools = new();
    private readonly Dictionary<int, IProjectilePool> _projectilePools = new();

    private const string TowerGroupName = "TowerGroup";

    public TowerFactory(TowerDatabase database)
    {
        _database = database;
        _towerGroup = new GameObject(TowerGroupName).transform;
        _towerGroup.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public TowerData GetTowerRandomData(int grade)
    {
        TowerData[] candidates = _database.GetTowersByGrade(grade);
        if (candidates.Length == 0) return null;
        return candidates[Random.Range(0, candidates.Length)];
    }

    public ITower CreateTower(TowerData data, Vector3 gridPosition, IEnemyProvider enemyProvider, int level = 1)
    {
        Pool<BaseTower> towerPool = GetTowerPool(data);
        BaseTower tower = towerPool.Get();
        IProjectilePool projectilePool = GetProjectilePool(data);
        tower.Initialize(data, gridPosition, projectilePool, enemyProvider, level);

        return tower;
    }

    private Pool<BaseTower> GetTowerPool(TowerData data)
    {
        if(!_towerPools.TryGetValue(data.ID, out Pool<BaseTower> pool))
        {
            pool = new Pool<BaseTower>(data.TowerPrefab, _towerGroup);
            _towerPools.Add(data.ID, pool);
        }
        return pool;
    }

    private IProjectilePool GetProjectilePool(TowerData data)
    {
        IProjectilePool pool = null;
        if (_projectilePools.TryGetValue(data.ID, out pool))
        {
            return pool;
        }

        Projectile projectile = data.ProjectilePrefab.GetComponent<Projectile>();

        if (projectile == null) return null;

        Type type = projectile.GetType();
        Type poolType = typeof(ProjectilePool<>).MakeGenericType(type);

        pool = (IProjectilePool)Activator.CreateInstance(poolType, data.ProjectilePrefab, _towerGroup);
        _projectilePools.Add(data.ID, pool);

        return pool;
    }

    public int GetTowerCount()
    {
        int count = 0;
        int[] keys = _towerPools.Keys.ToArray();

        for (int index = 0; index < keys.Length; index++)
        {
            count += _towerPools[keys[index]].Actived.Count;
        }

        return count;
    }

    public void Return(ITower tower)
    {
        if(tower is BaseTower baseTower &&
            _towerPools.TryGetValue(baseTower.Data.ID, out Pool<BaseTower> pool))
        {
            pool.Return(baseTower);
        }
    }

    public void ReturnAllTower()
    {
        foreach (var pool in _towerPools)
        {
            pool.Value.ReturnAll();
        }
    }

    public void ReturnAllProjectile()
    {
        foreach(var pool in _projectilePools)
        {
            pool.Value.ReturnAll();
        }
    }
}
