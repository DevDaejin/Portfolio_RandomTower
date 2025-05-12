using System.Collections.Generic;
using UnityEngine;

public class TowerFactory
{
    private readonly TowerDatabase _database;
    private readonly  Transform _towerGroup;

    private readonly Dictionary<int, Pool<BaseTower>> _towerPools = new();
    private readonly Dictionary<int, Pool<Projectile>> _projectilePools = new();

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

    public ITower CreateTower(TowerData data, IEnemyProvider enemyProvider, int level = 1)
    {
        Pool<BaseTower> towerPool = GetTowerPool(data);
        Pool<Projectile> projectilePool = GetProjectilePool(data);
        BaseTower tower = towerPool.Get();

        tower.Initialize(data, projectilePool, enemyProvider, level);

        return tower;
    }

    private Pool<T> GetPool<T>(Dictionary<int, Pool<T>> dict, int id, GameObject prefab)
    {
        if(dict.TryGetValue(id, out var existing))
        {
            return (Pool<T>)existing;
        }

        Pool<T> pool = new Pool<T>(prefab, _towerGroup);
        dict[id] = pool;

        return pool;
    }

    private Pool<BaseTower> GetTowerPool(TowerData data)
    {
        return GetPool<BaseTower>(_towerPools, data.ID, data.TowerPrefab);
    }

    private Pool<Projectile> GetProjectilePool(TowerData data)
    {
        return GetPool<Projectile>(_projectilePools, data.ID, data.ProjectilePrefab);
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
        foreach(var pool in _towerPools)
        {
            pool.Value.ReturnAll();
        }
    }
}
