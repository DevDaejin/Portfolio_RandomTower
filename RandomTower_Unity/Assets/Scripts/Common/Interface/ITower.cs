using System;
using UnityEngine;

public interface ITower
{
    Transform Transform { get; }
    TowerData Data { get; }

    float Damage { get; }
    float Range { get; }
    float FireRate { get; }
    
    ISelectable Selectable { get; }

    void Initialize(TowerData data, Vector3 gridPosition, IProjectilePool pool, IEnemyProvider enemyProvider, Action<int, ISyncObject> onActtack, Action<Projectile, ISyncObject> onSendProjectileReturn, int level = 1);
}