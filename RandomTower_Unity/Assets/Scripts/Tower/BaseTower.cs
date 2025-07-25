using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BaseTower : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Animator _animator;

    public Transform Transform => transform;
    public TowerData Data => _setting.Data;
    public GameObject Selectd => gameObject;
    private TowerCreateSetting _setting;
    private TowerRangeViewer _rangeViewer;

    private float _fireElapsed;
    private float _lookRotationSpeed = 3;
    private List<BaseEnemy> _targets;

    private const string AttackAnimationTrigger = "attack";
    private const string IdleState = "Idle";

    public void Initialize(TowerCreateSetting setting)
    {
        _setting = setting;
        _rangeViewer ??= GetComponentInChildren<TowerRangeViewer>();
        _rangeViewer.Deactive();

        _animator.GetComponent<TowerAnimationEventRouter>().AttackCallback = AttackAnimationEvent;
    }

    protected virtual void Update()
    {
        if (_setting?.EnemyProvider == null) return;

        var enemies = FindClosestEnemies();
        if (enemies.Count == 0) return;

        if (!IsIdle()) return;

        _fireElapsed += Time.deltaTime;

        Vector3 direction = enemies[0].transform.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _lookRotationSpeed);
        }

        if (_fireElapsed < 1f / Data.FireRate) return;
        {
            transform.rotation = Quaternion.LookRotation(direction);
            Attack(enemies);
            _fireElapsed = 0f;
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

        _targets = targets;
        _animator.SetTrigger(AttackAnimationTrigger);
    }

    private void AttackAnimationEvent()
    {
        foreach (BaseEnemy target in _targets)
        {
            ISyncObject syncObject = _setting.ProjectilePool.Get(target, _firePoint.position, Data.Damage, Data.ProjectileSpeed, _setting.OnSendReturnProjectile).GetComponent<ISyncObject>();
            _setting.OnAttack?.Invoke(Data.ID, syncObject);
        }
    }

    private bool IsIdle() => _animator.GetCurrentAnimatorStateInfo(0).IsName(IdleState);

    public void ShowRange(bool isAct)
    {
        if (isAct) _rangeViewer.Active(Data.Range);
        else _rangeViewer.Deactive();
    }
}