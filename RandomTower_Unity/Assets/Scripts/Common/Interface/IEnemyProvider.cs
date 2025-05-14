using System.Collections.Generic;
using UnityEngine;

public interface IEnemyProvider
{
    BaseEnemy FindClosest(Vector3 position, float range);
    List<BaseEnemy> FindAllInRange(Vector3 position, float range);
    List<BaseEnemy> FindClosestWithCount(Vector3 position, float range, int count);
}
