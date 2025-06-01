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

    public Action<int, ISyncObject> OnSendSpawnPacket;
    public Action<int> OnReward;

    private const float SpawnInterval = 0.5f;

    private void Awake()
    {
        _enemyFactory = new EnemyFactory();
    }

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
            OnSendSpawnPacket.Invoke(enemy.Data.ID, syncObject);
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
        EnemyDataConfig target = null;
        foreach (EnemyDataConfig config in _enemyDatas)
        {
            if(config.Data.ID == id)
            {
                target = config;
                break;
            }
        }

        return target.Data;
    }

    public BaseEnemy GetEnemy(EnemyData data)
    {
        BaseEnemy enemy = _enemyFactory.CreateEnemy(data, _routeGroup);
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

        _cachingSortedList.Sort((a, b) =>
        {
            float distA = (a.transform.position - position).sqrMagnitude;
            float distB = (b.transform.position - position).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        if (_cachingSortedList.Count > count)
            _cachingSortedList.RemoveRange(count, _cachingSortedList.Count - count);

        return _cachingSortedList;
    }

    public int GetCurrentEnemyCount()
    {
        return _spawnedEnemies.Count;
    }

    public bool IsSpawningState()
    {
        bool isSpawning = false;

        foreach (KeyValuePair<int, Coroutine> kv in _spawnCoroutine)
        {
            if(kv.Value != null)
            {
                isSpawning = true;
            }
        }

        return isSpawning;
    }

    public void ReleaseAll()
    {
        foreach (KeyValuePair<int, Coroutine> pair in _spawnCoroutine)
        {
            StopCoroutine(pair.Value);
        }

        for (int i = 0; i < _spawnedEnemies.Count; i++)
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
