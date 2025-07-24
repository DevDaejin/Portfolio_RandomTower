using System.Collections.Generic;
using UnityEngine;

public class BaseTower : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    public Transform Transform => transform;
    public TowerData Data => _setting.Data;
    public GameObject Selectd => gameObject;

    private TowerCreateSetting _setting;
    private float _fireElapsed;
    private TowerRangeViewer _rangeViewer;

    public void Initialize(TowerCreateSetting setting)
    {
        _setting = setting;
        _rangeViewer ??= GetComponentInChildren<TowerRangeViewer>();
        _rangeViewer.Deactive();
    }

    protected virtual void Update()
    {
        if (_setting?.EnemyProvider == null) return;

        _fireElapsed += Time.deltaTime;

        var enemies = FindClosestEnemies();
        if (enemies.Count == 0) return;

        Vector3 direction = enemies[0].transform.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 2);
        }

        if (_fireElapsed >= 1f / Data.FireRate)
        {  
            Attack(enemies);
        }
    }

    protected virtual List<BaseEnemy> FindClosestEnemies()
    {
        Vector3 pos = transform.position;
        return _setting?.EnemyProvider?.FindClosestWithCount(pos, Data.Range, Data.TargetCount);
    }

    protected virtual void Attack(List<BaseEnemy> targets)
    {
        if (targets.Count == 0) return;

        foreach (BaseEnemy target in targets)
        {
            ISyncObject syncObject = _setting.ProjectilePool.Get(target, _firePoint.position, Data.Damage, Data.ProjectileSpeed, _setting.OnSendReturnProjectile).GetComponent<ISyncObject>();

            _setting.OnAttack?.Invoke(Data.ID, syncObject);
            _fireElapsed = 0f;
        }
    }

    public void ShowRange(bool isAct)
    {
        if (isAct) _rangeViewer.Active(Data.Range);
        else _rangeViewer.Deactive();
    }
}