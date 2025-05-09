using System.Collections.Generic;
using UnityEngine;

public class BaseTower : MonoBehaviour, ITower
{
    [SerializeField] private TowerData _data;
    public TowerData Data => _data;

    public int Level { private set; get; }

    public int ID => _data.ID;
    public int Grade => _data.Grade;
    public string TowerName => _data.TowerName;

    public float Damage => _data.BaseDamage + ((Level - 1) * 0.1f);
    public float Range => _data.BaseRange + ((Level - 1) * 0.1f);
    public float FireRate => _data.BaseFireRate + ((Level - 1) * 0.1f);

    public Transform Transform => transform;

    public int TargetCount => _data.TargetCount;

    protected IEnemyProvider _enemyProvider;

    public void Initialize(TowerData data, IEnemyProvider enemyProvider, int level = 1)
    {
        _data = data;
        Level = level;

        if (_enemyProvider == null)
        {
            _enemyProvider = enemyProvider;
        }
    }

    protected virtual List<BaseEnemy> FindClosestEnemies()
    {
        return _enemyProvider.FindClosestWithCount(transform.position, Range, TargetCount);
    }
}
