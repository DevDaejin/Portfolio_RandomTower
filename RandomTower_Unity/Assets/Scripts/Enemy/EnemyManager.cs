using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IEnemyProvider
{
    private List<BaseEnemy> _enemies = new();
    private readonly List<BaseEnemy> _cachingList = new();
    private readonly List<BaseEnemy> _cachingSortedList = new();

    public BaseEnemy FindClosest(Vector3 position, float range)
    {
        float sqrRange = range * range;
        float minSqrDistance = float.MaxValue;
        BaseEnemy closest = null;

        foreach (BaseEnemy enemy in _enemies)
        {
            float sqrDistance = (position - enemy.transform.position).sqrMagnitude;
            if (sqrDistance <= sqrRange && sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                closest = enemy;
            }
        }

        return closest;
    }

    public List<BaseEnemy> FindAllInRange(Vector3 position, float range)
    {
        float sqrRange = range * range;

        _cachingList.Clear();

        foreach(BaseEnemy enemy in _enemies)
        {
            if ((enemy.transform.position - position).sqrMagnitude <= sqrRange)
            {
                _cachingList.Add(enemy);
            }
        }

        return _cachingList;
    }

    public List<BaseEnemy> FindClosestWithCount(Vector3 position, float range, int count)
    {
        FindAllInRange(position, range);
        _cachingSortedList.Clear();

        foreach(BaseEnemy enemy in _cachingList
            .OrderBy(element => (element.transform.position - position).sqrMagnitude)
            .Take(count))
        {
            _cachingSortedList.Add(enemy);
        }

        return _cachingSortedList;
    }
}
