using System;
using UnityEngine;

public class ProjectilePool<T> : IProjectilePool where T : Projectile
{
    private readonly Pool<T> _pool;
    private readonly Action<Projectile> _onReturn;
    public ProjectilePool(GameObject prefab, Transform parent)
    {
        _pool = new Pool<T>(prefab, parent);
        _onReturn = proejectile => _pool.Return((T)proejectile);
    }

    public Projectile Get(BaseEnemy target, Vector3 origin, float damage, float speed)
    {
        var projectile = _pool.Get();
        projectile.Initialize(target, origin, damage, speed, _onReturn);
        return projectile;
    }
}
