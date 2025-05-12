using System;
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

    protected IEnemyProvider _enemyProvider;

    private float _fireRateTime;
    private Pool<Projectile> _projectilePool;

    public void Initialize(TowerData data, Pool<Projectile> pool, IEnemyProvider enemyProvider, int level = 1)
    {
        Data = data;
        Level = level;
        _projectilePool = pool;
        if (_enemyProvider == null)
        {
            _enemyProvider = enemyProvider;
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
            projectile.Initialize(Damage, 10, null);
            projectile.Set(transform.position, target);
        }
    }


    protected virtual void Update()
    {
        _fireRateTime += Time.deltaTime;
        if (FireRate < _fireRateTime)
        {
            Debug.Log("Attack");
            _fireRateTime = 0;
            Attack(FindClosestEnemies());
        }
    }
}