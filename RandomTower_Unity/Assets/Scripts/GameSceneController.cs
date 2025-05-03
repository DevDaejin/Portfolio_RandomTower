using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    private GridController gridController;
    private TowerSpawner towerSpawner;

    private void Awake()
    {
        towerSpawner = GetComponent<TowerSpawner>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            towerSpawner.SpawnTower();
        }
    }
}
