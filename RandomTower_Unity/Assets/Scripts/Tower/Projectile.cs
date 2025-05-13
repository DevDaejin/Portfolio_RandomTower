using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool IsInitialized = false;

    private BaseEnemy _target;
    private float _damage;
    private float _speed;
    private Action<Projectile> _hitCallback;

    private const float threshold = 0.01f;

    private void Update()
    {
        if (_target == null) return;

        MoveToTarget();
        HitCheck();
    }

    public void Initialize(float damage, float speed, Action<Projectile> hitCallback)
    {
        if (IsInitialized) return;

        IsInitialized = true;

        _damage = damage;
        _speed = speed;

        if (_hitCallback == null)
        {
            _hitCallback = hitCallback;
        }
    }

    public void Set(Vector3 origin, BaseEnemy target)
    {
        transform.position = origin;
        _target = target;
    }
    private void MoveToTarget()
    {
        //TODO: 투사체 타입에 따라 다르게 날아감

        transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _speed * Time.deltaTime);
    }
    private void HitCheck()
    {
        if (Vector3.SqrMagnitude(transform.position - _target.transform.position) < threshold)
        {
            _target.TakeDamage(_damage);
            _hitCallback?.Invoke(this);
        }
    }
}
