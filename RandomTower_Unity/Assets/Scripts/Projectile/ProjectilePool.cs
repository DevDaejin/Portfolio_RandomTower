using System;
using UnityEngine;

public class ProjectilePool<T> : IProjectilePool where T : Projectile
{
    private readonly GameObjectPool<T> _pool;
    private readonly Action<Projectile> _onReturn;
    public ProjectilePool(GameObject prefab, Transform parent)
    {
        _pool = new GameObjectPool<T>(prefab, parent);
        _onReturn = projectile => _pool.Release((T)projectile);
    }

    public Projectile Get(BaseEnemy target, Vector3 origin, float damage, float speed, Action<Projectile, ISyncObject> onSendProjectileReturn)
    {
        var projectile = _pool.Get();
        projectile.Initialize(target, origin, damage, speed, _onReturn, onSendProjectileReturn);
        return projectile;
    }

    public void Release()
    {
        _pool.ReleaseAll();
    }
}
 