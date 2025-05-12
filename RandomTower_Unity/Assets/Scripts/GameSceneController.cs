using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private List<StageConfig> _stageConfigs;
    private TowerManager _towerSpawner;
    private EnemyManager _enemyManager;

    private void Awake()
    {
        _enemyManager = GetComponent<EnemyManager>();
        _towerSpawner = GetComponent<TowerManager>();
        _towerSpawner.Initialize(_enemyManager);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnTower();
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnEnemy();
        }
    }

    private void SpawnTower()
    {
        //TODO: 임시코드
        _towerSpawner.SpawnTower(1);
    }

    private void SpawnEnemy()
    {
        //TODO: 임시코드
        //TODO: 여기 구현 필요
        _enemyManager.SpawnWave(_stageConfigs[0]);
    }
}