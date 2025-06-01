using System;
using UnityEngine;

public class CurveProjectile : Projectile
{
    private Vector3 _origin;
    private float _elapsed;
    private const float Duration = 0.5f;
    private const float HitThreshold = 0.05f;

    public override void Initialize(BaseEnemy target, Vector3 origin, float damage, float speed, Action<Projectile> onHit, Action<Projectile, ISyncObject> onSyncReturn)
    {
        base.Initialize(target, origin, damage, speed, onHit, onSyncReturn);
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

        Vector3 mid = (_origin + target.transform.position) * 0.5f + Vector3.up * 2;
        Vector3 a = Vector3.Lerp(_origin, mid, timeRatio);
        Vector3 b = Vector3.Lerp(mid, target.transform.position, timeRatio);
        transform.position = Vector3.Lerp(a, b, timeRatio);
    }
}
