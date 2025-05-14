using UnityEngine;

public interface ITower
{
    Transform Transform { get; }
    TowerData Data { get; }

    float Damage { get; }
    float Range { get; }
    float FireRate { get; }

    public void OnSelect();
    public void OnDeselect();
    public void Initialize(TowerData data, IProjectilePool pool, IEnemyProvider enemyProvider, int level = 1);
}