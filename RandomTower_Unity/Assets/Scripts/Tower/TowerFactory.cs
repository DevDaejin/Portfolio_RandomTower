using System.Collections.Generic;
using UnityEngine;

public class TowerFactory
{
    private readonly TowerDatabaseSO _database;
    private readonly  Transform _towerGroup;

    private readonly Dictionary<int, Pool<BaseTower>> _pools = new();

    private const string TowerGroupName = "TowerGroup";
    public TowerFactory(TowerDatabaseSO database)
    {
        _database = database;
        _towerGroup = new GameObject(TowerGroupName).transform;
        _towerGroup.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public TowerDataConfig GetTowerRandomData(int grade)
    {
        List<TowerDataConfig> list = _database.GetTowersByGrade(grade);
        return list[Random.Range(0, list.Count)];
    }

    public ITower CreateTowerByData(TowerDataConfig data, IEnemyProvider enemyProvider)
    {
        Pool<BaseTower> pool = GetTowerPool(data);

        BaseTower tower = pool.Get();

        tower.Initialize(data, enemyProvider);

        //return tower;
        return null;
    }

    private Pool<BaseTower> GetTowerPool(TowerDataConfig config)
    {
        if(_pools.TryGetValue(config.Data.ID, out Pool<BaseTower> pool))
        {
            return pool;
        }

        pool = new Pool<BaseTower>(config.Data.Prefab, _towerGroup);
        _pools[config.Data.ID] = pool;

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
