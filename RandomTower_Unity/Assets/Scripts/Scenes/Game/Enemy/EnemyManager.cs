using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IEnemyProvider
{
    [SerializeField] private Transform _routeGroup;
    [SerializeField] private EnemyUIManager _enemyUIManager;
    [SerializeField] private EnemyDataConfig[] _enemyDatas;

    private EnemyFactory _enemyFactory;
    private List<BaseEnemy> _spawnedEnemies = new();
    private readonly List<BaseEnemy> _cachingList = new();
    private readonly List<BaseEnemy> _cachingSortedList = new();
    private Dictionary<int, Coroutine> _spawnCoroutine = new();

    public int GetAlivedEnemyCount => _spawnedEnemies.Count;

    public Action<int, ISyncObject> OnSendSpawnPacket;
    public Action<string> OnSendEnemyReturn;
    public Action<int> OnReward;

    private const float SpawnInterval = 0.5f;

    private class DistanceComparer : IComparer<BaseEnemy>
    {
        private readonly Vector3 _origin;

        public DistanceComparer(Vector3 origin)
        {
            _origin = origin;
        }

        public int Compare(BaseEnemy a, BaseEnemy b)
        {
            float distA = (a.transform.position - _origin).sqrMagnitude;
            float distB = (b.transform.position - _origin).sqrMagnitude;
            return distA.CompareTo(distB);
        }
    }

    private void Awake()
    {
        _enemyFactory = new EnemyFactory();
    }

    public void SetRouteGroup(Transform routeGroup) => _routeGroup = routeGroup;

    public void SpawnWave(StageConfig config, int waveIndex)
    {
        List<SpawnInfo> wave = config.WaveData.SpawnList;
        if (waveIndex >= wave.Count) return;

        _spawnCoroutine.Add(waveIndex, StartCoroutine(SpawnWaveRoutine(waveIndex, wave[waveIndex])));
    }

    private IEnumerator SpawnWaveRoutine(int id, SpawnInfo info)
    {
        for (int i = 0; i < info.Count; i++)
        {
            BaseEnemy enemy = GetEnemy(info.Config.Data);
            ISyncObject syncObject = enemy.GetComponent<ISyncObject>();
            OnSendSpawnPacket?.Invoke(enemy.Data.ID, syncObject);
            AddSpawnedEnemy(enemy);
            yield return new WaitForSecondsRealtime(SpawnInterval);
        }

        _spawnCoroutine.Remove(id);
    }

    public void AddSpawnedEnemy(BaseEnemy enemy)
    {
        _spawnedEnemies.Add(enemy);
    }

    public EnemyData GetEnemyDataWithID(int id)
    {
        foreach (var config in _enemyDatas)
        {
            if (config.Data.ID == id)
            {
                return config.Data;
            }
        }

        return null;
    }

    public BaseEnemy GetEnemy(EnemyData data)
    {
        BaseEnemy enemy = _enemyFactory.CreateEnemy(data, _routeGroup, OnSendEnemyReturn);
        enemy.OnDie = ReleaseEnemy;
        enemy.OnReward = OnReward;
        _enemyUIManager?.Register(enemy);

        return enemy;
    }

    public void ReleaseEnemy(BaseEnemy enemy)
    {
        if (!_spawnedEnemies.Contains(enemy)) return;

        enemy.OnDie = null;
        enemy.OnReward = null;

        _enemyUIManager?.Unregister(enemy);
        _spawnedEnemies.Remove(enemy);
        _enemyFactory.Release(enemy);
    }

    public BaseEnemy FindClosest(Vector3 position, float range)
    {
        float sqrRange = range * range;
        float minSqrDistance = float.MaxValue;
        BaseEnemy closest = null;

        foreach (BaseEnemy enemy in _spawnedEnemies)
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

        foreach (BaseEnemy enemy in _spawnedEnemies)
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

        foreach (var enemy in _cachingList)
        {
            _cachingSortedList.Add(enemy);
        }

        var comapre = new DistanceComparer(position);
        _cachingSortedList.Sort(comapre);

        if (_cachingSortedList.Count > count)
            _cachingSortedList.RemoveRange(count, _cachingSortedList.Count - count);

        return _cachingSortedList;
    }

    public bool IsSpawningState()
    {
        foreach (var pair in _spawnCoroutine)
        {
            if (pair.Value != null)
            {
                return true;
            }
        }

        return false;
    }

    public void ReleaseAll()
    {
        foreach (var pair in _spawnCoroutine)
        {
            StopCoroutine(pair.Value);
        }
        _spawnCoroutine.Clear();

        while (_spawnedEnemies.Count > 0)
        {
            ReleaseEnemy(_spawnedEnemies[0]);
        }
        _spawnedEnemies.Clear();

        _cachingList.Clear();
        _cachingSortedList.Clear();

        _enemyFactory.ReleaseAll();
        _enemyUIManager.ReleaseAll();
    }
}
