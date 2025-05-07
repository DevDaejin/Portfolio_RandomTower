using System.Linq;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform _installationGrid;
    [SerializeField] private TowerDatabase _towerDatabase;
    private GridController _gridController;
    private TowerFactory _towerFactory;
    private Transform _towerGroup;
    private const string TowerGroupName = "TowerGroup";

    private void Awake()
    {
        Transform[] tree = _installationGrid.GetComponentsInChildren<Transform>();
        tree = tree.Where(branch => branch != _installationGrid).ToArray();

        _gridController = new GridController(tree);
        _towerFactory = new TowerFactory(_towerDatabase, _towerGroup);
    }

    private void Start()
    {
        _towerGroup = new GameObject(TowerGroupName).transform;
        _towerGroup.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        AdjustTowerLevel();
    }

    private void AdjustTowerLevel()
    {// TODO : 타워 데이터베이스에서 타워들 레벨 업데이트 쳐야 함

    }

    public void SpawnTower()
    {
        // TODO : 타워 생성 확율 테이블 적용해야함
        TowerData data = _towerFactory.GetTowerRandomData(1);

        Grid grid = _gridController.GetTowerInstallableGrid(data);

        if(grid == null )
        {
            Debug.Log($"{data.name}을 소환할 공간이 없습니다.");
            return;
        }

        BaseTower tower = _towerFactory.CreateTowerByData(data);
        if (grid.TryAddTower(tower))
        {
            tower.transform.SetParent(_towerGroup);
        }
        else
        {
            _towerFactory.Return(tower);
        }
    }
}
