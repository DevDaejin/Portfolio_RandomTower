using System;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform _installationGrid;
    [SerializeField] private TowerChanceTable _towerChanceTable;
    [SerializeField] private TowerDatabase _towerDatabase;
    public TowerDatabase TowerDatabase => _towerDatabase;

    private IEnemyProvider _enemyProvider;
    private TowerGridController _gridController;
    private TowerFactory _towerFactory;

    private int _installableCount;

    public Action<int, ISyncObject> OnSendSpawnTowerPacket;
    public Action<int, ISyncObject> OnSendSpawnProjectilePacket;
    public Action<Projectile, ISyncObject> OnSendProejctileReturn;
    public Action<int, int> OnTowerUpdated;

    private void Awake()
    {
        Transform[] tree = GetChildrenTransformArray(_installationGrid);
        _gridController = new TowerGridController(tree);
        _towerFactory = new TowerFactory(TowerDatabase);
    }

    //TODO : 타워 강화 로직 파라미터로 전달 받기
    public void Initialize(IEnemyProvider enemyProvider, int installableCount)
    {
        _enemyProvider = enemyProvider;
        _installableCount = installableCount;
        ApplyTowerLevel();
    }

    private void ApplyTowerLevel()
    {// TODO : 타워 데이터베이스에서 타워들 레벨 업데이트 쳐야 함

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

        ITower tower = CreateTower(data, grid.transform.position, _enemyProvider, OnSendProejctileReturn, 1);

        if (!grid.TryAddTower(tower))
        {
            _towerFactory.Release(tower);
        }
        else
        { 
            ISyncObject syncObject = tower.Transform.gameObject.GetComponent<ISyncObject>();
            OnSendSpawnTowerPacket.Invoke(data.ID, syncObject);
            OnTowerUpdated(_towerFactory.GetTowerCount(), _installableCount);
            TowerGridSelectionHandler.Reselect();
        }
    }

    public ITower CreateTower(TowerData data, Vector3 position, IEnemyProvider enemyProvider, Action<Projectile, ISyncObject> onSendProjectileReturn, int level)
    {
        return _towerFactory.CreateTower(data, position, enemyProvider, OnTowerAttack, onSendProjectileReturn, level);
    }

    private void OnTowerAttack(int id, ISyncObject syncable)
    {
        OnSendSpawnProjectilePacket.Invoke(id, syncable);
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

    public IProjectilePool GetProjectilePool(TowerData data)
    {
        return _towerFactory.ProjectilePool[data.ID];
    }

    public void ReleaseAll()
    {
        _gridController.RemoveAllTower();
        _towerFactory.ReleaseAllTower();
        _towerFactory.ReleaseAllProjectile();
    }
}
