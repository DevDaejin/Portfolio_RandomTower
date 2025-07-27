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

    private bool TrySpawnToGrid(TowerData data)
    {
        TowerGrid grid = _gridController.GetTowerInstallableGrid(data);
        if (grid == null) return false;

        var tower = CreateTower(new TowerCreateSetting
        {
            Data = data,
            GridPosition = grid.transform.position,
            EnemyProvider = _enemyProvider,
            OnAttack = OnTowerAttack,
            OnSendReturnProjectile = OnSendReturnProejctile
        });

        if (!grid.TryAddTower(tower))
        {
            _gridController.RemoveTowerInGrid(tower);
            _towerFactory.Release(tower);
            return false;
        }

        if (tower.TryGetComponent<ISyncObject>(out var sync))
        {
            OnSendSpawnTowerPacket?.Invoke(data.ID, sync);
        }

        OnTowerUpdated(_towerFactory.GetTowerCount(), _installableCount);
        return true;
    }

    public void SpawnTower(int towerSpawnChancePassiveLevel, Action onFailed)
    {
        int towerGrade = _towerChanceTable.GetRandomGrade(towerSpawnChancePassiveLevel);
        TowerData data = _towerFactory.GetTowerRandomData(towerGrade);

        if(!TrySpawnToGrid(data))
        {
            onFailed?.Invoke();
        }
    }

    public void MergeTower(TowerGrid grid)
    {
        if (grid == null) return;

        int grade = grid.GetTower().Data.Grade + 1;
        var data = _towerFactory.GetTowerRandomData(grade);
        if (data == null) return;

        int count = grid.GetTowerList.Count;
        for (int i = 0; i < count; i++)
        {
            RemoveTower(grid.GetTower());
        }

        TrySpawnToGrid(data);
    }

    public void SellTower(BaseTower tower, Action<int> onSellTower)
    {
        onSellTower?.Invoke(Mathf.RoundToInt(tower.Data.SpawnCoast * 0.5f));
        RemoveTower(tower);
        OnTowerUpdated(_towerFactory.GetTowerCount(), _installableCount);
    }

    private void RemoveTower(BaseTower tower)
    {
        ISyncObject syncObject = tower.GetComponent<ISyncObject>();
        OnSendDespawnTowerPacket?.Invoke(tower.Data.ID, syncObject);
        _towerFactory.Release(tower);
        _gridController.RemoveTowerInGrid(tower);
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

    public List<TowerCombinationData> GetAvailableCombinations() => _towerFactory.GetAvailableCombinations();

    public void TryCombineTowers(TowerCombinationData data)
    {
        if (_towerFactory.TryCombine(data, out TowerData result, out List<BaseTower> usedTowers))
        {
            foreach(var tower in usedTowers)
            {
                RemoveTower(tower);
            }

            TrySpawnToGrid(result);
        }
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
