using UnityEngine;

public interface ITower
{
    Transform Transform { get; }

    TowerData Data { get; }

    float Damage { get; }
    float Range { get; }
    float FireRate { get; }

    public void Initialize(TowerData data, Pool<Projectile> pool, IEnemyProvider enemyProvider, int level = 1);
}