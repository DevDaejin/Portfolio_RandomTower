using System;
using UnityEngine;

public class TowerSetting
{
    public TowerData Data = null;
    public Vector3 GridPosition = Vector3.zero;
    public IEnemyProvider EnemyProvider = null;
    public IProjectilePool ProjectilePool = null;
    public Action<int, ISyncObject> OnAttack = null;
    public Action<string> OnSendReturnProjectile = null;
}
