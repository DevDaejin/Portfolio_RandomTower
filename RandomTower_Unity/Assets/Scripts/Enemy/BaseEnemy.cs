using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    public EnemyData Data { get; private set; }
    
    private bool _isInitailized = false;
    private Transform[] _routes;
    private NavMeshAgent _agent;
    private Vector3 _destination;
    private int _targetIndex;

    public float CurrentHP;

    public Action<int> OnReward;
    public Action<BaseEnemy> OnDie;
    public event Action<BaseEnemy, float> OnTakeDamage;

    private void Update()
    {
        if(_routes != null && IsArrived())
        {
            NextDestination();
        }
    }

    public void Initialize(EnemyData data, Transform routeGroup)
    {
        if (!_isInitailized)
        {
            _isInitailized = true;
            _agent = GetComponent<NavMeshAgent>();
            InitializeRoutes(routeGroup);
        }
        
        InitializeEnemyData(data);

        _targetIndex = 0;
        transform.position = _routes[_targetIndex].position;
        NextDestination();
    }

    private void InitializeRoutes(Transform routeGroup)
    {
        if (_routes != null) return;

        List<Transform> routeList = new();

        foreach (Transform route in routeGroup.GetComponentsInChildren<Transform>())
        {
            if (route != routeGroup)
                routeList.Add(route);
        }

        _routes = routeList.ToArray();
    }

    private void InitializeEnemyData(EnemyData data)
    {
        Data = data;
        CurrentHP = Data.MaxHP;
    }

    private void NextDestination()
    {
        _targetIndex++;
        
        if (_targetIndex >= _routes.Length) _targetIndex = 0;

        _destination = _routes[_targetIndex].position;
        _agent.SetDestination(_destination);
    }

    private bool IsArrived()
    {
        float threshold = (_agent.stoppingDistance + 0.1f) * (_agent.stoppingDistance + 0.1f);
        Vector3 delta = transform.position - _destination;
        delta.y = 0;

        return delta.sqrMagnitude <= threshold;
    }

    public virtual void TakeDamage(float amount)
    {
        CurrentHP -= amount;
        OnTakeDamage?.Invoke(this, amount);

        if(CurrentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        OnReward?.Invoke(Data.RewardGold);
        OnDie?.Invoke(this);
    }

    public float GetHPRatio()
    {
        return Mathf.Clamp01(CurrentHP / Data.MaxHP);
    }
}
