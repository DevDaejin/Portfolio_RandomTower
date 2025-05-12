using System.Linq;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform _installationGrid;
    [SerializeField] private TowerDatabase _towerDatabase;
    [SerializeField] private TowerChanceTable _towerChanceTable;

    private IEnemyProvider _enemyProvider;
    private GridController _gridController;
    private TowerFactory _towerFactory;

    private void Awake()
    {
        Transform[] tree =
            _installationGrid.GetComponentsInChildren<Transform>(true)
            .Where(branch => branch != _installationGrid)
            .ToArray();

        _gridController = new GridController(tree);
        _towerFactory = new TowerFactory(_towerDatabase);
    }

    //TODO : 타워 강화 로직 파라미터로 전달 받기
    public void Initialize(IEnemyProvider enemyProvider)
    {
        _enemyProvider = enemyProvider;
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

        if (grid == null)
        {
            grid = _gridController.GetGridDifferentID(data);
            
            if (grid == null)
            {
                Debug.Log($"{data.TowerName}을 소환할 공간이 없습니다.");
                return;
            }

            data = grid.GetTower().Data;
        }

        ITower tower = _towerFactory.CreateTower(data, _enemyProvider);

        if (!grid.TryAddTower(tower))
        {
            _towerFactory.Return(tower);
        }
    }
}
