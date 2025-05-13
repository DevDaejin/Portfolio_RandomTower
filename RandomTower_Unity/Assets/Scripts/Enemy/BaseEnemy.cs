using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    public EnemyData Data { get; private set; }
    private Transform[] _routes;
    protected NavMeshAgent _agent;

    private bool _isInitailized = false;

    private int _targetIndex;
    private Vector3 _destination;

    protected float _currentHP;

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
            InitializeEnemyData(data);

            _agent = GetComponent<NavMeshAgent>();
            InitializeRoutes(routeGroup);
        }

        _targetIndex = 0;
        transform.position = _routes[_targetIndex].position;
        NextDestination();
    }

    private void InitializeRoutes(Transform routeGroup)
    {
        if (_routes != null) return;

        _routes = routeGroup.GetComponentsInChildren<Transform>().Where(route => route != routeGroup).ToArray();
    }

    private void InitializeEnemyData(EnemyData data)
    {
        Data = data;
        _currentHP = Data.MaxHP;
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
        OnTakeDamage?.Invoke(this, amount);

        if(_currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {

    }

    public float GetHPRatio()
    {
        return Mathf.Clamp01(_currentHP / Data.MaxHP);
    }
}
