using System.Collections.Generic;
using UnityEngine;

public class TowerFactory
{
    private TowerDatabase _database;
    private Transform _towerGroup;

    private const string TowerGroupName = "TowerGroup";
    public TowerFactory(TowerDatabase database)
    {
        _database = database;

        _towerGroup = new GameObject(TowerGroupName).transform;
        _towerGroup.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        // 풀링 작업해야 함
    }

    public TowerData GetTowerRandomData(int grade)
    {
        List<TowerData> list = _database.GetTowersByGrade(grade);
        TowerData data = list[Random.Range(0, list.Count)];
        return data;
    }

    public BaseTower CreateTowerByData(TowerData data)
    {
        //TODO : 풀처리
        BaseTower tower = GameObject.Instantiate(data.Prefab).GetComponent<BaseTower>();
        tower.Initialize(data);
        return tower;
    }

    public void Return(BaseTower tower)
    {
        //TODO : 풀 처리
    }
}
