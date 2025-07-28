using System;
using UnityEngine;

public class StraightProjectile : Projectile
{
    private Vector3 _origin;
    private float _elapsed;
    private const float Duration = 0.5f;
    private const float HitThreshold = 0.01f;

    public override void Initialize(BaseEnemy target, Vector3 origin, float damage, float speed, Action<Projectile> onHit, Action<string> onSendProjectileReturn)
    {
        base.Initialize(target, origin, damage, speed, onHit, onSendProjectileReturn);
        _origin = transform.position;
        _elapsed = 0;
    }

    public override bool HasHit(Vector3 projectilePosition, BaseEnemy target)
    {
        return Vector3.SqrMagnitude(projectilePosition - target.transform.position) < HitThreshold;
    }

    public override void Move(Transform transform, BaseEnemy target, float speed)
    {
        _elapsed += Time.deltaTime;
        float timeRatio = Mathf.Clamp01(_elapsed / Duration);
        Vector3 destination = target.transform.position;
        transform.rotation = Quaternion.LookRotation(_origin - destination);
        transform.position = Vector3.Lerp(_origin, destination, timeRatio * speed);
    }
}
