using System.Collections.Generic;
using UnityEngine;

public class BaseTower : MonoBehaviour, ITower, ISelectable
{
    public Transform Transform => transform;    

    public TowerData Data { get; private set; }

    public int Level { get; private set; }

    public float Damage => Data.Damage + ((Level - 1) * 0.1f);
    public float Range => Data.Range + ((Level - 1) * 0.1f);
    public float FireRate => Data.FireRate + ((Level - 1) * 0.1f);

    public ISelectable Selectable => this;

    private float _fireRateTime;
    private Vector3 _gridPosition;

    private TowerRangeViewer _rangeViewer;
    protected IEnemyProvider _enemyProvider;
    private IProjectilePool _projectilePool;


    public void Initialize(TowerData data, Vector3 gridPosition, IProjectilePool pool, IEnemyProvider enemyProvider, int level = 1)
    {
        Data = data;
        _gridPosition = gridPosition;
        Level = level;
        _projectilePool ??= pool;
        _enemyProvider ??= enemyProvider;
        _rangeViewer ??= GetComponentInChildren<TowerRangeViewer>();
    }

    protected virtual List<BaseEnemy> FindClosestEnemies()
    {
        Vector3 pos = transform.position;
        return _enemyProvider.FindClosestWithCount(pos, Range, Data.TargetCount);
    }

    protected virtual void Attack(List<BaseEnemy> targets)
    {
        foreach (BaseEnemy target in targets)
        {
            Projectile projectile = _projectilePool.Get(target, transform.position, Damage, Data.ProjectileSpeed);
        }
    }

    protected virtual void Update()
    {
        var enemies = FindClosestEnemies();

        if (enemies.Count == 0)
            return;

        _fireRateTime += Time.deltaTime;
        if (_fireRateTime >= 1f / FireRate)
        {
            _fireRateTime = 0f;
            Attack(enemies);
        }
    }

    public void OnSelect()
    {
        _rangeViewer.Active(Range, _gridPosition);
    }

    public void OnDeselect()
    {
        _rangeViewer.Deactive();
    }
}