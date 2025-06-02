using System;
using UnityEngine;

public interface IProjectilePool
{
    public Projectile Get(BaseEnemy target, Vector3 origin, float damage, float speed, Action<Projectile, ISyncObject> onSendProjectileReturn);
    void Release();
}
