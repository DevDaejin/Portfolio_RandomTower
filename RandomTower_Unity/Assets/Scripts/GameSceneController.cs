using UnityEngine;

public class GameSceneController : MonoBehaviour
{
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _towerSpawner.SpawnTower(1);
        }
    }
}
