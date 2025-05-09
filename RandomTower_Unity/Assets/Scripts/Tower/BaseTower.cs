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

    public void Initialize(TowerDataConfig config, IEnemyProvider enemyProvider, int level = 1)
    {
        Data = config.Data;
        Level = level;

        if (_enemyProvider == null)
        {
            _enemyProvider = enemyProvider;
        }
    }

    protected virtual List<BaseEnemy> FindClosestEnemies()
    {
        return _enemyProvider.FindClosestWithCount(Transform.position, Range, Data.TargetCount);
    }
}
