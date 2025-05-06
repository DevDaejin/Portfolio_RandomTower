using System.Linq;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform installationGrid;
    [SerializeField] private TowerDatabase _towerDatabase;
    private GridController gridController;
    private TowerFactory towerFactory;
    private Transform towerGroup;
    private const string TowerGroupName = "TowerGroup";

    private void Awake()
    {
        Transform[] tree = installationGrid.GetComponentsInChildren<Transform>();
        tree = tree.Where(branch => branch != installationGrid).ToArray();

        gridController = new GridController(tree);
    }

    private void Start()
    {
        towerGroup = new GameObject(TowerGroupName).transform;
        towerGroup.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        AdjustTowerLevel();
    }

    private void AdjustTowerLevel()
    {// TODO : 타워 데이터베이스에서 타워들 레벨 업데이트 쳐야 함

    }

    public void SpawnTower()
    {
        // TODO : 타워 생성 확율 테이블 적용해야함
        TowerData data = towerFactory.GetTowerRandomData(1);

        Grid grid = gridController.GetTowerInstallableGrid(data);

        if(grid == null )
        {
            Debug.Log("설치 불가");
            return;
        }

        BaseTower tower = towerFactory.CreateTowerByData(data);
        if (grid.TryAddTower(tower))
        {
            tower.transform.SetParent(towerGroup);
        }
        else
        {
            towerFactory.Return(tower);
        }
    }
}
