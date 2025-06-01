using System;
using UnityEngine;

public class LaserProjectile : Projectile
{
    [SerializeField] private LineRenderer _line;
    private float _elapsed;
    private const float Duration = 0.2f;
    private Action<Projectile> _onHit;
    public override void Initialize(BaseEnemy target, Vector3 origin, float damage, float speed, Action<Projectile> onHit, Action<Projectile, ISyncObject> onSyncReturn)
    {
        base.Initialize(target, origin, damage, speed, onHit, onSyncReturn);
        target?.TakeDamage(damage);
        _elapsed = 0;
        _onHit = onHit;
        if (target != null)
        {
            _line.SetPosition(0, origin);
            _line.SetPosition(1, target.transform.position);
        }
    }

    public override bool HasHit(Vector3 projectilePosition, BaseEnemy target)
    {
        return true;
    }

    public override void Move(Transform transform, BaseEnemy target, float speed)
    {
        
    }

    protected override void Update()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed > Duration)
        {
            _onHit.Invoke(this);
        }
    }
}
