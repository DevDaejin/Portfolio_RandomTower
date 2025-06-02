using System;
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
    
    public Action<int, ISyncObject> OnAttack;
    public Action<Projectile, ISyncObject> OnSendProjectileReturn;

    public ISelectable Selectable => this;

    private float _fireElapsed;
    private Vector3 _gridPosition;

    private TowerRangeViewer _rangeViewer;
    protected IEnemyProvider _enemyProvider;
    private IProjectilePool _projectilePool;


    public void Initialize(TowerData data, Vector3 gridPosition, IProjectilePool pool, IEnemyProvider enemyProvider, Action<int, ISyncObject> onActtack, Action<Projectile, ISyncObject> onSendProjectileReturn, int level = 1)
    {
        Data = data;
        _gridPosition = gridPosition;
        Level = level;
        _projectilePool ??= pool;
        _enemyProvider ??= enemyProvider;
        _rangeViewer ??= GetComponentInChildren<TowerRangeViewer>();
        OnAttack = onActtack;
        OnSendProjectileReturn = onSendProjectileReturn;
    }

    protected virtual List<BaseEnemy> FindClosestEnemies()
    {
        Vector3 pos = transform.position;
        return _enemyProvider.FindClosestWithCount(pos, Range, Data.TargetCount);
    }

    protected virtual void Attack(List<BaseEnemy> targets)
    {
        if (targets.Count == 0) return;

        foreach (BaseEnemy target in targets)
        {
            ISyncObject syncObject = _projectilePool.Get(target, transform.position, Damage, Data.ProjectileSpeed, OnSendProjectileReturn).GetComponent<ISyncObject>();
            OnAttack.Invoke(Data.ID, syncObject);
            _fireElapsed = 0f;
        }
    }

    protected virtual void Update()
    {
        if (_enemyProvider == null) return;

        _fireElapsed += Time.deltaTime;

        var enemies = FindClosestEnemies();
        if (enemies.Count == 0) return;

        if (_fireElapsed >= 1f / FireRate)
        {
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