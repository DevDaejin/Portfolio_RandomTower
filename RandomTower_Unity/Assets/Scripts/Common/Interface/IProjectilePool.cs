using System;
using UnityEngine;

public interface IProjectilePool
{
    Projectile Get(BaseEnemy target, Vector3 origin, float damage, float speed);
}
