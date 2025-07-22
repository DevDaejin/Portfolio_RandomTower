using System;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, IProjectileMovement, IProjectileHitCheck
{
    [SerializeField] private AudioClip _spawnClip;
    [SerializeField] private AudioClip _hitClip;
    private BaseEnemy _target;
    private float _damage;
    private float _speed;
    private ISyncObject _syncObject;
    private Action<Projectile> _onReturn;
    private Action<string> _onSendProjectileReturn;

    private void Awake()
    {
        _syncObject = GetComponent<ISyncObject>();
    }

    private void Start()
    {
        GameManager.Instance.Sound.PlaySFX(_spawnClip);
    }

    protected virtual void Update()
    {
        if (_target == null)
        {
            if (_onSendProjectileReturn != null)
            {
                _onReturn?.Invoke(this);
            }
            return;
        }

        Move(transform, _target, _speed);

        if (HasHit(transform.position, _target))
        {
            GameManager.Instance.Sound.PlaySFX(_hitClip);
            _target?.TakeDamage(_damage);
            _onReturn?.Invoke(this);
        }
    }

    private void OnDisable()
    {
        _onSendProjectileReturn?.Invoke(_syncObject.ObjectID);
    }

    public virtual void Initialize(BaseEnemy target, Vector3 origin, float damage, float speed, Action<Projectile> onReutrn, Action<string> onSendProjectileReturn)
    {
        transform.position = origin;

        _target = target;
        _damage = damage;
        _speed = speed;
        _onReturn = onReutrn;
        _onSendProjectileReturn = onSendProjectileReturn;
    }

    public void ForceReturn()
    {
        _onReturn?.Invoke(this);
    }

    public abstract void Move(Transform transform, BaseEnemy target, float speed);

    public abstract bool HasHit(Vector3 projectilePosition, BaseEnemy target);
}
