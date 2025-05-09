using System;
using UnityEngine;

public interface ITower
{
    Transform Transform { get; }

    int ID { get; }
    int Grade { get; }
    string TowerName { get; }


    float Damage { get; }
    float Range { get; }
    float FireRate { get; }
    int TargetCount { get; }

    public void Initialize(TowerData data, IEnemyProvider enemyProvider, int level = 1);
}
