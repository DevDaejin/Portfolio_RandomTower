using System.Linq;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public EnemyData Data { get; private set; }
    protected float _currentHP;
    private Transform[] _routes;
    private bool isInitailized = false;

    public void Initialize(EnemyDataConfig config, Transform routeGroup)
    {
        if (!isInitailized)
        {
            isInitailized = true;
            InitializeRoutes(routeGroup);
            InitializeEnemyData();
        }
    }

    private void InitializeRoutes(Transform routeGroup)
    {
        if (_routes.Length != 0) return;
        _routes = routeGroup.GetComponentsInChildren<Transform>().Where(route => route != routeGroup).ToArray();
    }

    private void InitializeEnemyData()
    {
        _currentHP = Data.MaxHP;
    }

    public virtual void TakeDamage(float amount)
    {

    }

    protected virtual void Die()
    {

    }
}
