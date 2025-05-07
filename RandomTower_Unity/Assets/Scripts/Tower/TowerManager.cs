using System.Linq;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform _installationGrid;
    [SerializeField] private TowerDatabase _towerDatabase;
    [SerializeField] private TowerChanceTable _towerChanceTable;
    private GridController _gridController;
    private TowerFactory _towerFactory;
    

    private void Awake()
    {
        Transform[] tree = 
            _installationGrid.GetComponentsInChildren<Transform>()
            .Where(branch => branch != _installationGrid)
            .ToArray();

        _gridController = new GridController(tree);
        _towerFactory = new TowerFactory(_towerDatabase);
    }

    private void Start()
    {
        ApplyTowerLevel();
    }

    private void ApplyTowerLevel()
    {// TODO : 타워 데이터베이스에서 타워들 레벨 업데이트 쳐야 함

    }

    public void SpawnTower(int towerSpawnChancePassiveLevel)
    {
        int towerGrade = _towerChanceTable.GetRandomGrade(towerSpawnChancePassiveLevel);
        TowerData data = _towerFactory.GetTowerRandomData(towerGrade);

        Grid grid = _gridController.GetTowerInstallableGrid(data);

        if(grid == null )
        {
            Debug.Log($"{data.name}을 소환할 공간이 없습니다.");
            return;
        }

        BaseTower tower = _towerFactory.CreateTowerByData(data);

        if (!grid.TryAddTower(tower))
        {
            _towerFactory.Return(tower);
        }
    }
}
