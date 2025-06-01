using System;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, IProjectileMovement, IProjectileHitCheck
{
    private BaseEnemy _target;
    private float _damage;
    private float _speed;
    private ISyncObject _syncObject;
    private Action<Projectile> _onReturn;
    public Action<Projectile, ISyncObject> OnSyncReturn;

    private void Awake()
    {
        _syncObject = GetComponent<ISyncObject>();
    }

    protected virtual void Update()
    {
        if (_target == null)
        {
            if (OnSyncReturn != null)
            {
                _onReturn?.Invoke(this);
            }
            return;
        }

        Move(transform, _target, _speed);

        if (HasHit(transform.position, _target))
        {
            _target?.TakeDamage(_damage);
            _onReturn?.Invoke(this);
        }
    }

    private void OnDisable()
    {
        OnSyncReturn?.Invoke(this, _syncObject);
    }

    public virtual void Initialize(BaseEnemy target, Vector3 origin, float damage, float speed, Action<Projectile> onReutrn, Action<Projectile, ISyncObject> onSyncReturn)
    {
        _target = target;
        transform.position = origin;
        _damage = damage;
        _speed = speed;
        _onReturn = onReutrn;
        OnSyncReturn = onSyncReturn;
    }

    public void ForceReturn()
    {
        _onReturn?.Invoke(this);
    }

    public abstract void Move(Transform transform, BaseEnemy target, float speed);

    public abstract bool HasHit(Vector3 projectilePosition, BaseEnemy target);
}
