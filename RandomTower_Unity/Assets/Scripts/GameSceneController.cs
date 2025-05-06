using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    private GridController gridController;
    private TowerManager towerSpawner;

    private void Awake()
    {
        towerSpawner = GetComponent<TowerManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            towerSpawner.SpawnTower();
        }
    }
}
