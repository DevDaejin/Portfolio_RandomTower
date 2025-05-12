using System.Collections.Generic;
using UnityEngine;

public class TowerFactory
{
    private readonly TowerDatabase _database;
    private readonly  Transform _towerGroup;

    private readonly Dictionary<int, Pool<BaseTower>> _pools = new();

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

    public ITower CreateTower(TowerData data, IEnemyProvider enemyProvider)
    {
        Pool<BaseTower> pool = GetTowerPool(data);

        BaseTower tower = pool.Get();

        tower.Initialize(data, enemyProvider);

        return tower;
    }

    private Pool<BaseTower> GetTowerPool(TowerData data)
    {
        if(_pools.TryGetValue(data.ID, out Pool<BaseTower> pool))
        {
            return pool;
        }

        pool = new Pool<BaseTower>(data.TowerPrefab, _towerGroup);
        _pools[data.ID] = pool;

        return pool;
    }

    public void Return(ITower tower)
    {
        if(tower is BaseTower baseTower &&
            _pools.TryGetValue(baseTower.Data.ID, out Pool<BaseTower> pool))
        {
            pool.Return(baseTower);
        }
    }
}
