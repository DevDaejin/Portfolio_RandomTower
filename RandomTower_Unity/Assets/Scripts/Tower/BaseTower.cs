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

    private float fireRateTime;

    public void Initialize(TowerData data, IEnemyProvider enemyProvider, int level = 1)
    {
        Data = data;
        Level = level;

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
        Projectile projectile = null;
        foreach (BaseEnemy target in targets)
        {
            projectile.Initialize(Damage, 1, null);
            projectile.Set(transform.position, target);
        }
    }


    protected virtual void Update()
    {
        fireRateTime += Time.deltaTime;
        if (FireRate < fireRateTime)
        {
            fireRateTime = 0;
            Attack(FindClosestEnemies());
        }
    }
}