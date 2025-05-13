using System.Collections.Generic;
using UnityEngine;

public class BaseTower : MonoBehaviour, ITower
{
    public TowerData Data { get; private set; }

    public int Level { get; private set; }

    public Transform Transform => transform;

    public float Damage => Data.Damage + ((Level - 1) * 0.1f);
    public float Range => Data.Range + ((Level - 1) * 0.1f);
    public float FireRate => Data.FireRate + ((Level - 1) * 0.1f);

    private float _fireRateTime;

    protected IEnemyProvider _enemyProvider;

    private Pool<Projectile> _projectilePool;
    private TowerRangeViewer _rangeViewer;


    public void Initialize(TowerData data, Pool<Projectile> pool, IEnemyProvider enemyProvider, int level = 1)
    {
        Data = data;
        Level = level;
        if (_projectilePool == null)
        {
            _projectilePool = pool;
        }
        if (_enemyProvider == null)
        {
            _enemyProvider = enemyProvider;
        }
        if(_rangeViewer == null)
        {
            _rangeViewer = GetComponentInChildren<TowerRangeViewer>();
        }
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
            Projectile projectile = _projectilePool.Get();
            projectile.Initialize(Damage, 10, _projectilePool.Return);
            projectile.Set(transform.position, target);
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
        _rangeViewer.Active(Range);
    }

    public void OnDeselect()
    {
        _rangeViewer.Deactive();
    }
}