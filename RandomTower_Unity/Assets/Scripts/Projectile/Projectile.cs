using System;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, IProjectileMovement, IProjectileHitCheck
{
    private BaseEnemy _target;
    private float _damage;
    private float _speed;
    private Action<Projectile> _onHit;

    protected virtual void Update()
    {
        if (_target == null) return;

        Move(transform, _target, _speed);

        if(HasHit(transform.position, _target))
        {
            _target.TakeDamage(_damage);
            _onHit?.Invoke(this);
        }
    }

    public virtual void Initialize(BaseEnemy target, Vector3 origin, float damage, float speed, Action<Projectile> onHit)
    {
        _target = target;
        transform.position = origin;
        _damage = damage;
        _speed = speed;
        _onHit = onHit;
    }

    public abstract void Move(Transform transform, BaseEnemy target, float speed);

    public abstract bool HasHit(Vector3 projectilePosition, BaseEnemy target);
}
