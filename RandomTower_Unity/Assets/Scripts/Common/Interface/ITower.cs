using UnityEngine;

public interface ITower
{
    Transform Transform { get; }
    TowerData Data { get; }

    float Damage { get; }
    float Range { get; }
    float FireRate { get; }
    
    ISelectable Selectable { get; }

    void Initialize(TowerData data, Vector3 gridPosition, IProjectilePool pool, IEnemyProvider enemyProvider, int level = 1);
}