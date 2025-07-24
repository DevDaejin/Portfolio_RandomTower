using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform _installationGrid;

    private IEnemyProvider _enemyProvider;
    private TowerGridController _gridController;
    private TowerFactory _towerFactory;
    private TowerChanceTable _towerChanceTable;
    private GridDragIndicator _dragIndicator;

    public TowerDatabase TowerDB => _towerDB;
    private TowerDatabase _towerDB;

    public int MaxTower => _installableCount;
    private int _installableCount;
    public int InstalledCount => _towerFactory.GetTowerCount();

    public Action<int, ISyncObject> OnSendSpawnTowerPacket;
    public Action<int, ISyncObject> OnSendDespawnTowerPacket;
    public Action<int, ISyncObject> OnSendSpawnProjectilePacket;
    public Action<string> OnSendReturnProejctile;
    public Action<int, int> OnTowerUpdated;

    private void Awake()
    {
        Transform[] tree = GetChildrenTransformArray(_installationGrid);
        _dragIndicator = GetComponent<GridDragIndicator>();
        _towerFactory = new TowerFactory();
        _gridController = new TowerGridController(tree, _dragIndicator);
        _towerChanceTable = new();
    }

    public void Initialize(TowerDatabase towerDB, IEnemyProvider enemyProvider, int installableCount)
    {
        _towerDB = towerDB;
        _towerFactory.Initialize(_towerDB);
        _dragIndicator.Initialize();
        _enemyProvider = enemyProvider;
        _installableCount = installableCount;
    }

    public void SetInstallPoints(Transform points)
    {
        _installationGrid = points;
        Transform[] tree = GetChildrenTransformArray(_installationGrid);
        _gridController = new TowerGridController(tree, _dragIndicator);
    }

    public void SpawnTower(int towerSpawnChancePassiveLevel)
    {
        int towerGrade = _towerChanceTable.GetRandomGrade(towerSpawnChancePassiveLevel);
        TowerData data = _towerFactory.GetTowerRandomData(towerGrade);
        TowerGrid grid = _gridController.GetTowerInstallableGrid(data);

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

        BaseTower tower = CreateTower(new TowerCreateSetting
        {
            Data = data,
            GridPosition = grid.transform.position,
            EnemyProvider = _enemyProvider,
            OnSendReturnProjectile = OnSendReturnProejctile
        });

        if (!grid.TryAddTower(tower))
        {
            _towerFactory.Release(tower);
        }
        else
        {
            ISyncObject syncObject = tower.Transform.gameObject.GetComponent<ISyncObject>();
            OnSendSpawnTowerPacket?.Invoke(data.ID, syncObject);
            OnTowerUpdated(_towerFactory.GetTowerCount(), _installableCount);
        }
    }

    public void MergeTower(TowerGrid grid)
    {
        if (grid == null) return;

        int towerGrade = grid.GetTower().Data.Grade + 1;

        foreach (var tower in grid.GetTowerList)
        {
            RemoveTower(tower);
        }
        grid.RemoveTowerAll();

        var data = _towerFactory.GetTowerRandomData(towerGrade);

        var newGrid = _gridController.GetGridDifferentID(data);
        if (newGrid == null) newGrid = grid;
        BaseTower newTower = CreateTower(new TowerCreateSetting
        {
            Data = data,
            GridPosition = newGrid.transform.position,
            EnemyProvider = _enemyProvider,
            OnAttack = OnTowerAttack,
            OnSendReturnProjectile = OnSendReturnProejctile,
        });
        if (!grid.TryAddTower(newTower))
        {
            _towerFactory.Release(newTower);
        }
        else
        {
            ISyncObject syncObject = newTower.Transform.gameObject.GetComponent<ISyncObject>();
            OnSendSpawnTowerPacket?.Invoke(data.ID, syncObject);
            OnTowerUpdated(_towerFactory.GetTowerCount(), _installableCount);
        }
    }

    public void SellTower(BaseTower tower, Action<int> onSellTower)
    {
        onSellTower?.Invoke(Mathf.RoundToInt(tower.Data.SpawnCoast * 0.5f));
        RemoveTower(tower);
        int count = _towerFactory.GetTowerCount();
        OnTowerUpdated(count, _installableCount);
    }

    private void RemoveTower(BaseTower tower)
    {
        ISyncObject syncObject = tower.GetComponent<ISyncObject>();
        OnSendDespawnTowerPacket?.Invoke(tower.Data.ID, syncObject);
        _towerFactory.Release(tower);
    }

    public void SwapTower(Vector3 gridPosition1, Vector3 gridPosition2)
    {
        if (gridPosition1 == gridPosition2) return;

        var grid1 = _gridController.GetGridByPosition(gridPosition1);
        var grid2 = _gridController.GetGridByPosition(gridPosition2);

        var grid1Towers = new List<BaseTower>(grid1.GetTowerList);
        var grid2Towers = new List<BaseTower>(grid2.GetTowerList);

        grid1.RemoveTowerAll();
        grid2.RemoveTowerAll();

        if (grid1Towers.Count != 0)
        {   
            MoveToGrid(grid1Towers, grid2);
        }

        if (grid2Towers.Count != 0)
        {
            MoveToGrid(grid2Towers, grid1);
        }
    }

    private void MoveToGrid(List<BaseTower> towers, TowerGrid grid)
    {
        for (int index = 0; index < towers.Count; index++)
        {
            var tower = towers[index];
            grid.TryAddTower(tower);
        }
    }


    public BaseTower CreateTower(TowerCreateSetting setting)
    {
        return _towerFactory.CreateTower(setting);
    }

    private void OnTowerAttack(int id, ISyncObject syncable)
    {
        OnSendSpawnProjectilePacket?.Invoke(id, syncable);
    }

    public Transform[] GetChildrenTransformArray(Transform root)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);

        int count = 0;
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != root) count++;
        }

        Transform[] result = new Transform[count];
        int index = 0;
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != root)
                result[index++] = all[i];
        }

        return result;
    }

    public BaseTower GetTowerByObjectId(string dataId, string objectId)
    {
        BaseTower target = null;

        var pool = _towerFactory.GetTowerPool(int.Parse(dataId));

        foreach (var tower in pool.GetActivedTowers)
        {
            if (tower.TryGetComponent<ISyncable>(out var sync))
            {
                if (sync.ObjectID == objectId)
                {
                    target = tower;
                    break;
                }
            }
        }

        return target;
    }

    public IProjectilePool GetProjectilePool(TowerData data)
    {
        return _towerFactory.ProjectilePool[data.ID];
    }

    public int GetHighestLevel() => _towerChanceTable.HighestLevel;
    public int[] GetProbability(int level) => _towerChanceTable.GetProbability(level);
    public void ReleaseAll()
    {
        _gridController.RemoveAllTower();
        _towerFactory.ReleaseAllTower();
        _towerFactory.ReleaseAllProjectile();
    }
}
