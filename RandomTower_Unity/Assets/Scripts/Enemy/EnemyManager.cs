using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class EnemyManager : MonoBehaviour, IEnemyProvider
{
    [SerializeField] private Transform _routeGroup;

    private EnemyFactory _enemyFactory;
    private readonly List<BaseEnemy> _enemies = new();
    private readonly List<BaseEnemy> _cachingList = new();
    private readonly List<BaseEnemy> _cachingSortedList = new();

    private const float SpawnInterval = 0.5f;

    private void Awake()
    {
        _enemyFactory = new EnemyFactory();
    }

    public void SpawnWave(StageConfig config)
    {
        StartCoroutine(SpawnWaveRoutine(config.WaveData.SpawnList));
    }

    private IEnumerator SpawnWaveRoutine(List<SpawnInfo> list)
    {
        foreach (SpawnInfo info in list)
        {
            for (int i = 0; i < info.Count; i++)
            {
                BaseEnemy enemy = _enemyFactory.CreateEnemy(info.Config.Data, _routeGroup);
                _enemies.Add(enemy);
                yield return new WaitForSecondsRealtime(SpawnInterval);
            }
        }
    }

    public void ReturnEnemy(BaseEnemy enemy)
    {
        _enemies.Remove(enemy);
        _enemyFactory.Return(enemy);
    }

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
        Debug.Log("Range");
        float sqrRange = range * range;

        _cachingList.Clear();

        foreach(BaseEnemy enemy in _enemies)
        {
            Debug.Log($"Enemy {enemy.GetInstanceID()}");
            if ((enemy.transform.position - position).sqrMagnitude <= sqrRange)
            {
                Debug.Log($"Added Enemy {enemy.GetInstanceID()}");
                _cachingList.Add(enemy);
            }
        }

        return _cachingList;
    }

    public List<BaseEnemy> FindClosestWithCount(Vector3 position, float range, int count)
    {
        Debug.Log("Find");
        FindAllInRange(position, range);
        _cachingSortedList.Clear();

        foreach(BaseEnemy enemy in _cachingList
            .OrderBy(element => (element.transform.position - position).sqrMagnitude)
            .Take(count))
        {
            Debug.Log($"Enemy {enemy.GetInstanceID()}");
            _cachingSortedList.Add(enemy);
        }

        return _cachingSortedList;
    }
}
