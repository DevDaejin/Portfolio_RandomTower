using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerFactory
{
    private readonly Transform _towerGroup;
    public Dictionary<int, GameObjectPool<BaseTower>> TowerPools => _towerPools;
    private readonly Dictionary<int, GameObjectPool<BaseTower>> _towerPools = new();
    private readonly Dictionary<int, IProjectilePool> _projectilePools = new();
    public Dictionary<int, IProjectilePool> ProjectilePool => _projectilePools;

    private TowerDatabase _database;
    private const string TowerGroupName = "TowerGroup";

    public TowerFactory()
    {
        _towerGroup = new GameObject(TowerGroupName).transform;
        _towerGroup.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Initialize(TowerDatabase database)
    {
        _database = database;
    }

    public TowerData GetTowerRandomData(int grade)
    {
        TowerDataConfig[] candidates = _database.GetTowersByGrade(grade);
        if (candidates.Length == 0) return null;
        return candidates[Random.Range(0, candidates.Length)].Data;
    }

    public BaseTower CreateTower(TowerCreateSetting setting)
    {
        GameObjectPool<BaseTower> towerPool = GetTowerPool(setting.Data);
        BaseTower tower = towerPool.Get();
        IProjectilePool projectilePool = GetProjectilePool(setting.Data);
        setting.ProjectilePool = projectilePool;
        tower.Initialize(setting);

        return tower;
    }

    private GameObjectPool<BaseTower> GetTowerPool(TowerData data)
    {
        if (!_towerPools.TryGetValue(data.ID, out GameObjectPool<BaseTower> pool))
        {
            pool = new GameObjectPool<BaseTower>(data.TowerPrefab, _towerGroup);
            _towerPools.Add(data.ID, pool);
        }
        return pool;
    }

    public GameObjectPool<BaseTower> GetTowerPool(int dataId)
    {
        if (_towerPools.TryGetValue(dataId, out GameObjectPool<BaseTower> pool))
        {
            return pool;
        }
        return null;
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
            count += _towerPools[keys[index]].CountActived();
        }

        return count;
    }

    public List<TowerCombinationData> GetAvailableCombinations()
    {
        List<int> installedIds = new();

        foreach (var pool in _towerPools)
        {
            var towers = pool.Value.GetActivedTowers;

            for (int i = 0; i < towers.Count; i++)
            {
                installedIds.Add(towers[i].Data.ID);
            }
        }

        return _database.GetAvailableCombinations(installedIds);
    }

    public bool TryCombine(TowerCombinationData combineData, out TowerData resultData, out List<BaseTower> usedTowers)
    {
        resultData = null;
        usedTowers = new();

        Dictionary<int, List<BaseTower>> idToTowers = new();

        foreach (var pool in _towerPools)
        {
            foreach (var tower in pool.Value.GetActivedTowers)
            {
                int id = tower.Data.ID;
                if (!idToTowers.ContainsKey(id))
                {
                    idToTowers[id] = new();
                }

                idToTowers[id].Add(tower);
            }
        }

        Dictionary<int, int> requiredCounts = new();

        foreach (var required in combineData.RequiredTowers)
        {
            int id = required.Data.ID;
            if (!requiredCounts.ContainsKey(id)) requiredCounts[id] = 0;
            requiredCounts[id]++;
        }

        foreach (var pair in requiredCounts)
        {
            if (!idToTowers.ContainsKey(pair.Key) || idToTowers[pair.Key].Count < pair.Value)
            {
                return false;
            }
        }

        foreach (var pair in requiredCounts)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                usedTowers.Add(idToTowers[pair.Key][i]);
            }
        }

        resultData = combineData.ResultTower.Data;
        return true;
    }

    public void Release(BaseTower tower)
    {
        if (tower is BaseTower baseTower &&
            _towerPools.TryGetValue(baseTower.Data.ID, out GameObjectPool<BaseTower> pool))
        {
            pool.Release(baseTower);
        }
    }

    public void ReleaseAllTower()
    {
        foreach (var pool in _towerPools)
        {
            pool.Value.ReleaseAll();
        }
    }

    public void ReleaseAllProjectile()
    {
        foreach (var pool in _projectilePools)
        {
            pool.Value.Release();
        }
    }
}
