using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IEnemyProvider
{
    [SerializeField] private Transform _routeGroup;
    [SerializeField] private EnemyUIManager _enemyUIManager;

    private EnemyFactory _enemyFactory;
    private List<BaseEnemy> _enemies = new();
    private readonly List<BaseEnemy> _cachingList = new();
    private readonly List<BaseEnemy> _cachingSortedList = new();
    private Dictionary<int, Coroutine> _spawnCoroutine = new();

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
            BaseEnemy enemy = _enemyFactory.CreateEnemy(info.Config.Data, _routeGroup);
            enemy.OnDie = ReturnEnemy;
            enemy.OnReward = OnReward;
            _enemyUIManager?.Register(enemy);
            _enemies.Add(enemy);
            yield return new WaitForSecondsRealtime(SpawnInterval);
        }

        _spawnCoroutine.Remove(id);
    }

    public void ReturnEnemy(BaseEnemy enemy)
    {
        if (!_enemies.Contains(enemy)) return;

        enemy.OnDie = null;
        enemy.OnReward = null;

        _enemyUIManager?.Unregister(enemy);
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
        float sqrRange = range * range;

        _cachingList.Clear();

        foreach (BaseEnemy enemy in _enemies)
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
        return _enemies.Count;
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

    public void ReturnAll()
    {
        foreach (KeyValuePair<int, Coroutine> kvp in _spawnCoroutine)
        {
            StopCoroutine(kvp.Value);
        }
        _spawnCoroutine.Clear();

        for (int i = 0; i < _enemies.Count; i++)
        {
            ReturnEnemy(_enemies[0]);
        }
        _enemies.Clear();

        _enemyFactory.ReturnAll();
        _enemyUIManager.ReturnAll();
    }
}
